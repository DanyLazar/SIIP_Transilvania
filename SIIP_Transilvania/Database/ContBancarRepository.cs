using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Database
{
    // ═══════════════════════════════════════════════════════════════════════
    // ContBancarRepository — operatii CRUD pentru entitatea ContBancar
    // ═══════════════════════════════════════════════════════════════════════
    public class ContBancarRepository : AbstractRepository
    {
        public List<ContBancar> FindAll()
        {
            var list = new List<ContBancar>();
            var dt = ExecuteQuery("SELECT IBAN, sold, banca, titular FROM ContBancar");
            foreach (DataRow row in dt.Rows)
                list.Add(MapContBancar(row));
            return list;
        }

        public ContBancar FindByIBAN(string iban)
        {
            var dt = ExecuteQuery(
                "SELECT IBAN, sold, banca, titular FROM ContBancar WHERE IBAN=@iban",
                new[] { new SqlParameter("@iban", iban) });
            return dt.Rows.Count > 0 ? MapContBancar(dt.Rows[0]) : null;
        }

        public void UpdateSold(string iban, decimal sumaDelta)
        {
            ExecuteNonQuery(
                "UPDATE ContBancar SET sold=sold+@delta WHERE IBAN=@iban",
                new[] { new SqlParameter("@delta", sumaDelta), new SqlParameter("@iban", iban) });
        }

        private ContBancar MapContBancar(DataRow row) => new ContBancar
        {
            IBAN    = row["IBAN"].ToString(),
            Sold    = Convert.ToDecimal(row["sold"]),
            Banca   = row["banca"].ToString(),
            Titular = row["titular"].ToString()
        };
    }
}
