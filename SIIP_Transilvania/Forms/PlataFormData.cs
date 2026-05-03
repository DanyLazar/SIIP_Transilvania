using System;
using System.Collections.Generic;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    public class PlataFormData
    {
        // ── ZONA 0 ─────────────────────────────────────────────────────────
        private Plata _documentCurent;
        private readonly DocumentRepository _docRepo = new DocumentRepository();
        private readonly MasterRepository _masterRepo = new MasterRepository();
        private bool _esteDocumentNou = false;

        // ── ZONA 1 — furnizori ─────────────────────────────────────────────
        private List<Furnizori> _listaFurnizori;

        // ── ZONA 2 — facturi furnizor ──────────────────────────────────────
        private List<FacturaFurnizor> _listaFacturiFurnizor;
        private int _codFurnizorCache = -1;

        // ── ZONA 3 — date factura selectata ───────────────────────────────
        private decimal _restDisponibil = 0m;
        private decimal _valoareTotalaFactura = 0m;
        private DateTime _dataFactura = DateTime.MinValue;

        // ── ZONA 4 — caserii si conturi bancare ────────────────────────────
        private List<Caserie> _listaCaserii;
        private List<ContBancar> _listaConturiBancare;

        // ── ZONA 5 — totaluri luna ─────────────────────────────────────────
        private decimal _totalLuna = 0m;
        private int _numarPlatiLuna = 0;

        // ── ZONA 6 — document generat (dupa salvare) ───────────────────────
        private string _nrDocumentGenerat = "";
        private DateTime _dataEmitereDocument = DateTime.MinValue;
        private string _tipDocumentGenerat = "";

        // ── ZONA 0: getters/setters ────────────────────────────────────────
        public Plata GetDocumentCurent() => _documentCurent;
        public void SetDocumentCurent(Plata p) => _documentCurent = p;
        public bool EsteDocumentNou() => _esteDocumentNou;
        public void SetDocumentNou(bool val) => _esteDocumentNou = val;
        public DocumentRepository GetDocRepo() => _docRepo;
        public MasterRepository GetMasterRepo() => _masterRepo;

        // ── ZONA 1 ─────────────────────────────────────────────────────────
        public List<Furnizori> GetListaFurnizori()
        {
            if (_listaFurnizori == null)
                _listaFurnizori = _masterRepo.FindFurnizoriAll();
            return _listaFurnizori;
        }

        // ── ZONA 2 ─────────────────────────────────────────────────────────
        public List<FacturaFurnizor> GetFacturiFurnizor(int codFurnizor)
        {
            if (_listaFacturiFurnizor == null || _codFurnizorCache != codFurnizor)
            {
                _listaFacturiFurnizor = _docRepo.FindFacturiFurnizorByCod(codFurnizor);
                _codFurnizorCache = codFurnizor;
                ResetDateFactura();
            }
            return _listaFacturiFurnizor;
        }

        public void ResetFacturiFurnizor()
        {
            _listaFacturiFurnizor = null;
            _codFurnizorCache = -1;
            ResetDateFactura();
        }

        private void ResetDateFactura()
        {
            _restDisponibil = 0m;
            _valoareTotalaFactura = 0m;
            _dataFactura = DateTime.MinValue;
        }

        // ── ZONA 3 ─────────────────────────────────────────────────────────
        public void SetRestDisponibil(decimal r) => _restDisponibil = r;
        public decimal GetRestDisponibil() => _restDisponibil;
        public void SetValoareTotalaFactura(decimal v) => _valoareTotalaFactura = v;
        public decimal GetValoareTotalaFactura() => _valoareTotalaFactura;
        public void SetDataFactura(DateTime d) => _dataFactura = d;
        public DateTime GetDataFactura() => _dataFactura;

        public decimal CalculeazaRestDisponibil(string serie, string numar, decimal valTotala)
        {
            decimal achitat = _docRepo.GetSumaAchitata(serie, numar);
            _restDisponibil = Math.Max(0m, valTotala - achitat);
            return _restDisponibil;
        }

        // ── ZONA 4 ─────────────────────────────────────────────────────────
        public List<Caserie> GetListaCaserii()
        {
            if (_listaCaserii == null)
                _listaCaserii = _masterRepo.FindCaseriiAll();
            return _listaCaserii;
        }

        public List<ContBancar> GetListaConturiBancare()
        {
            if (_listaConturiBancare == null)
                _listaConturiBancare = _masterRepo.FindConturiBancareAll();
            return _listaConturiBancare;
        }

        // ── ZONA 5 ─────────────────────────────────────────────────────────
        public void RefreshTotaluriLuna()
        {
            var (total, nr) = _docRepo.GetTotaluriLunaPlati();
            _totalLuna = total;
            _numarPlatiLuna = nr;
        }

        public decimal GetTotalLuna() => _totalLuna;
        public int GetNumarPlatiLuna() => _numarPlatiLuna;

        // ── ZONA 6: document generat ───────────────────────────────────────
        public void SetDocumentGenerat(string tipDoc, string nrDoc, DateTime dataEmitere)
        {
            _tipDocumentGenerat = tipDoc;
            _nrDocumentGenerat = nrDoc;
            _dataEmitereDocument = dataEmitere;
        }

        public string GetTipDocumentGenerat() => _tipDocumentGenerat;
        public string GetNrDocumentGenerat() => _nrDocumentGenerat;
        public DateTime GetDataEmitereDocument() => _dataEmitereDocument;

        public void ResetDocumentGenerat()
        {
            _tipDocumentGenerat = "";
            _nrDocumentGenerat = "";
            _dataEmitereDocument = DateTime.MinValue;
        }
    }
}
