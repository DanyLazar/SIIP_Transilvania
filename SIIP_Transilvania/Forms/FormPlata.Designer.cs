namespace SIIP_Transilvania.Forms
{
    partial class FormPlata
    {
        private System.ComponentModel.IContainer components = null;

        // ── Grup 1: Furnizor ─────────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpFurnizor;
        private System.Windows.Forms.Label lblFurnizor;
        private System.Windows.Forms.ComboBox cmbFurnizor;
        private System.Windows.Forms.Label lblSoldFurnizor;
        private System.Windows.Forms.TextBox txtSoldFurnizor;

        // ── Grup 2: Factura Furnizor ──────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpFactura;
        private System.Windows.Forms.Label lblFactura;
        private System.Windows.Forms.ComboBox cmbFactura;
        private System.Windows.Forms.Label lblDataFact;
        private System.Windows.Forms.TextBox txtDataFactura;
        private System.Windows.Forms.Label lblValTotala;
        private System.Windows.Forms.TextBox txtValTotala;
        private System.Windows.Forms.Label lblRestDisp;
        private System.Windows.Forms.TextBox txtRestDisp;

        // ── Grup 3: Date Tranzactie ───────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpDateTranzactie;
        private System.Windows.Forms.Label lblNrPlata;
        private System.Windows.Forms.TextBox txtNrPlata;
        private System.Windows.Forms.Label lblStare;
        private System.Windows.Forms.TextBox txtStare;
        private System.Windows.Forms.Label lblData;
        private System.Windows.Forms.DateTimePicker dtpData;
        private System.Windows.Forms.Label lblTipRata;
        private System.Windows.Forms.ComboBox cmbTipRata;
        private System.Windows.Forms.Label lblProcent;
        private System.Windows.Forms.NumericUpDown numProcent;
        private System.Windows.Forms.Label lblScadenta;
        private System.Windows.Forms.DateTimePicker dtpScadenta;
        private System.Windows.Forms.Label lblValoare;
        private System.Windows.Forms.NumericUpDown numValoare;

        // ── Grup 4: Canal Plata ───────────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpCanal;
        private System.Windows.Forms.RadioButton rbNumerar;
        private System.Windows.Forms.RadioButton rbVirament;
        private System.Windows.Forms.Label lblCaserie;
        private System.Windows.Forms.ComboBox cmbCaserie;
        private System.Windows.Forms.Label lblContBancar;
        private System.Windows.Forms.ComboBox cmbContBancar;

        // ── Grup 5: Document Generat ──────────────────────────────────────────
        private System.Windows.Forms.GroupBox grpDocumentGenerat;
        private System.Windows.Forms.RadioButton rbChitanta;
        private System.Windows.Forms.RadioButton rbExtrasContPlata;
        private System.Windows.Forms.Label lblNrDocument;
        private System.Windows.Forms.TextBox txtNrDocument;
        private System.Windows.Forms.Label lblDataEmitere;
        private System.Windows.Forms.DateTimePicker dtpDataEmitere;

        // ── Totaluri + Butoane ────────────────────────────────────────────────
        private System.Windows.Forms.Panel pnlTotaluri;
        private System.Windows.Forms.Label lblTotalLunaTitle;
        private System.Windows.Forms.Label lblTotalLuna;
        private System.Windows.Forms.Label lblNrPlatiTitle;
        private System.Windows.Forms.Label lblNrPlati;

        private System.Windows.Forms.Panel pnlButoane;
        private System.Windows.Forms.Button btnAdaugare;
        private System.Windows.Forms.Button btnAnulare;
        private System.Windows.Forms.Button btnSalvare;
        private System.Windows.Forms.Button btnRenuntare;

        // ── Grila dreapta ────────────────────────────────────────────────────
        private System.Windows.Forms.Label lblTitluGrila;
        private System.Windows.Forms.DataGridView dgvPlati;

        // ── StatusStrip ──────────────────────────────────────────────────────
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel tsslStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Instantiere
            this.grpFurnizor = new System.Windows.Forms.GroupBox();
            this.lblFurnizor = new System.Windows.Forms.Label();
            this.cmbFurnizor = new System.Windows.Forms.ComboBox();
            this.lblSoldFurnizor = new System.Windows.Forms.Label();
            this.txtSoldFurnizor = new System.Windows.Forms.TextBox();

            this.grpFactura = new System.Windows.Forms.GroupBox();
            this.lblFactura = new System.Windows.Forms.Label();
            this.cmbFactura = new System.Windows.Forms.ComboBox();
            this.lblDataFact = new System.Windows.Forms.Label();
            this.txtDataFactura = new System.Windows.Forms.TextBox();
            this.lblValTotala = new System.Windows.Forms.Label();
            this.txtValTotala = new System.Windows.Forms.TextBox();
            this.lblRestDisp = new System.Windows.Forms.Label();
            this.txtRestDisp = new System.Windows.Forms.TextBox();

            this.grpDateTranzactie = new System.Windows.Forms.GroupBox();
            this.lblNrPlata = new System.Windows.Forms.Label();
            this.txtNrPlata = new System.Windows.Forms.TextBox();
            this.lblStare = new System.Windows.Forms.Label();
            this.txtStare = new System.Windows.Forms.TextBox();
            this.lblData = new System.Windows.Forms.Label();
            this.dtpData = new System.Windows.Forms.DateTimePicker();
            this.lblTipRata = new System.Windows.Forms.Label();
            this.cmbTipRata = new System.Windows.Forms.ComboBox();
            this.lblProcent = new System.Windows.Forms.Label();
            this.numProcent = new System.Windows.Forms.NumericUpDown();
            this.lblScadenta = new System.Windows.Forms.Label();
            this.dtpScadenta = new System.Windows.Forms.DateTimePicker();
            this.lblValoare = new System.Windows.Forms.Label();
            this.numValoare = new System.Windows.Forms.NumericUpDown();

            this.grpCanal = new System.Windows.Forms.GroupBox();
            this.rbNumerar = new System.Windows.Forms.RadioButton();
            this.rbVirament = new System.Windows.Forms.RadioButton();
            this.lblCaserie = new System.Windows.Forms.Label();
            this.cmbCaserie = new System.Windows.Forms.ComboBox();
            this.lblContBancar = new System.Windows.Forms.Label();
            this.cmbContBancar = new System.Windows.Forms.ComboBox();

            this.grpDocumentGenerat = new System.Windows.Forms.GroupBox();
            this.rbChitanta = new System.Windows.Forms.RadioButton();
            this.rbExtrasContPlata = new System.Windows.Forms.RadioButton();
            this.lblNrDocument = new System.Windows.Forms.Label();
            this.txtNrDocument = new System.Windows.Forms.TextBox();
            this.lblDataEmitere = new System.Windows.Forms.Label();
            this.dtpDataEmitere = new System.Windows.Forms.DateTimePicker();

            this.pnlTotaluri = new System.Windows.Forms.Panel();
            this.lblTotalLunaTitle = new System.Windows.Forms.Label();
            this.lblTotalLuna = new System.Windows.Forms.Label();
            this.lblNrPlatiTitle = new System.Windows.Forms.Label();
            this.lblNrPlati = new System.Windows.Forms.Label();

            this.pnlButoane = new System.Windows.Forms.Panel();
            this.btnAdaugare = new System.Windows.Forms.Button();
            this.btnAnulare = new System.Windows.Forms.Button();
            this.btnSalvare = new System.Windows.Forms.Button();
            this.btnRenuntare = new System.Windows.Forms.Button();

            this.lblTitluGrila = new System.Windows.Forms.Label();
            this.dgvPlati = new System.Windows.Forms.DataGridView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsslStatus = new System.Windows.Forms.ToolStripStatusLabel();

            // SuspendLayout
            this.grpFurnizor.SuspendLayout(); this.grpFactura.SuspendLayout();
            this.grpDateTranzactie.SuspendLayout(); this.grpCanal.SuspendLayout();
            this.grpDocumentGenerat.SuspendLayout();
            this.pnlTotaluri.SuspendLayout(); this.pnlButoane.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)this.numProcent).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.numValoare).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvPlati).BeginInit();
            this.SuspendLayout();

            var RO = System.Drawing.Color.WhiteSmoke;
            var fontN = new System.Drawing.Font("Segoe UI", 9F);
            var fontB = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            int lx = 10; // left panel x
            int lw = 590; // left panel width

            // ════════════════════════════════════════════════════════════════
            // GRP 1 — Furnizor (y=10)
            // ════════════════════════════════════════════════════════════════
            this.grpFurnizor.Location = new System.Drawing.Point(lx, 10);
            this.grpFurnizor.Size = new System.Drawing.Size(lw, 95);
            this.grpFurnizor.Text = "1. Furnizor"; this.grpFurnizor.Font = fontB;
            this.grpFurnizor.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblFurnizor, cmbFurnizor, lblSoldFurnizor, txtSoldFurnizor });

            this.lblFurnizor.Text = "Furnizor:"; this.lblFurnizor.Location = new System.Drawing.Point(8, 30);
            this.lblFurnizor.Size = new System.Drawing.Size(70, 22); this.lblFurnizor.Font = fontN;
            this.lblFurnizor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmbFurnizor.Location = new System.Drawing.Point(82, 27); this.cmbFurnizor.Size = new System.Drawing.Size(290, 24);
            this.cmbFurnizor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList; this.cmbFurnizor.Font = fontN;
            this.cmbFurnizor.SelectedIndexChanged += new System.EventHandler(this.cmbFurnizor_SelectedIndexChanged);

            this.lblSoldFurnizor.Text = "Sold furnizor:"; this.lblSoldFurnizor.Location = new System.Drawing.Point(8, 58);
            this.lblSoldFurnizor.Size = new System.Drawing.Size(70, 22); this.lblSoldFurnizor.Font = fontN;
            this.lblSoldFurnizor.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtSoldFurnizor.Location = new System.Drawing.Point(82, 55); this.txtSoldFurnizor.Size = new System.Drawing.Size(150, 23);
            this.txtSoldFurnizor.ReadOnly = true; this.txtSoldFurnizor.BackColor = RO; this.txtSoldFurnizor.Font = fontB;
            this.txtSoldFurnizor.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ════════════════════════════════════════════════════════════════
            // GRP 2 — Factura Furnizor (y=113)
            // ════════════════════════════════════════════════════════════════
            this.grpFactura.Location = new System.Drawing.Point(lx, 113);
            this.grpFactura.Size = new System.Drawing.Size(lw, 120);
            this.grpFactura.Text = "2. Factura Furnizor"; this.grpFactura.Font = fontB;
            this.grpFactura.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblFactura, cmbFactura, lblDataFact, txtDataFactura,
                lblValTotala, txtValTotala, lblRestDisp, txtRestDisp });

            this.lblFactura.Text = "Factura:"; this.lblFactura.Location = new System.Drawing.Point(8, 28);
            this.lblFactura.Size = new System.Drawing.Size(70, 22); this.lblFactura.Font = fontN;
            this.lblFactura.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmbFactura.Location = new System.Drawing.Point(82, 25); this.cmbFactura.Size = new System.Drawing.Size(495, 24);
            this.cmbFactura.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFactura.Enabled = false; this.cmbFactura.Font = fontN;
            this.cmbFactura.SelectedIndexChanged += new System.EventHandler(this.cmbFactura_SelectedIndexChanged);

            this.lblDataFact.Text = "Data:"; this.lblDataFact.Location = new System.Drawing.Point(8, 58);
            this.lblDataFact.Size = new System.Drawing.Size(70, 22); this.lblDataFact.Font = fontN;
            this.lblDataFact.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtDataFactura.Location = new System.Drawing.Point(82, 55); this.txtDataFactura.Size = new System.Drawing.Size(110, 23);
            this.txtDataFactura.ReadOnly = true; this.txtDataFactura.BackColor = RO; this.txtDataFactura.Font = fontN;

            this.lblValTotala.Text = "Val. totala:"; this.lblValTotala.Location = new System.Drawing.Point(205, 58);
            this.lblValTotala.Size = new System.Drawing.Size(85, 22); this.lblValTotala.Font = fontN;
            this.lblValTotala.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtValTotala.Location = new System.Drawing.Point(293, 55); this.txtValTotala.Size = new System.Drawing.Size(130, 23);
            this.txtValTotala.ReadOnly = true; this.txtValTotala.BackColor = RO; this.txtValTotala.Font = fontN;
            this.txtValTotala.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            this.lblRestDisp.Text = "Rest plata:"; this.lblRestDisp.Location = new System.Drawing.Point(8, 86);
            this.lblRestDisp.Size = new System.Drawing.Size(70, 22); this.lblRestDisp.Font = fontN;
            this.lblRestDisp.ForeColor = System.Drawing.Color.DarkRed;
            this.lblRestDisp.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtRestDisp.Location = new System.Drawing.Point(82, 83); this.txtRestDisp.Size = new System.Drawing.Size(140, 23);
            this.txtRestDisp.ReadOnly = true; this.txtRestDisp.BackColor = System.Drawing.Color.FromArgb(255, 240, 240);
            this.txtRestDisp.Font = fontB; this.txtRestDisp.ForeColor = System.Drawing.Color.DarkRed;
            this.txtRestDisp.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;

            // ════════════════════════════════════════════════════════════════
            // GRP 3 — Date Tranzactie (y=241)
            // ════════════════════════════════════════════════════════════════
            this.grpDateTranzactie.Location = new System.Drawing.Point(lx, 241);
            this.grpDateTranzactie.Size = new System.Drawing.Size(lw, 175);
            this.grpDateTranzactie.Text = "3. Date Tranzactie"; this.grpDateTranzactie.Font = fontB;
            this.grpDateTranzactie.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblNrPlata, txtNrPlata, lblStare, txtStare,
                lblData, dtpData, lblTipRata, cmbTipRata,
                lblProcent, numProcent, lblScadenta, dtpScadenta,
                lblValoare, numValoare });

            // Rand 1: Nr Plata + Stare
            this.lblNrPlata.Text = "Nr. plata:"; this.lblNrPlata.Location = new System.Drawing.Point(8, 28);
            this.lblNrPlata.Size = new System.Drawing.Size(75, 22); this.lblNrPlata.Font = fontN;
            this.lblNrPlata.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtNrPlata.Location = new System.Drawing.Point(86, 25); this.txtNrPlata.Size = new System.Drawing.Size(80, 23);
            this.txtNrPlata.ReadOnly = true; this.txtNrPlata.BackColor = RO; this.txtNrPlata.Font = fontN;

            this.lblStare.Text = "Stare:"; this.lblStare.Location = new System.Drawing.Point(180, 28);
            this.lblStare.Size = new System.Drawing.Size(55, 22); this.lblStare.Font = fontN;
            this.lblStare.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtStare.Location = new System.Drawing.Point(238, 25); this.txtStare.Size = new System.Drawing.Size(120, 23);
            this.txtStare.ReadOnly = true; this.txtStare.BackColor = RO; this.txtStare.Font = fontN;

            // Rand 1: Data
            this.lblData.Text = "Data:"; this.lblData.Location = new System.Drawing.Point(375, 28);
            this.lblData.Size = new System.Drawing.Size(45, 22); this.lblData.Font = fontN;
            this.lblData.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.dtpData.Location = new System.Drawing.Point(423, 25); this.dtpData.Size = new System.Drawing.Size(150, 23);
            this.dtpData.Format = System.Windows.Forms.DateTimePickerFormat.Short; this.dtpData.Enabled = false;

            // Rand 2: TipRata + Procent
            this.lblTipRata.Text = "Tip rata:"; this.lblTipRata.Location = new System.Drawing.Point(8, 60);
            this.lblTipRata.Size = new System.Drawing.Size(75, 22); this.lblTipRata.Font = fontN;
            this.lblTipRata.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmbTipRata.Location = new System.Drawing.Point(86, 57); this.cmbTipRata.Size = new System.Drawing.Size(130, 24);
            this.cmbTipRata.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTipRata.Enabled = false; this.cmbTipRata.Font = fontN;
            this.cmbTipRata.SelectedIndexChanged += new System.EventHandler(this.cmbTipRata_SelectedIndexChanged);

            this.lblProcent.Text = "Procent (%):"; this.lblProcent.Location = new System.Drawing.Point(228, 60);
            this.lblProcent.Size = new System.Drawing.Size(85, 22); this.lblProcent.Font = fontN;
            this.lblProcent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.numProcent.Location = new System.Drawing.Point(316, 57); this.numProcent.Size = new System.Drawing.Size(80, 23);
            this.numProcent.DecimalPlaces = 2; this.numProcent.Maximum = 100; this.numProcent.Minimum = 0;
            this.numProcent.Enabled = false; this.numProcent.Font = fontN;
            this.numProcent.ValueChanged += new System.EventHandler(this.numProcent_ValueChanged);

            // Rand 2: Scadenta
            this.lblScadenta.Text = "Scadenta:"; this.lblScadenta.Location = new System.Drawing.Point(406, 60);
            this.lblScadenta.Size = new System.Drawing.Size(68, 22); this.lblScadenta.Font = fontN;
            this.lblScadenta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.dtpScadenta.Location = new System.Drawing.Point(477, 57); this.dtpScadenta.Size = new System.Drawing.Size(100, 23);
            this.dtpScadenta.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpScadenta.Enabled = false;

            // Rand 3: Valoare
            this.lblValoare.Text = "Valoare (RON):"; this.lblValoare.Location = new System.Drawing.Point(8, 100);
            this.lblValoare.Size = new System.Drawing.Size(100, 22); this.lblValoare.Font = fontB;
            this.lblValoare.ForeColor = System.Drawing.Color.FromArgb(30, 60, 114);
            this.lblValoare.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.numValoare.Location = new System.Drawing.Point(112, 97); this.numValoare.Size = new System.Drawing.Size(180, 28);
            this.numValoare.DecimalPlaces = 2; this.numValoare.Maximum = 9999999; this.numValoare.Minimum = 0;
            this.numValoare.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.numValoare.Enabled = false; this.numValoare.ThousandsSeparator = true;
            this.numValoare.ValueChanged += new System.EventHandler(this.numValoare_ValueChanged);

            // ════════════════════════════════════════════════════════════════
            // GRP 4 — Canal Plata (y=424)
            // ════════════════════════════════════════════════════════════════
            this.grpCanal.Location = new System.Drawing.Point(lx, 424);
            this.grpCanal.Size = new System.Drawing.Size(lw, 110);
            this.grpCanal.Text = "4. Canal Plata"; this.grpCanal.Font = fontB;
            this.grpCanal.Controls.AddRange(new System.Windows.Forms.Control[] {
                rbNumerar, rbVirament, lblCaserie, cmbCaserie, lblContBancar, cmbContBancar });

            this.rbNumerar.Text = "Numerar (casa)"; this.rbNumerar.Location = new System.Drawing.Point(15, 25);
            this.rbNumerar.Size = new System.Drawing.Size(150, 22); this.rbNumerar.Checked = true;
            this.rbNumerar.Font = fontN; this.rbNumerar.Enabled = false;
            this.rbNumerar.CheckedChanged += new System.EventHandler(this.rbCanal_CheckedChanged);

            this.rbVirament.Text = "Cont Bancar (IBAN)"; this.rbVirament.Location = new System.Drawing.Point(175, 25);
            this.rbVirament.Size = new System.Drawing.Size(170, 22); this.rbVirament.Font = fontN;
            this.rbVirament.Enabled = false;
            this.rbVirament.CheckedChanged += new System.EventHandler(this.rbCanal_CheckedChanged);

            this.lblCaserie.Text = "Caserie:"; this.lblCaserie.Location = new System.Drawing.Point(8, 57);
            this.lblCaserie.Size = new System.Drawing.Size(70, 22); this.lblCaserie.Font = fontN;
            this.lblCaserie.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmbCaserie.Location = new System.Drawing.Point(82, 54); this.cmbCaserie.Size = new System.Drawing.Size(495, 24);
            this.cmbCaserie.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCaserie.Enabled = false; this.cmbCaserie.Font = fontN;

            this.lblContBancar.Text = "IBAN:"; this.lblContBancar.Location = new System.Drawing.Point(8, 57);
            this.lblContBancar.Size = new System.Drawing.Size(70, 22); this.lblContBancar.Font = fontN;
            this.lblContBancar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.lblContBancar.Visible = false;
            this.cmbContBancar.Location = new System.Drawing.Point(82, 54); this.cmbContBancar.Size = new System.Drawing.Size(495, 24);
            this.cmbContBancar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbContBancar.Enabled = false; this.cmbContBancar.Visible = false; this.cmbContBancar.Font = fontN;

            // ════════════════════════════════════════════════════════════════
            // GRP 5 — Document Generat (y=542)
            // ════════════════════════════════════════════════════════════════
            this.grpDocumentGenerat.Location = new System.Drawing.Point(lx, 542);
            this.grpDocumentGenerat.Size = new System.Drawing.Size(lw, 100);
            this.grpDocumentGenerat.Text = "5. Document Generat (generate automat la salvare)";
            this.grpDocumentGenerat.Font = fontB;
            this.grpDocumentGenerat.Controls.AddRange(new System.Windows.Forms.Control[] {
                rbChitanta, rbExtrasContPlata, lblNrDocument, txtNrDocument,
                lblDataEmitere, dtpDataEmitere });

            this.rbChitanta.Text = "Chitanta"; this.rbChitanta.Location = new System.Drawing.Point(15, 25);
            this.rbChitanta.Size = new System.Drawing.Size(100, 22); this.rbChitanta.Checked = true;
            this.rbChitanta.Font = fontN; this.rbChitanta.Enabled = false;

            this.rbExtrasContPlata.Text = "Extras Cont Plata"; this.rbExtrasContPlata.Location = new System.Drawing.Point(125, 25);
            this.rbExtrasContPlata.Size = new System.Drawing.Size(160, 22);
            this.rbExtrasContPlata.Font = fontN; this.rbExtrasContPlata.Enabled = false;

            this.lblNrDocument.Text = "Nr. document:"; this.lblNrDocument.Location = new System.Drawing.Point(8, 57);
            this.lblNrDocument.Size = new System.Drawing.Size(95, 22); this.lblNrDocument.Font = fontN;
            this.lblNrDocument.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.txtNrDocument.Location = new System.Drawing.Point(106, 54); this.txtNrDocument.Size = new System.Drawing.Size(140, 23);
            this.txtNrDocument.ReadOnly = true; this.txtNrDocument.BackColor = RO; this.txtNrDocument.Font = fontB;

            this.lblDataEmitere.Text = "Data emitere:"; this.lblDataEmitere.Location = new System.Drawing.Point(260, 57);
            this.lblDataEmitere.Size = new System.Drawing.Size(90, 22); this.lblDataEmitere.Font = fontN;
            this.lblDataEmitere.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.dtpDataEmitere.Location = new System.Drawing.Point(353, 54); this.dtpDataEmitere.Size = new System.Drawing.Size(140, 23);
            this.dtpDataEmitere.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDataEmitere.Enabled = false;

            // ════════════════════════════════════════════════════════════════
            // Panel Totaluri (y=650)
            // ════════════════════════════════════════════════════════════════
            this.pnlTotaluri.Location = new System.Drawing.Point(lx, 650);
            this.pnlTotaluri.Size = new System.Drawing.Size(lw, 52);
            this.pnlTotaluri.BackColor = System.Drawing.Color.FromArgb(235, 245, 255);
            this.pnlTotaluri.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTotaluri.Controls.AddRange(new System.Windows.Forms.Control[] {
                lblTotalLunaTitle, lblTotalLuna, lblNrPlatiTitle, lblNrPlati });

            this.lblTotalLunaTitle.Text = "Total plati luna curenta:";
            this.lblTotalLunaTitle.Location = new System.Drawing.Point(8, 8); this.lblTotalLunaTitle.Size = new System.Drawing.Size(175, 18);
            this.lblTotalLunaTitle.Font = fontN;
            this.lblTotalLuna.Text = "0,00 RON"; this.lblTotalLuna.Location = new System.Drawing.Point(185, 8);
            this.lblTotalLuna.Size = new System.Drawing.Size(130, 18); this.lblTotalLuna.Font = fontB;
            this.lblTotalLuna.ForeColor = System.Drawing.Color.DarkBlue;

            this.lblNrPlatiTitle.Text = "Numar plati:";
            this.lblNrPlatiTitle.Location = new System.Drawing.Point(330, 8); this.lblNrPlatiTitle.Size = new System.Drawing.Size(90, 18);
            this.lblNrPlatiTitle.Font = fontN;
            this.lblNrPlati.Text = "0"; this.lblNrPlati.Location = new System.Drawing.Point(423, 8);
            this.lblNrPlati.Size = new System.Drawing.Size(60, 18); this.lblNrPlati.Font = fontB;

            // ════════════════════════════════════════════════════════════════
            // Panel Butoane (y=710)
            // ════════════════════════════════════════════════════════════════
            this.pnlButoane.Location = new System.Drawing.Point(lx, 710);
            this.pnlButoane.Size = new System.Drawing.Size(lw, 52);
            this.pnlButoane.Controls.AddRange(new System.Windows.Forms.Control[] {
                btnAdaugare, btnAnulare, btnSalvare, btnRenuntare });

            // Adaugare — verde #1F6B35
            this.btnAdaugare.Text = "Adaugare";
            this.btnAdaugare.Location = new System.Drawing.Point(0, 2); this.btnAdaugare.Size = new System.Drawing.Size(140, 44);
            this.btnAdaugare.BackColor = System.Drawing.Color.FromArgb(31, 107, 53);
            this.btnAdaugare.ForeColor = System.Drawing.Color.White; this.btnAdaugare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdaugare.Font = fontB; this.btnAdaugare.FlatAppearance.BorderSize = 0;
            this.btnAdaugare.Click += new System.EventHandler(this.btnAdaugare_Click);

            // Anulare — rosu #CC0000
            this.btnAnulare.Text = "Anulare";
            this.btnAnulare.Location = new System.Drawing.Point(146, 2); this.btnAnulare.Size = new System.Drawing.Size(140, 44);
            this.btnAnulare.BackColor = System.Drawing.Color.FromArgb(204, 0, 0);
            this.btnAnulare.ForeColor = System.Drawing.Color.White; this.btnAnulare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAnulare.Font = fontB; this.btnAnulare.FlatAppearance.BorderSize = 0;
            this.btnAnulare.Enabled = false;
            this.btnAnulare.Click += new System.EventHandler(this.btnAnulare_Click);

            // Salvare — albastru #2E4A78
            this.btnSalvare.Text = "Salvare";
            this.btnSalvare.Location = new System.Drawing.Point(292, 2); this.btnSalvare.Size = new System.Drawing.Size(140, 44);
            this.btnSalvare.BackColor = System.Drawing.Color.FromArgb(46, 74, 120);
            this.btnSalvare.ForeColor = System.Drawing.Color.White; this.btnSalvare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalvare.Font = fontB; this.btnSalvare.FlatAppearance.BorderSize = 0;
            this.btnSalvare.Enabled = false;
            this.btnSalvare.Click += new System.EventHandler(this.btnSalvare_Click);

            // Renuntare — gri #6D6D6D
            this.btnRenuntare.Text = "Renuntare";
            this.btnRenuntare.Location = new System.Drawing.Point(438, 2); this.btnRenuntare.Size = new System.Drawing.Size(140, 44);
            this.btnRenuntare.BackColor = System.Drawing.Color.FromArgb(109, 109, 109);
            this.btnRenuntare.ForeColor = System.Drawing.Color.White; this.btnRenuntare.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRenuntare.Font = fontB; this.btnRenuntare.FlatAppearance.BorderSize = 0;
            this.btnRenuntare.Enabled = false;
            this.btnRenuntare.Click += new System.EventHandler(this.btnRenuntare_Click);

            // ════════════════════════════════════════════════════════════════
            // Grila dreapta (x=616)
            // ════════════════════════════════════════════════════════════════
            this.lblTitluGrila.Text = "Plati anterioare furnizor";
            this.lblTitluGrila.Location = new System.Drawing.Point(616, 10);
            this.lblTitluGrila.Size = new System.Drawing.Size(630, 22);
            this.lblTitluGrila.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblTitluGrila.ForeColor = System.Drawing.Color.FromArgb(30, 60, 114);

            this.dgvPlati.Location = new System.Drawing.Point(616, 36);
            this.dgvPlati.Size = new System.Drawing.Size(630, 880);
            this.dgvPlati.AllowUserToAddRows = false; this.dgvPlati.AllowUserToDeleteRows = false;
            this.dgvPlati.ReadOnly = true;
            this.dgvPlati.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvPlati.MultiSelect = false;
            this.dgvPlati.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPlati.BackgroundColor = System.Drawing.Color.White;
            this.dgvPlati.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvPlati.ColumnHeadersHeight = 34;
            this.dgvPlati.ColumnHeadersDefaultCellStyle.Font = fontB;
            this.dgvPlati.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(30, 60, 114);
            this.dgvPlati.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvPlati.DefaultCellStyle.Font = fontN;
            this.dgvPlati.DefaultCellStyle.SelectionBackColor = System.Drawing.Color.FromArgb(0, 102, 204);
            this.dgvPlati.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.White;
            this.dgvPlati.RowTemplate.Height = 30;
            this.dgvPlati.GridColor = System.Drawing.Color.FromArgb(220, 220, 220);
            this.dgvPlati.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvPlati_CellFormatting);
            this.dgvPlati.SelectionChanged += new System.EventHandler(this.dgvPlati_SelectionChanged);

            var c1 = new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "colNr", HeaderText = "Nr. Plata", FillWeight = 70 };
            var c2 = new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "colData", HeaderText = "Data", FillWeight = 75 };
            var c3 = new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "colFactura", HeaderText = "Factura", FillWeight = 90 };
            var c4 = new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "colValoare", HeaderText = "Valoare RON", FillWeight = 90 };
            var c5 = new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "colTipRata", HeaderText = "Tip Rata", FillWeight = 75 };
            var c6 = new System.Windows.Forms.DataGridViewTextBoxColumn { Name = "colStare", HeaderText = "Stare", FillWeight = 70 };
            c4.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.dgvPlati.Columns.AddRange(c1, c2, c3, c4, c5, c6);

            // ════════════════════════════════════════════════════════════════
            // StatusStrip
            // ════════════════════════════════════════════════════════════════
            this.tsslStatus.Text = "Mod: Vizualizare"; this.tsslStatus.Spring = true;
            this.statusStrip.Items.Add(this.tsslStatus);
            this.statusStrip.Font = fontN;
            this.statusStrip.BackColor = System.Drawing.Color.FromArgb(30, 60, 114);
            this.tsslStatus.ForeColor = System.Drawing.Color.White;

            // ════════════════════════════════════════════════════════════════
            // Form
            // ════════════════════════════════════════════════════════════════
            this.ClientSize = new System.Drawing.Size(1262, 952);
            this.Text = "Inregistrare Plata Furnizor — SC Transilvania General Import-Export SRL";
            this.Font = fontN;
            this.BackColor = System.Drawing.Color.White;
            this.MinimumSize = new System.Drawing.Size(1262, 952);
            this.KeyPreview = true;
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormPlata_KeyDown);

            this.Controls.AddRange(new System.Windows.Forms.Control[] {
                grpFurnizor, grpFactura, grpDateTranzactie, grpCanal,
                grpDocumentGenerat, pnlTotaluri, pnlButoane,
                lblTitluGrila, dgvPlati, statusStrip });

            // ResumeLayout
            this.grpFurnizor.ResumeLayout(false); this.grpFactura.ResumeLayout(false);
            this.grpDateTranzactie.ResumeLayout(false); this.grpCanal.ResumeLayout(false);
            this.grpDocumentGenerat.ResumeLayout(false);
            this.pnlTotaluri.ResumeLayout(false); this.pnlButoane.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)this.numProcent).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.numValoare).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.dgvPlati).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}