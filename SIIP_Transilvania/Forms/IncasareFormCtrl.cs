using System;
using System.Windows.Forms;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    // ═══════════════════════════════════════════════════════════════════════
    // IncasareFormCtrl — Controller pentru formularul Inregistrare Incasare Client
    // Echivalent cu ReturFormCtrl / AchizitiiFormCtrl din modelul Java/JPA (ghid PSI Partea 4)
    // Translateaza evenimentele din UI in apeluri catre ModelAdapter.
    // ═══════════════════════════════════════════════════════════════════════
    public class IncasareFormCtrl
    {
        // Relatia de compunere Controller -> ModelAdapter
        private readonly IncasareFormData _formData;

        public IncasareFormCtrl()
        {
            _formData = new IncasareFormData();
            // Incarca totalurile lunare la initializare
            _formData.RefreshTotaluri();
        }

        public IncasareFormData GetFormData() => _formData;

        // ── Eveniment: selectia unui client ──────────────────────────────
        // Echivalent cu: setFurnizorSelectat() din AchizitiiFormCtrl
        public void OnClientSelected(int cod, string nume)
        {
            _formData.SetClientSelectat(cod, nume);
            _formData.RefreshIncasari();
        }

        // ── Eveniment: selectia unei facturi ─────────────────────────────
        public void OnFacturaSelected(string serie, string numar, DateTime data, decimal valTotala)
        {
            _formData.SetFacturaSelectata(serie, numar, data, valTotala);
        }

        // ── Eveniment: selectia canalului (Numerar / ContBancar) ─────────
        public void OnCanalChanged(string canal)
        {
            _formData.SetCanal(canal);
        }

        // ── Eveniment: click Adaugare ─────────────────────────────────────
        // Echivalent cu: documentNou() din AchizitiiFormCtrl
        public void DocumentNou()
        {
            _formData.GenerateIdIncasare();
            var incasare = new Incasare
            {
                DataIncasare = DateTime.Now,
                Canal = _formData.GetCanal()
            };
            _formData.SetDocumentCurent(incasare);
        }

        // ── Eveniment: click Salvare ──────────────────────────────────────
        // Conform diagramei de secvente din Figura 14 a ghidului:
        // 1. BeginTransaction
        // 2. GetDocumentCurent
        // 3. SaveIncasare (INSERT)
        // 4. SaveBonFiscal / SaveExtrasContIncasare (dupa canal)
        // 5. UpdateSoldClient (delta pozitiv)
        // 6. UpdateSoldCaserie / UpdateSoldContBancar (dupa canal)
        // 7. CommitTransaction
        public bool SalveazaIncasare(decimal sumaIncasata, string canal,
                                      DateTime dataIncasare, int idCaserie, string iban)
        {
            // Validari
            if (_formData.GetCodClientSelectat() == 0)
            { ShowError("Selectati un client."); return false; }
            if (string.IsNullOrWhiteSpace(_formData.GetSerieFact()))
            { ShowError("Selectati factura."); return false; }
            if (sumaIncasata <= 0)
            { ShowError("Introduceti suma de incasat."); return false; }
            if (sumaIncasata > _formData.GetRestDePlata())
            {
                ShowError($"Suma introdusa ({sumaIncasata:F2} RON) depaseste\n" +
                          $"restul de plata ({_formData.GetRestDePlata():F2} RON).");
                return false;
            }

            // Completeaza documentul curent
            var incasare = _formData.GetDocumentCurent();
            incasare.IdIncasare  = _formData.GetIdIncasare();
            incasare.DataIncasare = dataIncasare;
            incasare.SumaIncasata = sumaIncasata;
            incasare.Canal        = canal;
            incasare.SerieFact    = _formData.GetSerieFact();
            incasare.NumarFact    = _formData.GetNumarFact();

            try
            {
                _formData.GetDocRepo().BeginTransaction();

                // Pas 1 — salveaza incasarea
                _formData.GetDocRepo().SaveIncasare(incasare);

                // Pas 2 — document secundar (BonFiscal sau ExtrasContIncasare)
                if (canal == "Numerar")
                {
                    var bon = new BonFiscal
                    {
                        DataEmitere  = dataIncasare,
                        TotalValoare = sumaIncasata,
                        IdCaserie    = idCaserie,
                        IdIncasare   = incasare.IdIncasare
                    };
                    _formData.GetDocRepo().SaveBonFiscal(bon);
                    // Pas 3 — actualizeaza soldul caseriei
                    _formData.GetMasterRepo().UpdateSoldCaserie(idCaserie, sumaIncasata);
                }
                else
                {
                    var extras = new ExtrasContIncasare
                    {
                        DataEmitere  = dataIncasare,
                        SumaIncasata = sumaIncasata,
                        IBAN         = iban,
                        IdIncasare   = incasare.IdIncasare
                    };
                    _formData.GetDocRepo().SaveExtrasContIncasare(extras);
                }

                // Pas 4 — actualizeaza soldul clientului (scade datoria)
                _formData.GetMasterRepo().UpdateSoldClient(_formData.GetCodClientSelectat(), -sumaIncasata);

                // Pas 5 — actualizeaza stareIncasare pe FacturaClient
                _formData.GetDocRepo().UpdateStareIncasareFactura(
                    _formData.GetSerieFact(), _formData.GetNumarFact(),
                    sumaIncasata, _formData.GetRestDePlata());

                _formData.GetDocRepo().CommitTransaction();

                _formData.RefreshIncasari();
                _formData.RefreshTotaluri();
                _formData.ResetDocumentCurent();
                return true;
            }
            catch (Exception ex)
            {
                _formData.GetDocRepo().RollbackTransaction();
                ShowError("Eroare la salvare: " + ex.Message);
                return false;
            }
        }

        // ── Eveniment: click Renuntare ────────────────────────────────────
        public void Renunta()
        {
            _formData.ResetDocumentCurent();
        }

        // ── Helper ────────────────────────────────────────────────────────
        private void ShowError(string msg) =>
            MessageBox.Show(msg, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public int GetIdGenerat() => _formData.GetIdIncasare();
    }
}
