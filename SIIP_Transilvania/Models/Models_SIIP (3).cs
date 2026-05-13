// ═══════════════════════════════════════════════════════════════════════════
// SIIP — SC Transilvania General Import-Export SRL
// Capitolul 3 — Modelul Fizic — Clase Entitate C#
// Echipa: Crenganiș, Iosub, Lazăr, Podina
// ═══════════════════════════════════════════════════════════════════════════

using System;
using System.Collections.Generic;

namespace SIIP_Transilvania.Models
{
    // ═══════════════════════════════════════════════════════════════════════
    // SUPERCLASA ABSTRACTA — echivalent AbstractEntity din modelul Java/JPA
    // ═══════════════════════════════════════════════════════════════════════
    public abstract class AbstractEntity
    {
        // Cheie primara generata automat (IDENTITY in SQL Server)
        public int Id { get; set; }

        // 1) OPTIMISTIC LOCKING — echivalent @Version din JPA/Hibernate
        // Previne conflictele in aplicatii client-server:
        // la fiecare UPDATE, sistemul verifica ca versiunea din BD
        // este aceeasi cu cea citita initial. Daca nu, alt utilizator
        // a modificat intre timp — operatiunea este respinsa.
        public byte[] RowVersion { get; set; }

        // Audit
        public string CreatedByUser { get; set; }
        public string UpdatedByUser { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;

        // 2) EQUALS si GETHASHCODE — echivalent Java equals()/hashCode()
        // Obligatoriu pentru functionarea corecta in colectii (List, HashSet).
        // Implementarea se bazeaza pe ID, ca in AbstractEntity Java din ghid.
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            var other = (AbstractEntity)obj;
            // Daca ID = 0, obiectul nu a fost inca salvat in BD
            // -> folosim egalitatea by reference (comportament implicit)
            if (Id == 0 || other.Id == 0)
                return ReferenceEquals(this, obj);
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            // Daca ID = 0 (nesalvat), folosim hash-ul implicit al obiectului
            return Id == 0 ? base.GetHashCode() : Id.GetHashCode();
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CLIENT
    // ═══════════════════════════════════════════════════════════════════════
    public class Client : AbstractEntity
    {
        public int CodClient { get; set; }
        public string Nume { get; set; }
        public string Adresa { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public decimal SoldClient { get; set; } = 0;

        private readonly List<FacturaClient> _facturiClient = new List<FacturaClient>();
        private readonly List<FacturaRetur> _facturiRetur = new List<FacturaRetur>();

        public void AddFacturaClient(FacturaClient f) { _facturiClient.Add(f); f.Client = this; }
        public void RemoveFacturaClient(FacturaClient f) { _facturiClient.Remove(f); f.Client = null; }
        public IReadOnlyList<FacturaClient> GetFacturiClient() => _facturiClient.AsReadOnly();

        public void AddFacturaRetur(FacturaRetur f) { _facturiRetur.Add(f); f.Client = this; }
        public IReadOnlyList<FacturaRetur> GetFacturiRetur() => _facturiRetur.AsReadOnly();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // FURNIZORI
    // ═══════════════════════════════════════════════════════════════════════
    public class Furnizori : AbstractEntity
    {
        public int CodFurnizor { get; set; }
        public string NumeFurnizor { get; set; }
        public string Adresa { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }
        public decimal SoldFurnizor { get; set; } = 0;
        public string IBAN { get; set; }

        private readonly List<FacturaFurnizor> _facturiFurnizor = new List<FacturaFurnizor>();
        private readonly List<FacturaRetur> _facturiRetur = new List<FacturaRetur>();

        public void AddFacturaFurnizor(FacturaFurnizor f) { _facturiFurnizor.Add(f); f.Furnizor = this; }
        public void RemoveFacturaFurnizor(FacturaFurnizor f) { _facturiFurnizor.Remove(f); f.Furnizor = null; }
        public IReadOnlyList<FacturaFurnizor> GetFacturiFurnizor() => _facturiFurnizor.AsReadOnly();

        public void AddFacturaRetur(FacturaRetur f) { _facturiRetur.Add(f); f.Furnizor = this; }
        public IReadOnlyList<FacturaRetur> GetFacturiRetur() => _facturiRetur.AsReadOnly();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // ANGAJAT — ierarhie cu strategia Single Table (TPH)
    // Discriminant: Functie = "Sofer" | "AngajatRH" | "DirectorFinanciar"
    // O singura tabela Angajat in BD cu toate atributele subclaselor.
    // Atributele specifice subclaselor au valori NULL pentru celelalte.
    // ═══════════════════════════════════════════════════════════════════════
    public abstract class Angajat : AbstractEntity
    {
        public int IdAngajat { get; set; }
        public string Functie { get; set; }   // Discriminant TPH
        public string Nume { get; set; }
        public string Prenume { get; set; }
        public string CNP { get; set; }
        public DateTime DataNastere { get; set; }
        public DateTime DataAngajare { get; set; }
    }

    public class Sofer : Angajat
    {
        // Atribute specifice — NULL in BD pentru AngajatRH si DirectorFinanciar
        public string NrPermis { get; set; }
        public string Categorie { get; set; }
        public Sofer() { Functie = "Sofer"; }

        private readonly List<FluturasaSalariu> _fluturasi = new List<FluturasaSalariu>();
        public void AddFluturas(FluturasaSalariu f) { _fluturasi.Add(f); f.Angajat = this; }
        public IReadOnlyList<FluturasaSalariu> GetFluturasi() => _fluturasi.AsReadOnly();
    }

    public class AngajatRH : Angajat
    {
        public string FunctieRH { get; set; }
        public AngajatRH() { Functie = "AngajatRH"; }

        private readonly List<StatDePlata> _state = new List<StatDePlata>();
        public void AddStatDePlata(StatDePlata s) { _state.Add(s); s.AngajatRH = this; }
        public IReadOnlyList<StatDePlata> GetStateDePlata() => _state.AsReadOnly();
    }

    public class DirectorFinanciar : Angajat
    {
        public string Nivel { get; set; }
        public DirectorFinanciar() { Functie = "DirectorFinanciar"; }

        private readonly List<Decont> _deconturi = new List<Decont>();
        public void AddDecont(Decont d) { _deconturi.Add(d); d.Director = this; }
        public IReadOnlyList<Decont> GetDeconturi() => _deconturi.AsReadOnly();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // DOCUMENT — superclasa abstracta pentru ierarhia de documente
    //
    // 3) STRATEGIA DE MOSTENIRE: Table Per Type (TPT) = Joined in JPA
    // Fiecare subclasa are propria tabela in BD, fara tabela Document.
    // Atributele comune (serie, numar, dataDocument, etc.) sunt replicate
    // in fiecare tabela-subclasa — identic cu strategia JOINED din JPA.
    //
    // Subclase: FacturaClient, FacturaFurnizor, FacturaRetur, Decont, StatDePlata
    // ═══════════════════════════════════════════════════════════════════════
    public abstract class Document
    {
        // PK compus (practica fiscala romaneasca)
        public string Serie { get; set; }
        public string Numar { get; set; }

        // Atribute comune — replicate in fiecare tabela-subclasa (TPT/Joined)
        public DateTime DataDocument { get; set; }
        public DateTime DataOperare { get; set; } = DateTime.Now;
        public decimal ValoareTotala { get; set; }
        public string Stare { get; set; }

        // Equals si GetHashCode bazate pe PK compus (Serie + Numar)
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            if (ReferenceEquals(this, obj)) return true;
            var other = (Document)obj;
            return Serie == other.Serie && Numar == other.Numar;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (Serie?.GetHashCode() ?? 0);
                hash = hash * 23 + (Numar?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // FACTURA CLIENT
    // ═══════════════════════════════════════════════════════════════════════
    public class FacturaClient : Document
    {
        public DateTime Scadenta { get; set; }
        public decimal TVA { get; set; }
        public string StareIncasare { get; set; } = "Neincasat";
        public int CodClient { get; set; }
        public Client Client { get; set; }
        // Calculat din BD la interogare — nu stocat in tabela
        public decimal RestDisponibil { get; set; }

        private readonly List<Incasare> _incasari = new List<Incasare>();
        public void AddIncasare(Incasare i) { _incasari.Add(i); i.FacturaClient = this; }
        public void RemoveIncasare(Incasare i) { _incasari.Remove(i); i.FacturaClient = null; }
        public IReadOnlyList<Incasare> GetIncasari() => _incasari.AsReadOnly();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // FACTURA FURNIZOR
    // ═══════════════════════════════════════════════════════════════════════
    public class FacturaFurnizor : Document
    {
        public DateTime Scadenta { get; set; }
        public decimal TVA { get; set; }
        public int CodFurnizor { get; set; }
        public Furnizori Furnizor { get; set; }

        private readonly List<PlataEsalonata> _plate = new List<PlataEsalonata>();
        public void AddPlataEsalonata(PlataEsalonata pe) { _plate.Add(pe); pe.FacturaFurnizor = this; }
        public void RemovePlataEsalonata(PlataEsalonata pe) { _plate.Remove(pe); pe.FacturaFurnizor = null; }
        public IReadOnlyList<PlataEsalonata> GetPlateEsalonate() => _plate.AsReadOnly();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // FACTURA RETUR
    // codClient sau codFurnizor — unul NULL in functie de tipRetur
    // ═══════════════════════════════════════════════════════════════════════
    public class FacturaRetur : Document
    {
        public string MotivRetur { get; set; }
        public decimal ValoareRetur { get; set; }
        public string StareRetur { get; set; } = "Emis";
        public string TipRetur { get; set; }  // "Client" sau "Furnizor"

        public int? CodClient { get; set; }
        public Client Client { get; set; }

        public int? CodFurnizor { get; set; }
        public Furnizori Furnizor { get; set; }

        public string SerieFactInit { get; set; }
        public string NumarFactInit { get; set; }

        private readonly List<PlataEsalonata> _plate = new List<PlataEsalonata>();
        public void AddPlataEsalonata(PlataEsalonata pe) { _plate.Add(pe); pe.FacturaRetur = this; }
        public IReadOnlyList<PlataEsalonata> GetPlateEsalonate() => _plate.AsReadOnly();

        public ChitantaRetur ChitantaRetur { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // DECONT
    // ═══════════════════════════════════════════════════════════════════════
    public class Decont : Document
    {
        public DateTime PerioadaStart { get; set; }
        public DateTime PerioadaEnd { get; set; }
        public decimal ValoareDecontata { get; set; }
        public int CodDirector { get; set; }
        public DirectorFinanciar Director { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // STAT DE PLATA
    // ═══════════════════════════════════════════════════════════════════════
    public class StatDePlata : Document
    {
        public int Luna { get; set; }
        public int An { get; set; }
        public decimal TotalBrut { get; set; }
        public decimal TotalNet { get; set; }
        public int IdAngajatRH { get; set; }
        public AngajatRH AngajatRH { get; set; }

        private readonly List<FluturasaSalariu> _fluturasi = new List<FluturasaSalariu>();
        public void AddFluturas(FluturasaSalariu f) { _fluturasi.Add(f); f.StatDePlata = this; }
        public void RemoveFluturas(FluturasaSalariu f) { _fluturasi.Remove(f); f.StatDePlata = null; }
        public IReadOnlyList<FluturasaSalariu> GetFluturasi() => _fluturasi.AsReadOnly();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // INCASARE
    // ═══════════════════════════════════════════════════════════════════════
    public class Incasare : AbstractEntity
    {
        public int IdIncasare { get; set; }
        public DateTime DataIncasare { get; set; }
        public decimal SumaIncasata { get; set; }
        public string Canal { get; set; }  // "Numerar" sau "ContBancar"

        public string SerieFact { get; set; }
        public string NumarFact { get; set; }
        public FacturaClient FacturaClient { get; set; }

        public BonFiscal BonFiscal { get; set; }
        public ExtrasContIncasare ExtrasContIncasare { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // PLATA
    // ═══════════════════════════════════════════════════════════════════════
    public class Plata : AbstractEntity
    {
        public int IdPlata { get; set; }
        public DateTime DataPlata { get; set; }
        public decimal Suma { get; set; }
        public string TipPlata { get; set; }  // "PlataFurnizor"|"ReturClient"|"Salarii"|"Decont"
        public string Canal { get; set; }      // "Numerar" sau "ContBancar"
        public string Stare { get; set; } = "Procesat";

        private readonly List<PlataEsalonata> _plate = new List<PlataEsalonata>();
        public void AddPlataEsalonata(PlataEsalonata pe) { _plate.Add(pe); pe.Plata = this; }
        public void RemovePlataEsalonata(PlataEsalonata pe) { _plate.Remove(pe); pe.Plata = null; }
        public IReadOnlyList<PlataEsalonata> GetPlateEsalonate() => _plate.AsReadOnly();

        public ExtrasContPlata ExtrasContPlata { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // PLATA ESALONATA — clasa de asociere Plata <-> FacturaFurnizor/Retur
    // ═══════════════════════════════════════════════════════════════════════
    public class PlataEsalonata : AbstractEntity
    {
        public int IdPlataEsalonata { get; set; }
        public string TipRata { get; set; }         // "Avans"|"Diferenta"|"Integral"
        public decimal ProcentAcoperit { get; set; }
        public DateTime DataScadenta { get; set; }

        public int IdPlata { get; set; }
        public Plata Plata { get; set; }

        // FK nullable -> FacturaFurnizor
        public string SerieFurnizor { get; set; }
        public string NumarFurnizor { get; set; }
        public FacturaFurnizor FacturaFurnizor { get; set; }

        // FK nullable -> FacturaRetur
        public string SerieRetur { get; set; }
        public string NumarRetur { get; set; }
        public FacturaRetur FacturaRetur { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CONT BANCAR
    // ═══════════════════════════════════════════════════════════════════════
    public class ContBancar : AbstractEntity
    {
        public string IBAN { get; set; }  // PK unic
        public decimal Sold { get; set; }
        public string Banca { get; set; }
        public string Titular { get; set; }

        private readonly List<ExtrasContIncasare> _extraseInc = new List<ExtrasContIncasare>();
        private readonly List<ExtrasContPlata> _extraseP = new List<ExtrasContPlata>();

        public void AddExtrasIncasare(ExtrasContIncasare e) { _extraseInc.Add(e); e.ContBancar = this; }
        public IReadOnlyList<ExtrasContIncasare> GetExtraseIncasare() => _extraseInc.AsReadOnly();
        public void AddExtrasPlata(ExtrasContPlata e) { _extraseP.Add(e); e.ContBancar = this; }
        public IReadOnlyList<ExtrasContPlata> GetExtrasePlata() => _extraseP.AsReadOnly();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CASERIE
    // ═══════════════════════════════════════════════════════════════════════
    public class Caserie : AbstractEntity
    {
        public int IdCaserie { get; set; }
        public decimal SoldNumerar { get; set; }
        public string Responsabil { get; set; }
        public string Locatie { get; set; }

        private readonly List<BonFiscal> _bonuri = new List<BonFiscal>();
        public void AddBonFiscal(BonFiscal b) { _bonuri.Add(b); b.Caserie = this; }
        public IReadOnlyList<BonFiscal> GetBonuri() => _bonuri.AsReadOnly();
    }

    // ═══════════════════════════════════════════════════════════════════════
    // BON FISCAL
    // ═══════════════════════════════════════════════════════════════════════
    public class BonFiscal : AbstractEntity
    {
        public int NumarBon { get; set; }
        public DateTime DataEmitere { get; set; }
        public decimal TotalValoare { get; set; }
        public int IdCaserie { get; set; }
        public Caserie Caserie { get; set; }
        public int IdIncasare { get; set; }
        public Incasare Incasare { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // CHITANTA RETUR
    // ═══════════════════════════════════════════════════════════════════════
    public class ChitantaRetur : AbstractEntity
    {
        public int IdChitanta { get; set; }
        public DateTime DataEmitere { get; set; }
        public decimal SumaRestituita { get; set; }
        public string TipRetur { get; set; }
        public string SerieRetur { get; set; }
        public string NumarRetur { get; set; }
        public FacturaRetur FacturaRetur { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // FLUTURASA SALARIU
    // ═══════════════════════════════════════════════════════════════════════
    public class FluturasaSalariu : AbstractEntity
    {
        public int IdFluturas { get; set; }
        public int Luna { get; set; }
        public int An { get; set; }
        public decimal SalariuNet { get; set; }
        public decimal Retineri { get; set; }
        public int IdAngajat { get; set; }
        public Sofer Angajat { get; set; }
        public string SerieStat { get; set; }
        public string NumarStat { get; set; }
        public StatDePlata StatDePlata { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // EXTRAS CONT INCASARE
    // ═══════════════════════════════════════════════════════════════════════
    public class ExtrasContIncasare : AbstractEntity
    {
        public int NumarExtras { get; set; }
        public DateTime DataEmitere { get; set; }
        public decimal SumaIncasata { get; set; }
        public string IBAN { get; set; }
        public ContBancar ContBancar { get; set; }
        public int IdIncasare { get; set; }
        public Incasare Incasare { get; set; }
    }

    // ═══════════════════════════════════════════════════════════════════════
    // EXTRAS CONT PLATA
    // ═══════════════════════════════════════════════════════════════════════
    public class ExtrasContPlata : AbstractEntity
    {
        public int NumarExtras { get; set; }
        public DateTime DataEmitere { get; set; }
        public decimal SumaPlata { get; set; }
        public string IBAN { get; set; }
        public ContBancar ContBancar { get; set; }
        public int IdPlata { get; set; }
        public Plata Plata { get; set; }
    }
}