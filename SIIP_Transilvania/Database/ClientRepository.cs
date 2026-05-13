using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Database
{
    // ═══════════════════════════════════════════════════════════════════════
    // ClientRepository — operatii CRUD pentru entitatea Client
    // ═══════════════════════════════════════════════════════════════════════
    public class ClientRepository : AbstractRepository
    {
        public List<Client> FindAll()
        {
            var list = new List<Client>();
            var dt = ExecuteQuery("SELECT codClient, nume, adresa, telefon, email, soldClient FROM Client ORDER BY nume");
            foreach (DataRow row in dt.Rows)
                list.Add(MapClient(row));
            return list;
        }

        public Client FindById(int codClient)
        {
            var dt = ExecuteQuery(
                "SELECT codClient, nume, adresa, telefon, email, soldClient FROM Client WHERE codClient=@id",
                new[] { new SqlParameter("@id", codClient) });
            return dt.Rows.Count > 0 ? MapClient(dt.Rows[0]) : null;
        }

        public Client Add(Client client)
        {
            ExecuteNonQuery(
                "INSERT INTO Client (nume, adresa, telefon, email, soldClient) VALUES (@n,@a,@t,@e,@s)",
                new[] {
                    new SqlParameter("@n", client.Nume),
                    new SqlParameter("@a", client.Adresa ?? (object)DBNull.Value),
                    new SqlParameter("@t", client.Telefon ?? (object)DBNull.Value),
                    new SqlParameter("@e", client.Email ?? (object)DBNull.Value),
                    new SqlParameter("@s", client.SoldClient)
                });
            return client;
        }

        public void Update(Client client)
        {
            ExecuteNonQuery(
                "UPDATE Client SET nume=@n, adresa=@a, telefon=@t, email=@e, soldClient=@s WHERE codClient=@id",
                new[] {
                    new SqlParameter("@n",  client.Nume),
                    new SqlParameter("@a",  client.Adresa ?? (object)DBNull.Value),
                    new SqlParameter("@t",  client.Telefon ?? (object)DBNull.Value),
                    new SqlParameter("@e",  client.Email ?? (object)DBNull.Value),
                    new SqlParameter("@s",  client.SoldClient),
                    new SqlParameter("@id", client.CodClient)
                });
        }

        public void UpdateSold(int codClient, decimal sumaDelta)
        {
            ExecuteNonQuery(
                "UPDATE Client SET soldClient=soldClient+@delta WHERE codClient=@id",
                new[] { new SqlParameter("@delta", sumaDelta), new SqlParameter("@id", codClient) });
        }

        private Client MapClient(DataRow row) => new Client
        {
            CodClient  = Convert.ToInt32(row["codClient"]),
            Nume       = row["nume"].ToString(),
            Adresa     = row["adresa"] == DBNull.Value ? null : row["adresa"].ToString(),
            Telefon    = row["telefon"] == DBNull.Value ? null : row["telefon"].ToString(),
            Email      = row["email"] == DBNull.Value ? null : row["email"].ToString(),
            SoldClient = Convert.ToDecimal(row["soldClient"])
        };
    }
}
