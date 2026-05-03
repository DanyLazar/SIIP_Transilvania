using System;

namespace SIIP_Transilvania.Forms
{
    public class ComboItem
    {
        public string Cod { get; }
        public string Denumire { get; }
        public string Extra { get; }  // camp suplimentar (ex. Functie pentru Angajat)
        public ComboItem(string cod, string den, string extra = "")
        { Cod = cod; Denumire = den; Extra = extra; }
        public override string ToString() => Denumire;
    }

    public class FacturaItem
    {
        public string Serie { get; }
        public string Numar { get; }
        public DateTime DataDocument { get; }
        public decimal ValoareTotala { get; }
        public decimal RestDisponibil { get; }
        public FacturaItem(string serie, string numar, DateTime data, decimal val, decimal rest)
        { Serie = serie; Numar = numar; DataDocument = data; ValoareTotala = val; RestDisponibil = rest; }
        public override string ToString() =>
            $"{Serie}-{Numar}  |  {DataDocument:dd/MM/yyyy}  |  Total: {ValoareTotala:F2} RON  |  Rest: {RestDisponibil:F2} RON";
    }
}