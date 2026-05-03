using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using SIIP_Transilvania.Database;
using SIIP_Transilvania.Models;

namespace SIIP_Transilvania.Forms
{
    // ═══════════════════════════════════════════════════════════════════════
    // FormIncasare — VIEW in pattern-ul MVC
    // Nu contine logica de business — doar afiseaza date si trimite
    // evenimente catre Controller (IncasareFormCtrl).
    // Crenganiș Andreea-Bianca — Inregistrare Incasare Client
    // ═══════════════════════════════════════════════════════════════════════
    public partial class FormIncasare : Form
    {
        private readonly IncasareFormCtrl _ctrl;

        private readonly Color READONLY_COLOR = Color.FromArgb(245, 245, 245);
        private readonly Color WHITE          = Color.White;
        private readonly Color ERR_COLOR      = Color.FromArgb(255, 240, 240);
        private readonly Color HEADER_COLOR   = Color.FromArgb(31, 56, 100);

        public FormIncasare()
        {
            _ctrl = new IncasareFormCtrl();
            BuildUI();
            WireEvents();
            LoadClienti();
            LoadCaseriiSiConturi();
            SetModeVizualizare();
        }

        // ══════════════════════════════════════════════════════════════════
        // BUILD UI
        // ══════════════════════════════════════════════════════════════════
        private void BuildUI()
        {
            this.Text = "Inregistrare Incasare Client — SC Transilvania General Import-Export SRL";
            this.Size = new Size(1100, 800);
            this.MinimumSize = new Size(1100, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(240, 240, 240);
            this.KeyPreview = true;

            // StatusStrip
            var status = new StatusStrip { Dock = DockStyle.Bottom, BackColor = Color.FromArgb(232, 232, 232) };
            lblStare      = new ToolStripStatusLabel("Stare: Vizualizare") { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblCanal      = new ToolStripStatusLabel("Canal: Numerar") { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblLuna       = new ToolStripStatusLabel("Luna: " + DateTime.Now.ToString("MMMM yyyy")) { BorderSides = ToolStripStatusLabelBorderSides.Right };
            lblUtilizator = new ToolStripStatusLabel("Utilizator: casier01");
            status.Items.AddRange(new ToolStripItem[] { lblStare, lblCanal, lblLuna, lblUtilizator });
            this.Controls.Add(status);

            tabControl      = new TabControl { Dock = DockStyle.Fill, Font = new Font("Arial", 9f) };
            tabInregistrare = new TabPage("Inregistrare")     { BackColor = Color.FromArgb(240, 240, 240) };
            tabIstoric      = new TabPage("Istoric Incasari")  { BackColor = Color.FromArgb(240, 240, 240) };
            tabRaport       = new TabPage("Raport Incasari")   { BackColor = Color.FromArgb(240, 240, 240) };
            tabControl.TabPages.AddRange(new TabPage[] { tabInregistrare, tabIstoric, tabRaport });
            this.Controls.Add(tabControl);

            BuildTabInregistrare();
            BuildTabIstoric();
            BuildTabRaport();
        }

        private void BuildTabInregistrare()
        {
            var tab = tabInregistrare;

            // GRP 1: Client
            grpClient = MakeGroup("1. Client", 8, 8, 500, 50);
            MakeLabel("Client:", 12, 24, grpClient);
            cboClient = MakeCombo(60, 21, 430, grpClient);
            tab.Controls.Add(grpClient);

            // GRP 2: Factura
            grpFactura = MakeGroup("2. Factura de Incasat", 8, 64, 500, 96);
            MakeLabel("Selectati factura:", 12, 24, grpFactura);
            cboFactura = MakeCombo(116, 21, 374, grpFactura);
            cboFactura.DropDownWidth = 500;
            MakeLabel("Data factura:", 12, 56, grpFactura);
            txtDataFact       = MakeText(96, 53, 90, grpFactura, ro: true);
            MakeLabel("Val. totala:", 200, 56, grpFactura);
            txtValTotala      = MakeText(272, 53, 90, grpFactura, ro: true);
            MakeLabel("Rest plata:", 374, 56, grpFactura);
            txtRestDePlata    = MakeText(444, 53, 46, grpFactura, ro: true);
            tab.Controls.Add(grpFactura);

            // GRP 3: Date Incasare
            grpIncasare = MakeGroup("3. Date Incasare", 8, 166, 500, 80);
            MakeLabel("ID Incasare:", 12, 24, grpIncasare);
            txtIdIncasare  = MakeText(96, 21, 90, grpIncasare, ro: true);
            MakeLabel("Data incasare:", 200, 24, grpIncasare);
            dtpDataIncasare = MakeDtp(290, 21, 200, grpIncasare);
            MakeLabel("Suma incasata:", 12, 54, grpIncasare);
            txtSumaIncasata = MakeText(112, 51, 200, grpIncasare);
            MakeLabel("RON", 320, 54, grpIncasare);
            tab.Controls.Add(grpIncasare);

            // GRP 4: Canal
            grpCanal = MakeGroup("4. Canal Incasare", 8, 252, 500, 100);
            MakeLabel("Canal:", 12, 24, grpCanal);
            rdoNumerar = MakeRadio("Numerar",     64, 22, true,  grpCanal);
            rdoContBancar = MakeRadio("Cont Bancar", 174, 22, false, grpCanal);

            // Numerar row
            lblCaserie = MakeLabel("Caserie:", 12, 54, grpCanal);
            cboCaserie = MakeCombo(72, 51, 220, grpCanal);
            lblSoldCaserie = MakeLabel("Sold:", 304, 54, grpCanal);
            txtSoldCaserie = MakeText(340, 51, 150, grpCanal, ro: true);

            // ContBancar row
            lblIBAN = MakeLabel("Cont firma (IBAN):", 12, 78, grpCanal);
            cboIBAN = MakeCombo(124, 75, 366, grpCanal);
            lblIBAN.Visible = false;
            cboIBAN.Visible = false;

            tab.Controls.Add(grpCanal);

            // GRP 5: Document Generat
            grpDoc = MakeGroup("5. Document Generat", 8, 358, 500, 86);
            rdoBonFiscal   = MakeRadio("Bon Fiscal",          16, 22, true,  grpDoc);
            rdoExtras      = MakeRadio("Extras Cont Incasare", 16, 22, false, grpDoc);
            rdoExtras.Visible = false;
            MakeLabel("Nr. document:", 12, 52, grpDoc);
            txtNrDoc       = MakeText(108, 49, 120, grpDoc, ro: true);
            MakeLabel("Data emitere:", 246, 52, grpDoc);
            txtDataEmitere = MakeText(338, 49, 152, grpDoc, ro: true);
            var lblAuto = MakeLabel("(generate automat la salvare)", 12, 70, grpDoc);
            lblAuto.ForeColor = Color.Gray;
            lblAuto.Font = new Font("Arial", 7.5f, FontStyle.Italic);
            tab.Controls.Add(grpDoc);

            // GRP dreapta: incasari anterioare
            grpGrid = MakeGroup("6. Incasari anterioare (client curent)", 516, 8, 552, 436);
            grdIncasari = new DataGridView
            {
                Location = new System.Drawing.Point(8, 20),
                Size = new System.Drawing.Size(536, 380),
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
            grdIncasari.RowPrePaint += GrdIncasari_RowPrePaint;
            grpGrid.Controls.Add(grdIncasari);
            grpGrid.Controls.Add(new Label
            {
                Text = "* Filtrat dupa clientul curent.",
                Location = new System.Drawing.Point(8, 408), AutoSize = true,
                Font = new Font("Arial", 8f), ForeColor = Color.Gray
            });
            tab.Controls.Add(grpGrid);

            // Totaluri
            grpTotaluri = MakeGroup("Totaluri luna curenta", 516, 452, 552, 76);
            MakeLabel("Total incasat luna:", 12, 24, grpTotaluri);
            txtTotalLuna  = MakeText(212, 21, 150, grpTotaluri, ro: true, val: "0.00");
            MakeLabel("RON", 370, 24, grpTotaluri);
            MakeLabel("Nr. incasari luna:", 12, 50, grpTotaluri);
            txtNrIncasari = MakeText(212, 47, 150, grpTotaluri, ro: true, val: "0");
            tab.Controls.Add(grpTotaluri);

            // BUTOANE
            var pnl = new Panel
            {
                Location = new System.Drawing.Point(0, 540),
                Size = new System.Drawing.Size(1080, 44),
                BackColor = Color.FromArgb(232, 232, 232)
            };
            pnl.Controls.Add(new Label { Location = new System.Drawing.Point(0, 0), Size = new System.Drawing.Size(1080, 1), BackColor = Color.Silver });
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
                Text = "Tab Istoric Incasari — toate incasarile inregistrate cu filtre extinse.",
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
                Text = "Tab Raport Incasari Clienti — genereaza raportul centralizator.",
                Location = new System.Drawing.Point(16, 16), AutoSize = true,
                Font = new Font("Arial", 9f), ForeColor = Color.Gray
            });
            tabRaport.Controls.Add(new Button
            {
                Text = "Genereaza Raport",
                Location = new System.Drawing.Point(16, 46),
                Size = new System.Drawing.Size(150, 28),
                BackColor = Color.FromArgb(31, 107, 53), ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            });
        }

        // ══════════════════════════════════════════════════════════════════
        // EVENTS
        // ══════════════════════════════════════════════════════════════════
        private void WireEvents()
        {
            cboClient.SelectedIndexChanged  += CboClient_Changed;
            cboFactura.SelectedIndexChanged  += CboFactura_Changed;
            rdoNumerar.CheckedChanged        += Canal_Changed;
            rdoContBancar.CheckedChanged     += Canal_Changed;
            cboCaserie.SelectedIndexChanged  += CboCaserie_Changed;
            txtSumaIncasata.TextChanged      += TxtSuma_Changed;
            btnAdaugare.Click  += BtnAdaugare_Click;
            btnAnulare.Click   += BtnAnulare_Click;
            btnSalvare.Click   += BtnSalvare_Click;
            btnRenuntare.Click += BtnRenuntare_Click;
            this.KeyDown       += FormIncasare_KeyDown;
        }

        private void CboClient_Changed(object sender, EventArgs e)
        {
            if (!(cboClient.SelectedItem is ComboItem c)) return;
            _ctrl.OnClientSelected(int.Parse(c.Cod), c.Denumire);
            LoadFacturiClient();
            RefreshGrid();
            RefreshTotaluri();
        }

        private void CboFactura_Changed(object sender, EventArgs e)
        {
            if (!(cboFactura.SelectedItem is FacturaItem fi)) return;
            _ctrl.OnFacturaSelected(fi.Serie, fi.Numar, fi.DataDocument, fi.ValoareTotala);
            txtDataFact.Text    = fi.DataDocument.ToShortDateString();
            txtValTotala.Text   = fi.ValoareTotala.ToString("F2");
            txtRestDePlata.Text = _ctrl.GetFormData().GetRestDePlata().ToString("F2");
            txtSumaIncasata.BackColor = WHITE;
        }

        private void Canal_Changed(object sender, EventArgs e)
        {
            bool num = rdoNumerar.Checked;
            lblCaserie.Visible     = num;  cboCaserie.Visible    = num;
            lblSoldCaserie.Visible = num;  txtSoldCaserie.Visible = num;
            lblIBAN.Visible        = !num; cboIBAN.Visible       = !num;
            rdoBonFiscal.Visible   = num;  rdoExtras.Visible     = !num;
            UpdateDocumentGenerat();
            lblCanal.Text = "Canal: " + (num ? "Numerar" : "Cont Bancar");
            _ctrl.OnCanalChanged(num ? "Numerar" : "ContBancar");
        }

        private void CboCaserie_Changed(object sender, EventArgs e)
        {
            if (cboCaserie.SelectedItem is ComboItem ci)
            {
                _ctrl.GetFormData().SetCaserieSelectata(int.Parse(ci.Cod));
                // Actualizeaza soldul afisat
                var caserii = _ctrl.GetFormData().GetListaCaserii();
                var caserieSelectata = caserii.Find(c => c.IdCaserie == int.Parse(ci.Cod));
                if (caserieSelectata != null)
                    txtSoldCaserie.Text = caserieSelectata.SoldNumerar.ToString("F2");
            }
        }

        private void TxtSuma_Changed(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtSumaIncasata.Text, out decimal val)) return;
            decimal rest = _ctrl.GetFormData().GetRestDePlata();
            txtSumaIncasata.BackColor = (rest > 0 && val > rest) ? ERR_COLOR : WHITE;
        }

