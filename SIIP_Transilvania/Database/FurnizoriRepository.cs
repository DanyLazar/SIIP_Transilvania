using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Database
{
    // ═══════════════════════════════════════════════════════════════════════
    // FurnizoriRepository — operatii CRUD pentru entitatea Furnizori
    // ═══════════════════════════════════════════════════════════════════════
    public class FurnizoriRepository : AbstractRepository
    {
        public List<Furnizori> FindAll()
        {
            var list = new List<Furnizori>();
            var dt = ExecuteQuery("SELECT codFurnizor, numeFurnizor, adresa, telefon, email, soldFurnizor, IBAN FROM Furnizori ORDER BY numeFurnizor");
            foreach (DataRow row in dt.Rows)
                list.Add(MapFurnizor(row));
            return list;
        }

        public Furnizori FindById(int codFurnizor)
        {
            var dt = ExecuteQuery(
                "SELECT codFurnizor, numeFurnizor, adresa, telefon, email, soldFurnizor, IBAN FROM Furnizori WHERE codFurnizor=@id",
                new[] { new SqlParameter("@id", codFurnizor) });
            return dt.Rows.Count > 0 ? MapFurnizor(dt.Rows[0]) : null;
        }

        public void UpdateSold(int codFurnizor, decimal sumaDelta)
        {
            ExecuteNonQuery(
                "UPDATE Furnizori SET soldFurnizor=soldFurnizor+@delta WHERE codFurnizor=@id",
                new[] { new SqlParameter("@delta", sumaDelta), new SqlParameter("@id", codFurnizor) });
        }

        private Furnizori MapFurnizor(DataRow row) => new Furnizori
        {
            CodFurnizor   = Convert.ToInt32(row["codFurnizor"]),
            NumeFurnizor  = row["numeFurnizor"].ToString(),
            Adresa        = row["adresa"] == DBNull.Value ? null : row["adresa"].ToString(),
            Telefon       = row["telefon"] == DBNull.Value ? null : row["telefon"].ToString(),
            Email         = row["email"] == DBNull.Value ? null : row["email"].ToString(),
            SoldFurnizor  = Convert.ToDecimal(row["soldFurnizor"]),
            IBAN          = row["IBAN"] == DBNull.Value ? null : row["IBAN"].ToString()
        };
    }
}
