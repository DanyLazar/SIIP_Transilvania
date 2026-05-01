using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Database
{
    // ═══════════════════════════════════════════════════════════════════════
    // MasterRepository — operatii CRUD pentru entitatile-nomenclator:
    // Client, Furnizori, Angajat, ContBancar, Caserie
    // Echivalent cu MasterRepository din modelul Java/JPA
    // ═══════════════════════════════════════════════════════════════════════
    public class MasterRepository : AbstractRepository
    {
        // ── CLIENT ──────────────────────────────────────────────────────

        public List<Client> FindClientiAll()
        {
            var list = new List<Client>();
            var dt = ExecuteQuery("SELECT codClient, nume, adresa, telefon, email, soldClient FROM Client ORDER BY nume");
            foreach (DataRow row in dt.Rows)
                list.Add(MapClient(row));
            return list;
        }

        public Client FindClientById(int codClient)
        {
            var dt = ExecuteQuery(
                "SELECT codClient, nume, adresa, telefon, email, soldClient FROM Client WHERE codClient=@id",
                new[] { new SqlParameter("@id", codClient) });
            return dt.Rows.Count > 0 ? MapClient(dt.Rows[0]) : null;
        }

        public Client AddClient(Client client)
        {
            ExecuteNonQuery(
                @"INSERT INTO Client (nume, adresa, telefon, email, soldClient)
                  VALUES (@nume, @adresa, @telefon, @email, @sold)",
                new[]
                {
                    new SqlParameter("@nume",    client.Nume),
                    new SqlParameter("@adresa",  client.Adresa ?? (object)DBNull.Value),
                    new SqlParameter("@telefon", client.Telefon ?? (object)DBNull.Value),
                    new SqlParameter("@email",   client.Email ?? (object)DBNull.Value),
                    new SqlParameter("@sold",    client.SoldClient)
                });
            return client;
        }

        public void UpdateSoldClient(int codClient, decimal sumaDelta)
        {
            ExecuteNonQuery(
                "UPDATE Client SET soldClient = soldClient + @delta WHERE codClient = @id",
                new[]
                {
                    new SqlParameter("@delta", sumaDelta),
                    new SqlParameter("@id",    codClient)
                });
        }

        private Client MapClient(DataRow row) => new Client
        {
            CodClient = Convert.ToInt32(row["codClient"]),
            Nume = row["nume"].ToString(),
            Adresa = row["adresa"] == DBNull.Value ? null : row["adresa"].ToString(),
            Telefon = row["telefon"] == DBNull.Value ? null : row["telefon"].ToString(),
            Email = row["email"] == DBNull.Value ? null : row["email"].ToString(),
            SoldClient = Convert.ToDecimal(row["soldClient"])
        };

        // ── FURNIZORI ───────────────────────────────────────────────────

        public List<Furnizori> FindFurnizoriAll()
        {
            var list = new List<Furnizori>();
            var dt = ExecuteQuery("SELECT codFurnizor, numeFurnizor, adresa, telefon, email, soldFurnizor, IBAN FROM Furnizori ORDER BY numeFurnizor");
            foreach (DataRow row in dt.Rows)
                list.Add(MapFurnizor(row));
            return list;
        }

        public Furnizori FindFurnizorById(int codFurnizor)
        {
            var dt = ExecuteQuery(
                "SELECT codFurnizor, numeFurnizor, adresa, telefon, email, soldFurnizor, IBAN FROM Furnizori WHERE codFurnizor=@id",
                new[] { new SqlParameter("@id", codFurnizor) });
            return dt.Rows.Count > 0 ? MapFurnizor(dt.Rows[0]) : null;
        }

        public void UpdateSoldFurnizor(int codFurnizor, decimal sumaDelta)
        {
            ExecuteNonQuery(
                "UPDATE Furnizori SET soldFurnizor = soldFurnizor + @delta WHERE codFurnizor = @id",
                new[]
                {
                    new SqlParameter("@delta", sumaDelta),
                    new SqlParameter("@id",    codFurnizor)
                });
        }

        private Furnizori MapFurnizor(DataRow row) => new Furnizori
        {
            CodFurnizor = Convert.ToInt32(row["codFurnizor"]),
            NumeFurnizor = row["numeFurnizor"].ToString(),
            Adresa = row["adresa"] == DBNull.Value ? null : row["adresa"].ToString(),
            Telefon = row["telefon"] == DBNull.Value ? null : row["telefon"].ToString(),
            Email = row["email"] == DBNull.Value ? null : row["email"].ToString(),
            SoldFurnizor = Convert.ToDecimal(row["soldFurnizor"]),
            IBAN = row["IBAN"] == DBNull.Value ? null : row["IBAN"].ToString()
        };

        // ── CONTBANCAR ──────────────────────────────────────────────────

        public List<ContBancar> FindConturiBancareAll()
        {
            var list = new List<ContBancar>();
            var dt = ExecuteQuery("SELECT IBAN, sold, banca, titular FROM ContBancar");
            foreach (DataRow row in dt.Rows)
                list.Add(new ContBancar
                {
                    IBAN = row["IBAN"].ToString(),
                    Sold = Convert.ToDecimal(row["sold"]),
                    Banca = row["banca"].ToString(),
                    Titular = row["titular"].ToString()
                });
            return list;
        }

        // ── CASERIE ─────────────────────────────────────────────────────

        public List<Caserie> FindCaseriiAll()
        {
            var list = new List<Caserie>();
            var dt = ExecuteQuery("SELECT idCaserie, soldNumerar, responsabil, locatie FROM Caserie");
            foreach (DataRow row in dt.Rows)
                list.Add(new Caserie
                {
                    IdCaserie = Convert.ToInt32(row["idCaserie"]),
                    SoldNumerar = Convert.ToDecimal(row["soldNumerar"]),
                    Responsabil = row["responsabil"].ToString(),
                    Locatie = row["locatie"].ToString()
                });
            return list;
        }

        public void UpdateSoldCaserie(int idCaserie, decimal sumaDelta)
        {
            ExecuteNonQuery(
                "UPDATE Caserie SET soldNumerar = soldNumerar + @delta WHERE idCaserie = @id",
                new[]
                {
                    new SqlParameter("@delta", sumaDelta),
                    new SqlParameter("@id",    idCaserie)
                });
        }
    }
}