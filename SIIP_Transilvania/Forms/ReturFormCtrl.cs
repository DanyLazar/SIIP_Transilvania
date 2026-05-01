using System;
using System.Windows.Forms;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    // ═══════════════════════════════════════════════════════════════════════
    // ReturFormCtrl — Controller pentru formularul Inregistrare Retur
    // Echivalent cu AchizitiiFormCtrl din modelul Java/JPA (ghid PSI Partea 4)
    // Translateaza evenimentele din UI in apeluri catre ModelAdapter.
    // ═══════════════════════════════════════════════════════════════════════
    public class ReturFormCtrl
    {
        // Relatia de compunere Controller -> ModelAdapter
        // Echivalent cu: AchizitiiFormCtrl -> AchizitiiFormData
        private readonly ReturFormData _formData;

        public ReturFormCtrl()
        {
            _formData = new ReturFormData();
            // Incarca totalurile lunare la initializare
            _formData.RefreshTotaluri();
        }

        public ReturFormData GetFormData() => _formData;

        // ── Eveniment: schimbarea tipului de retur ────────────────────────
        // Echivalent cu: setOperatieSelectata() din AchizitiiFormCtrl
        public void OnTipReturChanged(string tipRetur)
        {
            _formData.SetTipRetur(tipRetur);
            _formData.ResetDocumentCurent();
            _formData.RefreshRetururi();
        }

        // ── Eveniment: selectia unui partener ─────────────────────────────
        // Echivalent cu: setFurnizorSelectat() din AchizitiiFormCtrl
        public void OnPartenerSelected(int cod, string nume)
        {
            _formData.SetPartenerSelectat(cod, nume);
            _formData.RefreshRetururi();
        }

        // ── Eveniment: selectia unei facturi initiale ─────────────────────
        public void OnFacturaSelected(string serie, string numar, DateTime data, decimal valTotala)
        {
            _formData.SetFacturaInitSelectata(serie, numar, data, valTotala);
        }

        // ── Eveniment: click Adaugare ─────────────────────────────────────
        // Echivalent cu: documentNou() din AchizitiiFormCtrl
        public void DocumentNou()
        {
            var retur = new FacturaRetur
            {
                Serie = "RET",
                TipRetur = _formData.GetTipRetur(),
                StareRetur = "Emis",
                DataDocument = DateTime.Now
            };
            _formData.SetDocumentCurent(retur);
            _formData.GenerateNumarRetur();
        }

        // ── Eveniment: click Salvare ──────────────────────────────────────
        // Echivalent cu: salveazaModificariDocument() din AchizitiiFormCtrl
        // Conform diagramei de secvente din Figura 14 a ghidului:
        // 1. BeginTransaction
        // 2. GetDocumentCurent
        // 3. SaveFacturaRetur (decide INSERT sau UPDATE)
        // 4. CommitTransaction
        public bool SalveazaRetur(string motiv, string stare, decimal valRetur,
                                   string canal, DateTime data)
        {
            // Validari
            if (_formData.GetCodPartenerSelectat() == 0)
            { ShowError("Selectati un partener."); return false; }
            if (string.IsNullOrWhiteSpace(_formData.GetSerieFactInit()))
            { ShowError("Selectati factura initiala."); return false; }
            if (valRetur <= 0)
            { ShowError("Introduceti valoarea returului."); return false; }
            if (valRetur > _formData.GetRestDisponibil())
            {
                ShowError($"Valoarea returului ({valRetur:F2} RON) depaseste\n" +
                          $"restul disponibil ({_formData.GetRestDisponibil():F2} RON).");
                return false;
            }

            // Completeaza documentul curent
            var retur = _formData.GetDocumentCurent();
            retur.Numar = _formData.GetNumar();
            retur.DataDocument = data;
            retur.ValoareRetur = valRetur;
            retur.MotivRetur = motiv;
            retur.StareRetur = stare;
            retur.SerieFactInit = _formData.GetSerieFactInit();
            retur.NumarFactInit = _formData.GetNumarFactInit();

            if (_formData.GetTipRetur() == "Client")
                retur.CodClient = _formData.GetCodPartenerSelectat();
            else
                retur.CodFurnizor = _formData.GetCodPartenerSelectat();

            try
            {
                // BeginTransaction — tranzactia e gestionata de Controller (client)
                _formData.GetDocRepo().BeginTransaction();

                // SaveFacturaRetur — decide INSERT sau UPDATE
                _formData.GetDocRepo().SaveFacturaRetur(retur);

                // CommitTransaction
                _formData.GetDocRepo().CommitTransaction();

                // Refresh date dupa salvare
                _formData.RefreshRetururi();
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

        // ── Eveniment: click Anulare ──────────────────────────────────────
        public bool AnuleazaRetur(string serie, string numar)
        {
            if (string.IsNullOrEmpty(serie)) return false;

            if (MessageBox.Show("Doriti sa anulati acest retur?", "Confirmare",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return false;

            try
            {
                _formData.GetDocRepo().BeginTransaction();
                _formData.GetDocRepo().AnuleazaRetur(serie, numar);
                _formData.GetDocRepo().CommitTransaction();

                _formData.RefreshRetururi();
                _formData.RefreshTotaluri();
                return true;
            }
            catch (Exception ex)
            {
                _formData.GetDocRepo().RollbackTransaction();
                ShowError("Eroare la anulare: " + ex.Message);
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

        // Proprietate pentru accesul la numarul generat
        public string GetNumarGenerat() => _formData.GetNumar();
    }
}