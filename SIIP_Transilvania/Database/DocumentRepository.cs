using Microsoft.Data.SqlClient;
using SIIP_Transilvania.Forms;
using SIIP_Transilvania.Models;
using System;
using System.Collections.Generic;
using System.Data;

namespace SIIP_Transilvania.Database
{
    // ═══════════════════════════════════════════════════════════════════════
    // DocumentRepository — contine DOAR query-uri SQL si mapare DataRow->obiect
    // Fara logica de business, fara filtrari — acestea apartin FormData/Controller
    // ═══════════════════════════════════════════════════════════════════════
    public class PlataDetail
    {
        public Plata Plata { get; set; }
        public string SerieFurnizor { get; set; }
        public string NumarFurnizor { get; set; }
        public int CodFurnizor { get; set; }
        public string NumeFurnizor { get; set; }
        public string TipRata { get; set; }
        public string IBAN { get; set; }
    }

    public class DocumentRepository : AbstractRepository
    {
        // ══════════════════════════════════════════════════════════════════
        // FACTURA CLIENT
        // ══════════════════════════════════════════════════════════════════

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
                list.Add(MapFacturaClient(row));
            return list;
        }

        public void UpdateStareIncasareFactura(string serie, string numar, string stareNoua)
        {
            ExecuteNonQuery(
                "UPDATE FacturaClient SET stareIncasare=@stare WHERE serie=@s AND numar=@n",
                new[] {
                    new SqlParameter("@stare", stareNoua),
                    new SqlParameter("@s", serie),
                    new SqlParameter("@n", numar)
                });
        }

