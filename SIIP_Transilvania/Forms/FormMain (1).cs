using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SIIP_Transilvania.Forms
{
    public class FormMain : Form
    {
        public FormMain()
        {
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "SIIP — SC Transilvania General Import-Export SRL";
            this.Size = new Size(560, 460);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 243);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Header
            var pnlHeader = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(560, 80),
                BackColor = Color.FromArgb(31, 56, 100)
            };
            pnlHeader.Controls.Add(new Label
            {
                Text = "SIIP",
                Font = new Font("Arial", 22f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(24, 14),
                AutoSize = true
            });
            pnlHeader.Controls.Add(new Label
            {
                Text = "Sistem Informatic de Incasari si Plati",
                Font = new Font("Arial", 10f),
                ForeColor = Color.FromArgb(158, 181, 212),
                Location = new Point(26, 50),
                AutoSize = true
            });
            this.Controls.Add(pnlHeader);

            // Subtitle
            this.Controls.Add(new Label
            {
                Text = "Selectati modulul",
                Font = new Font("Arial", 9f),
                ForeColor = Color.FromArgb(120, 120, 110),
                Location = new Point(24, 96),
                AutoSize = true
            });

            // Module cards
            var modules = new[]
            {
                new { Name = "Inregistrare Incasare Client",   Sub = "Incasari de la clienti",  Accent = Color.FromArgb(46, 109, 164) },
                new { Name = "Inregistrare Plata Furnizor",    Sub = "Plati catre furnizori",   Accent = Color.FromArgb(58, 122, 58)  },
                new { Name = "Inregistrare Retur",             Sub = "Retururi marfa",          Accent = Color.FromArgb(107, 74, 158) },
                new { Name = "Inregistrare Decont Cheltuieli", Sub = "Deconturi de cheltuieli", Accent = Color.FromArgb(139, 105, 20) },
            };

            int y = 118;
            foreach (var m in modules)
            {
                var card = new ModuleCard(m.Name, m.Sub, m.Accent)
                {
                    Location = new Point(24, y),
                    Size = new Size(512, 62),
                    Cursor = Cursors.Hand
                };
                var nameCopy = m.Name;
                card.Click += (s, e) => OpenForm(nameCopy);
                this.Controls.Add(card);
                y += 74;
            }

            // Footer
            var pnlFooter = new Panel
            {
                Location = new Point(0, 428),
                Size = new Size(560, 32),
                BackColor = Color.FromArgb(232, 232, 228)
            };
            pnlFooter.Controls.Add(new Label
            {
                Text = "An 3 IE — UAIC Iași — Proiectarea Sistemelor Informationale",
                Font = new Font("Arial", 8f),
                ForeColor = Color.FromArgb(160, 160, 150),
                Location = new Point(0, 9),
                Size = new Size(560, 16),
                TextAlign = ContentAlignment.MiddleCenter
            });
            this.Controls.Add(pnlFooter);
        }

        private void OpenForm(string name)
        {
            Form form = null;
            switch (name)
            {
                case "Inregistrare Incasare Client": form = new FormIncasare(); break;
                case "Inregistrare Plata Furnizor": form = new FormPlata(); break;
                case "Inregistrare Retur": form = new FormRetur(); break;
                case "Inregistrare Decont Cheltuieli": form = new FormDecont(); break;
            }
            if (form != null)
            {
                form.StartPosition = FormStartPosition.CenterScreen;
                form.ShowDialog();
            }
        }
    }

    public class ModuleCard : Control
    {
        private readonly string _name;
        private readonly string _sub;
        private readonly Color _accent;
        private bool _hover;

        public ModuleCard(string name, string sub, Color accent)
        {
            _name = name;
            _sub = sub;
            _accent = accent;
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.DoubleBuffer, true);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hover = true;
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hover = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            var bg = _hover
                ? Color.FromArgb(235, 240, 248)
                : Color.FromArgb(248, 248, 246);

            // Card background
            GraphicsExtensions.FillRoundedRect(g, new Rectangle(0, 0, Width, Height), 8,
                new SolidBrush(bg));

            // Border
            GraphicsExtensions.DrawRoundedRect(g, new Rectangle(0, 0, Width - 1, Height - 1), 8,
                new Pen(Color.FromArgb(220, 220, 216), 1f));

            // Left accent bar
            g.FillRectangle(new SolidBrush(_accent), new Rectangle(0, 0, 4, Height));

            // Icon circle
            int cx = 34, cy = Height / 2;
            g.FillEllipse(new SolidBrush(Color.FromArgb(30, _accent)), cx - 16, cy - 16, 32, 32);
            g.DrawEllipse(new Pen(_accent, 1.5f), cx - 16, cy - 16, 32, 32);

            // Title
            g.DrawString(_name,
                new Font("Arial", 11f, FontStyle.Bold),
                new SolidBrush(Color.FromArgb(30, 30, 30)),
                new PointF(60, Height / 2 - 16));

            // Subtitle
            g.DrawString(_sub,
                new Font("Arial", 9f),
                new SolidBrush(Color.FromArgb(130, 130, 120)),
                new PointF(61, Height / 2 + 2));

            // Arrow
            g.DrawString("›",
                new Font("Arial", 16f),
                new SolidBrush(Color.FromArgb(180, 180, 170)),
                new PointF(Width - 28, Height / 2 - 12));
        }
    }

    public static class GraphicsExtensions
    {
        public static void FillRoundedRect(Graphics g, Rectangle r, int radius, Brush brush)
        {
            GraphicsPath path = GetRoundedPath(r, radius);
            g.FillPath(brush, path);
            path.Dispose();
            brush.Dispose();
        }

        public static void DrawRoundedRect(Graphics g, Rectangle r, int radius, Pen pen)
        {
            GraphicsPath path = GetRoundedPath(r, radius);
            g.DrawPath(pen, path);
            path.Dispose();
            pen.Dispose();
        }

        private static GraphicsPath GetRoundedPath(Rectangle r, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(r.X, r.Y, radius * 2, radius * 2, 180, 90);
            path.AddArc(r.Right - radius * 2, r.Y, radius * 2, radius * 2, 270, 90);
            path.AddArc(r.Right - radius * 2, r.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(r.X, r.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}