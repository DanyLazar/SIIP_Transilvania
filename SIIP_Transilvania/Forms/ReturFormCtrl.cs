using System;
using System.Windows.Forms;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    public class ReturFormCtrl
    {
        private readonly ReturFormData _formData;

        public ReturFormCtrl()
        {
            _formData = new ReturFormData();
            _formData.RefreshTotaluri();
        }

        public ReturFormData GetFormData() => _formData;

        public void OnTipReturChanged(string tipRetur)
        {
            _formData.SetTipRetur(tipRetur);
            _formData.ResetDocumentCurent();
            _formData.RefreshRetururi();
        }

        public void OnPartenerSelected(int cod, string nume)
        {
            _formData.SetPartenerSelectat(cod, nume);
            _formData.RefreshRetururi();
        }

        public void OnFacturaSelected(string serie, string numar, DateTime data, decimal valTotala)
        {
            _formData.SetFacturaInitSelectata(serie, numar, data, valTotala);
        }

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

            // Daca documentul curent e null (edge case), il recreem
            var retur = _formData.GetDocumentCurent();
            if (retur == null)
            {
                retur = new FacturaRetur
                {
                    Serie = "RET",
                    TipRetur = _formData.GetTipRetur(),
                    StareRetur = "Emis",
                    DataDocument = DateTime.Now
                };
                _formData.SetDocumentCurent(retur);
                _formData.GenerateNumarRetur();
            }

            // Completeaza documentul curent
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
                _formData.GetDocRepo().BeginTransaction();
                _formData.GetDocRepo().SaveFacturaRetur(retur);
                _formData.GetDocRepo().CommitTransaction();
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

        public void Renunta()
        {
            _formData.ResetDocumentCurent();
        }

        // Incarca IBAN-urile firmei pentru canal Card/IBAN
        public System.Collections.Generic.List<string> GetIBANuriFirma()
        {
            var list = new System.Collections.Generic.List<string>();
            var conturi = _formData.GetContBancarRepo().FindAll();
            foreach (var c in conturi)
                list.Add($"{c.IBAN} — {c.Banca}");
            return list;
        }

        private void ShowError(string msg) =>
            MessageBox.Show(msg, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);

        public string GetNumarGenerat() => _formData.GetNumar();
    }
}