        public void UpdateStareIncasareFactura(string serie, string numar, decimal sumaNoua, decimal restCurent)
        {
            string stareNoua = (restCurent - sumaNoua) <= 0 ? "Achitat" : "PartialIncasat";
            ExecuteNonQuery(
                "UPDATE FacturaClient SET stareIncasare=@stare, dataOperare=GETDATE() WHERE serie=@s AND numar=@n",
                new[] {
                    new SqlParameter("@stare", stareNoua),
                    new SqlParameter("@s", serie),
                    new SqlParameter("@n", numar)
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
            CodClient = Convert.ToInt32(row["codClient"]),
            RestDisponibil = Convert.ToDecimal(row["restDisponibil"])
        };

        // ══════════════════════════════════════════════════════════════════
        // FACTURA FURNIZOR
        // ══════════════════════════════════════════════════════════════════

        public List<FacturaFurnizor> FindFacturiFurnizorByCod(int codFurnizor)
        {
            var list = new List<FacturaFurnizor>();
            var dt = ExecuteQuery(
                @"SELECT serie, numar, dataDocument, valoareTotala, TVA, scadenta, stare, codFurnizor
                  FROM FacturaFurnizor
                  WHERE codFurnizor = @cod
                  ORDER BY dataDocument DESC",
                new[] { new SqlParameter("@cod", codFurnizor) });
            foreach (DataRow row in dt.Rows)
                list.Add(MapFacturaFurnizor(row));
            return list;
        }

        public void UpdateStarePlataFactura(string serie, string numar, string stare)
        {
            ExecuteNonQuery(
                "UPDATE FacturaFurnizor SET stare=@stare WHERE serie=@s AND numar=@n",
                new[] {
                    new SqlParameter("@stare", stare),
                    new SqlParameter("@s", serie),
                    new SqlParameter("@n", numar)
                });
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

        // ══════════════════════════════════════════════════════════════════
        // FACTURA RETUR (Lazăr Maria-Daniela)
        // ══════════════════════════════════════════════════════════════════

        public FacturaRetur SaveFacturaRetur(FacturaRetur retur)
        {
            bool exists = Convert.ToInt32(ExecuteScalar(
                "SELECT COUNT(*) FROM FacturaRetur WHERE serie=@s AND numar=@n",
                new[] { new SqlParameter("@s", retur.Serie), new SqlParameter("@n", retur.Numar) })) > 0;
            if (!exists) return CreateFacturaRetur(retur);
            else return UpdateStareFacturaRetur(retur);
        }

        private FacturaRetur CreateFacturaRetur(FacturaRetur retur)
        {
            ExecuteNonQuery(
                @"INSERT INTO FacturaRetur
                  (serie, numar, dataDocument, dataOperare, valoareTotala, valoareRetur,
                   motivRetur, stareRetur, tipRetur, codClient, codFurnizor, serieFactInit, numarFactInit)
                  VALUES (@serie, @numar, @dataDoc, @dataOp, 0, @val, @motiv,
                          @stare, @tip, @codClient, @codFurnizor, @serieInit, @numarInit)",
                new[] {
                    new SqlParameter("@serie",      retur.Serie),
                    new SqlParameter("@numar",       retur.Numar),
                    new SqlParameter("@dataDoc",     retur.DataDocument),
                    new SqlParameter("@dataOp",      DateTime.Now.Date),
                    new SqlParameter("@val",         retur.ValoareRetur),
                    new SqlParameter("@motiv",       retur.MotivRetur ?? (object)DBNull.Value),
                    new SqlParameter("@stare",       retur.StareRetur),
                    new SqlParameter("@tip",         retur.TipRetur),
                    new SqlParameter("@codClient",   retur.CodClient.HasValue   ? (object)retur.CodClient.Value   : DBNull.Value),
                    new SqlParameter("@codFurnizor", retur.CodFurnizor.HasValue ? (object)retur.CodFurnizor.Value : DBNull.Value),
                    new SqlParameter("@serieInit",   retur.SerieFactInit ?? (object)DBNull.Value),
                    new SqlParameter("@numarInit",   retur.NumarFactInit ?? (object)DBNull.Value)
                });
            return retur;
        }

        private FacturaRetur UpdateStareFacturaRetur(FacturaRetur retur)
        {
            ExecuteNonQuery(
                "UPDATE FacturaRetur SET stareRetur=@stare WHERE serie=@s AND numar=@n",
                new[] {
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
                   FROM FacturaRetur WHERE {fk}=@cod AND tipRetur=@tip
                   ORDER BY dataDocument DESC",
                new[] { new SqlParameter("@cod", codPartener), new SqlParameter("@tip", tipRetur) });
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
            var r = ExecuteScalar("SELECT ISNULL(MAX(CAST(numar AS INT)),0)+1 FROM FacturaRetur WHERE serie='RET'");
            return r?.ToString().PadLeft(3, '0') ?? "001";
        }

        public decimal GetSumaReturnata(string serie, string numar)
        {
            var r = ExecuteScalar(
                @"SELECT ISNULL(SUM(valoareRetur),0) FROM FacturaRetur
                  WHERE serieFactInit=@s AND numarFactInit=@n AND stareRetur != 'Anulat'",
                new[] { new SqlParameter("@s", serie), new SqlParameter("@n", numar) });
            return r == null ? 0 : Convert.ToDecimal(r);
        }

        public void AnuleazaRetur(string serie, string numar)
        {
            ExecuteNonQuery(
                "UPDATE FacturaRetur SET stareRetur='Anulat' WHERE serie=@s AND numar=@n",
                new[] { new SqlParameter("@s", serie), new SqlParameter("@n", numar) });
        }

        public (decimal total, int nr) GetTotaluriLunaRetur()
        {
            var dt = ExecuteQuery(
                @"SELECT COUNT(*) AS nr, ISNULL(SUM(valoareRetur),0) AS total
                  FROM FacturaRetur
                  WHERE MONTH(dataDocument)=@luna AND YEAR(dataDocument)=@an AND stareRetur != 'Anulat'",
                new[] { new SqlParameter("@luna", DateTime.Now.Month), new SqlParameter("@an", DateTime.Now.Year) });
            if (dt.Rows.Count > 0)
                return (Convert.ToDecimal(dt.Rows[0]["total"]), Convert.ToInt32(dt.Rows[0]["nr"]));
            return (0, 0);
        }

        // ══════════════════════════════════════════════════════════════════
        // INCASARE CLIENT (Crenganiș Andreea-Bianca)
        // ══════════════════════════════════════════════════════════════════

        public decimal GetSumaIncasata(string serie, string numar)
        {
            var r = ExecuteScalar(
                "SELECT ISNULL(SUM(sumaIncasata),0) FROM Incasare WHERE serie=@s AND numar=@n",
                new[] { new SqlParameter("@s", serie), new SqlParameter("@n", numar) });
            return Convert.ToDecimal(r);
        }

        public Incasare SaveIncasare(Incasare inc)
        {
            var result = ExecuteScalar(
                @"INSERT INTO Incasare (dataIncasare, sumaIncasata, canal, serie, numar)
                  OUTPUT INSERTED.idIncasare
                  VALUES (@data, @suma, @canal, @serie, @numar)",
                new[] {
                    new SqlParameter("@data",  inc.DataIncasare),
                    new SqlParameter("@suma",  inc.SumaIncasata),
                    new SqlParameter("@canal", inc.Canal),
                    new SqlParameter("@serie", inc.SerieFact),
                    new SqlParameter("@numar", inc.NumarFact)
                });
            inc.IdIncasare = Convert.ToInt32(result);
            return inc;
        }

        public BonFiscal SaveBonFiscal(BonFiscal bon)
        {
            ExecuteNonQuery(
                @"INSERT INTO BonFiscal (DataEmitere, TotalValoare, IdCaserie, IdIncasare)
                  VALUES (@data, @total, @caserie, @incasare)",
                new[] {
                    new SqlParameter("@data",     bon.DataEmitere),
                    new SqlParameter("@total",    bon.TotalValoare),
                    new SqlParameter("@caserie",  bon.IdCaserie),
                    new SqlParameter("@incasare", bon.IdIncasare)
                });
            return bon;
        }

        public ExtrasContIncasare SaveExtrasContIncasare(ExtrasContIncasare extras)
        {
            ExecuteNonQuery(
                @"INSERT INTO ExtrasContIncasare (DataEmitere, SumaIncasata, IBAN, IdIncasare)
                  VALUES (@data, @suma, @iban, @incasare)",
                new[] {
                    new SqlParameter("@data",     extras.DataEmitere),
                    new SqlParameter("@suma",     extras.SumaIncasata),
                    new SqlParameter("@iban",     extras.IBAN),
                    new SqlParameter("@incasare", extras.IdIncasare)
                });
            return extras;
        }

        public List<Incasare> FindIncasariByClient(int codClient)
        {
            var list = new List<Incasare>();
            var dt = ExecuteQuery(
                @"SELECT i.idIncasare, i.dataIncasare, i.sumaIncasata, i.canal, i.serie, i.numar
                  FROM Incasare i
                  INNER JOIN FacturaClient fc ON i.serie=fc.serie AND i.numar=fc.numar
                  WHERE fc.codClient=@cod ORDER BY i.dataIncasare DESC",
                new[] { new SqlParameter("@cod", codClient) });
            foreach (DataRow row in dt.Rows)
                list.Add(MapIncasare(row));
            return list;
        }

        public (decimal total, int nr) GetTotaluriLunaIncasari()
        {
            var dt = ExecuteQuery(
                @"SELECT ISNULL(SUM(sumaIncasata),0), COUNT(*) FROM Incasare
                  WHERE MONTH(dataIncasare)=MONTH(GETDATE()) AND YEAR(dataIncasare)=YEAR(GETDATE())");
            if (dt.Rows.Count == 0) return (0, 0);
            return (Convert.ToDecimal(dt.Rows[0][0]), Convert.ToInt32(dt.Rows[0][1]));
        }

        private Incasare MapIncasare(DataRow row) => new Incasare
        {
            IdIncasare = Convert.ToInt32(row["idIncasare"]),
            DataIncasare = Convert.ToDateTime(row["dataIncasare"]),
            SumaIncasata = Convert.ToDecimal(row["sumaIncasata"]),
            Canal = row["canal"].ToString(),
            SerieFact = row["serie"].ToString(),
            NumarFact = row["numar"].ToString()
        };

        // ══════════════════════════════════════════════════════════════════
        // PLATA FURNIZOR (Iosub Maria-Catalina)
        // ══════════════════════════════════════════════════════════════════

        public List<PlataDetail> FindPlatiByFurnizor(int codFurnizor)
        {
            var list = new List<PlataDetail>();
            var dt = ExecuteQuery(
                @"SELECT DISTINCT p.idPlata, p.dataPlata, p.suma, p.tipPlata, p.canal, p.stare AS stareaPlata,
                         MIN(pe.serieFurnizor) AS serieFurnizor, MIN(pe.numarFurnizor) AS numarFurnizor,
                         MIN(pe.tipRata) AS tipRata,
                         ff.codFurnizor, f.numeFurnizor,
                         MIN(ecp.iban) AS iban
                  FROM Plata p
                  INNER JOIN PlataEsalonata pe ON pe.idPlata=p.idPlata
                  INNER JOIN FacturaFurnizor ff ON ff.serie=pe.serieFurnizor AND ff.numar=pe.numarFurnizor
                  INNER JOIN Furnizori f ON f.codFurnizor=ff.codFurnizor
                  LEFT JOIN ExtrasContPlata ecp ON ecp.idPlata=p.idPlata
                  WHERE ff.codFurnizor=@cod AND p.tipPlata='PlataFurnizor'
                  GROUP BY p.idPlata, p.dataPlata, p.suma, p.tipPlata, p.canal, p.stare, ff.codFurnizor, f.numeFurnizor
                  ORDER BY p.dataPlata DESC",
                new[] { new SqlParameter("@cod", codFurnizor) });
            foreach (DataRow row in dt.Rows)
                list.Add(new PlataDetail
                {
                    Plata = new Plata
                    {
                        IdPlata = Convert.ToInt32(row["idPlata"]),
                        DataPlata = Convert.ToDateTime(row["dataPlata"]),
                        Suma = Convert.ToDecimal(row["suma"]),
                        TipPlata = row["tipPlata"].ToString(),
                        Canal = row["canal"].ToString(),
                        Stare = row["stareaPlata"].ToString()
                    },
                    SerieFurnizor = row["serieFurnizor"].ToString(),
                    NumarFurnizor = row["numarFurnizor"].ToString(),
                    TipRata = row["tipRata"].ToString(),
                    CodFurnizor = Convert.ToInt32(row["codFurnizor"]),
                    NumeFurnizor = row["numeFurnizor"].ToString(),
                    IBAN = row["iban"] == DBNull.Value ? null : row["iban"].ToString()
                });
            return list;
        }

        public Plata SavePlata(Plata plata)
        {
            var result = ExecuteScalar(
                @"INSERT INTO Plata (dataPlata, suma, tipPlata, canal, stare)
                  OUTPUT INSERTED.idPlata VALUES (@data, @suma, @tip, @canal, @stare)",
                new[] {
                    new SqlParameter("@data",  plata.DataPlata),
                    new SqlParameter("@suma",  plata.Suma),
                    new SqlParameter("@tip",   plata.TipPlata),
                    new SqlParameter("@canal", plata.Canal),
                    new SqlParameter("@stare", plata.Stare)
                });
            plata.IdPlata = Convert.ToInt32(result);
            return plata;
        }

        public void SavePlataEsalonata(PlataEsalonata pe)
        {
            ExecuteNonQuery(
                @"INSERT INTO PlataEsalonata
                  (idPlata, tipRata, procentAcoperit, dataScadenta, serieFurnizor, numarFurnizor)
                  VALUES (@idPlata, @tipRata, @procent, @dataSc, @serieF, @numarF)",
                new[] {
                    new SqlParameter("@idPlata", pe.IdPlata),
                    new SqlParameter("@tipRata", pe.TipRata),
                    new SqlParameter("@procent", pe.ProcentAcoperit),
                    new SqlParameter("@dataSc",  pe.DataScadenta),
                    new SqlParameter("@serieF",  pe.SerieFurnizor),
                    new SqlParameter("@numarF",  pe.NumarFurnizor)
                });
        }

        public void AnuleazaPlata(int idPlata)
        {
            ExecuteNonQuery("UPDATE Plata SET stare='Anulat' WHERE idPlata=@id",
                new[] { new SqlParameter("@id", idPlata) });
        }

        public decimal GetSumaAchitata(string serieFurnizor, string numarFurnizor)
        {
            var r = ExecuteScalar(
                @"SELECT ISNULL(SUM(p.suma),0) FROM Plata p
                  INNER JOIN PlataEsalonata pe ON pe.idPlata=p.idPlata
                  WHERE pe.serieFurnizor=@s AND pe.numarFurnizor=@n AND p.stare != 'Anulat'",
                new[] { new SqlParameter("@s", serieFurnizor), new SqlParameter("@n", numarFurnizor) });
            return r == null ? 0 : Convert.ToDecimal(r);
        }

        public (decimal total, int nr) GetTotaluriLunaPlati()
        {
            var dt = ExecuteQuery(
                @"SELECT COUNT(*) AS nr, ISNULL(SUM(suma),0) AS total FROM Plata
                  WHERE MONTH(dataPlata)=@luna AND YEAR(dataPlata)=@an
                  AND stare != 'Anulat' AND tipPlata='PlataFurnizor'",
                new[] { new SqlParameter("@luna", DateTime.Now.Month), new SqlParameter("@an", DateTime.Now.Year) });
            if (dt.Rows.Count > 0)
                return (Convert.ToDecimal(dt.Rows[0]["total"]), Convert.ToInt32(dt.Rows[0]["nr"]));
            return (0, 0);
        }

        public int SaveExtrasContPlata(ExtrasContPlata extras)
        {
            var result = ExecuteScalar(
                @"INSERT INTO ExtrasContPlata (dataEmitere, sumaPlata, iban, idPlata)
                  OUTPUT INSERTED.numarExtras
                  VALUES (@data, @suma, @iban, @idPlata)",
                new[] {
                    new SqlParameter("@data",    extras.DataEmitere),
                    new SqlParameter("@suma",    extras.SumaPlata),
                    new SqlParameter("@iban",    extras.IBAN),
                    new SqlParameter("@idPlata", extras.IdPlata)
                });
            extras.NumarExtras = Convert.ToInt32(result);
            return extras.NumarExtras;
        }

        // ══════════════════════════════════════════════════════════════════
        // DECONT (Podina Sabina-Alexia)
        // ══════════════════════════════════════════════════════════════════

        public List<Decont> FindDeconturiByAngajat(int codAngajat)
        {
            var list = new List<Decont>();
            var dt = ExecuteQuery(
                @"SELECT serie, numar, dataDocument, perioadaStart, perioadaEnd,
                         valoareDecontata, stare, codDirector
                  FROM Decont WHERE codAngajat=@cod ORDER BY dataDocument DESC",
                new[] { new SqlParameter("@cod", codAngajat) });
            foreach (DataRow row in dt.Rows)
                list.Add(MapDecont(row));
            return list;
        }

        public string GetNextNumarDecont()
        {
            var r = ExecuteScalar("SELECT ISNULL(MAX(CAST(numar AS INT)),0)+1 FROM Decont WHERE serie='DC'");
            return r?.ToString().PadLeft(3, '0') ?? "001";
        }

        public (decimal total, int depuse, int aprobate, decimal totalAprobat) GetTotaluriLunaDecont()
        {
            try
            {
                var dt = ExecuteQuery(
                    @"SELECT ISNULL(SUM(valoareDecontata),0) AS total,
                             ISNULL(SUM(CASE WHEN stare='Depus'   THEN 1 ELSE 0 END),0) AS depuse,
                             ISNULL(SUM(CASE WHEN stare='Aprobat' THEN 1 ELSE 0 END),0) AS aprobate,
                             ISNULL(SUM(CASE WHEN stare='Aprobat' THEN valoareDecontata ELSE 0 END),0) AS totalAprobat
                      FROM Decont WHERE MONTH(dataDocument)=@luna AND YEAR(dataDocument)=@an",
                    new[] { new SqlParameter("@luna", DateTime.Now.Month), new SqlParameter("@an", DateTime.Now.Year) });
                if (dt.Rows.Count > 0)
                    return (Convert.ToDecimal(dt.Rows[0]["total"]), Convert.ToInt32(dt.Rows[0]["depuse"]),
                            Convert.ToInt32(dt.Rows[0]["aprobate"]), Convert.ToDecimal(dt.Rows[0]["totalAprobat"]));
                return (0, 0, 0, 0);
            }
            catch { return (0, 0, 0, 0); }
        }

        public Decont SaveDecont(Decont decont, List<ArticolDecont> articole, int codAngajat)
        {
            bool exists = Convert.ToInt32(ExecuteScalar(
                "SELECT COUNT(*) FROM Decont WHERE serie=@s AND numar=@n",
                new[] { new SqlParameter("@s", decont.Serie), new SqlParameter("@n", decont.Numar) })) > 0;

            if (!exists)
            {
                ExecuteNonQuery(
                    @"INSERT INTO Decont (serie, numar, dataDocument, dataOperare, perioadaStart,
                                         perioadaEnd, valoareDecontata, stare, codDirector, codAngajat)
                      VALUES (@serie, @numar, @dataDoc, @dataOp, @start, @end, @val, @stare, @dir, @ang)",
                    new[] {
                        new SqlParameter("@serie",   decont.Serie),
                        new SqlParameter("@numar",   decont.Numar),
                        new SqlParameter("@dataDoc", decont.DataDocument),
                        new SqlParameter("@dataOp",  DateTime.Now),
                        new SqlParameter("@start",   decont.PerioadaStart),
                        new SqlParameter("@end",     decont.PerioadaEnd),
                        new SqlParameter("@val",     decont.ValoareDecontata),
                        new SqlParameter("@stare",   decont.Stare),
                        new SqlParameter("@dir",     decont.CodDirector),
                        new SqlParameter("@ang",     codAngajat)
                    });
                foreach (var a in articole)
                    ExecuteNonQuery(
                        @"INSERT INTO ArticolDecont (serieDecont, numarDecont, tipCheltuiala, documentJustificativ, valoare, moneda)
                          VALUES (@serie, @numar, @tip, @doc, @val, @mon)",
                        new[] {
                            new SqlParameter("@serie", decont.Serie),
                            new SqlParameter("@numar", decont.Numar),
                            new SqlParameter("@tip",   a.TipCheltuiala),
                            new SqlParameter("@doc",   a.DocumentJustificativ ?? (object)DBNull.Value),
                            new SqlParameter("@val",   a.Valoare),
                            new SqlParameter("@mon",   a.Moneda)
                        });
            }
            else
                ExecuteNonQuery("UPDATE Decont SET stare=@stare WHERE serie=@s AND numar=@n",
                    new[] { new SqlParameter("@stare", decont.Stare), new SqlParameter("@s", decont.Serie), new SqlParameter("@n", decont.Numar) });
            return decont;
        }

        public void AprobazaDecont(string serie, string numar)
        {
            ExecuteNonQuery("UPDATE Decont SET stare='Aprobat' WHERE serie=@s AND numar=@n",
                new[] { new SqlParameter("@s", serie), new SqlParameter("@n", numar) });
        }

        public void RespingeDecont(string serie, string numar, string motiv)
        {
            ExecuteNonQuery("UPDATE Decont SET stare='Respins', motivRespingere=@motiv WHERE serie=@s AND numar=@n",
                new[] { new SqlParameter("@motiv", motiv), new SqlParameter("@s", serie), new SqlParameter("@n", numar) });
        }

        private Decont MapDecont(DataRow row) => new Decont
        {
            Serie = row["serie"].ToString(),
            Numar = row["numar"].ToString(),
            DataDocument = Convert.ToDateTime(row["dataDocument"]),
            PerioadaStart = Convert.ToDateTime(row["perioadaStart"]),
            PerioadaEnd = Convert.ToDateTime(row["perioadaEnd"]),
            ValoareDecontata = Convert.ToDecimal(row["valoareDecontata"]),
            Stare = row["stare"].ToString(),
            CodDirector = Convert.ToInt32(row["codDirector"])
        };
    }
}