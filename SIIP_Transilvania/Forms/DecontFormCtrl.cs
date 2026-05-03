using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    public class DecontFormCtrl
    {
        private readonly DecontFormData _formData;

        public DecontFormCtrl()
        {
            _formData = new DecontFormData();
            _formData.RefreshTotaluri();
        }

        public DecontFormData GetFormData() => _formData;

        public void OnAngajatSelected(int cod, string nume, string functie)
        {
            _formData.SetAngajatSelectat(cod, nume, functie);
            _formData.RefreshDeconturi();
        }

        public void DocumentNou()
        {
            var decont = new Decont
            {
                Serie = "DC",
                Stare = "Depus",
                DataDocument = DateTime.Now
            };
            _formData.SetDocumentCurent(decont);
            _formData.GenerateNumarDecont();
        }

        public bool SalveazaDecont(DateTime perioadaStart, DateTime perioadaEnd,
                                    List<ArticolDecont> articole, int codDirector,
                                    string canal, DateTime data)
        {
            if (_formData.GetCodAngajatSelectat() == 0)
            { ShowError("Selectati un angajat."); return false; }
            if (articole == null || articole.Count == 0)
            { ShowError("Adaugati cel putin un articol."); return false; }
            if (codDirector == 0)
            { ShowError("Selectati directorul aprobator."); return false; }
            if (perioadaStart >= perioadaEnd)
            { ShowError("Perioada introdusa nu este valida."); return false; }

            var decont = _formData.GetDocumentCurent();
            decont.Numar = _formData.GetNumar();
            decont.DataDocument = data;
            decont.PerioadaStart = perioadaStart;
            decont.PerioadaEnd = perioadaEnd;
            decimal totalArticole = 0;
            foreach (var a in articole) totalArticole += a.Valoare;
            decont.ValoareDecontata = totalArticole;
            decont.Stare = "Depus";
            decont.CodDirector = codDirector;
            _formData.SetArticole(articole);
            _formData.SetCanal(canal);

            try
            {
                _formData.GetDocRepo().BeginTransaction();
                _formData.GetDocRepo().SaveDecont(decont, articole, _formData.GetCodAngajatSelectat());
                _formData.GetDocRepo().CommitTransaction();
                _formData.RefreshDeconturi();
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

        public bool AprobazaDecont(string serie, string numar)
        {
            if (string.IsNullOrEmpty(serie)) return false;
            if (MessageBox.Show("Aprobati acest decont?", "Confirmare",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return false;

            try
            {
                _formData.GetDocRepo().BeginTransaction();
                _formData.GetDocRepo().AprobazaDecont(serie, numar);
                _formData.GetDocRepo().CommitTransaction();
                _formData.RefreshDeconturi();
                _formData.RefreshTotaluri();
                return true;
            }
            catch (Exception ex)
            {
                _formData.GetDocRepo().RollbackTransaction();
                ShowError("Eroare la aprobare: " + ex.Message);
                return false;
            }
        }

        public bool RespingeDecont(string serie, string numar, string motiv)
        {
            if (string.IsNullOrEmpty(serie)) return false;
            if (string.IsNullOrEmpty(motiv))
            { ShowError("Introduceti motivul respingerii."); return false; }

            try
            {
                _formData.GetDocRepo().BeginTransaction();
                _formData.GetDocRepo().RespingeDecont(serie, numar, motiv);
                _formData.GetDocRepo().CommitTransaction();
                _formData.RefreshDeconturi();
                _formData.RefreshTotaluri();
                return true;
            }
            catch (Exception ex)
            {
                _formData.GetDocRepo().RollbackTransaction();
                ShowError("Eroare la respingere: " + ex.Message);
                return false;
            }
        }

        public void Renunta() { _formData.ResetDocumentCurent(); }

        public string GetNumarGenerat() => _formData.GetNumar();

        private void ShowError(string msg) =>
            MessageBox.Show(msg, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}