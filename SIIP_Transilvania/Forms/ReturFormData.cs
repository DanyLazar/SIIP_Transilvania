using System;
using System.Collections.Generic;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    // ═══════════════════════════════════════════════════════════════════════
    // ReturFormData — ModelAdapter pentru formularul Inregistrare Retur
    // Echivalent cu AchizitiiFormData din modelul Java/JPA (ghid PSI Partea 4)
    // Pastreaza datele pentru instanta curenta a formularului.
    // ═══════════════════════════════════════════════════════════════════════
    public class ReturFormData
    {
        // ── ZONA 0 — Comuna tuturor formularelor ─────────────────────────
        // Obiectul-tinta al formularului (documentul curent in editare)
        private FacturaRetur _documentCurent;

        // Referinte catre Repository-uri — clientii nu stiu cum se acceseaza BD
        private readonly DocumentRepository _docRepo = new DocumentRepository();
        private readonly ClientRepository _clientRepo = new ClientRepository();
        private readonly FurnizoriRepository _furnizoriRepo = new FurnizoriRepository();
        private readonly ContBancarRepository _contBancarRepo = new ContBancarRepository();

        public DocumentRepository GetDocRepo() => _docRepo;
        public ClientRepository GetClientRepo() => _clientRepo;
        public FurnizoriRepository GetFurnizoriRepo() => _furnizoriRepo;
        public ContBancarRepository GetContBancarRepo() => _contBancarRepo;

        public FacturaRetur GetDocumentCurent() => _documentCurent;
        public void SetDocumentCurent(FacturaRetur retur) { _documentCurent = retur; }

        // ── ZONA 1 — Tip Retur ───────────────────────────────────────────
        // "Client" sau "Furnizor" — determina ce partener si ce FK se completeaza
        private string _tipRetur = "Client";
        public string GetTipRetur() => _tipRetur;
        public void SetTipRetur(string tip) { _tipRetur = tip; }

        // ── ZONA 2 — Parteneri (Clienti sau Furnizori) ───────────────────
        // Cache — lista se incarca o singura data la prima invocare (optimizare BD)
        private List<Client> _listaClienti;
        private List<Furnizori> _listaFurnizori;

        public List<Client> GetListaClienti()
        {
            if (_listaClienti == null)
                _listaClienti = _clientRepo.FindAll();
            return _listaClienti;
        }

        public List<Furnizori> GetListaFurnizori()
        {
            if (_listaFurnizori == null)
                _listaFurnizori = _furnizoriRepo.FindAll();
            return _listaFurnizori;
        }

        // Partenerul selectat curent
        private int _codPartenerSelectat;
        private string _numePartenerSelectat;

        public int GetCodPartenerSelectat() => _codPartenerSelectat;
        public string GetNumePartenerSelectat() => _numePartenerSelectat;

        public void SetPartenerSelectat(int cod, string nume)
        {
            _codPartenerSelectat = cod;
            _numePartenerSelectat = nume;
            // Reseteaza cache-ul facturilor la schimbarea partenerului
            _listaFacturi = null;
        }

        // ── ZONA 3 — Facturi ale partenerului selectat ───────────────────
        // Cache — se incarca la schimbarea partenerului
        private List<FacturaClient> _listaFacturiClient;
        private List<FacturaFurnizor> _listaFacturiFurnizor;
        private object _listaFacturi; // referinta generica pentru reset

        public List<FacturaClient> GetFacturiClient()
        {
            if (_listaFacturiClient == null && _codPartenerSelectat > 0)
                _listaFacturiClient = _docRepo.FindFacturiClientByCod(_codPartenerSelectat);
            return (_listaFacturiClient ?? new List<FacturaClient>()).FindAll(fc => fc.RestDisponibil > 0);
        }

        public List<FacturaFurnizor> GetFacturiFurnizor()
        {
            if (_listaFacturiFurnizor == null && _codPartenerSelectat > 0)
                _listaFacturiFurnizor = _docRepo.FindFacturiFurnizorByCod(_codPartenerSelectat);
            return (_listaFacturiFurnizor ?? new List<FacturaFurnizor>()).FindAll(ff => ff.Stare != "Achitat");
        }

        // Factura initiala selectata curent
        private string _serieFactInit;
        private string _numarFactInit;
        private decimal _valoareTotalaFactInit;
        private decimal _restDisponibil;
        private DateTime _dataFactInit;

        public string GetSerieFactInit() => _serieFactInit;
        public string GetNumarFactInit() => _numarFactInit;
        public decimal GetValoareTotalaFactInit() => _valoareTotalaFactInit;
        public decimal GetRestDisponibil() => _restDisponibil;
        public DateTime GetDataFactInit() => _dataFactInit;

        public void SetFacturaInitSelectata(string serie, string numar, DateTime data, decimal valTotala)
        {
            _serieFactInit = serie;
            _numarFactInit = numar;
            _dataFactInit = data;
            _valoareTotalaFactInit = valTotala;
            // Calculeaza restul disponibil din BD
            decimal sumaReturnata = _docRepo.GetSumaReturnata(serie, numar);
            _restDisponibil = valTotala - sumaReturnata;
        }

        // ── ZONA 4 — Retururi anterioare ale partenerului ────────────────
        private List<FacturaRetur> _listaRetururi;

        public List<FacturaRetur> GetRetururi()
        {
            if (_codPartenerSelectat > 0)
                _listaRetururi = _docRepo.FindRetururiByPartener(_codPartenerSelectat, _tipRetur);
            return _listaRetururi ?? new List<FacturaRetur>();
        }

        // Forteaza reincarcarea retururilor din BD (dupa salvare/anulare)
        public void RefreshRetururi()
        {
            _listaRetururi = null;
        }

        // ── ZONA 5 — Totaluri luna curenta ───────────────────────────────
        private decimal _totalLuna;
        private int _nrRetururi;

        public decimal GetTotalLuna() => _totalLuna;
        public int GetNrRetururi() => _nrRetururi;

        public void RefreshTotaluri()
        {
            var (total, nr) = _docRepo.GetTotaluriLunaRetur();
            _totalLuna = total;
            _nrRetururi = nr;
        }

        // ── ZONA 6 — Date document curent (in editare) ───────────────────
        private string _serie = "RET";
        private string _numar;
        private DateTime _data = DateTime.Now;
        private string _motiv = "Marfa deteriorata";
        private string _stare = "Emis";
        private decimal _valRetur;
        private string _canal = "Numerar";

        public string GetSerie() => _serie;
        public string GetNumar() => _numar;
        public DateTime GetData() => _data;
        public string GetMotiv() => _motiv;
        public string GetStare() => _stare;
        public decimal GetValRetur() => _valRetur;
        public string GetCanal() => _canal;

        public void SetSerie(string v) { _serie = v; }
        public void SetNumar(string v) { _numar = v; }
        public void SetData(DateTime v) { _data = v; }
        public void SetMotiv(string v) { _motiv = v; }
        public void SetStare(string v) { _stare = v; }
        public void SetValRetur(decimal v) { _valRetur = v; }
        public void SetCanal(string v) { _canal = v; }

        // Genereaza numarul urmator disponibil pentru FacturaRetur
        public void GenerateNumarRetur()
        {
            _numar = _docRepo.GetNextNumarRetur();
        }

        // Reseteaza datele documentului curent (dupa salvare sau renuntare)
        public void ResetDocumentCurent()
        {
            _documentCurent = null;
            _serieFactInit = null;
            _numarFactInit = null;
            _valoareTotalaFactInit = 0;
            _restDisponibil = 0;
            _valRetur = 0;
            _motiv = "Marfa deteriorata";
            _stare = "Emis";
            _canal = "Numerar";
            _listaFacturiClient = null;
            _listaFacturiFurnizor = null;
        }
    }
}