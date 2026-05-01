using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    // ═══════════════════════════════════════════════════════════════════════
    // FormRetur — VIEW in pattern-ul MVC
    // Nu contine logica de business — doar afiseaza date si trimite
    // evenimente catre Controller (ReturFormCtrl).
    // Echivalent cu formularul GUI din modelul Java/JPA (ghid PSI Partea 4)
    // ═══════════════════════════════════════════════════════════════════════
    public partial class FormRetur : Form
    {
        // Controller-ul formularului — singurul punct de contact cu logica
        private readonly ReturFormCtrl _ctrl;
        private bool _modAdaugare = false;

        private readonly Color READONLY_COLOR = Color.FromArgb(245, 245, 245);
        private readonly Color WHITE          = Color.White;
        private readonly Color ERR_COLOR      = Color.FromArgb(255, 240, 240);
        private readonly Color HEADER_COLOR   = Color.FromArgb(31, 56, 100);

        public FormRetur()
        {
            _ctrl = new ReturFormCtrl();
            BuildUI();
            WireEvents();
            LoadParteneri();
            SetModeVizualizare();
        }

        // ══════════════════════════════════════════════════════════════════
        // BUILD UI
        // ══════════════════════════════════════════════════════════════════
        private void BuildUI()
        {
            this.Text = "Inregistrare Retur — SC Transilvania General Import-Export SRL";
            this.Size = new Size(1024, 768);
            this.MinimumSize = new Size(1024, 768);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.KeyPreview = true;

            var status = new StatusStrip { Dock = DockStyle.Bottom, BackColor = Color.FromArgb(232, 232, 232) };
            lblStare      = new ToolStripStatusLabel("Stare: Vizualizare") { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblCanal      = new ToolStripStatusLabel("Canal: Numerar") { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblLuna       = new ToolStripStatusLabel("Luna: " + DateTime.Now.ToString("MMMM yyyy")) { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblUtilizator = new ToolStripStatusLabel("Utilizator: casier01");
            status.Items.AddRange(new ToolStripItem[] { lblStare, lblCanal, lblLuna, lblUtilizator });
            this.Controls.Add(status);

            tabControl      = new TabControl { Dock = DockStyle.Fill, Font = new Font("Arial", 9f) };
            tabInregistrare = new TabPage("Inregistrare")    { BackColor = Color.FromArgb(240, 240, 240) };
            tabIstoric      = new TabPage("Istoric Retururi") { BackColor = Color.FromArgb(240, 240, 240) };
            tabRaport       = new TabPage("Raport Retururi")  { BackColor = Color.FromArgb(240, 240, 240) };
            tabControl.TabPages.AddRange(new TabPage[] { tabInregistrare, tabIstoric, tabRaport });
            this.Controls.Add(tabControl);

            BuildTabInregistrare();
            BuildTabIstoric();
            BuildTabRaport();
        }

        private void BuildTabInregistrare()
        {
            var tab = tabInregistrare;

            // GRP 1: Tip Retur
            grpTipRetur = MakeGroup("1. Tip Retur", 8, 8, 460, 62);
            MakeLabel("Tip:", 8, 22, grpTipRetur);
            rdoClient   = MakeRadio("Retur Client",   44, 18, true,  grpTipRetur);
            rdoFurnizor = MakeRadio("Retur Furnizor", 160, 18, false, grpTipRetur);
            MakeLabel("Partener:", 8, 42, grpTipRetur);
            cboPartener = MakeCombo(72, 39, 378, grpTipRetur);
            tab.Controls.Add(grpTipRetur);

            // GRP 2: Factura Initiala
            grpFactInit = MakeGroup("2. Factura Initiala", 8, 76, 460, 76);
            MakeLabel("Selectati factura:", 8, 22, grpFactInit);
            cboFactura = MakeCombo(104, 19, 346, grpFactInit);
            cboFactura.DropDownWidth = 420;
            MakeLabel("Data factura:", 8, 50, grpFactInit);
            txtDataFactInit   = MakeText(84, 47, 100, grpFactInit, ro: true);
            MakeLabel("Val. totala:", 192, 50, grpFactInit);
            txtValTotala      = MakeText(262, 47, 90, grpFactInit, ro: true);
            MakeLabel("Rest disponibil:", 360, 50, grpFactInit);
            txtRestDisponibil = MakeText(448, 47, 90, grpFactInit, ro: true);
            tab.Controls.Add(grpFactInit);

            // GRP 3: Date FacturaRetur
            grpFactRetur = MakeGroup("3. Date FacturaRetur", 8, 158, 460, 94);
            MakeLabel("Serie:",  8, 22, grpFactRetur);
            txtSerieRetur = MakeText(50, 19, 70, grpFactRetur, ro: true);
            MakeLabel("Numar:", 128, 22, grpFactRetur);
            txtNumarRetur = MakeText(172, 19, 86, grpFactRetur, ro: true);
            MakeLabel("Data:", 266, 22, grpFactRetur);
            dtpDataRetur  = MakeDtp(294, 19, 158, grpFactRetur);
            MakeLabel("Motiv:", 8, 48, grpFactRetur);
            cboMotiv = MakeCombo(50, 45, 240, grpFactRetur);
            cboMotiv.Items.AddRange(new object[] {
                "Marfa deteriorata", "Eroare facturare",
                "Surplus", "Calitate necorespunzatoare", "Altul" });
            cboMotiv.SelectedIndex = 0;
            MakeLabel("Stare:", 298, 48, grpFactRetur);
            cboStare = MakeCombo(334, 45, 118, grpFactRetur);
            cboStare.Items.AddRange(new object[] { "Emis", "In curs", "Finalizat", "Anulat" });
            cboStare.SelectedIndex = 0;
            MakeLabel("Val. retur:", 8, 72, grpFactRetur);
            txtValRetur = MakeText(72, 69, 120, grpFactRetur);
            tab.Controls.Add(grpFactRetur);

            // GRP 4: Canal
            grpCanal = MakeGroup("4. Canal Plata / Incasare", 8, 258, 460, 60);
            MakeLabel("Canal:", 8, 22, grpCanal);
            rdoNumerar = MakeRadio("Numerar",     56, 18, true,  grpCanal);
            rdoIBAN    = MakeRadio("Card / IBAN", 154, 18, false, grpCanal);
            lblCaserie = MakeLabel("Caserie:", 8, 42, grpCanal);
            cboCaserie = MakeCombo(64, 39, 180, grpCanal);
            cboCaserie.Items.Add("Casierie principala");
            cboCaserie.SelectedIndex = 0;
            lblSold  = MakeLabel("Sold:", 252, 42, grpCanal);
            txtSold  = MakeText(284, 39, 166, grpCanal, ro: true, val: "12500.00");
            lblIBAN  = MakeLabel("IBAN cont firma:", 8, 42, grpCanal);
            cboIBAN  = MakeCombo(112, 39, 338, grpCanal);
            lblIBAN.Visible = false;
            cboIBAN.Visible = false;
            tab.Controls.Add(grpCanal);

            // GRP 5: Document Generat
            grpDoc = MakeGroup("5. Document Generat", 8, 324, 460, 82);
            rdoChitanta = MakeRadio("Chitanta Retur",    16, 18, true,  grpDoc);
            rdoOP       = MakeRadio("Ordin Plata Retur", 170, 18, false, grpDoc);
            rdoOP.Visible = false;
            MakeLabel("Nr. document:", 8, 42, grpDoc);
            txtNrDoc       = MakeText(96, 39, 130, grpDoc, ro: true);
            MakeLabel("Data emitere:", 234, 42, grpDoc);
            txtDataEmitere = MakeText(318, 39, 134, grpDoc, ro: true);
            var lblAutoGen = MakeLabel("(generate automat la salvare)", 8, 62, grpDoc);
            lblAutoGen.ForeColor = Color.Gray;
            lblAutoGen.Font = new Font("Arial", 7.5f, FontStyle.Italic);
            tab.Controls.Add(grpDoc);

            // PANEL DREAPTA
            grpGrid = MakeGroup("6. Retururi anterioare", 476, 8, 524, 380);
            grdRetururi = new DataGridView
            {
                Location = new System.Drawing.Point(8, 18),
                Size = new System.Drawing.Size(508, 300),
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                RowHeadersVisible = false, Font = new Font("Arial", 8.5f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = HEADER_COLOR, ForeColor = Color.White,
                    Font = new Font("Arial", 8.5f, FontStyle.Bold)
                }
            };
            grdRetururi.RowPrePaint += GrdRetururi_RowPrePaint;
            grpGrid.Controls.Add(grdRetururi);
            grpGrid.Controls.Add(new Label
            {
                Text = "* Filtrat dupa partener si tipRetur curent.",
                Location = new System.Drawing.Point(8, 322), AutoSize = true,
                Font = new Font("Arial", 8f), ForeColor = Color.Gray
            });
            tab.Controls.Add(grpGrid);

            grpTotaluri = MakeGroup("Totaluri luna curenta", 476, 396, 524, 66);
            MakeLabel("Total valoare retur luna:", 8, 20, grpTotaluri);
            txtTotalLuna  = MakeText(192, 17, 150, grpTotaluri, ro: true, val: "0.00");
            MakeLabel("Nr. retururi luna:", 8, 44, grpTotaluri);
            txtNrRetururi = MakeText(192, 41, 150, grpTotaluri, ro: true, val: "0");
            tab.Controls.Add(grpTotaluri);

            // BUTOANE
            var pnl = new Panel
            {
                Location = new System.Drawing.Point(0, 476),
                Size = new System.Drawing.Size(1006, 44),
                BackColor = Color.FromArgb(232, 232, 232)
            };
            pnl.Controls.Add(new Label { Location = new System.Drawing.Point(0, 0), Size = new System.Drawing.Size(1006, 1), BackColor = Color.Silver });
            btnAdaugare  = MakeButton("Adaugare",  8,   8, 150, 28, pnl, Color.FromArgb(31, 107, 53));
            btnAnulare   = MakeButton("Anulare",   166, 8, 150, 28, pnl, Color.FromArgb(192, 0, 0));
            pnl.Controls.Add(new Label { Location = new System.Drawing.Point(324, 4), Size = new System.Drawing.Size(1, 36), BackColor = Color.Silver });
            btnSalvare   = MakeButton("Salvare",   332, 8, 150, 28, pnl, HEADER_COLOR);
            btnRenuntare = MakeButton("Renuntare", 490, 8, 150, 28, pnl, Color.FromArgb(119, 119, 119));
            tab.Controls.Add(pnl);
        }

        private void BuildTabIstoric()
        {
            tabIstoric.Controls.Add(new Label
            {
                Text = "Tab Istoric Retururi — toate retururile inregistrate cu filtre extinse.",
                Location = new System.Drawing.Point(16, 16), AutoSize = true,
                Font = new Font("Arial", 9f), ForeColor = Color.Gray
            });
            grdIstoric = new DataGridView
            {
                Location = new System.Drawing.Point(8, 46), Size = new System.Drawing.Size(990, 610),
                ReadOnly = true, AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = Color.White, RowHeadersVisible = false,
                Font = new Font("Arial", 8.5f),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
                {
                    BackColor = HEADER_COLOR, ForeColor = Color.White,
                    Font = new Font("Arial", 8.5f, FontStyle.Bold)
                }
            };
            tabIstoric.Controls.Add(grdIstoric);
        }

        private void BuildTabRaport()
        {
            tabRaport.Controls.Add(new Label
            {
                Text = "Tab Raport Retururi — genereaza raportul centralizator.",
                Location = new System.Drawing.Point(16, 16), AutoSize = true,
                Font = new Font("Arial", 9f), ForeColor = Color.Gray
            });
            tabRaport.Controls.Add(new Button
            {
                Text = "Genereaza Raport", Location = new System.Drawing.Point(16, 46),
                Size = new System.Drawing.Size(150, 28),
                BackColor = Color.FromArgb(31, 107, 53), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            });
        }

        // ══════════════════════════════════════════════════════════════════
        // EVENTS — VIEW trimite evenimente catre CONTROLLER
        // ══════════════════════════════════════════════════════════════════
        private void WireEvents()
        {
            rdoClient.CheckedChanged   += TipRetur_Changed;
            rdoFurnizor.CheckedChanged += TipRetur_Changed;
            rdoNumerar.CheckedChanged  += Canal_Changed;
            rdoIBAN.CheckedChanged     += Canal_Changed;
            cboPartener.SelectedIndexChanged += CboPartener_Changed;
            cboFactura.SelectedIndexChanged  += CboFactura_Changed;
            btnAdaugare.Click  += BtnAdaugare_Click;
            btnAnulare.Click   += BtnAnulare_Click;
            btnSalvare.Click   += BtnSalvare_Click;
            btnRenuntare.Click += BtnRenuntare_Click;
            txtValRetur.TextChanged += TxtValRetur_Changed;
            this.KeyDown += FormRetur_KeyDown;
        }

        private void TipRetur_Changed(object sender, EventArgs e)
        {
            string tip = rdoClient.Checked ? "Client" : "Furnizor";
            _ctrl.OnTipReturChanged(tip);
            LoadParteneri();
            RefreshGrid();
            UpdateDocumentGenerat();
        }

        private void Canal_Changed(object sender, EventArgs e)
        {
            bool num = rdoNumerar.Checked;
            lblCaserie.Visible = num; cboCaserie.Visible = num;
            lblSold.Visible    = num; txtSold.Visible    = num;
            lblIBAN.Visible    = !num; cboIBAN.Visible   = !num;
            UpdateDocumentGenerat();
            lblCanal.Text = "Canal: " + (num ? "Numerar" : "Card / IBAN");
        }

        private void CboPartener_Changed(object sender, EventArgs e)
        {
            if (!(cboPartener.SelectedItem is ComboItem p)) return;
            // Notifica Controller-ul — VIEW nu accesează direct BD
            _ctrl.OnPartenerSelected(int.Parse(p.Cod), p.Denumire);
            LoadFacturiPartener();
            RefreshGrid();
            RefreshTotaluri();
        }

        private void CboFactura_Changed(object sender, EventArgs e)
        {
            if (!(cboFactura.SelectedItem is FacturaItem fi)) return;
            // Notifica Controller-ul
            _ctrl.OnFacturaSelected(fi.Serie, fi.Numar, fi.DataDocument, fi.ValoareTotala);
            // VIEW actualizeaza campurile ReadOnly din ModelAdapter
            txtDataFactInit.Text   = fi.DataDocument.ToShortDateString();
            txtValTotala.Text      = fi.ValoareTotala.ToString("F2");
            txtRestDisponibil.Text = _ctrl.GetFormData().GetRestDisponibil().ToString("F2");
            txtValRetur.BackColor  = WHITE;
        }

        private void BtnAdaugare_Click(object sender, EventArgs e)
        {
            // Controller creeaza documentul nou
            _ctrl.DocumentNou();
            SetModeAdaugare();
            txtSerieRetur.Text = "RET";
            txtNumarRetur.Text = _ctrl.GetNumarGenerat();
        }

        private void BtnAnulare_Click(object sender, EventArgs e)
        {
            if (grdRetururi.CurrentRow == null || grdRetururi.CurrentRow.Index < 0) return;
            string serie = grdRetururi.CurrentRow.Cells["Serie"].Value?.ToString();
            string numar = grdRetururi.CurrentRow.Cells["Nr."].Value?.ToString();
            if (_ctrl.AnuleazaRetur(serie, numar))
            {
                RefreshGrid();
                RefreshTotaluri();
                CboFactura_Changed(null, null);
            }
        }

        private void BtnSalvare_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtValRetur.Text, out decimal val))
            { MessageBox.Show("Valoarea returului nu este valida.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            string canal = rdoNumerar.Checked ? "Numerar" : "Card/IBAN";
            bool saved = _ctrl.SalveazaRetur(
                cboMotiv.Text, cboStare.Text, val, canal, dtpDataRetur.Value.Date);

            if (saved)
            {
                txtNrDoc.Text       = $"CR-{txtNumarRetur.Text}";
                txtDataEmitere.Text = DateTime.Now.ToShortDateString();
                MessageBox.Show("Returul a fost inregistrat cu succes!", "Succes",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadFacturiPartener();
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

        private void TxtValRetur_Changed(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtValRetur.Text, out decimal val)) return;
            decimal rest = _ctrl.GetFormData().GetRestDisponibil();
            txtValRetur.BackColor = (rest > 0 && val > rest) ? ERR_COLOR : WHITE;
        }

        private void FormRetur_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A) { BtnAdaugare_Click(null, null);  e.Handled = true; }
            if (e.Control && e.KeyCode == Keys.N) { BtnAnulare_Click(null, null);   e.Handled = true; }
            if (e.Control && e.KeyCode == Keys.S) { BtnSalvare_Click(null, null);   e.Handled = true; }
            if (e.KeyCode == Keys.Escape)          { BtnRenuntare_Click(null, null); e.Handled = true; }
        }

        private void GrdRetururi_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            grdRetururi.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                e.RowIndex == grdRetururi.CurrentCell?.RowIndex
                    ? Color.FromArgb(189, 215, 238)
                    : (e.RowIndex % 2 == 0 ? Color.FromArgb(240, 244, 250) : Color.White);
        }

        // ══════════════════════════════════════════════════════════════════
        // MODURI DE LUCRU
        // ══════════════════════════════════════════════════════════════════
        private void SetModeVizualizare()
        {
            _modAdaugare = false;
            SetFieldsEnabled(false);
            btnSalvare.Enabled = false; btnRenuntare.Enabled = false;
            btnAdaugare.Enabled = true; btnAnulare.Enabled = true;
            lblStare.Text = "Stare: Vizualizare  |  Campuri ReadOnly";
            ClearFields();
        }

        private void SetModeAdaugare()
        {
            _modAdaugare = true;
            SetFieldsEnabled(true);
            foreach (var tb in new[] { txtSerieRetur, txtNumarRetur, txtNrDoc,
                                       txtDataEmitere, txtDataFactInit,
                                       txtValTotala, txtRestDisponibil })
            { tb.ReadOnly = true; tb.BackColor = READONLY_COLOR; }
            btnSalvare.Enabled = true; btnRenuntare.Enabled = true;
            btnAdaugare.Enabled = false;
            lblStare.Text = "Stare: Adaugare  |  Completati campurile si apasati Salvare [Ctrl+S]";
            cboPartener.Focus();
        }

        private void SetFieldsEnabled(bool on)
        {
            cboPartener.Enabled = on; cboFactura.Enabled = on;
            cboMotiv.Enabled = on;    cboStare.Enabled = on;
            dtpDataRetur.Enabled = on;
            rdoClient.Enabled = on;   rdoFurnizor.Enabled = on;
            rdoNumerar.Enabled = on;  rdoIBAN.Enabled = on;
            cboCaserie.Enabled = on;  cboIBAN.Enabled = on;
            txtValRetur.ReadOnly = !on;
            txtValRetur.BackColor = on ? WHITE : READONLY_COLOR;
        }

        // ══════════════════════════════════════════════════════════════════
        // INCARCARE DATE — VIEW citeste din ModelAdapter prin Controller
        // ══════════════════════════════════════════════════════════════════
        private void LoadParteneri()
        {
            cboPartener.Items.Clear();
            var data = _ctrl.GetFormData();
            var lista = data.GetTipRetur() == "Client"
                ? data.GetListaClienti().ConvertAll(c => new ComboItem(c.CodClient.ToString(), c.Nume))
                : data.GetListaFurnizori().ConvertAll(f => new ComboItem(f.CodFurnizor.ToString(), f.NumeFurnizor));
            lista.ForEach(item => cboPartener.Items.Add(item));
            if (cboPartener.Items.Count > 0) cboPartener.SelectedIndex = 0;
        }

        private void LoadFacturiPartener()
        {
            cboFactura.Items.Clear();
            var data = _ctrl.GetFormData();
            if (data.GetTipRetur() == "Client")
            {
                foreach (var fc in data.GetFacturiClient())
                {
                    decimal rest = fc.ValoareTotala - _ctrl.GetFormData().GetDocRepo().GetSumaReturnata(fc.Serie, fc.Numar);
                    if (rest > 0)
                        cboFactura.Items.Add(new FacturaItem(fc.Serie, fc.Numar, fc.DataDocument, fc.ValoareTotala, rest));
                }
            }
            else
            {
                foreach (var ff in data.GetFacturiFurnizor())
                    cboFactura.Items.Add(new FacturaItem(ff.Serie, ff.Numar, ff.DataDocument, ff.ValoareTotala, ff.ValoareTotala));
            }
            if (cboFactura.Items.Count > 0) cboFactura.SelectedIndex = 0;
            else { txtDataFactInit.Text = ""; txtValTotala.Text = ""; txtRestDisponibil.Text = ""; }
        }

        private void RefreshGrid()
        {
            var retururi = _ctrl.GetFormData().GetRetururi();
            var dt = new DataTable();
            dt.Columns.AddRange(new[] {
                new DataColumn("Serie"), new DataColumn("Nr."),
                new DataColumn("Data"),  new DataColumn("Val.Retur"), new DataColumn("Stare")
            });
            foreach (var r in retururi)
                dt.Rows.Add(r.Serie, r.Numar,
                    r.DataDocument == DateTime.MinValue ? "" : r.DataDocument.ToString("dd/MM/yyyy"),
                    r.ValoareRetur.ToString("F2"), r.StareRetur);
            grdRetururi.DataSource = dt;
        }

        private void RefreshTotaluri()
        {
            _ctrl.GetFormData().RefreshTotaluri();
            txtTotalLuna.Text  = _ctrl.GetFormData().GetTotalLuna().ToString("F2");
            txtNrRetururi.Text = _ctrl.GetFormData().GetNrRetururi().ToString();
        }

        private void UpdateDocumentGenerat()
        {
            rdoChitanta.Text = rdoNumerar.Checked ? "Chitanta Retur" : "Ordin Plata Retur";
        }

        private void ClearFields()
        {
            cboFactura.Items.Clear();
            txtDataFactInit.Text = ""; txtValTotala.Text = ""; txtRestDisponibil.Text = "";
            txtValRetur.Text = ""; txtSerieRetur.Text = ""; txtNumarRetur.Text = "";
            txtNrDoc.Text = ""; txtDataEmitere.Text = "";
            txtValRetur.BackColor = WHITE;
        }

        // ══════════════════════════════════════════════════════════════════
        // HELPERS CONSTRUCTORI CONTROALE
        // ══════════════════════════════════════════════════════════════════
        private GroupBox MakeGroup(string text, int x, int y, int w, int h)
            => new GroupBox { Text=text, Location=new System.Drawing.Point(x,y), Size=new System.Drawing.Size(w,h),
                Font=new Font("Arial",8.5f,FontStyle.Bold), ForeColor=HEADER_COLOR, BackColor=Color.FromArgb(250,250,250) };

        private Label MakeLabel(string text, int x, int y, Control parent=null)
        {
            var l = new Label { Text=text, Location=new System.Drawing.Point(x,y), AutoSize=true, Font=new Font("Arial",8.5f) };
            parent?.Controls.Add(l); return l;
        }
        private TextBox MakeText(int x, int y, int w, Control parent=null, bool ro=false, string val="")
        {
            var tb = new TextBox { Location=new System.Drawing.Point(x,y), Size=new System.Drawing.Size(w,21),
                ReadOnly=ro, BackColor=ro?READONLY_COLOR:WHITE, Font=new Font("Arial",8.5f), Text=val };
            parent?.Controls.Add(tb); return tb;
        }
        private ComboBox MakeCombo(int x, int y, int w, Control parent=null)
        {
            var cb = new ComboBox { Location=new System.Drawing.Point(x,y), Size=new System.Drawing.Size(w,21),
                DropDownStyle=ComboBoxStyle.DropDownList, Font=new Font("Arial",8.5f) };
            parent?.Controls.Add(cb); return cb;
        }
        private DateTimePicker MakeDtp(int x, int y, int w, Control parent=null)
        {
            var dtp = new DateTimePicker { Location=new System.Drawing.Point(x,y), Size=new System.Drawing.Size(w,21),
                Format=DateTimePickerFormat.Short, Font=new Font("Arial",8.5f) };
            parent?.Controls.Add(dtp); return dtp;
        }
        private RadioButton MakeRadio(string text, int x, int y, bool chk, Control parent=null)
        {
            var rb = new RadioButton { Text=text, Location=new System.Drawing.Point(x,y), AutoSize=true,
                Checked=chk, Font=new Font("Arial",8.5f) };
            parent?.Controls.Add(rb); return rb;
        }
        private Button MakeButton(string text, int x, int y, int w, int h, Control parent, Color bg)
        {
            var btn = new Button { Text=text, Location=new System.Drawing.Point(x,y), Size=new System.Drawing.Size(w,h),
                BackColor=bg, ForeColor=Color.White, FlatStyle=FlatStyle.Flat,
                Font=new Font("Arial",8.5f,FontStyle.Bold), Cursor=Cursors.Hand };
            btn.FlatAppearance.BorderSize=0;
            parent?.Controls.Add(btn); return btn;
        }

        // ══════════════════════════════════════════════════════════════════
        // DECLARATII CONTROALE
        // ══════════════════════════════════════════════════════════════════
        private TabControl tabControl;
        private TabPage tabInregistrare, tabIstoric, tabRaport;
        private GroupBox grpTipRetur, grpFactInit, grpFactRetur, grpCanal, grpDoc, grpGrid, grpTotaluri;
        private RadioButton rdoClient, rdoFurnizor, rdoNumerar, rdoIBAN, rdoChitanta, rdoOP;
        private ComboBox cboPartener, cboFactura, cboMotiv, cboStare, cboCaserie, cboIBAN;
        private TextBox txtDataFactInit, txtValTotala, txtRestDisponibil;
        private TextBox txtSerieRetur, txtNumarRetur;
        private TextBox txtValRetur, txtNrDoc, txtDataEmitere, txtSold;
        private TextBox txtTotalLuna, txtNrRetururi;
        private DateTimePicker dtpDataRetur;
        private Button btnAdaugare, btnAnulare, btnSalvare, btnRenuntare;
        private DataGridView grdRetururi, grdIstoric;
        private Label lblCaserie, lblSold, lblIBAN;
        private ToolStripStatusLabel lblStare, lblCanal, lblLuna, lblUtilizator;
    }
}
