using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    /// <summary>
    /// View MVC — Inregistrare Plata Furnizor (Iosub Maria-Catalina).
    /// Conform spec. cap.2: 6 grupuri stanga, grila dreapta, 1262x952.
    /// Moduri: Vizualizare / Adaugare.
    /// Shortcuts: Ctrl+A=Adaugare, Ctrl+S=Salvare, Ctrl+N=Anulare, Esc=Renuntare.
    /// </summary>
    public partial class FormPlata : Form
    {
        private readonly PlataFormCtrl _ctrl;
        private PlataDetail _pdSelectat;

        // Flag pentru a preveni evenimentele circulare la sincronizare valoare<->procent
        private bool _sincronizare = false;

        public FormPlata()
        {
            InitializeComponent();
            _ctrl = new PlataFormCtrl(
                msg => MessageBox.Show(msg, "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error),
                msg => MessageBox.Show(msg, "Informatie", MessageBoxButtons.OK, MessageBoxIcon.Information)
            );
        }

        // ═══════════════════════════════════════════════════════════════════
        // Incarcare
        // ═══════════════════════════════════════════════════════════════════

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _ctrl.Initializeaza();
            LoadFurnizori();
            LoadCaserii();
            LoadConturiBancare();
            LoadTipRata();
            RefreshTotaluriLuna();
            SetModVizualizare();
        }

        // ── Populare controale ────────────────────────────────────────────

        private void LoadFurnizori()
        {
            cmbFurnizor.Items.Clear();
            cmbFurnizor.Items.Add(new ComboItem("0", "-- Selectati furnizorul --"));
            foreach (var f in _ctrl.GetFormData().GetListaFurnizori())
                cmbFurnizor.Items.Add(new ComboItem(f.CodFurnizor.ToString(), f.NumeFurnizor));
            cmbFurnizor.SelectedIndex = 0;
        }

        private void LoadFacturiFurnizor(int codFurnizor)
        {
            cmbFactura.Items.Clear();
            cmbFactura.Items.Add(new FacturaItem("", "", DateTime.MinValue, 0, 0));
            foreach (var f in _ctrl.GetFormData().GetFacturiFurnizor(codFurnizor))
            {
                decimal rest = _ctrl.GetFormData().CalculeazaRestDisponibil(f.Serie, f.Numar, f.ValoareTotala);
                cmbFactura.Items.Add(new FacturaItem(f.Serie, f.Numar, f.DataDocument, f.ValoareTotala, rest));
            }
            cmbFactura.SelectedIndex = 0;
            ClearDetaliiFactura();
        }

        private void LoadCaserii()
        {
            cmbCaserie.Items.Clear();
            cmbCaserie.Items.Add(new ComboItem("0", "-- Selectati caseria --"));
            foreach (var c in _ctrl.GetFormData().GetListaCaserii())
            {
                string display = string.IsNullOrEmpty(c.Locatie)
                    ? $"Caserie #{c.IdCaserie} ({c.Responsabil})"
                    : $"{c.Locatie} — {c.Responsabil}";
                cmbCaserie.Items.Add(new ComboItem(c.IdCaserie.ToString(), display));
            }
            cmbCaserie.SelectedIndex = 0;
        }

        private void LoadConturiBancare()
        {
            cmbContBancar.Items.Clear();
            cmbContBancar.Items.Add(new ComboItem("", "-- Selectati contul --"));
            foreach (var cb in _ctrl.GetFormData().GetListaConturiBancare())
                cmbContBancar.Items.Add(new ComboItem(cb.IBAN, $"{cb.IBAN}  —  {cb.Banca}"));
            cmbContBancar.SelectedIndex = 0;
        }

        private void LoadTipRata()
        {
            cmbTipRata.Items.Clear();
            cmbTipRata.Items.Add("Avans");
            cmbTipRata.Items.Add("Diferenta");
            cmbTipRata.Items.Add("Integral");
            cmbTipRata.SelectedIndex = 2; // Integral default
        }

        // ── Grila ─────────────────────────────────────────────────────────

        private void PopulateGrila(int codFurnizor)
        {
            dgvPlati.Rows.Clear();
            _pdSelectat = null;

            foreach (var pd in _ctrl.GetPlatiByFurnizor(codFurnizor))
            {
                int idx = dgvPlati.Rows.Add(
                    $"#{pd.Plata.IdPlata}",
                    pd.Plata.DataPlata.ToString("dd/MM/yyyy"),
                    $"{pd.SerieFurnizor}-{pd.NumarFurnizor}",
                    $"{pd.Plata.Suma:N2}",
                    pd.TipRata,
                    pd.Plata.Stare
                );
                dgvPlati.Rows[idx].Tag = pd;
            }
        }

        // ═══════════════════════════════════════════════════════════════════
        // Moduri formular
        // ═══════════════════════════════════════════════════════════════════

        private void SetModVizualizare()
        {
            // Stanga — campuri
            cmbFurnizor.Enabled = true;
            cmbFactura.Enabled = false;
            dtpData.Enabled = false;
            cmbTipRata.Enabled = false;
            numProcent.Enabled = false;
            dtpScadenta.Enabled = false;
            numValoare.Enabled = false;
            rbNumerar.Enabled = false;
            rbVirament.Enabled = false;
            cmbCaserie.Enabled = false;
            cmbContBancar.Enabled = false;

            // Butoane
            btnAdaugare.Enabled = true;
            btnAnulare.Enabled = _pdSelectat != null
                                && _pdSelectat.Plata.Stare == "Inregistrat";
            btnSalvare.Enabled = false;
            btnRenuntare.Enabled = false;

            SetStatus("Mod: Vizualizare  |  Ctrl+A = Adaugare  |  Ctrl+N = Anulare plata selectata");
        }

        private void SetModAdaugare()
        {
            cmbFactura.Enabled = true;
            dtpData.Enabled = true;
            cmbTipRata.Enabled = true;
            numProcent.Enabled = cmbTipRata.SelectedItem?.ToString() != "Integral";
            dtpScadenta.Enabled = true;
            numValoare.Enabled = true;
            rbNumerar.Enabled = true;
            rbVirament.Enabled = true;
            cmbCaserie.Enabled = rbNumerar.Checked;
            cmbContBancar.Enabled = rbVirament.Checked;

            btnAdaugare.Enabled = false;
            btnAnulare.Enabled = false;
            btnSalvare.Enabled = true;
            btnRenuntare.Enabled = true;

            SetStatus("Mod: Adaugare  |  Ctrl+S = Salvare  |  Esc = Renuntare");
        }

        // ═══════════════════════════════════════════════════════════════════
        // Handlers ComboBox-uri
        // ═══════════════════════════════════════════════════════════════════

        private void cmbFurnizor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cmbFurnizor.SelectedItem is ComboItem item)) return;
            if (!int.TryParse(item.Cod, out int cod) || cod <= 0)
            {
                txtSoldFurnizor.Text = "";
                cmbFactura.Items.Clear();
                dgvPlati.Rows.Clear();
                ClearDetaliiFactura();
                return;
            }

            _ctrl.OnFurnizorSelected(cod);

            // Afiseaza soldul furnizorului (ReadOnly)
            var furnizori = _ctrl.GetFormData().GetListaFurnizori();
            var furn = furnizori.Find(f => f.CodFurnizor == cod);
            txtSoldFurnizor.Text = furn != null ? $"{furn.SoldFurnizor:N2} RON" : "";

            LoadFacturiFurnizor(cod);
            PopulateGrila(cod);
            ClearDocumentGenerat();
        }

        private void cmbFactura_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(cmbFactura.SelectedItem is FacturaItem item)) return;
            if (string.IsNullOrEmpty(item.Serie)) { ClearDetaliiFactura(); return; }

            _ctrl.OnFacturaSelected(item.Serie, item.Numar, item.DataDocument, item.ValoareTotala);
            AfiseazaDetaliiFactura();
            SincronizeazaIntegral();
        }

        private void AfiseazaDetaliiFactura()
        {
            var fd = _ctrl.GetFormData();
            txtDataFactura.Text = fd.GetDataFactura() == DateTime.MinValue
                ? "" : fd.GetDataFactura().ToString("dd/MM/yyyy");
            txtValTotala.Text = $"{fd.GetValoareTotalaFactura():N2} RON";
            txtRestDisp.Text = $"{fd.GetRestDisponibil():N2} RON";
        }

        private void ClearDetaliiFactura()
        {
            txtDataFactura.Text = "";
            txtValTotala.Text = "";
            txtRestDisp.Text = "";
            numValoare.Maximum = 9999999;
            numValoare.Value = 0;
            numProcent.Value = 0;
        }

        // ── TipRata ────────────────────────────────────────────────────────

        private void cmbTipRata_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tip = cmbTipRata.SelectedItem?.ToString() ?? "";

            if (tip == "Integral")
            {
                numProcent.Value = 100;
                numProcent.Enabled = false;
                numValoare.Maximum = 9999999;
                numValoare.Value = _ctrl.GetFormData().GetRestDisponibil();
            }
            else
            {
                numProcent.Enabled = _ctrl.GetFormData().EsteDocumentNou();
                numValoare.Maximum = _ctrl.GetFormData().GetRestDisponibil();
            }
        }

        // Sincronizare bidirectionala valoare <-> procent
        private void numValoare_ValueChanged(object sender, EventArgs e)
        {
            if (_sincronizare) return;
            _sincronizare = true;
            decimal procent = (decimal)_ctrl.CalculeazaProcentDinValoare(numValoare.Value);

            if (procent > numProcent.Maximum)
                procent = numProcent.Maximum;

            if (procent < numProcent.Minimum)
                procent = numProcent.Minimum;

            numProcent.Value = procent;
            _sincronizare = false;
        }

        private void numProcent_ValueChanged(object sender, EventArgs e)
        {
            if (_sincronizare || cmbTipRata.SelectedItem?.ToString() == "Integral") return;
            _sincronizare = true;
            decimal valCalc = _ctrl.CalculeazaValoareDinProcent(numProcent.Value);
            numValoare.Value = Math.Min(valCalc, _ctrl.GetFormData().GetRestDisponibil());
            _sincronizare = false;
        }

        private void SincronizeazaIntegral()
        {
            decimal rest = _ctrl.GetFormData().GetRestDisponibil();
            numValoare.Maximum = 9999999; // fara restrictie — validam la Salvare
            if (cmbTipRata.SelectedItem?.ToString() == "Integral")
            {
                numProcent.Value = 100;
                numValoare.Value = rest;
            }
        }

        // ── Canal plata ────────────────────────────────────────────────────

        private void rbCanal_CheckedChanged(object sender, EventArgs e)
        {
            bool esteNumerar = rbNumerar.Checked;

            lblCaserie.Visible = esteNumerar;
            cmbCaserie.Visible = esteNumerar;
            cmbCaserie.Enabled = esteNumerar && _ctrl.GetFormData().EsteDocumentNou();

            lblContBancar.Visible = !esteNumerar;
            cmbContBancar.Visible = !esteNumerar;
            cmbContBancar.Enabled = !esteNumerar && _ctrl.GetFormData().EsteDocumentNou();

            // Actualizeaza grupul Document Generat
            rbChitanta.Checked = esteNumerar;
            rbExtrasContPlata.Checked = !esteNumerar;

            _ctrl.OnCanalChanged(esteNumerar ? "Numerar" : "ContBancar");
        }

        // ═══════════════════════════════════════════════════════════════════
        // Butoane
        // ═══════════════════════════════════════════════════════════════════

        private void btnAdaugare_Click(object sender, EventArgs e)
        {
            var plata = _ctrl.DocumentNou();

            txtNrPlata.Text = "(nou)";
            txtStare.Text = plata.Stare;
            dtpData.Value = plata.DataPlata;
            dtpScadenta.Value = plata.DataPlata.AddDays(30);
            cmbTipRata.SelectedIndex = 2; // Integral
            numValoare.Value = 0;
            numProcent.Value = 100;
            rbNumerar.Checked = true;
            if (cmbFactura.Items.Count > 0) cmbFactura.SelectedIndex = 0;
            if (cmbCaserie.Items.Count > 0) cmbCaserie.SelectedIndex = 0;
            if (cmbContBancar.Items.Count > 0) cmbContBancar.SelectedIndex = 0;
            ClearDocumentGenerat();

            SetModAdaugare();
        }

        private void btnSalvare_Click(object sender, EventArgs e)
        {
            int codFurnizor = GetCodFurnizorSelectat();

            if (!(cmbFactura.SelectedItem is FacturaItem facturaItem)
                || string.IsNullOrEmpty(facturaItem.Serie))
            {
                MessageBox.Show("Selectati o factura furnizor!", "Validare",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string tipRata = cmbTipRata.SelectedItem?.ToString() ?? "Integral";
            string canal = rbNumerar.Checked ? "Numerar" : "ContBancar";
            int? idCas = rbNumerar.Checked ? GetIdCaserieSelectat() : (int?)null;
            string iban = rbVirament.Checked ? GetIbanSelectat() : null;

            _ctrl.GetFormData().GetDocumentCurent().DataPlata = dtpData.Value.Date;

            bool succes = _ctrl.SalveazaPlata(
                codFurnizor,
                facturaItem.Serie, facturaItem.Numar,
                numValoare.Value,
                tipRata,
                dtpScadenta.Value.Date,
                canal, idCas, iban);

            if (succes)
            {
                // Afiseaza in grupul Document Generat
                var fd = _ctrl.GetFormData();
                txtNrPlata.Text = _ctrl.GetFormData().GetDocumentCurent().IdPlata.ToString("D4");
                txtStare.Text = "Inregistrat";
                txtNrDocument.Text = fd.GetNrDocumentGenerat();
                dtpDataEmitere.Value = fd.GetDataEmitereDocument() == DateTime.MinValue
                    ? DateTime.Today : fd.GetDataEmitereDocument();

                RefreshTotaluriLuna();
                PopulateGrila(codFurnizor);
                SetModVizualizare();

                MessageBox.Show(
                    $"Plata inregistrata cu succes!\n\n" +
                    $"Nr. plata: #{_ctrl.GetFormData().GetDocumentCurent().IdPlata}\n" +
                    $"Document generat: {fd.GetNrDocumentGenerat()}",
                    "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnAnulare_Click(object sender, EventArgs e)
        {
            if (_pdSelectat == null) return;
            if (_pdSelectat.Plata.Stare != "Inregistrat")
            {
                MessageBox.Show("Pot fi anulate doar platile cu starea 'Inregistrat'.",
                    "Anulare imposibila", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool succes = _ctrl.AnuleazaPlata(_pdSelectat);
            if (succes)
            {
                int cod = GetCodFurnizorSelectat();
                if (cod > 0) PopulateGrila(cod);
                RefreshTotaluriLuna();
                _pdSelectat = null;
                SetModVizualizare();
            }
        }

        private void btnRenuntare_Click(object sender, EventArgs e)
        {
            _ctrl.Renunta();
            txtNrPlata.Text = "";
            txtStare.Text = "";
            dtpData.Value = DateTime.Today;
            numValoare.Value = 0;
            numProcent.Value = 0;
            rbNumerar.Checked = true;
            if (cmbFactura.Items.Count > 0) cmbFactura.SelectedIndex = 0;
            if (cmbCaserie.Items.Count > 0) cmbCaserie.SelectedIndex = 0;
            if (cmbContBancar.Items.Count > 0) cmbContBancar.SelectedIndex = 0;
            cmbTipRata.SelectedIndex = 2;
            ClearDetaliiFactura();
            ClearDocumentGenerat();
            SetModVizualizare();
        }

        // ═══════════════════════════════════════════════════════════════════
        // Grila
        // ═══════════════════════════════════════════════════════════════════

        private void dgvPlati_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPlati.SelectedRows.Count == 0) return;
            _pdSelectat = dgvPlati.SelectedRows[0].Tag as PlataDetail;

            if (!_ctrl.GetFormData().EsteDocumentNou())
                btnAnulare.Enabled = _pdSelectat != null
                                  && _pdSelectat.Plata.Stare == "Inregistrat";
        }

        private void dgvPlati_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgvPlati.Columns[e.ColumnIndex].Name != "colStare" || e.Value == null) return;

            switch (e.Value.ToString())
            {
                case "Inregistrat":
                    e.CellStyle.BackColor = Color.FromArgb(212, 237, 218);
                    e.CellStyle.ForeColor = Color.FromArgb(21, 87, 36);
                    break;
                case "Anulat":
                    e.CellStyle.BackColor = Color.FromArgb(248, 215, 218);
                    e.CellStyle.ForeColor = Color.FromArgb(114, 28, 36);
                    break;
            }
            e.FormattingApplied = true;
        }

        // ═══════════════════════════════════════════════════════════════════
        // Shortcuts: Ctrl+A / Ctrl+S / Ctrl+N / Escape
        // ═══════════════════════════════════════════════════════════════════

        private void FormPlata_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A && btnAdaugare.Enabled)
            { btnAdaugare_Click(sender, e); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.S && btnSalvare.Enabled)
            { btnSalvare_Click(sender, e); e.SuppressKeyPress = true; }
            else if (e.Control && e.KeyCode == Keys.N && btnAnulare.Enabled)
            { btnAnulare_Click(sender, e); e.SuppressKeyPress = true; }
            else if (e.KeyCode == Keys.Escape && btnRenuntare.Enabled)
            { btnRenuntare_Click(sender, e); e.SuppressKeyPress = true; }
        }

        // ═══════════════════════════════════════════════════════════════════
        // Helpers
        // ═══════════════════════════════════════════════════════════════════

        private int GetCodFurnizorSelectat()
        {
            if (cmbFurnizor.SelectedItem is ComboItem item && int.TryParse(item.Cod, out int cod)) return cod;
            return 0;
        }

        private int? GetIdCaserieSelectat()
        {
            if (cmbCaserie.SelectedItem is ComboItem item && int.TryParse(item.Cod, out int id) && id > 0) return id;
            return null;
        }

        private string GetIbanSelectat()
        {
            if (cmbContBancar.SelectedItem is ComboItem item && !string.IsNullOrWhiteSpace(item.Cod)) return item.Cod;
            return null;
        }

        private void ClearDocumentGenerat()
        {
            rbChitanta.Checked = rbNumerar.Checked;
            rbExtrasContPlata.Checked = rbVirament.Checked;
            txtNrDocument.Text = "";
            dtpDataEmitere.Value = DateTime.Today;
        }

        private void RefreshTotaluriLuna()
        {
            var fd = _ctrl.GetFormData();
            lblTotalLuna.Text = $"{fd.GetTotalLuna():N2} RON";
            lblNrPlati.Text = fd.GetNumarPlatiLuna().ToString();
        }

        private void SetStatus(string mesaj) => tsslStatus.Text = mesaj;
    }
}