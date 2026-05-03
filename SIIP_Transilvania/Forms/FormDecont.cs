using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;
using System.Linq;

namespace SIIP_Transilvania.Forms
{
    public class FormDecont : Form
    {
        private readonly DecontFormCtrl _ctrl;
        private bool _modAdaugare = false;

        private readonly Color READONLY_COLOR = Color.FromArgb(245, 245, 245);
        private readonly Color WHITE = Color.White;
        private readonly Color ERR_COLOR = Color.FromArgb(255, 240, 240);
        private readonly Color HEADER_COLOR = Color.FromArgb(31, 56, 100);

        public FormDecont()
        {
            _ctrl = new DecontFormCtrl();
            BuildUI();
            WireEvents();
            LoadAngajati();
            SetModeVizualizare();
        }

        private void BuildUI()
        {
            this.Text = "Inregistrare Decont Cheltuieli — SC Transilvania General Import-Export SRL";
            this.Size = new Size(1024, 768);
            this.MinimumSize = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.KeyPreview = true;

            var status = new StatusStrip { Dock = DockStyle.Bottom, BackColor = Color.FromArgb(232, 232, 232) };
            lblStare = new ToolStripStatusLabel("Stare: Vizualizare") { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblCanal = new ToolStripStatusLabel("Canal: Numerar") { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblLuna = new ToolStripStatusLabel("Luna: " + DateTime.Now.ToString("MMMM yyyy")) { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblUtilizator = new ToolStripStatusLabel("Utilizator: contabil01");
            status.Items.AddRange(new ToolStripItem[] { lblStare, lblCanal, lblLuna, lblUtilizator });
            this.Controls.Add(status);

            tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Arial", 9f) };
            tabInregistrare = new TabPage("Inregistrare") { BackColor = Color.FromArgb(240, 240, 240) };
            tabIstoric = new TabPage("Istoric Deconturi") { BackColor = Color.FromArgb(240, 240, 240) };
            tabRaport = new TabPage("Raport Deconturi") { BackColor = Color.FromArgb(240, 240, 240) };
            tabControl.TabPages.AddRange(new TabPage[] { tabInregistrare, tabIstoric, tabRaport });
            this.Controls.Add(tabControl);

            BuildTabInregistrare();
            BuildTabIstoric();
            BuildTabRaport();
        }

        private void BuildTabInregistrare()
        {
            var tab = tabInregistrare;

            // GRP 1: Date Angajat
            grpAngajat = MakeGroup("1. Date Angajat", 8, 8, 460, 80);
            MakeLabel("ID Angajat:", 8, 22, grpAngajat);
            cboAngajat = MakeCombo(72, 19, 60, grpAngajat);
            MakeLabel("Nume:", 140, 22, grpAngajat);
            cboNumeAngajat = MakeCombo(178, 19, 272, grpAngajat);
            MakeLabel("Functie:", 8, 50, grpAngajat);
            txtFunctie = MakeText(72, 47, 140, grpAngajat, ro: true);
            MakeLabel("Departament:", 220, 50, grpAngajat);
            txtDepartament = MakeText(300, 47, 150, grpAngajat, ro: true);
            tab.Controls.Add(grpAngajat);

            // GRP 2: Date Decont
            grpDecont = MakeGroup("2. Date Decont", 8, 96, 460, 80);
            MakeLabel("Serie:", 8, 22, grpDecont);
            txtSerie = MakeText(50, 19, 60, grpDecont, ro: false, val: "DC");
            MakeLabel("Numar:", 118, 22, grpDecont);
            txtNumar = MakeText(162, 19, 80, grpDecont, ro: false);
            MakeLabel("Data:", 250, 22, grpDecont);
            dtpData = MakeDtp(285, 19, 167, grpDecont);
            MakeLabel("Perioada:", 8, 50, grpDecont);
            dtpPerioadaStart = MakeDtp(68, 47, 180, grpDecont);
            MakeLabel("—", 252, 50, grpDecont);
            dtpPerioadaEnd = MakeDtp(265, 47, 187, grpDecont);
            tab.Controls.Add(grpDecont);

            // GRP 3: Articole Decont
            grpArticole = MakeGroup("3. Articole Decont", 8, 184, 460, 170);
            grdArticole = new DataGridView
            {
                Location = new Point(8, 18),
                Size = new Size(444, 110),
                AllowUserToAddRows = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Arial", 8.5f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = HEADER_COLOR,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 8.5f, FontStyle.Bold)
                }
            };
            var colTip = new DataGridViewComboBoxColumn { HeaderText = "Tip Cheltuiala", Name = "TipCheltuiala", Width = 130 };
            colTip.Items.AddRange("Transport", "Cazare", "Masa", "Combustibil", "Altele");
            var colDoc = new DataGridViewTextBoxColumn { HeaderText = "Document Justificativ", Name = "DocumentJustificativ", Width = 160 };
            var colVal = new DataGridViewTextBoxColumn { HeaderText = "Valoare", Name = "Valoare", Width = 80 };
            var colMon = new DataGridViewComboBoxColumn { HeaderText = "Moneda", Name = "Moneda", Width = 70 };
            colMon.Items.AddRange("RON", "EUR", "USD");
            grdArticole.Columns.AddRange(new DataGridViewColumn[] { colTip, colDoc, colVal, colMon });
            grdArticole.CellValueChanged += GrdArticole_CellValueChanged;
            grdArticole.RowsAdded += GrdArticole_RowsAdded;
            grpArticole.Controls.Add(grdArticole);
            tab.Controls.Add(grpArticole);

            // GRP 4: Totalizare si Aprobare
            grpTotalizare = MakeGroup("4. Totaluri", 8, 362, 460, 76);
            MakeLabel("Total solicitat:", 8, 22, grpTotalizare);
            txtTotalSolicitat = MakeText(100, 19, 110, grpTotalizare, ro: true, val: "0.00");
            MakeLabel("Avans primit:", 220, 22, grpTotalizare);
            txtAvansPrimit = MakeText(300, 19, 110, grpTotalizare, ro: false, val: "0.00");
            MakeLabel("Diferenta:", 8, 50, grpTotalizare);
            txtDiferenta = MakeText(100, 47, 110, grpTotalizare, ro: true, val: "0.00");
            MakeLabel("Aprobat de:", 220, 50, grpTotalizare);
            cboDirector = MakeCombo(295, 47, 157, grpTotalizare);
            tab.Controls.Add(grpTotalizare);

            // GRP 5: Canal Plata
            grpDirector = MakeGroup("5. Canal Plata", 8, 446, 460, 62);
            rdoNumerar = MakeRadio("Numerar [Caserie]", 8, 18, true, grpDirector);
            rdoTransfer = MakeRadio("Transfer Bancar", 160, 18, false, grpDirector);
            MakeLabel("IBAN angajat:", 8, 42, grpDirector);
            cboIBAN = MakeCombo(90, 39, 362, grpDirector);
            cboIBAN.Enabled = false;
            tab.Controls.Add(grpDirector);

            // GRP 6: Document Generat
            grpDocGenerat = MakeGroup("6. Document Generat", 8, 516, 460, 62);
            rdoChitanta = MakeRadio("Chitanta Decont", 8, 18, true, grpDocGenerat);
            rdoOrdinPlata = MakeRadio("Ordin de Plata Decont", 160, 18, false, grpDocGenerat);
            rdoChitanta.Enabled = false;
            rdoOrdinPlata.Enabled = false;
            var lblInfoDoc = new Label
            {
                Text = "Serie, numar si data generate automat de sistem la salvare.",
                Location = new Point(8, 40),
                AutoSize = true,
                Font = new Font("Arial", 7.5f, FontStyle.Italic),
                ForeColor = Color.Gray
            };
            grpDocGenerat.Controls.Add(lblInfoDoc);
            tab.Controls.Add(grpDocGenerat);

            // PANEL DREAPTA
            grpGrid = MakeGroup("6. Deconturi anterioare", 476, 8, 524, 460);
            MakeLabel("Filtru:", 340, 14, grpGrid);
            cboFiltruStare = MakeCombo(374, 11, 140, grpGrid);
            cboFiltruStare.Items.AddRange(new object[] { "Toate", "Depus", "Aprobat", "Respins" });
            cboFiltruStare.SelectedIndex = 0;
            grdDeconturi = new DataGridView
            {
                Location = new Point(8, 34),
                Size = new Size(508, 414),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowHeadersVisible = false,
                Font = new Font("Arial", 8.5f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = HEADER_COLOR,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 8.5f, FontStyle.Bold)
                }
            };
            grdDeconturi.RowPrePaint += GrdDeconturi_RowPrePaint;
            grpGrid.Controls.Add(grdDeconturi);
            tab.Controls.Add(grpGrid);

            // Totaluri luna
            grpTotaluri = MakeGroup("Totaluri luna curenta", 476, 476, 524, 62);
            MakeLabel("Deconturi depuse:", 8, 20, grpTotaluri);
            txtNrDepuse = MakeText(130, 17, 60, grpTotaluri, ro: true, val: "0");
            MakeLabel("Deconturi aprobate:", 200, 20, grpTotaluri);
            txtNrAprobate = MakeText(320, 17, 60, grpTotaluri, ro: true, val: "0");
            MakeLabel("Total aprobat luna:", 8, 42, grpTotaluri);
            txtTotalAprobat = MakeText(130, 39, 180, grpTotaluri, ro: true, val: "0.00");
            tab.Controls.Add(grpTotaluri);

            // BUTOANE
            var pnl = new Panel
            {
                Location = new Point(0, 586),
                Size = new Size(1006, 44),
                BackColor = Color.FromArgb(232, 232, 232)
            };
            btnAdaugare = MakeButton("Adaugare", 8, 8, 130, 28, pnl, Color.FromArgb(31, 107, 53));
            btnAprobare = MakeButton("Aprobare", 146, 8, 130, 28, pnl, Color.FromArgb(189, 119, 0));
            btnRespingere = MakeButton("Respingere", 284, 8, 130, 28, pnl, Color.FromArgb(192, 0, 0));
            btnSalvare = MakeButton("Salvare", 422, 8, 130, 28, pnl, Color.FromArgb(20, 80, 150));
            btnRenuntare = MakeButton("Renuntare", 560, 8, 130, 28, pnl, Color.FromArgb(80, 80, 80));
            tab.Controls.Add(pnl);

            lblMotivRespingere = MakeLabel("Motiv respingere:", 8, 516, tab);
            lblMotivRespingere.Visible = false;
            txtMotivRespingere = MakeText(120, 513, 340, tab);
            txtMotivRespingere.Visible = false;
        }

        private void BuildTabIstoric()
        {
            tabIstoric.Controls.Add(new Label
            {
                Text = "Tab Istoric Deconturi — toate deconturile inregistrate cu filtre extinse.",
                Location = new Point(16, 16),
                AutoSize = true,
                Font = new Font("Arial", 9f),
                ForeColor = Color.Gray
            });
            grdIstoric = new DataGridView
            {
                Location = new Point(8, 46),
                Size = new Size(990, 610),
                ReadOnly = true,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                Font = new Font("Arial", 8.5f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = HEADER_COLOR,
                    ForeColor = Color.White,
                    Font = new Font("Arial", 8.5f, FontStyle.Bold)
                }
            };
            tabIstoric.Controls.Add(grdIstoric);
        }

        private void BuildTabRaport()
        {
            tabRaport.Controls.Add(new Label
            {
                Text = "Tab Raport Deconturi — genereaza raportul centralizator.",
                Location = new Point(16, 16),
                AutoSize = true,
                Font = new Font("Arial", 9f),
                ForeColor = Color.Gray
            });
            tabRaport.Controls.Add(new Button
            {
                Text = "Genereaza Raport",
                Location = new Point(16, 46),
                Size = new Size(150, 28),
                BackColor = Color.FromArgb(31, 107, 53),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            });
        }

        private void WireEvents()
        {
            cboAngajat.SelectedIndexChanged += (s, e) => {
                if (cboAngajat.SelectedIndex >= 0 && cboAngajat.SelectedIndex < cboNumeAngajat.Items.Count)
                {
                    cboNumeAngajat.SelectedIndexChanged -= CboNumeAngajat_Changed;
                    cboNumeAngajat.SelectedIndex = cboAngajat.SelectedIndex;
                    cboNumeAngajat.SelectedIndexChanged += CboNumeAngajat_Changed;
                    UpdateAngajatFields();
                }
            };
            cboNumeAngajat.SelectedIndexChanged += CboNumeAngajat_Changed;
            cboFiltruStare.SelectedIndexChanged += (s, e) => RefreshGrid();
            rdoNumerar.CheckedChanged += Canal_Changed;
            rdoTransfer.CheckedChanged += Canal_Changed;
            btnAdaugare.Click += BtnAdaugare_Click;
            btnAprobare.Click += BtnAprobare_Click;
            btnRespingere.Click += BtnRespingere_Click;
            btnSalvare.Click += BtnSalvare_Click;
            btnRenuntare.Click += BtnRenuntare_Click;
            this.KeyDown += FormDecont_KeyDown;
            txtAvansPrimit.TextChanged += (s, e) => RecalculeazaTotal();
        }

        private void CboNumeAngajat_Changed(object sender, EventArgs e)
        {
            if (cboNumeAngajat.SelectedIndex >= 0)
            {
                cboAngajat.SelectedIndexChanged -= null;
                cboAngajat.SelectedIndex = cboNumeAngajat.SelectedIndex;
            }
            UpdateAngajatFields();
        }

        private void UpdateAngajatFields()
        {
            if (!(cboNumeAngajat.SelectedItem is ComboItem a)) return;
            _ctrl.OnAngajatSelected(int.Parse(a.Cod), a.Denumire, a.Extra);
            txtFunctie.Text = a.Extra;
            txtDepartament.Text = a.Extra == "Sofer" ? "Transport" :
                                  a.Extra == "AngajatRH" ? "Resurse Umane" :
                                  a.Extra == "DirectorFinanciar" ? "Financiar" : "";
            RefreshGrid();
            RefreshTotaluri();
        }

        private void Canal_Changed(object sender, EventArgs e)
        {
            bool transfer = rdoTransfer.Checked;
            cboIBAN.Enabled = transfer;
            lblCanal.Text = "Canal: " + (transfer ? "Transfer Bancar" : "Numerar");
            rdoChitanta.Checked = !transfer;
            rdoOrdinPlata.Checked = transfer;
        }

        private void BtnAdaugare_Click(object sender, EventArgs e)
        {
            _ctrl.DocumentNou();
            SetModeAdaugare();
            txtSerie.Text = "DC";
            txtNumar.Text = _ctrl.GetNumarGenerat();
        }

        private void BtnAprobare_Click(object sender, EventArgs e)
        {
            if (grdDeconturi.CurrentRow == null) return;
            string serie = grdDeconturi.CurrentRow.Cells["Serie"].Value?.ToString();
            string numar = grdDeconturi.CurrentRow.Cells["Nr."].Value?.ToString();
            if (_ctrl.AprobazaDecont(serie, numar))
            {
                RefreshGrid();
                RefreshTotaluri();
            }
        }

        private void BtnRespingere_Click(object sender, EventArgs e)
        {
            if (grdDeconturi.CurrentRow == null) return;
            string serie = grdDeconturi.CurrentRow.Cells["Serie"].Value?.ToString();
            string numar = grdDeconturi.CurrentRow.Cells["Nr."].Value?.ToString();

            string motiv = Microsoft.VisualBasic.Interaction.InputBox(
                "Introduceti motivul respingerii:", "Respingere Decont", "");

            if (string.IsNullOrWhiteSpace(motiv)) return;

            if (_ctrl.RespingeDecont(serie, numar, motiv))
            {
                MessageBox.Show("Decontul a fost respins!", "Succes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshGrid();
                RefreshTotaluri();
            }
        }

        private void BtnSalvare_Click(object sender, EventArgs e)
        {
            var articole = new List<ArticolDecont>();
            foreach (DataGridViewRow row in grdArticole.Rows)
            {
                if (row.IsNewRow) continue;
                if (!decimal.TryParse(row.Cells["Valoare"].Value?.ToString(), out decimal val)) continue;
                articole.Add(new ArticolDecont
                {
                    TipCheltuiala = row.Cells["TipCheltuiala"].Value?.ToString(),
                    DocumentJustificativ = row.Cells["DocumentJustificativ"].Value?.ToString(),
                    Valoare = val,
                    Moneda = row.Cells["Moneda"].Value?.ToString() ?? "RON"
                });
            }

            int codDirector = cboDirector.SelectedItem is ComboItem d ? int.Parse(d.Cod) : 0;
            string canal = rdoNumerar.Checked ? "Numerar" : "Transfer Bancar";

            bool saved = _ctrl.SalveazaDecont(
                dtpPerioadaStart.Value.Date, dtpPerioadaEnd.Value.Date,
                articole, codDirector, canal, dtpData.Value.Date);

            if (saved)
            {
                MessageBox.Show("Decontul a fost inregistrat cu succes!", "Succes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshGrid();
                RefreshTotaluri();
                SetModeVizualizare();
            }
        }

        private void BtnRenuntare_Click(object sender, EventArgs e)
        {
            _ctrl.Renunta();
            SetModeVizualizare();
        }

        private void FormDecont_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A) { BtnAdaugare_Click(null, null); e.Handled = true; }
            if (e.Control && e.KeyCode == Keys.P) { BtnAprobare_Click(null, null); e.Handled = true; }
            if (e.Control && e.KeyCode == Keys.R) { BtnRespingere_Click(null, null); e.Handled = true; }
            if (e.Control && e.KeyCode == Keys.S) { BtnSalvare_Click(null, null); e.Handled = true; }
            if (e.KeyCode == Keys.Escape) { BtnRenuntare_Click(null, null); e.Handled = true; }
        }

        private void GrdDeconturi_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            grdDeconturi.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                e.RowIndex == grdDeconturi.CurrentCell?.RowIndex
                    ? Color.FromArgb(189, 215, 238)
                    : (e.RowIndex % 2 == 0 ? Color.FromArgb(240, 244, 250) : Color.White);
        }

