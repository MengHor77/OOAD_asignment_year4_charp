using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Inventory
{
    public partial class InventoryPage : UserControl
    {
        private DataGridView dgvInventory;
        private TextBox txtSearch, txtAdjustQuantity;
        private Button btnAddStock, btnRemoveStock, btnRefresh;
        private Label lblLowStockWarning;
        private int selectedId = -1;
        private int currentStockValue = 0;

        public InventoryPage()
        {
            InitializeComponent();
            SetupLayout();
            LoadInventoryData();
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // --- 1. TOP CONTROL PANEL ---
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 100, BackColor = Color.FromArgb(240, 240, 240) };

            Label lblTitle = new Label
            {
                Text = "Inventory & Stock Control",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 10),
                AutoSize = true
            };

            Label lblSearch = new Label { Text = "Search Product:", Location = new Point(20, 45), AutoSize = true };
            txtSearch = new TextBox { Location = new Point(20, 65), Width = 250, Font = new Font("Segoe UI", 10) };
            txtSearch.TextChanged += (s, e) => { /* Add Filter Logic Here */ };

            lblLowStockWarning = new Label
            {
                Text = "⚠️ Low stock items detected!",
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Location = new Point(300, 68),
                Visible = false,
                AutoSize = true
            };

            pnlTop.Controls.AddRange(new Control[] { lblTitle, lblSearch, txtSearch, lblLowStockWarning });

            // --- 2. RIGHT ADJUSTMENT PANEL ---
            Panel pnlAdjust = new Panel { Dock = DockStyle.Right, Width = 200, BackColor = Color.FromArgb(230, 230, 230), Padding = new Padding(15) };

            Label lblAdjustTitle = new Label { Text = "Stock Adjustment", Font = new Font("Segoe UI", 10, FontStyle.Bold), Dock = DockStyle.Top, Height = 30 };

            Label lblQty = new Label { Text = "Quantity to Change:", Dock = DockStyle.Top, Height = 25 };
            txtAdjustQuantity = new TextBox { Dock = DockStyle.Top, Font = new Font("Segoe UI", 12) };

            btnAddStock = CreateActionBtn("Add to Stock", Color.SeaGreen);
            btnRemoveStock = CreateActionBtn("Remove from Stock", Color.Crimson);
            btnRefresh = CreateActionBtn("Refresh List", Color.RoyalBlue);

            btnAddStock.Click += (s, e) => UpdateStock(true);
            btnRemoveStock.Click += (s, e) => UpdateStock(false);
            btnRefresh.Click += (s, e) => LoadInventoryData();

            pnlAdjust.Controls.Add(btnRefresh);
            pnlAdjust.Controls.Add(btnRemoveStock);
            pnlAdjust.Controls.Add(btnAddStock);
            pnlAdjust.Controls.Add(txtAdjustQuantity);
            pnlAdjust.Controls.Add(lblQty);
            pnlAdjust.Controls.Add(lblAdjustTitle);

            // --- 3. GRID VIEW ---
            dgvInventory = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false
            };
            dgvInventory.CellClick += DgvInventory_CellClick;

            this.Controls.Add(dgvInventory);
            this.Controls.Add(pnlAdjust);
            this.Controls.Add(pnlTop);
        }

        private Button CreateActionBtn(string text, Color color)
        {
            Button btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 40,
                Margin = new Padding(0, 10, 0, 0),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void LoadInventoryData()
        {
            // Dummy Data - Replace with SQL: 
            // SELECT id, product_name, category, stock_level, min_threshold FROM products
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Product Name");
            dt.Columns.Add("Stock Level");
            dt.Columns.Add("Status");

            dt.Rows.Add(1, "Coca Cola", 50, "In Stock");
            dt.Rows.Add(2, "Lays Chips", 3, "Low Stock"); // Trigger red warning

            dgvInventory.DataSource = dt;
            CheckLowStock(dt);
        }

        private void CheckLowStock(DataTable dt)
        {
            bool hasLowStock = false;
            foreach (DataRow row in dt.Rows)
            {
                if (Convert.ToInt32(row["Stock Level"]) < 10) // Example threshold
                {
                    hasLowStock = true;
                    break;
                }
            }
            lblLowStockWarning.Visible = hasLowStock;
        }

        private void DgvInventory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                selectedId = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells["ID"].Value);
                currentStockValue = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells["Stock Level"].Value);
            }
        }
        private void UpdateStock(bool isAdd)
        {
            if (selectedId == -1)
            {
                MessageBox.Show("Please select a product first.");
                return;
            }

            if (!int.TryParse(txtAdjustQuantity.Text, out int qty))
            {
                MessageBox.Show("Invalid quantity.");
                return;
            }

            if (isAdd)
                currentStockValue += qty;
            else
                currentStockValue -= qty;

            if (currentStockValue < 0)
                currentStockValue = 0;

            dgvInventory.SelectedRows[0].Cells["Stock Level"].Value = currentStockValue;

            MessageBox.Show("Stock updated successfully!");
        }
    }
}