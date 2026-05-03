using System;
using System.Collections.Generic;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    public class DecontFormData
    {
        private Decont _documentCurent;
        private readonly DocumentRepository _docRepo = new DocumentRepository();
        private readonly MasterRepository _masterRepo = new MasterRepository();

        public DocumentRepository GetDocRepo() => _docRepo;
        public MasterRepository GetMasterRepo() => _masterRepo;
        public Decont GetDocumentCurent() => _documentCurent;
        public void SetDocumentCurent(Decont d) { _documentCurent = d; }

        // Angajati
        private List<Angajat> _listaAngajati;
        private List<Angajat> _listaDirectori;
        private int _codAngajatSelectat;
        private string _numeAngajatSelectat;
        private string _functieAngajat;

        public int GetCodAngajatSelectat() => _codAngajatSelectat;
        public string GetNumeAngajatSelectat() => _numeAngajatSelectat;
        public string GetFunctieAngajat() => _functieAngajat;

        public List<Angajat> GetListaAngajati()
        {
            if (_listaAngajati == null)
                _listaAngajati = _masterRepo.FindAngajatiAll();
            return _listaAngajati;
        }

        public List<Angajat> GetListaDirectori()
        {
            if (_listaDirectori == null)
                _listaDirectori = _masterRepo.FindDirectoriAll();
            return _listaDirectori;
        }

        public void SetAngajatSelectat(int cod, string nume, string functie)
        {
            _codAngajatSelectat = cod;
            _numeAngajatSelectat = nume;
            _functieAngajat = functie;
            _listaDeconturi = null;
        }

        // Deconturi anterioare
        private List<Decont> _listaDeconturi;

        public List<Decont> GetDeconturi()
        {
            if (_codAngajatSelectat > 0)
                _listaDeconturi = _docRepo.FindDeconturiByAngajat(_codAngajatSelectat);
            return _listaDeconturi ?? new List<Decont>();
        }

        public void RefreshDeconturi() { _listaDeconturi = null; }

        // Articole decont
        private List<ArticolDecont> _articole = new List<ArticolDecont>();
        public List<ArticolDecont> GetArticole() => _articole;
        public void SetArticole(List<ArticolDecont> a) { _articole = a; }

        public decimal GetTotalSolicitat()
        {
            decimal total = 0;
            foreach (var a in _articole) total += a.Valoare;
            return total;
        }

        // Director aprobator
        private int _codDirectorSelectat;
        public int GetCodDirectorSelectat() => _codDirectorSelectat;
        public void SetCodDirectorSelectat(int cod) { _codDirectorSelectat = cod; }

        // Canal plata
        private string _canal = "Numerar";
        public string GetCanal() => _canal;
        public void SetCanal(string v) { _canal = v; }

        // Totaluri luna
        private decimal _totalLuna;
        private int _nrDeponturiDepuse;
        private int _nrDeconturiAprobate;
        private decimal _totalAprobatLuna;

        public decimal GetTotalLuna() => _totalLuna;
        public int GetNrDeponturiDepuse() => _nrDeponturiDepuse;
        public int GetNrDeconturiAprobate() => _nrDeconturiAprobate;
        public decimal GetTotalAprobatLuna() => _totalAprobatLuna;

        public void RefreshTotaluri()
        {
            var (total, depuse, aprobate, totalAprobat) = _docRepo.GetTotaluriLunaDecont();
            _totalLuna = total;
            _nrDeponturiDepuse = depuse;
            _nrDeconturiAprobate = aprobate;
            _totalAprobatLuna = totalAprobat;
        }

        // Numar decont
        private string _serie = "DC";
        private string _numar;
        public string GetSerie() => _serie;
        public string GetNumar() => _numar;

        public void GenerateNumarDecont()
        {
            _numar = _docRepo.GetNextNumarDecont();
        }

        public void ResetDocumentCurent()
        {
            _documentCurent = null;
            _articole = new List<ArticolDecont>();
            _canal = "Numerar";
            _codDirectorSelectat = 0;
        }
    }

    // Clasa pentru articolele din grila decontului
    public class ArticolDecont
    {
        public string TipCheltuiala { get; set; }
        public string DocumentJustificativ { get; set; }
        public decimal Valoare { get; set; }
        public string Moneda { get; set; } = "RON";
    }
}