        private void GrdArticole_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            RecalculeazaTotal();
        }

        private void GrdArticole_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            RecalculeazaTotal();
        }

        private void RecalculeazaTotal()
        {
            decimal total = 0;
            foreach (DataGridViewRow row in grdArticole.Rows)
            {
                if (row.IsNewRow) continue;
                if (decimal.TryParse(row.Cells["Valoare"].Value?.ToString(), out decimal val))
                    total += val;
            }
            txtTotalSolicitat.Text = total.ToString("F2");
            if (decimal.TryParse(txtAvansPrimit.Text.Replace(",", "."), out decimal avans))
                txtDiferenta.Text = (total - avans).ToString("F2");
        }

        private void SetModeVizualizare()
        {
            _modAdaugare = false;
            SetFieldsEnabled(false);
            btnSalvare.Enabled = false; btnRenuntare.Enabled = false;
            btnAdaugare.Enabled = true; btnAprobare.Enabled = true;
            btnRespingere.Enabled = true;
            lblStare.Text = "Stare: Vizualizare  |  Campuri ReadOnly";
            ClearFields();
        }

        private void SetModeAdaugare()
        {
            _modAdaugare = true;
            SetFieldsEnabled(true);
            txtTotalSolicitat.ReadOnly = true; txtTotalSolicitat.BackColor = READONLY_COLOR;
            txtDiferenta.ReadOnly = true; txtDiferenta.BackColor = READONLY_COLOR;
            txtFunctie.ReadOnly = true; txtFunctie.BackColor = READONLY_COLOR;
            txtDepartament.ReadOnly = true; txtDepartament.BackColor = READONLY_COLOR;
            btnSalvare.Enabled = true; btnRenuntare.Enabled = true;
            btnAdaugare.Enabled = false;
            lblStare.Text = "Stare: Adaugare  |  Completati campurile si apasati Salvare [Ctrl+S]";
            cboNumeAngajat.Focus();
        }

        private void SetFieldsEnabled(bool on)
        {
            cboAngajat.Enabled = on;
            cboNumeAngajat.Enabled = on;
            dtpData.Enabled = on; dtpPerioadaStart.Enabled = on; dtpPerioadaEnd.Enabled = on;
            grdArticole.Enabled = on;
            cboDirector.Enabled = on;
            rdoNumerar.Enabled = on; rdoTransfer.Enabled = on;
        }

        private void LoadAngajati()
        {
            cboAngajat.Items.Clear();
            cboNumeAngajat.Items.Clear();
            var data = _ctrl.GetFormData();
            foreach (var a in data.GetListaAngajati().OrderBy(x => x.IdAngajat))
            {
                cboAngajat.Items.Add(new ComboItem(a.IdAngajat.ToString(), a.IdAngajat.ToString(), a.Functie));
                cboNumeAngajat.Items.Add(new ComboItem(a.IdAngajat.ToString(), $"{a.Nume} {a.Prenume}", a.Functie));
            }
            if (cboAngajat.Items.Count > 0) cboAngajat.SelectedIndex = 0;
            if (cboNumeAngajat.Items.Count > 0) cboNumeAngajat.SelectedIndex = 0;

            cboDirector.Items.Clear();
            foreach (var d in data.GetListaDirectori())
                cboDirector.Items.Add(new ComboItem(d.IdAngajat.ToString(), $"{d.Nume} {d.Prenume}", d.Functie));
            if (cboDirector.Items.Count > 0) cboDirector.SelectedIndex = 0;

            var conturi = data.GetMasterRepo().FindConturiBancareAll();
            foreach (var c in conturi)
                cboIBAN.Items.Add(c.IBAN);
        }

        private void RefreshGrid()
        {
            var deconturi = _ctrl.GetFormData().GetDeconturi();
            var dt = new DataTable();
            dt.Columns.AddRange(new[] {
                new DataColumn("Serie"), new DataColumn("Nr."),
                new DataColumn("Data"), new DataColumn("Perioada"),
                new DataColumn("Total"), new DataColumn("Stare")
            });
            string filtru = cboFiltruStare?.SelectedItem?.ToString() ?? "Toate";
            foreach (var d in deconturi)
            {
                if (filtru != "Toate" && d.Stare != filtru) continue;
                dt.Rows.Add(d.Serie, d.Numar,
                    d.DataDocument.ToString("dd/MM/yyyy"),
                    $"{d.PerioadaStart:dd/MM/yy} - {d.PerioadaEnd:dd/MM/yy}",
                    d.ValoareDecontata.ToString("F2"), d.Stare);
            }
            grdDeconturi.DataSource = dt;
        }

        private void RefreshTotaluri()
        {
            _ctrl.GetFormData().RefreshTotaluri();
            txtNrDepuse.Text = _ctrl.GetFormData().GetNrDeponturiDepuse().ToString();
            txtNrAprobate.Text = _ctrl.GetFormData().GetNrDeconturiAprobate().ToString();
            txtTotalAprobat.Text = _ctrl.GetFormData().GetTotalAprobatLuna().ToString("F2");
        }

        private void ClearFields()
        {
            txtSerie.Text = "DC"; txtNumar.Text = "";
            txtTotalSolicitat.Text = "0.00"; txtAvansPrimit.Text = "0.00"; txtDiferenta.Text = "0.00";
            txtFunctie.Text = ""; txtDepartament.Text = "";
            grdArticole.Rows.Clear();
            txtMotivRespingere.Text = "";
            txtMotivRespingere.Visible = false;
            lblMotivRespingere.Visible = false;
        }

        private GroupBox MakeGroup(string text, int x, int y, int w, int h)
            => new GroupBox
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                Font = new Font("Arial", 8.5f, FontStyle.Bold),
                ForeColor = HEADER_COLOR,
                BackColor = Color.FromArgb(250, 250, 250)
            };

        private Label MakeLabel(string text, int x, int y, Control parent = null)
        {
            var l = new Label { Text = text, Location = new Point(x, y), AutoSize = true, Font = new Font("Arial", 8.5f) };
            parent?.Controls.Add(l); return l;
        }

        private TextBox MakeText(int x, int y, int w, Control parent = null, bool ro = false, string val = "")
        {
            var tb = new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 21),
                ReadOnly = ro,
                BackColor = ro ? READONLY_COLOR : WHITE,
                Font = new Font("Arial", 8.5f),
                Text = val
            };
            parent?.Controls.Add(tb); return tb;
        }

        private ComboBox MakeCombo(int x, int y, int w, Control parent = null)
        {
            var cb = new ComboBox
            {
                Location = new Point(x, y),
                Size = new Size(w, 21),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 8.5f)
            };
            parent?.Controls.Add(cb); return cb;
        }

        private DateTimePicker MakeDtp(int x, int y, int w, Control parent = null)
        {
            var dtp = new DateTimePicker
            {
                Location = new Point(x, y),
                Size = new Size(w, 21),
                Format = DateTimePickerFormat.Short,
                Font = new Font("Arial", 8.5f)
            };
            parent?.Controls.Add(dtp); return dtp;
        }

        private RadioButton MakeRadio(string text, int x, int y, bool chk, Control parent = null)
        {
            var rb = new RadioButton
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Checked = chk,
                Font = new Font("Arial", 8.5f)
            };
            parent?.Controls.Add(rb); return rb;
        }

        private Button MakeButton(string text, int x, int y, int w, int h, Control parent, Color bg)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(w, h),
                BackColor = bg,
                ForeColor = Color.White,
                UseVisualStyleBackColor = false,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.BorderColor = bg;
            btn.Paint += (s, ev) => {
                ev.Graphics.FillRectangle(new SolidBrush(bg), ev.ClipRectangle);
                TextRenderer.DrawText(ev.Graphics, text, btn.Font, ev.ClipRectangle, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            };
            parent?.Controls.Add(btn);
            return btn;
        }

        // Declaratii controale
        private TabControl tabControl;
        private TabPage tabInregistrare, tabIstoric, tabRaport;
        private GroupBox grpAngajat, grpDecont, grpArticole, grpDirector, grpGrid, grpTotaluri, grpTotalizare, grpDocGenerat;
        private ComboBox cboAngajat, cboNumeAngajat, cboDirector, cboIBAN, cboStareDecont, cboFiltruStare;
        private RadioButton rdoNumerar, rdoTransfer, rdoChitanta, rdoOrdinPlata;
        private TextBox txtSerie, txtNumar, txtFunctie, txtDepartament;
        private TextBox txtTotalSolicitat, txtAvansPrimit, txtDiferenta;
        private TextBox txtNrDepuse, txtNrAprobate, txtTotalAprobat;
        private TextBox txtMotivRespingere;
        private DateTimePicker dtpData, dtpPerioadaStart, dtpPerioadaEnd;
        private Button btnAdaugare, btnAprobare, btnRespingere, btnSalvare, btnRenuntare;
        private DataGridView grdArticole, grdDeconturi, grdIstoric;
        private Label lblMotivRespingere;
        private ToolStripStatusLabel lblStare, lblCanal, lblLuna, lblUtilizator;
    }
}