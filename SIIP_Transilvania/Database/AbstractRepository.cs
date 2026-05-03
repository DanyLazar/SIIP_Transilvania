using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Configuration;

namespace SIIP_Transilvania.Database
{
    // ═══════════════════════════════════════════════════════════════════════
    // AbstractRepository — echivalent AbstractRepository din modelul Java/JPA
    // Incapsuleaza operatiile fundamentale CRUD folosind ADO.NET.
    // Ascunde implementarea accesului la BD fata de clienti (formulare).
    // Clientii (formularele) nu stiu ca se foloseste SQL Server / ADO.NET.
    // ═══════════════════════════════════════════════════════════════════════
    public abstract class AbstractRepository
    {
        // Conexiunea statica — Singleton pattern:
        // O singura conexiune activa pentru toata aplicatia.
        // Echivalent cu EntityManager static din modelul Java/JPA.
        private static readonly string _connectionString =
            ConfigurationManager.ConnectionStrings["SIIP"].ConnectionString;

        // ── Gestiunea tranzactiei ────────────────────────────────────────
        // Tranzactia este intotdeauna gestionata de CLIENT (formular),
        // nu de Repository. Principiu din ghid: ACID.
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        protected SqlConnection GetConnection()
        {
            if (_connection == null || _connection.State != ConnectionState.Open)
            {
                _connection = new SqlConnection(_connectionString);
                _connection.Open();
            }
            return _connection;
        }

        // BeginTransaction — initiaza o tranzactie
        // Echivalent cu: repo.beginTransaction() din Java
        public void BeginTransaction()
        {
            _connection = GetConnection();
            _transaction = _connection.BeginTransaction();
        }

        // CommitTransaction — comite tranzactia
        // Echivalent cu: repo.commitTransaction() din Java
        public void CommitTransaction()
        {
            try
            {
                _transaction?.Commit();
            }
            finally
            {
                _transaction = null;
            }
        }

        // RollbackTransaction — anuleaza tranzactia in caz de eroare
        public void RollbackTransaction()
        {
            try
            {
                _transaction?.Rollback();
            }
            finally
            {
                _transaction = null;
            }
        }

        // ── Operatii CRUD de baza ────────────────────────────────────────
        // Echivalent cu metodele create/update/delete din AbstractRepository Java

        // ExecuteQuery — SELECT (citire date)
        protected DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            DataTable dt = new DataTable();
            using (var cmd = new SqlCommand(query, GetConnection(), _transaction))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                using (var adapter = new SqlDataAdapter(cmd))
                    adapter.Fill(dt);
            }
            return dt;
        }

        // ExecuteNonQuery — INSERT / UPDATE / DELETE
        protected int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (var cmd = new SqlCommand(query, GetConnection(), _transaction))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        // ExecuteScalar — SELECT cu rezultat unic (COUNT, MAX etc.)
        protected object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (var cmd = new SqlCommand(query, GetConnection(), _transaction))
            {
                if (parameters != null) cmd.Parameters.AddRange(parameters);
                return cmd.ExecuteScalar();
            }
        }
    }
}
