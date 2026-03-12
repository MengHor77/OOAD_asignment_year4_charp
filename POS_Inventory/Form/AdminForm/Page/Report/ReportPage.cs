using POS_Inventory.Config;
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace POS_Inventory.Form.AdminForm.Page.Report
{
    public partial class ReportPage : UserControl
    {
        private DataGridView dgvReports;
        private DateTimePicker dtFrom, dtTo;
        private Button btnFilter, btnExport, btnPrint;
        private Label lblTotalRevenue, lblTotalOrders, lblTitle;
        private Panel pnlSummary, pnlFilter;

        // --- Repositories ---
        private readonly SaleConfig _saleConfig = new SaleConfig();

        public ReportPage()
        {
            InitializeComponent();
            SetupLayout();
            LoadReportData();
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppColorConfig.ContentBackground;
            this.Padding = new Padding(20,0,20,20);

            

            // =============================================
            // 2. FILTER PANEL
            // =============================================
            pnlFilter = new Panel
            {
                Dock = DockStyle.Top,
                Height = 90,
                BackColor = AppColorConfig.GrayLight,
                Padding = new Padding(20, 0, 20, 0)
            };

            pnlFilter.Paint += (s, e) =>
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    int r = 10;
                    gp.AddArc(0, 0, r, r, 180, 90);
                    gp.AddArc(pnlFilter.Width - r, 0, r, r, 270, 90);
                    gp.AddArc(pnlFilter.Width - r, pnlFilter.Height - r, r, r, 0, 90);
                    gp.AddArc(0, pnlFilter.Height - r, r, r, 90, 90);
                    gp.CloseAllFigures();
                    pnlFilter.Region = new Region(gp);
                }
            };

            Label lblFrom = new Label
            {
                Text = "📅  From:",
                Location = new Point(20, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = AppColorConfig.TextDark
            };

            dtFrom = new DateTimePicker
            {
                Location = new Point(20, 45),
                Width = 160,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Value = DateTime.Today.AddDays(-30) // default: last 30 days
            };

            Label lblTo = new Label
            {
                Text = "📅  To:",
                Location = new Point(200, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = AppColorConfig.TextDark
            };

            dtTo = new DateTimePicker
            {
                Location = new Point(200, 45),
                Width = 160,
                Format = DateTimePickerFormat.Short,
                Font = new Font("Segoe UI", 10),
                Value = DateTime.Today // default: today
            };

            btnFilter = CreateBtn("🔍  Filter", AppColorConfig.BrandBlue, 390, 40);
            btnExport = CreateBtn("📥  Export", AppColorConfig.BtnSave, 510, 40);
            btnPrint = CreateBtn("🖨️  Print", AppColorConfig.LightBlue, 630, 40);

            btnFilter.Click += (s, e) => LoadReportData();
            btnExport.Click += (s, e) =>
            {
                if (dgvReports.Rows.Count == 0)
                {
                    MessageBox.Show("No data to export!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = "Text File|*.txt";
                    sfd.FileName = $"Report_{dtFrom.Value:yyyyMMdd}_to_{dtTo.Value:yyyyMMdd}.txt";

                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            using (StreamWriter sw = new StreamWriter(sfd.FileName))
                            {
                                // 1. Calculate max length for each column
                                int[] maxLengths = new int[dgvReports.Columns.Count];

                                // Check headers first
                                for (int c = 0; c < dgvReports.Columns.Count; c++)
                                    maxLengths[c] = dgvReports.Columns[c].HeaderText.Length;

                                // Check rows
                                foreach (DataGridViewRow row in dgvReports.Rows)
                                {
                                    if (!row.IsNewRow)
                                    {
                                        for (int c = 0; c < dgvReports.Columns.Count; c++)
                                        {
                                            string cellText = row.Cells[c].Value?.ToString() ?? "";
                                            if (cellText.Length > maxLengths[c])
                                                maxLengths[c] = cellText.Length;
                                        }
                                    }
                                }

                                // 2. Write headers
                                for (int c = 0; c < dgvReports.Columns.Count; c++)
                                {
                                    sw.Write(dgvReports.Columns[c].HeaderText.PadRight(maxLengths[c] + 2));
                                }
                                sw.WriteLine();

                                // 3. Write separator line
                                for (int c = 0; c < dgvReports.Columns.Count; c++)
                                {
                                    sw.Write(new string('-', maxLengths[c]) + "  ");
                                }
                                sw.WriteLine();

                                // 4. Write rows
                                foreach (DataGridViewRow row in dgvReports.Rows)
                                {
                                    if (!row.IsNewRow)
                                    {
                                        for (int c = 0; c < dgvReports.Columns.Count; c++)
                                        {
                                            string cellText = row.Cells[c].Value?.ToString() ?? "";
                                            sw.Write(cellText.PadRight(maxLengths[c] + 2));
                                        }
                                        sw.WriteLine();
                                    }
                                }

                                // Optional: add summary at bottom
                                sw.WriteLine();
                                sw.WriteLine($"Total Orders: {lblTotalOrders.Text}");
                                sw.WriteLine($"Total Revenue: {lblTotalRevenue.Text}");
                            }

                            MessageBox.Show("Report exported successfully!", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error exporting report:\n" + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };
            pnlFilter.Controls.AddRange(new Control[] { lblFrom, dtFrom, lblTo, dtTo, btnFilter, btnExport, btnPrint });

            // =============================================
            // 3. SUMMARY CARDS PANEL
            // =============================================
            pnlSummary = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                BackColor = AppColorConfig.ContentBackground,
                Padding = new Padding(0, 10, 0, 0)
            };

            Panel cardRevenue = CreateSummaryCard(
                "💰  Total Revenue",
                "$0.00",
                AppColorConfig.CardProduct,
                AppColorConfig.CardProductHover,
                out lblTotalRevenue
            );
            cardRevenue.Location = new Point(20, 10);

            Panel cardOrders = CreateSummaryCard(
                "🧾  Total Orders",
                "0",
                AppColorConfig.CardStaff,
                AppColorConfig.CardStaffHover,
                out lblTotalOrders
            );
            cardOrders.Location = new Point(280, 10);

            pnlSummary.Controls.Add(cardRevenue);
            pnlSummary.Controls.Add(cardOrders);

            // =============================================
            // 4. DATA GRID
            // =============================================
            dgvReports = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = AppColorConfig.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                GridColor = Color.FromArgb(220, 230, 245),
                RowTemplate = { Height = 40 },
                ColumnHeadersHeight = 45,
                EnableHeadersVisualStyles = false,
                Font = new Font("Segoe UI", 10)
            };

            dgvReports.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.CardProduct,
                ForeColor = AppColorConfig.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            dgvReports.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.White,
                ForeColor = AppColorConfig.TextDark,
                SelectionBackColor = Color.FromArgb(210, 228, 255),
                SelectionForeColor = AppColorConfig.TextDark,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            dgvReports.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(240, 245, 255),
                ForeColor = AppColorConfig.TextDark,
                SelectionBackColor = Color.FromArgb(210, 228, 255),
                SelectionForeColor = AppColorConfig.TextDark,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            // =============================================
            // 5. ADD ALL TO FORM (ORDER MATTERS FOR DOCKING)
            // =============================================

            // Title panel at very top
            Panel pnlTitle = new Panel
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = AppColorConfig.ContentBackground
            };

            lblTitle = new Label
            {
                Text = "📊  Report Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = AppColorConfig.TextDark,
                AutoSize = true,
                Location = new Point(0, 20),
            };

            pnlTitle.Controls.Add(lblTitle);

            // Add in reverse dock order: Fill first, then Bottom, then Top panels
            this.Controls.Add(dgvReports);      // Fill
            this.Controls.Add(pnlSummary);      // Bottom
            this.Controls.Add(pnlFilter);       // Top (second)
            this.Controls.Add(pnlTitle);        // Top (first — added last so it appears at very top)


        }
        // =============================================
        // LOAD REAL DATA FROM DATABASE
        // =============================================
        private void LoadReportData()
        {
            try
            {
                DateTime from = dtFrom.Value.Date;
                DateTime to = dtTo.Value.Date.AddDays(1).AddSeconds(-1); // include full end day

                DataTable dt = _saleConfig.GetSalesByDateRange(from, to);

                // Build display table
                DataTable displayTable = new DataTable();
                displayTable.Columns.Add("No");
                displayTable.Columns.Add("Date");
                displayTable.Columns.Add("Total Amount ($)");
                displayTable.Columns.Add("Status");

                decimal grandTotal = 0;
                int orderCount = 0;

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        orderCount++;
                        decimal rowTotal = Convert.ToDecimal(row["total"]);
                        grandTotal += rowTotal;

                        displayTable.Rows.Add(
                            orderCount.ToString(),
                            Convert.ToDateTime(row["created_at"]).ToString("dd/MM/yyyy  HH:mm"),
                            "$" + rowTotal.ToString("0.00"),
                            "✅ Completed"
                        );
                    }
                }

                dgvReports.DataSource = displayTable;
                UpdateSummary(grandTotal, orderCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSummary(decimal revenue, int orders)
        {
            lblTotalRevenue.Text = "$" + revenue.ToString("0.00");
            lblTotalOrders.Text = orders.ToString();
        }

        // =============================================
        // HELPER: Create action button
        // =============================================
        private Button CreateBtn(string text, Color color, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(110, 36),
                Location = new Point(x, y),
                BackColor = color,
                ForeColor = AppColorConfig.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;

            Color original = color;
            btn.MouseEnter += (s, e) => btn.BackColor = ControlPaint.Dark(color, 0.1f);
            btn.MouseLeave += (s, e) => btn.BackColor = original;

            btn.Paint += (s, e) =>
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    int r = 8;
                    gp.AddArc(0, 0, r, r, 180, 90);
                    gp.AddArc(btn.Width - r, 0, r, r, 270, 90);
                    gp.AddArc(btn.Width - r, btn.Height - r, r, r, 0, 90);
                    gp.AddArc(0, btn.Height - r, r, r, 90, 90);
                    gp.CloseAllFigures();
                    btn.Region = new Region(gp);
                }
            };

            return btn;
        }

        // =============================================
        // HELPER: Create summary card
        // =============================================
        private Panel CreateSummaryCard(string title, string value, Color bgColor, Color hoverColor, out Label lblValue)
        {
            Panel pnl = new Panel
            {
                Size = new Size(240, 65),
                BackColor = bgColor,
                Cursor = Cursors.Default
            };

            pnl.Paint += (s, e) =>
            {
                using (GraphicsPath gp = new GraphicsPath())
                {
                    int r = 12;
                    gp.AddArc(0, 0, r, r, 180, 90);
                    gp.AddArc(pnl.Width - r, 0, r, r, 270, 90);
                    gp.AddArc(pnl.Width - r, pnl.Height - r, r, r, 0, 90);
                    gp.AddArc(0, pnl.Height - r, r, r, 90, 90);
                    gp.CloseAllFigures();
                    pnl.Region = new Region(gp);
                }
            };

            Label lblCardTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = AppColorConfig.White,
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = ContentAlignment.MiddleCenter
            };

            lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = AppColorConfig.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };

            pnl.Controls.Add(lblValue);
            pnl.Controls.Add(lblCardTitle);

            return pnl;
        }
    }
}