using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config; // Assuming this contains your AppColorConfix

namespace POS_Inventory.Form.AdminForm.Page.Product
{
    public partial class ProductPage : UserControl
    {
        private DataGridView dgvProduct;
        private TextBox txtProductName, txtPrice, txtStock;
        private ComboBox cmbCategory;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private int selectedId = -1;

        public ProductPage()
        {
            InitializeComponent();
            SetupLayout();
            LoadCategories(); // Load categories for the dropdown
            LoadData();       // Load product list
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // --- 1. TOP INPUT PANEL ---
            Panel pnlInputs = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = Color.FromArgb(240, 240, 240), Padding = new Padding(10) };

            Label lblTitle = new Label { Text = "Product Management", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 10), AutoSize = true };

            // Input Fields
            AddLabelAndControl(pnlInputs, "Product Name:", txtProductName = new TextBox { Width = 200 }, 20, 50);
            AddLabelAndControl(pnlInputs, "Category:", cmbCategory = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDownList }, 240, 50);
            AddLabelAndControl(pnlInputs, "Price:", txtPrice = new TextBox { Width = 100 }, 20, 100);
            AddLabelAndControl(pnlInputs, "Stock:", txtStock = new TextBox { Width = 100 }, 140, 100);

            // Buttons
            btnAdd = CreateBtn("Add", Color.SeaGreen, 300, 115);
            btnUpdate = CreateBtn("Update", Color.Orange, 410, 115);
            btnDelete = CreateBtn("Delete", Color.Crimson, 520, 115);
            btnClear = CreateBtn("Clear", Color.Gray, 630, 115);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearForm();

            pnlInputs.Controls.AddRange(new Control[] { lblTitle, btnAdd, btnUpdate, btnDelete, btnClear });
            this.Controls.Add(pnlInputs);

            // --- 2. GRID VIEW ---
            dgvProduct = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false
            };
            dgvProduct.CellClick += DgvProduct_CellClick;
            this.Controls.Add(dgvProduct);
            dgvProduct.BringToFront();
        }

        private void AddLabelAndControl(Panel p, string labelText, Control ctrl, int x, int y)
        {
            Label lbl = new Label { Text = labelText, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9) };
            ctrl.Location = new Point(x, y + 20);
            p.Controls.Add(lbl);
            p.Controls.Add(ctrl);
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

        // --- DATA LOGIC ---

        private void LoadCategories()
        {
            // Dummy Data - Replace with: SELECT id, category_name FROM categories
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("Food");
            cmbCategory.Items.Add("Drinks");
            cmbCategory.SelectedIndex = 0;
        }

        private void LoadData()
        {
            // Dummy Data - Replace with: SELECT p.id, p.name, c.name as Category, p.price, p.stock FROM products...
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Product Name");
            dt.Columns.Add("Category");
            dt.Columns.Add("Price");
            dt.Columns.Add("Stock");
            dt.Rows.Add(1, "Coca Cola", "Drinks", "1.50", "50");
            dgvProduct.DataSource = dt;
        }

        private void DgvProduct_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvProduct.Rows[e.RowIndex];
                selectedId = Convert.ToInt32(row.Cells["ID"].Value);
                txtProductName.Text = row.Cells["Product Name"].Value.ToString();
                cmbCategory.SelectedItem = row.Cells["Category"].Value.ToString();
                txtPrice.Text = row.Cells["Price"].Value.ToString();
                txtStock.Text = row.Cells["Stock"].Value.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            // Logic: Validate inputs -> Execute INSERT SQL -> LoadData()
            MessageBox.Show($"Product '{txtProductName.Text}' Added!");
            LoadData();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            MessageBox.Show("Product Updated!");
            LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            if (MessageBox.Show("Delete this product?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                LoadData();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            txtProductName.Clear();
            txtPrice.Clear();
            txtStock.Clear();
            cmbCategory.SelectedIndex = 0;
            selectedId = -1;
        }
    }
}