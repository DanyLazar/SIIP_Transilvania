using System;
using System.Collections.Generic;
using System.Diagnostics;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;
using SIIP_Transilvania.Forms;

namespace SIIP_Transilvania.Tests
{
    // ═══════════════════════════════════════════════════════════════════════
    // TestSIIP — Teste unitare pentru API-ul Repository
    // Echivalent cu TestLocalitati / TestAchizitiiForm din modelul Java/JPA
    // Ghid PSI Partea 4, sectiunea 4.2 si 4.3.2
    //
    // Cum se ruleaza:
    // 1. Click dreapta pe proiect -> Add -> New Item -> Class -> TestSIIP.cs
    // 2. In Program.cs adauga: SIIP_Transilvania.Tests.TestSIIP.RunAll();
    // 3. Ruleaza cu F5 si verifica Output-ul din Debug
    // ═══════════════════════════════════════════════════════════════════════
    public static class TestSIIP
    {
        private static MasterRepository   _masterRepo = new MasterRepository();
        private static DocumentRepository _docRepo    = new DocumentRepository();
        private static int _passed = 0;
        private static int _failed = 0;

        // ── Entry point ──────────────────────────────────────────────────
        public static void RunAll()
        {
            Debug.WriteLine("═══════════════════════════════════════════");
            Debug.WriteLine("  SIIP — Teste unitare Repository API");
            Debug.WriteLine("═══════════════════════════════════════════");

            TestClientiExista();
            TestFurnizoriExista();
            TestFacturiClientExista();
            TestFacturiFurnizorExista();
            TestGetSumaReturnata();
            TestGetNextNumarRetur();
            TestSaveFacturaRetur();
            TestAnuleazaRetur();
            TestTotaluriLunaRetur();
            TestReturFormCtrl_Init();
            TestReturFormCtrl_DocumentNou();
            TestReturFormCtrl_ValidareValoare();

            Debug.WriteLine("═══════════════════════════════════════════");
            Debug.WriteLine($"  Rezultat: {_passed} PASSED | {_failed} FAILED");
            Debug.WriteLine("═══════════════════════════════════════════");
        }

        // ── TEST 1: Clienti exista in BD ─────────────────────────────────
        private static void TestClientiExista()
        {
            List<Client> clienti = _masterRepo.FindClientiAll();
            AssertTrue("TestClientiExista",
                clienti.Count > 0,
                $"Exista {clienti.Count} clienti in BD.");
        }

        // ── TEST 2: Furnizori exista in BD ───────────────────────────────
        private static void TestFurnizoriExista()
        {
            List<Furnizori> furnizori = _masterRepo.FindFurnizoriAll();
            AssertTrue("TestFurnizoriExista",
                furnizori.Count > 0,
                $"Exista {furnizori.Count} furnizori in BD.");
        }

        // ── TEST 3: Facturi client pentru Alpha SRL (codClient=1) ────────
        private static void TestFacturiClientExista()
        {
            List<FacturaClient> facturi = _docRepo.FindFacturiClientByCod(1);
            AssertTrue("TestFacturiClientExista",
                facturi.Count > 0,
                $"Exista {facturi.Count} facturi client cu rest > 0 pentru codClient=1.");
        }

        // ── TEST 4: Facturi furnizor pentru Dist Nord SRL (codFurnizor=1) 
        private static void TestFacturiFurnizorExista()
        {
            List<FacturaFurnizor> facturi = _docRepo.FindFacturiFurnizorByCod(1);
            AssertTrue("TestFacturiFurnizorExista",
                facturi.Count > 0,
                $"Exista {facturi.Count} facturi furnizor pentru codFurnizor=1.");
        }

        // ── TEST 5: Suma returnata pentru o factura ──────────────────────
        private static void TestGetSumaReturnata()
        {
            // FC-001 are restul calculat corect
            decimal suma = _docRepo.GetSumaReturnata("FC", "001");
            AssertTrue("TestGetSumaReturnata",
                suma >= 0,
                $"Suma returnata pentru FC-001: {suma:F2} RON.");
        }

        // ── TEST 6: Numarul urmator FacturaRetur ─────────────────────────
        private static void TestGetNextNumarRetur()
        {
            string numar = _docRepo.GetNextNumarRetur();
            AssertTrue("TestGetNextNumarRetur",
                !string.IsNullOrEmpty(numar) && numar.Length == 3,
                $"Urmatorul numar RET: {numar}.");
        }

        // ── TEST 7: Salvare FacturaRetur nou ─────────────────────────────
        private static void TestSaveFacturaRetur()
        {
            string numarTest = "T" + DateTime.Now.Ticks.ToString().Substring(10, 5);
            var retur = new FacturaRetur
            {
                Serie          = "TST",
                Numar          = numarTest,
                DataDocument   = DateTime.Now.Date,
                ValoareRetur   = 100,
                MotivRetur     = "Test automat",
                StareRetur     = "Emis",
                TipRetur       = "Client",
                CodClient      = 1,
                SerieFactInit  = "FC",
                NumarFactInit  = "001"
            };

            try
            {
                _docRepo.BeginTransaction();
                FacturaRetur saved = _docRepo.SaveFacturaRetur(retur);
                _docRepo.CommitTransaction();

                AssertTrue("TestSaveFacturaRetur_Create",
                    saved != null,
                    $"FacturaRetur TST-{numarTest} salvata cu succes.");

                // Verifica ca UPDATE functioneaza — schimba starea
                saved.StareRetur = "In curs";
                _docRepo.BeginTransaction();
                _docRepo.SaveFacturaRetur(saved);
                _docRepo.CommitTransaction();

                AssertTrue("TestSaveFacturaRetur_Update",
                    true,
                    $"FacturaRetur TST-{numarTest} actualizata cu succes (stare -> In curs).");

                // Curata datele de test
                _docRepo.BeginTransaction();
                _docRepo.AnuleazaRetur("TST", numarTest);
                _docRepo.CommitTransaction();
            }
            catch (Exception ex)
            {
                _docRepo.RollbackTransaction();
                AssertTrue("TestSaveFacturaRetur", false, "Eroare: " + ex.Message);
            }
        }

