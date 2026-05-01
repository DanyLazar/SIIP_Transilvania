using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Database
{
    // ═══════════════════════════════════════════════════════════════════════
    // DocumentRepository — operatii CRUD pentru documente:
    // FacturaClient, FacturaFurnizor, FacturaRetur, Decont, StatDePlata
    // Echivalent cu DocumentRepository din modelul Java/JPA
    // Metodele se adauga progresiv, pe masura ce formularele le necesita.
    // ═══════════════════════════════════════════════════════════════════════
    public class DocumentRepository : AbstractRepository
    {
        // ── FACTURA CLIENT ──────────────────────────────────────────────

        public List<FacturaClient> FindFacturiClientByCod(int codClient)
        {
            var list = new List<FacturaClient>();
            var dt = ExecuteQuery(
                @"SELECT fc.serie, fc.numar, fc.dataDocument, fc.valoareTotala, fc.TVA,
                         fc.scadenta, fc.stareIncasare, fc.codClient,
                         fc.valoareTotala - ISNULL((
                             SELECT SUM(fr.valoareRetur) FROM FacturaRetur fr
                             WHERE fr.serieFactInit=fc.serie AND fr.numarFactInit=fc.numar
                             AND fr.stareRetur != 'Anulat'),0) AS restDisponibil
                  FROM FacturaClient fc
                  WHERE fc.codClient = @cod
                  ORDER BY fc.dataDocument DESC",
                new[] { new SqlParameter("@cod", codClient) });

            foreach (DataRow row in dt.Rows)
            {
                decimal restDisp = Convert.ToDecimal(row["restDisponibil"]);
                if (restDisp > 0)
                    list.Add(MapFacturaClient(row));
            }
            return list;
        }

        public void UpdateStareIncasareFactura(string serie, string numar, string stareNoua)
        {
            ExecuteNonQuery(
                "UPDATE FacturaClient SET stareIncasare=@stare WHERE serie=@s AND numar=@n",
                new[]
                {
                    new SqlParameter("@stare", stareNoua),
                    new SqlParameter("@s",     serie),
                    new SqlParameter("@n",     numar)
                });
        }

        private FacturaClient MapFacturaClient(DataRow row) => new FacturaClient
        {
            Serie = row["serie"].ToString(),
            Numar = row["numar"].ToString(),
            DataDocument = Convert.ToDateTime(row["dataDocument"]),
            ValoareTotala = Convert.ToDecimal(row["valoareTotala"]),
            TVA = Convert.ToDecimal(row["TVA"]),
            Scadenta = row["scadenta"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["scadenta"]),
            StareIncasare = row["stareIncasare"].ToString(),
            CodClient = Convert.ToInt32(row["codClient"])
        };

        // ── FACTURA FURNIZOR ────────────────────────────────────────────

        public List<FacturaFurnizor> FindFacturiFurnizorByCod(int codFurnizor)
        {
            var list = new List<FacturaFurnizor>();
            var dt = ExecuteQuery(
                @"SELECT serie, numar, dataDocument, valoareTotala, TVA, scadenta, stare, codFurnizor
                  FROM FacturaFurnizor
                  WHERE codFurnizor = @cod AND stare != 'Achitat'
                  ORDER BY dataDocument DESC",
                new[] { new SqlParameter("@cod", codFurnizor) });
            foreach (DataRow row in dt.Rows)
                list.Add(MapFacturaFurnizor(row));
            return list;
        }

        private FacturaFurnizor MapFacturaFurnizor(DataRow row) => new FacturaFurnizor
        {
            Serie = row["serie"].ToString(),
            Numar = row["numar"].ToString(),
            DataDocument = Convert.ToDateTime(row["dataDocument"]),
            ValoareTotala = Convert.ToDecimal(row["valoareTotala"]),
            TVA = Convert.ToDecimal(row["TVA"]),
            Scadenta = row["scadenta"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(row["scadenta"]),
            Stare = row["stare"].ToString(),
            CodFurnizor = Convert.ToInt32(row["codFurnizor"])
        };

        // ── FACTURA RETUR ───────────────────────────────────────────────

        // SaveFacturaRetur — echivalent saveDocument() din Java
        // Decide automat INSERT (obiect nou) sau UPDATE (existent in BD)
        // bazat pe existenta in BD — identic cu logica din ghid
        public FacturaRetur SaveFacturaRetur(FacturaRetur retur)
        {
            bool exists = FacturaReturExists(retur.Serie, retur.Numar);
            if (!exists)
                return CreateFacturaRetur(retur);
            else
                return UpdateStareFacturaRetur(retur);
        }

        private bool FacturaReturExists(string serie, string numar)
        {
            var result = ExecuteScalar(
                "SELECT COUNT(*) FROM FacturaRetur WHERE serie=@s AND numar=@n",
                new[] { new SqlParameter("@s", serie), new SqlParameter("@n", numar) });
            return Convert.ToInt32(result) > 0;
        }

        private FacturaRetur CreateFacturaRetur(FacturaRetur retur)
        {
            ExecuteNonQuery(
                @"INSERT INTO FacturaRetur
                  (serie, numar, dataDocument, dataOperare, valoareTotala, valoareRetur,
                   motivRetur, stareRetur, tipRetur, codClient, codFurnizor,
                   serieFactInit, numarFactInit)
                  VALUES (@serie, @numar, @dataDoc, @dataOp, 0, @val, @motiv,
                          @stare, @tip, @codClient, @codFurnizor, @serieInit, @numarInit)",
                new[]
                {
                    new SqlParameter("@serie",       retur.Serie),
                    new SqlParameter("@numar",        retur.Numar),
                    new SqlParameter("@dataDoc",      retur.DataDocument),
                    new SqlParameter("@dataOp",       DateTime.Now.Date),
                    new SqlParameter("@val",          retur.ValoareRetur),
                    new SqlParameter("@motiv",        retur.MotivRetur ?? (object)DBNull.Value),
                    new SqlParameter("@stare",        retur.StareRetur),
                    new SqlParameter("@tip",          retur.TipRetur),
                    new SqlParameter("@codClient",    retur.CodClient.HasValue ? (object)retur.CodClient.Value : DBNull.Value),
                    new SqlParameter("@codFurnizor",  retur.CodFurnizor.HasValue ? (object)retur.CodFurnizor.Value : DBNull.Value),
                    new SqlParameter("@serieInit",    retur.SerieFactInit ?? (object)DBNull.Value),
                    new SqlParameter("@numarInit",    retur.NumarFactInit ?? (object)DBNull.Value)
                });
            return retur;
        }

        private FacturaRetur UpdateStareFacturaRetur(FacturaRetur retur)
        {
            ExecuteNonQuery(
                "UPDATE FacturaRetur SET stareRetur=@stare WHERE serie=@s AND numar=@n",
                new[]
                {
                    new SqlParameter("@stare", retur.StareRetur),
                    new SqlParameter("@s",     retur.Serie),
                    new SqlParameter("@n",     retur.Numar)
                });
            return retur;
        }

        public List<FacturaRetur> FindRetururiByPartener(int codPartener, string tipRetur)
        {
            var list = new List<FacturaRetur>();
            string fk = tipRetur == "Client" ? "codClient" : "codFurnizor";
            var dt = ExecuteQuery(
                $@"SELECT serie, numar, CONVERT(varchar,dataDocument,103) AS dataDoc,
                          valoareRetur, stareRetur, tipRetur
                   FROM FacturaRetur
                   WHERE {fk}=@cod AND tipRetur=@tip
                   ORDER BY dataDocument DESC",
                new[]
                {
                    new SqlParameter("@cod", codPartener),
                    new SqlParameter("@tip", tipRetur)
                });
            foreach (DataRow row in dt.Rows)
                list.Add(new FacturaRetur
                {
                    Serie = row["serie"].ToString(),
                    Numar = row["numar"].ToString(),
                    ValoareRetur = Convert.ToDecimal(row["valoareRetur"]),
                    StareRetur = row["stareRetur"].ToString(),
                    TipRetur = row["tipRetur"].ToString()
                });
            return list;
        }

        public string GetNextNumarRetur()
        {
            var result = ExecuteScalar(
                "SELECT ISNULL(MAX(CAST(numar AS INT)),0)+1 FROM FacturaRetur WHERE serie='RET'");
            return result?.ToString().PadLeft(3, '0') ?? "001";
        }

        public decimal GetSumaReturnata(string serie, string numar)
        {
            var result = ExecuteScalar(
                @"SELECT ISNULL(SUM(valoareRetur),0) FROM FacturaRetur
                  WHERE serieFactInit=@s AND numarFactInit=@n AND stareRetur != 'Anulat'",
                new[] { new SqlParameter("@s", serie), new SqlParameter("@n", numar) });
            return result == null ? 0 : Convert.ToDecimal(result);
        }

        public void AnuleazaRetur(string serie, string numar)
        {
            ExecuteNonQuery(
                "UPDATE FacturaRetur SET stareRetur='Anulat' WHERE serie=@s AND numar=@n",
                new[] { new SqlParameter("@s", serie), new SqlParameter("@n", numar) });
        }

        // Totaluri lunare pentru panoul din dreapta jos
        public (decimal total, int nr) GetTotaluriLunaRetur()
        {
            var dt = ExecuteQuery(
                @"SELECT COUNT(*) AS nr, ISNULL(SUM(valoareRetur),0) AS total
                  FROM FacturaRetur
                  WHERE MONTH(dataDocument)=@luna AND YEAR(dataDocument)=@an
                  AND stareRetur != 'Anulat'",
                new[]
                {
                    new SqlParameter("@luna", DateTime.Now.Month),
                    new SqlParameter("@an",   DateTime.Now.Year)
                });
            if (dt.Rows.Count > 0)
                return (Convert.ToDecimal(dt.Rows[0]["total"]), Convert.ToInt32(dt.Rows[0]["nr"]));
            return (0, 0);
        }
    }
}