using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Database
{
    // ═══════════════════════════════════════════════════════════════════════
    // CaserieRepository — operatii CRUD pentru entitatea Caserie
    // ═══════════════════════════════════════════════════════════════════════
    public class CaserieRepository : AbstractRepository
    {
        public List<Caserie> FindAll()
        {
            var list = new List<Caserie>();
            var dt = ExecuteQuery("SELECT idCaserie, soldNumerar, responsabil, locatie FROM Caserie");
            foreach (DataRow row in dt.Rows)
                list.Add(MapCaserie(row));
            return list;
        }

        public void UpdateSold(int idCaserie, decimal sumaDelta)
        {
            ExecuteNonQuery(
                "UPDATE Caserie SET soldNumerar=soldNumerar+@delta WHERE idCaserie=@id",
                new[] { new SqlParameter("@delta", sumaDelta), new SqlParameter("@id", idCaserie) });
        }

        private Caserie MapCaserie(DataRow row) => new Caserie
        {
            IdCaserie   = Convert.ToInt32(row["idCaserie"]),
            SoldNumerar = Convert.ToDecimal(row["soldNumerar"]),
            Responsabil = row["responsabil"].ToString(),
            Locatie     = row["locatie"].ToString()
        };
    }
}