        // ── TEST 8: Anulare FacturaRetur ─────────────────────────────────
        private static void TestAnuleazaRetur()
        {
            // Verifica ca AnuleazaRetur nu arunca exceptie pentru un retur inexistent
            try
            {
                _docRepo.BeginTransaction();
                _docRepo.AnuleazaRetur("TST", "00000");
                _docRepo.CommitTransaction();
                AssertTrue("TestAnuleazaRetur", true, "AnuleazaRetur executat fara eroare.");
            }
            catch (Exception ex)
            {
                _docRepo.RollbackTransaction();
                AssertTrue("TestAnuleazaRetur", false, "Eroare: " + ex.Message);
            }
        }

        // ── TEST 9: Totaluri luna curenta ────────────────────────────────
        private static void TestTotaluriLunaRetur()
        {
            var (total, nr) = _docRepo.GetTotaluriLunaRetur();
            AssertTrue("TestTotaluriLunaRetur",
                total >= 0 && nr >= 0,
                $"Totaluri luna curenta: {nr} retururi, {total:F2} RON.");
        }

        // ── TEST 10: ReturFormCtrl — initializare ─────────────────────────
        // Echivalent cu prima secventa din TestAchizitiiForm din ghid
        private static void TestReturFormCtrl_Init()
        {
            ReturFormCtrl ctrl = new ReturFormCtrl();
            AssertTrue("TestReturFormCtrl_Init — clienti incarcati",
                ctrl.GetFormData().GetListaClienti().Count > 0,
                $"FormCtrl initializat. Clienti: {ctrl.GetFormData().GetListaClienti().Count}.");
            AssertTrue("TestReturFormCtrl_Init — furnizori incarcati",
                ctrl.GetFormData().GetListaFurnizori().Count > 0,
                $"Furnizori: {ctrl.GetFormData().GetListaFurnizori().Count}.");
        }

        // ── TEST 11: ReturFormCtrl — documentNou ─────────────────────────
        private static void TestReturFormCtrl_DocumentNou()
        {
            ReturFormCtrl ctrl = new ReturFormCtrl();
            ctrl.OnPartenerSelected(1, "Alpha SRL");
            ctrl.DocumentNou();

            AssertTrue("TestReturFormCtrl_DocumentNou — document creat",
                ctrl.GetFormData().GetDocumentCurent() != null,
                "DocumentNou() a creat un obiect FacturaRetur.");
            AssertTrue("TestReturFormCtrl_DocumentNou — serie corecta",
                ctrl.GetFormData().GetDocumentCurent().Serie == "RET",
                $"Serie: {ctrl.GetFormData().GetDocumentCurent().Serie}.");
            AssertTrue("TestReturFormCtrl_DocumentNou — numar generat",
                !string.IsNullOrEmpty(ctrl.GetNumarGenerat()),
                $"Numar generat: {ctrl.GetNumarGenerat()}.");
        }

        // ── TEST 12: ReturFormCtrl — validare valoare retur ──────────────
        private static void TestReturFormCtrl_ValidareValoare()
        {
            ReturFormCtrl ctrl = new ReturFormCtrl();
            ctrl.OnTipReturChanged("Client");
            ctrl.OnPartenerSelected(1, "Alpha SRL");
            ctrl.OnFacturaSelected("FC", "001", DateTime.Now, 5000);
            ctrl.DocumentNou();

            decimal restDisp = ctrl.GetFormData().GetRestDisponibil();
            AssertTrue("TestReturFormCtrl_ValidareValoare — rest calculat",
                restDisp >= 0,
                $"Rest disponibil pentru FC-001: {restDisp:F2} RON.");

            // Valoare mai mare decat restul trebuie sa fie respinsa
            bool respins = !ctrl.SalveazaRetur("Test", "Emis", restDisp + 1000, "Numerar", DateTime.Now);
            AssertTrue("TestReturFormCtrl_ValidareValoare — valoare invalida respinsa",
                respins,
                $"Valoarea {restDisp + 1000:F2} RON (> rest {restDisp:F2}) a fost respinsa corect.");
        }

        // ── Assert helper — echivalent JUnit Assert.assertTrue ────────────
        private static void AssertTrue(string testName, bool condition, string message = "")
        {
            if (condition)
            {
                _passed++;
                Debug.WriteLine($"  ✓ PASS | {testName}");
                if (!string.IsNullOrEmpty(message))
                    Debug.WriteLine($"         {message}");
            }
            else
            {
                _failed++;
                Debug.WriteLine($"  ✗ FAIL | {testName}");
                if (!string.IsNullOrEmpty(message))
                    Debug.WriteLine($"         {message}");
            }
        }
    }
}
