using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Report
{
    public partial class ReportPage : UserControl
    {
        private DataGridView dgvReports;
        private DateTimePicker dtFrom, dtTo;
        private Button btnFilter, btnExport, btnPrint;
        private Label lblTotalRevenue, lblTotalOrders;

        public ReportPage()
        {
            InitializeComponent();
            SetupLayout();
            LoadReportData(); // Load default data (e.g., today's sales)
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppColorConfig.ContentBackground;
            // --- 1. FILTER PANEL (TOP) ---
            Panel pnlFilter = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(245, 245, 245) };

            Label lblFrom = new Label { Text = "From:", Location = new Point(20, 15), AutoSize = true };
            dtFrom = new DateTimePicker { Location = new Point(20, 35), Width = 150, Format = DateTimePickerFormat.Short };

            Label lblTo = new Label { Text = "To:", Location = new Point(190, 15), AutoSize = true };
            dtTo = new DateTimePicker { Location = new Point(190, 35), Width = 150, Format = DateTimePickerFormat.Short };

            btnFilter = CreateBtn("Filter Data", Color.RoyalBlue, 360, 32);
            btnExport = CreateBtn("Export Excel", Color.SeaGreen, 470, 32);
            btnPrint = CreateBtn("Print PDF", Color.DarkOrange, 580, 32);

            btnFilter.Click += (s, e) => LoadReportData();
            btnExport.Click += (s, e) => MessageBox.Show("Exporting to Excel...");

            pnlFilter.Controls.AddRange(new Control[] { lblFrom, dtFrom, lblTo, dtTo, btnFilter, btnExport, btnPrint });

            // --- 2. SUMMARY PANEL (BOTTOM) ---
            Panel pnlSummary = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = Color.FromArgb(230, 230, 230) };

            lblTotalRevenue = new Label
            {
                Text = "Total Revenue: $0.00",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.DarkGreen,
                Location = new Point(20, 20),
                AutoSize = true
            };

            lblTotalOrders = new Label
            {
                Text = "Total Orders: 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(300, 20),
                AutoSize = true
            };

            pnlSummary.Controls.AddRange(new Control[] { lblTotalRevenue, lblTotalOrders });

            // --- 3. DATA GRID ---
            dgvReports = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false
            };

            this.Controls.Add(dgvReports);
            this.Controls.Add(pnlSummary);
            this.Controls.Add(pnlFilter);
        }

        private Button CreateBtn(string text, Color color, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(100, 35),
                Location = new Point(x, y),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadReportData()
        {
            // Dummy logic to simulate a database query
            // SQL: SELECT order_id, date, total_amount, staff_name FROM sales WHERE date BETWEEN @from AND @to

            DataTable dt = new DataTable();
            dt.Columns.Add("Order ID");
            dt.Columns.Add("Date");
            dt.Columns.Add("Customer/Staff");
            dt.Columns.Add("Total Amount");

            // Example Data
            dt.Rows.Add("ORD-001", DateTime.Now.ToShortDateString(), "Admin", "$150.00");
            dt.Rows.Add("ORD-002", DateTime.Now.ToShortDateString(), "Cashier_John", "$45.50");

            dgvReports.DataSource = dt;

            // Update Summary
            UpdateSummary(195.50, 2);
        }

        private void UpdateSummary(double revenue, int orders)
        {
            lblTotalRevenue.Text = $"Total Revenue: ${revenue:N2}";
            lblTotalOrders.Text = $"Total Orders: {orders}";
        }
    }
}