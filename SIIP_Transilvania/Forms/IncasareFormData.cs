using System;
using System.Collections.Generic;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    // ═══════════════════════════════════════════════════════════════════════
    // IncasareFormData — ModelAdapter pentru formularul Inregistrare Incasare Client
    // Echivalent cu ReturFormData / AchizitiiFormData din modelul Java/JPA (ghid PSI Partea 4)
    // Pastreaza datele pentru instanta curenta a formularului.
    // ═══════════════════════════════════════════════════════════════════════
    public class IncasareFormData
    {
        // ── ZONA 0 — Comuna tuturor formularelor ─────────────────────────
        // Obiectul-tinta al formularului (documentul curent in editare)
        private Incasare _documentCurent;

        // Referinte catre Repository-uri
        private readonly DocumentRepository _docRepo = new DocumentRepository();
        private readonly MasterRepository _masterRepo = new MasterRepository();

        public DocumentRepository GetDocRepo() => _docRepo;
        public MasterRepository GetMasterRepo() => _masterRepo;

        public Incasare GetDocumentCurent() => _documentCurent;
        public void SetDocumentCurent(Incasare incasare) { _documentCurent = incasare; }

        // ── ZONA 1 — Clienti ─────────────────────────────────────────────
        // Cache — lista se incarca o singura data la prima invocare (optimizare BD)
        private List<Client> _listaClienti;

        public List<Client> GetListaClienti()
        {
            if (_listaClienti == null)
                _listaClienti = _masterRepo.FindClientiAll();
            return _listaClienti;
        }

        // Clientul selectat curent
        private int _codClientSelectat;
        private string _numeClientSelectat;

        public int GetCodClientSelectat() => _codClientSelectat;
        public string GetNumeClientSelectat() => _numeClientSelectat;

        public void SetClientSelectat(int cod, string nume)
        {
            _codClientSelectat = cod;
            _numeClientSelectat = nume;
            // Reseteaza cache-ul facturilor la schimbarea clientului
            _listaFacturi = null;
        }

        // ── ZONA 2 — Facturi ale clientului selectat ─────────────────────
        // Cache — se incarca la schimbarea clientului
        private List<FacturaClient> _listaFacturi;

        public List<FacturaClient> GetFacturiClient()
        {
            if (_listaFacturi == null && _codClientSelectat > 0)
                _listaFacturi = _docRepo.FindFacturiClientByCod(_codClientSelectat);
            return _listaFacturi ?? new List<FacturaClient>();
        }

        // Factura selectata curent
        private string _serieFact;
        private string _numarFact;
        private decimal _valoareTotalaFact;
        private decimal _restDePlata;
        private DateTime _dataFact;

        public string GetSerieFact() => _serieFact;
        public string GetNumarFact() => _numarFact;
        public decimal GetValoareTotalaFact() => _valoareTotalaFact;
        public decimal GetRestDePlata() => _restDePlata;
        public DateTime GetDataFact() => _dataFact;

        public void SetFacturaSelectata(string serie, string numar, DateTime data, decimal valTotala)
        {
            _serieFact = serie;
            _numarFact = numar;
            _dataFact = data;
            _valoareTotalaFact = valTotala;
            // Calculeaza restul de plata din BD (ValTotala - suma deja incasata)
            decimal sumaIncasata = _docRepo.GetSumaIncasata(serie, numar);
            _restDePlata = valTotala - sumaIncasata;
        }

        // ── ZONA 3 — Caserii si Conturi Bancare ──────────────────────────
        // Cache — se incarca o singura data
        private List<Caserie> _listaCaserii;
        private List<ContBancar> _listaConturi;

        public List<Caserie> GetListaCaserii()
        {
            if (_listaCaserii == null)
                _listaCaserii = _masterRepo.FindCaseriiAll();
            return _listaCaserii;
        }

        public List<ContBancar> GetListaConturi()
        {
            if (_listaConturi == null)
                _listaConturi = _masterRepo.FindConturiBancareAll();
            return _listaConturi;
        }

        // Caseria / Contul selectat curent
        private int _idCaserieSelectata;
        private string _ibanSelectat;

        public int GetIdCaserieSelectata() => _idCaserieSelectata;
        public string GetIbanSelectat() => _ibanSelectat;
        public void SetCaserieSelectata(int id) { _idCaserieSelectata = id; }
        public void SetIbanSelectat(string iban) { _ibanSelectat = iban; }

        // ── ZONA 4 — Incasari anterioare ale clientului ──────────────────
        private List<Incasare> _listaIncasari;

        public List<Incasare> GetIncasari()
        {
            if (_codClientSelectat > 0)
                _listaIncasari = _docRepo.FindIncasariByClient(_codClientSelectat);
            return _listaIncasari ?? new List<Incasare>();
        }

        // Forteaza reincarcarea incasarilor din BD (dupa salvare)
        public void RefreshIncasari()
        {
            _listaIncasari = null;
        }

        // ── ZONA 5 — Totaluri luna curenta ───────────────────────────────
        private decimal _totalLuna;
        private int _nrIncasari;

        public decimal GetTotalLuna() => _totalLuna;
        public int GetNrIncasari() => _nrIncasari;

        public void RefreshTotaluri()
        {
            var (total, nr) = _docRepo.GetTotaluriLunaIncasari();
            _totalLuna = total;
            _nrIncasari = nr;
        }

        // ── ZONA 6 — Date document curent (in editare) ───────────────────
        private string _canal = "Numerar";
        private int _idIncasare;

        public string GetCanal() => _canal;
        public int GetIdIncasare() => _idIncasare;
        public void SetCanal(string v) { _canal = v; }

        // Genereaza ID-ul urmator disponibil pentru Incasare
        public void GenerateIdIncasare()
        {
            _idIncasare = _docRepo.GetNextIdIncasare();
        }

        // Reseteaza datele documentului curent (dupa salvare sau renuntare)
        public void ResetDocumentCurent()
        {
            _documentCurent = null;
            _serieFact = null;
            _numarFact = null;
            _valoareTotalaFact = 0;
            _restDePlata = 0;
            _canal = "Numerar";
            _listaFacturi = null;
        }
    }
}
