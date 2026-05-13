using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Database
{
    // ═══════════════════════════════════════════════════════════════════════
    // AngajatRepository — operatii CRUD pentru entitatea Angajat
    // ═══════════════════════════════════════════════════════════════════════
    public class AngajatRepository : AbstractRepository
    {
        public List<Angajat> FindAll()
        {
            var list = new List<Angajat>();
            var dt = ExecuteQuery("SELECT idAngajat, functie, nume, prenume, CNP, dataNastere, dataAngajare FROM Angajat ORDER BY nume");
            foreach (DataRow row in dt.Rows)
                list.Add(MapAngajat(row));
            return list;
        }

        public List<Angajat> FindByFunctie(string functie)
        {
            var list = new List<Angajat>();
            var dt = ExecuteQuery(
                "SELECT idAngajat, functie, nume, prenume, CNP, dataNastere, dataAngajare FROM Angajat WHERE functie=@f ORDER BY nume",
                new[] { new SqlParameter("@f", functie) });
            foreach (DataRow row in dt.Rows)
                list.Add(MapAngajat(row));
            return list;
        }

        public List<Angajat> FindDirectori() => FindByFunctie("DirectorFinanciar");
        public List<Angajat> FindSoferi()    => FindByFunctie("Sofer");
        public List<Angajat> FindAngajatiRH() => FindByFunctie("AngajatRH");

        private Angajat MapAngajat(DataRow row)
        {
            string functie = row["functie"].ToString();
            Angajat a;
            switch (functie)
            {
                case "Sofer":             a = new Sofer();             break;
                case "AngajatRH":         a = new AngajatRH();         break;
                case "DirectorFinanciar": a = new DirectorFinanciar(); break;
                default:                  a = new Sofer();             break;
            }
            a.IdAngajat    = Convert.ToInt32(row["idAngajat"]);
            a.Functie      = functie;
            a.Nume         = row["nume"].ToString();
            a.Prenume      = row["prenume"].ToString();
            a.CNP          = row["CNP"] == DBNull.Value ? null : row["CNP"].ToString();
            a.DataNastere  = row["dataNastere"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["dataNastere"]);
            a.DataAngajare = row["dataAngajare"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["dataAngajare"]);
            return a;
        }
    }
}
