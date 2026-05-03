using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    /// <summary>
    /// Controller MVC — Inregistrare Plata Furnizor (Iosub Maria-Catalina).
    /// Fluxul tranzactional (conform spec cap.2):
    ///   BeginTransaction -> SavePlata -> SavePlataEsalonata
    ///   -> Numerar: UpdateSoldCaserie | ContBancar: SaveExtrasContPlata
    ///   -> UpdateSoldFurnizor -> UpdateStarePlataFactura -> CommitTransaction
    /// Stare: "Inregistrat" | "Anulat"
    /// </summary>
    public class PlataFormCtrl
    {
        private readonly PlataFormData _formData;
        private readonly Action<string> _showError;
        private readonly Action<string> _showInfo;

        public PlataFormCtrl(Action<string> showError, Action<string> showInfo)
        {
            _formData = new PlataFormData();
            _showError = showError;
            _showInfo = showInfo;
        }

        public PlataFormData GetFormData() => _formData;

        // ── Initializare ───────────────────────────────────────────────────
        public void Initializeaza()
        {
            _formData.RefreshTotaluriLuna();
        }

        // ── Evenimente UI ──────────────────────────────────────────────────

        public void OnFurnizorSelected(int codFurnizor)
        {
            _formData.ResetFacturiFurnizor();
            _formData.ResetDocumentGenerat();
        }

        public void OnFacturaSelected(string serie, string numar,
                                      DateTime dataDocument, decimal valTotala)
        {
            _formData.SetDataFactura(dataDocument);
            _formData.SetValoareTotalaFactura(valTotala);
            _formData.CalculeazaRestDisponibil(serie, numar, valTotala);
        }

        public void OnCanalChanged(string canal)
        {
            if (_formData.GetDocumentCurent() != null)
                _formData.GetDocumentCurent().Canal = canal;
        }

        // ── Calcul procent / valoare (sincronizare bidirectionala) ─────────

        /// <summary>Calculeaza procentul din valoarea introdusa.</summary>
        public decimal CalculeazaProcentDinValoare(decimal valoare)
        {
            decimal total = _formData.GetValoareTotalaFactura();
            return total > 0 ? Math.Round((valoare / total) * 100m, 2) : 0m;
        }

        /// <summary>Calculeaza valoarea din procentul introdus.</summary>
        public decimal CalculeazaValoareDinProcent(decimal procent)
        {
            decimal total = _formData.GetValoareTotalaFactura();
            return Math.Round((procent / 100m) * total, 2);
        }

        // ── Butoane ────────────────────────────────────────────────────────

        public Plata DocumentNou()
        {
            var plata = new Plata
            {
                IdPlata = 0,
                DataPlata = DateTime.Today,
                Suma = 0m,
                TipPlata = "PlataFurnizor",
                Canal = "Numerar",
                Stare = "Inregistrat"
            };
            _formData.SetDocumentCurent(plata);
            _formData.SetDocumentNou(true);
            _formData.ResetDocumentGenerat();
            return plata;
        }

        /// <summary>
        /// Salveaza plata conform fluxului tranzactional din spec.
        /// </summary>
        public bool SalveazaPlata(
            int codFurnizor,
            string serieFurnizor,
            string numarFurnizor,
            decimal valPlata,
            string tipRata,
            DateTime scadenta,
            string canal,
            int? idCaserie,
            string iban)
        {
            // 1. Validari
            if (codFurnizor <= 0)
            { _showError("Selectati un furnizor!"); return false; }

            if (string.IsNullOrWhiteSpace(serieFurnizor))
            { _showError("Selectati o factura furnizor!"); return false; }

            if (valPlata <= 0)
            { _showError("Valoarea platii trebuie sa fie mai mare decat 0!"); return false; }

            if (valPlata > _formData.GetRestDisponibil())
            {
                _showError($"Valoarea platii ({valPlata:F2} RON) depaseste\n" +
                           $"restul de plata al facturii ({_formData.GetRestDisponibil():F2} RON)!");
                return false;
            }

            if (canal == "Numerar" && (idCaserie == null || idCaserie <= 0))
            { _showError("Selectati caseria pentru plata numerar!"); return false; }

            if (canal == "ContBancar" && string.IsNullOrWhiteSpace(iban))
            { _showError("Selectati contul bancar (IBAN) pentru virament!"); return false; }

            // 2. Tranzactie
            try
            {
                _formData.GetDocRepo().BeginTransaction();

                // 3. SavePlata
                var plata = _formData.GetDocumentCurent();
                plata.Suma = valPlata;
                plata.Canal = canal;
                plata.TipPlata = "PlataFurnizor";
                plata.Stare = "Inregistrat";

                plata = _formData.GetDocRepo().SavePlata(plata);
                _formData.SetDocumentCurent(plata);

                // 4. SavePlataEsalonata
                decimal valTotala = _formData.GetValoareTotalaFactura();
                decimal procent = valTotala > 0
                    ? Math.Round((valPlata / valTotala) * 100m, 2)
                    : 100m;

                _formData.GetDocRepo().SavePlataEsalonata(new PlataEsalonata
                {
                    IdPlata = plata.IdPlata,
                    TipRata = tipRata,
                    ProcentAcoperit = procent,
                    DataScadenta = scadenta,
                    SerieFurnizor = serieFurnizor,
                    NumarFurnizor = numarFurnizor
                });

                // 5. Canal-specific + document generat
                string tipDoc = "";
                string nrDoc = "";

                if (canal == "Numerar" && idCaserie.HasValue)
                {
                    _formData.GetMasterRepo().UpdateSoldCaserie(idCaserie.Value, -valPlata);
                    // Document: Chitanta (nr = PLF-IdPlata)
                    tipDoc = "Chitanta";
                    nrDoc = $"CH-{plata.IdPlata:D4}";
                }
                else if (canal == "ContBancar" && !string.IsNullOrWhiteSpace(iban))
                {
                    int nrExtras = _formData.GetDocRepo().SaveExtrasContPlata(new ExtrasContPlata
                    {
                        DataEmitere = plata.DataPlata,
                        SumaPlata = valPlata,
                        IBAN = iban,
                        IdPlata = plata.IdPlata
                    });
                    // Document: Extras Cont Plata
                    tipDoc = "Extras Cont Plata";
                    nrDoc = $"ECP-{nrExtras:D4}";
                }

                // 6. UpdateSoldFurnizor
                _formData.GetMasterRepo().UpdateSoldFurnizor(codFurnizor, -valPlata);

                // 7. UpdateStarePlataFactura
                decimal totalAchitat = _formData.GetDocRepo().GetSumaAchitata(serieFurnizor, numarFurnizor);
                string stareNoua = totalAchitat >= valTotala ? "Achitat" : "PartialAchitat";
                _formData.GetDocRepo().UpdateStarePlataFactura(serieFurnizor, numarFurnizor, stareNoua);

                // 8. Commit
                _formData.GetDocRepo().CommitTransaction();

                // Seteaza documentul generat (afisat in Grup 5 din form)
                _formData.SetDocumentGenerat(tipDoc, nrDoc, plata.DataPlata);
                _formData.RefreshTotaluriLuna();
                _formData.SetDocumentNou(false);
                return true;
            }
            catch (Exception ex)
            {
                _formData.GetDocRepo().RollbackTransaction();
                _showError($"Eroare la salvare:\n{ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Anulare [Ctrl+N] — disponibila doar pentru Stare = 'Inregistrat'.
        /// </summary>
        public bool AnuleazaPlata(PlataDetail pd)
        {
            if (pd == null) return false;

            var confirm = MessageBox.Show(
                $"Confirmati anularea platii #{pd.Plata.IdPlata} de {pd.Plata.Suma:F2} RON?\n\n" +
                "Operatiunea este ireversibila.\n" +
                "Soldul furnizorului va fi reversat.",
                "Confirmare Anulare",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return false;

            try
            {
                _formData.GetDocRepo().BeginTransaction();
                _formData.GetDocRepo().AnuleazaPlata(pd.Plata.IdPlata);
                _formData.GetMasterRepo().UpdateSoldFurnizor(pd.CodFurnizor, pd.Plata.Suma);
                _formData.GetDocRepo().CommitTransaction();

                _formData.RefreshTotaluriLuna();
                _showInfo($"Plata #{pd.Plata.IdPlata} a fost anulata cu succes.");
                return true;
            }
            catch (Exception ex)
            {
                _formData.GetDocRepo().RollbackTransaction();
                _showError($"Eroare la anulare:\n{ex.Message}");
                return false;
            }
        }

        public void Renunta()
        {
            _formData.SetDocumentCurent(null);
            _formData.SetDocumentNou(false);
            _formData.ResetDocumentGenerat();
        }

        public List<PlataDetail> GetPlatiByFurnizor(int codFurnizor)
        {
            return _formData.GetDocRepo().FindPlatiByFurnizor(codFurnizor);
        }
    }
}