        private void BtnAdaugare_Click(object sender, EventArgs e)
        {
            _ctrl.DocumentNou();
            SetModeAdaugare();
            txtIdIncasare.Text = _ctrl.GetIdGenerat().ToString("D4");
        }

        private void BtnAnulare_Click(object sender, EventArgs e)
        {
            // Anulare = stergere incasare selectata in grid (simplificat)
            if (grdIncasari.CurrentRow == null || grdIncasari.CurrentRow.Index < 0) return;
            MessageBox.Show("Functionalitate anulare incasare — implementare viitoare.", "Info",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnSalvare_Click(object sender, EventArgs e)
        {
            if (!decimal.TryParse(txtSumaIncasata.Text, out decimal suma))
            { MessageBox.Show("Suma introdusa nu este valida.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }

            string canal = rdoNumerar.Checked ? "Numerar" : "ContBancar";
            int idCaserie = 0;
            string iban = "";

            if (canal == "Numerar")
            {
                if (!(cboCaserie.SelectedItem is ComboItem ci))
                { MessageBox.Show("Selectati caseria.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                idCaserie = int.Parse(ci.Cod);
            }
            else
            {
                if (!(cboIBAN.SelectedItem is ComboItem ii))
                { MessageBox.Show("Selectati contul bancar.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
                iban = ii.Cod;
            }

            bool saved = _ctrl.SalveazaIncasare(suma, canal, dtpDataIncasare.Value.Date, idCaserie, iban);

            if (saved)
            {
                string nrDoc = canal == "Numerar"
                    ? $"BF-{txtIdIncasare.Text}"
                    : $"ECI-{txtIdIncasare.Text}";
                string dataEmit = DateTime.Now.ToShortDateString();

                txtNrDoc.Text       = nrDoc;
                txtDataEmitere.Text = dataEmit;

                MessageBox.Show(
                    $"Incasarea a fost inregistrata cu succes!\n\n" +
                    $"Document generat: {nrDoc}\n" +
                    $"Data emitere: {dataEmit}",
                    "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadFacturiClient();
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

        private void FormIncasare_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A) { BtnAdaugare_Click(null, null);  e.Handled = true; }
            if (e.Control && e.KeyCode == Keys.S) { BtnSalvare_Click(null, null);   e.Handled = true; }
            if (e.KeyCode == Keys.Escape)          { BtnRenuntare_Click(null, null); e.Handled = true; }
        }

        private void GrdIncasari_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            grdIncasari.Rows[e.RowIndex].DefaultCellStyle.BackColor =
                e.RowIndex == grdIncasari.CurrentCell?.RowIndex
                    ? Color.FromArgb(189, 215, 238)
                    : (e.RowIndex % 2 == 0 ? Color.FromArgb(240, 244, 250) : Color.White);
        }

        // ══════════════════════════════════════════════════════════════════
        // MODURI DE LUCRU
        // ══════════════════════════════════════════════════════════════════
        private void SetModeVizualizare()
        {
            SetFieldsEnabled(false);
            btnSalvare.Enabled = false; btnRenuntare.Enabled = false;
            btnAdaugare.Enabled = true; btnAnulare.Enabled = true;
            lblStare.Text = "Stare: Vizualizare  |  Campuri ReadOnly";
            ClearFields();
        }

        private void SetModeAdaugare()
        {
            SetFieldsEnabled(true);
            txtIdIncasare.ReadOnly = true; txtIdIncasare.BackColor = READONLY_COLOR;
            btnSalvare.Enabled = true; btnRenuntare.Enabled = true;
            btnAdaugare.Enabled = false;
            lblStare.Text = "Stare: Adaugare  |  Completati campurile si apasati Salvare [Ctrl+S]";
            cboClient.Focus();
        }

        private void SetFieldsEnabled(bool on)
        {
            cboClient.Enabled   = on; cboFactura.Enabled = on;
            dtpDataIncasare.Enabled = on;
            rdoNumerar.Enabled  = on; rdoContBancar.Enabled = on;
            cboCaserie.Enabled  = on; cboIBAN.Enabled = on;
            txtSumaIncasata.ReadOnly = !on;
            txtSumaIncasata.BackColor = on ? WHITE : READONLY_COLOR;
        }

        // ══════════════════════════════════════════════════════════════════
        // INCARCARE DATE
        // ══════════════════════════════════════════════════════════════════
        private void LoadClienti()
        {
            cboClient.Items.Clear();
            var lista = _ctrl.GetFormData().GetListaClienti();
            lista.ForEach(c => cboClient.Items.Add(new ComboItem(c.CodClient.ToString(), c.Nume)));
            if (cboClient.Items.Count > 0) cboClient.SelectedIndex = 0;
        }

        private void LoadCaseriiSiConturi()
        {
            cboCaserie.Items.Clear();
            _ctrl.GetFormData().GetListaCaserii()
                 .ForEach(c => cboCaserie.Items.Add(new ComboItem(c.IdCaserie.ToString(),
                     $"{c.Locatie} (sold: {c.SoldNumerar:F2} RON)")));
            if (cboCaserie.Items.Count > 0) cboCaserie.SelectedIndex = 0;

            cboIBAN.Items.Clear();
            _ctrl.GetFormData().GetListaConturi()
                 .ForEach(cb => cboIBAN.Items.Add(new ComboItem(cb.IBAN,
                     $"{cb.IBAN} — {cb.Banca}")));
            if (cboIBAN.Items.Count > 0) cboIBAN.SelectedIndex = 0;
        }

        private void LoadFacturiClient()
        {
            cboFactura.Items.Clear();
            foreach (var fc in _ctrl.GetFormData().GetFacturiClient())
            {
                decimal rest = fc.ValoareTotala -
                    _ctrl.GetFormData().GetDocRepo().GetSumaIncasata(fc.Serie, fc.Numar);
                if (rest > 0)
                    cboFactura.Items.Add(new FacturaItem(fc.Serie, fc.Numar, fc.DataDocument, fc.ValoareTotala, rest));
            }
            if (cboFactura.Items.Count > 0) cboFactura.SelectedIndex = 0;
            else { txtDataFact.Text = ""; txtValTotala.Text = ""; txtRestDePlata.Text = ""; }
        }

        private void RefreshGrid()
        {
            var incasari = _ctrl.GetFormData().GetIncasari();
            var dt = new DataTable();
            dt.Columns.AddRange(new[] {
                new DataColumn("ID"),
                new DataColumn("Data"),
                new DataColumn("Suma (RON)"),
                new DataColumn("Canal"),
                new DataColumn("Factura")
            });
            foreach (var i in incasari)
                dt.Rows.Add(
                    i.IdIncasare,
                    i.DataIncasare.ToString("dd/MM/yyyy"),
                    i.SumaIncasata.ToString("F2"),
                    i.Canal,
                    $"{i.SerieFact}-{i.NumarFact}");
            grdIncasari.DataSource = dt;
        }

        private void RefreshTotaluri()
        {
            _ctrl.GetFormData().RefreshTotaluri();
            txtTotalLuna.Text  = _ctrl.GetFormData().GetTotalLuna().ToString("F2");
            txtNrIncasari.Text = _ctrl.GetFormData().GetNrIncasari().ToString();
        }

        private void UpdateDocumentGenerat()
        {
            rdoBonFiscal.Text = rdoNumerar.Checked ? "Bon Fiscal" : "Extras Cont Incasare";
        }

        private void ClearFields()
        {
            cboFactura.Items.Clear();
            txtDataFact.Text = ""; txtValTotala.Text = ""; txtRestDePlata.Text = "";
            txtSumaIncasata.Text = ""; txtIdIncasare.Text = "";
            txtNrDoc.Text = ""; txtDataEmitere.Text = "";
            txtSumaIncasata.BackColor = WHITE;
        }

        // ══════════════════════════════════════════════════════════════════
        // HELPERS CONSTRUCTORI CONTROALE
        // ══════════════════════════════════════════════════════════════════
        private GroupBox MakeGroup(string text, int x, int y, int w, int h)
            => new GroupBox { Text = text, Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, h),
                Font = new Font("Arial", 8.5f, FontStyle.Bold), ForeColor = HEADER_COLOR, BackColor = Color.FromArgb(250, 250, 250) };

        private Label MakeLabel(string text, int x, int y, Control parent = null)
        {
            var l = new Label { Text = text, Location = new System.Drawing.Point(x, y), AutoSize = true, Font = new Font("Arial", 8.5f) };
            parent?.Controls.Add(l); return l;
        }
        private TextBox MakeText(int x, int y, int w, Control parent = null, bool ro = false, string val = "")
        {
            var tb = new TextBox { Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, 21),
                ReadOnly = ro, BackColor = ro ? READONLY_COLOR : WHITE, Font = new Font("Arial", 8.5f), Text = val };
            parent?.Controls.Add(tb); return tb;
        }
        private ComboBox MakeCombo(int x, int y, int w, Control parent = null)
        {
            var cb = new ComboBox { Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, 21),
                DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Arial", 8.5f) };
            parent?.Controls.Add(cb); return cb;
        }
        private DateTimePicker MakeDtp(int x, int y, int w, Control parent = null)
        {
            var dtp = new DateTimePicker { Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, 21),
                Format = DateTimePickerFormat.Short, Font = new Font("Arial", 8.5f) };
            parent?.Controls.Add(dtp); return dtp;
        }
        private RadioButton MakeRadio(string text, int x, int y, bool chk, Control parent = null)
        {
            var rb = new RadioButton { Text = text, Location = new System.Drawing.Point(x, y), AutoSize = true,
                Checked = chk, Font = new Font("Arial", 8.5f) };
            parent?.Controls.Add(rb); return rb;
        }
        private Button MakeButton(string text, int x, int y, int w, int h, Control parent, Color bg)
        {
            var btn = new Button { Text = text, Location = new System.Drawing.Point(x, y), Size = new System.Drawing.Size(w, h),
                BackColor = bg, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 8.5f, FontStyle.Bold), Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            parent?.Controls.Add(btn); return btn;
        }

        // ══════════════════════════════════════════════════════════════════
        // DECLARATII CONTROALE
        // ══════════════════════════════════════════════════════════════════
        private TabControl tabControl;
        private TabPage tabInregistrare, tabIstoric, tabRaport;
        private GroupBox grpClient, grpFactura, grpIncasare, grpCanal, grpDoc, grpGrid, grpTotaluri;
        private ComboBox cboClient, cboFactura, cboCaserie, cboIBAN;
        private RadioButton rdoNumerar, rdoContBancar, rdoBonFiscal, rdoExtras;
        private TextBox txtDataFact, txtValTotala, txtRestDePlata;
        private TextBox txtIdIncasare, txtSumaIncasata;
        private TextBox txtNrDoc, txtDataEmitere, txtSoldCaserie;
        private TextBox txtTotalLuna, txtNrIncasari;
        private DateTimePicker dtpDataIncasare;
        private Button btnAdaugare, btnAnulare, btnSalvare, btnRenuntare;
        private DataGridView grdIncasari, grdIstoric;
        private Label lblCaserie, lblSoldCaserie, lblIBAN;
        private ToolStripStatusLabel lblStare, lblCanal, lblLuna, lblUtilizator;
    }
}
