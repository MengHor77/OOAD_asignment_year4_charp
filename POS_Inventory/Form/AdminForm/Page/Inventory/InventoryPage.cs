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
        private TextBox txtSearch;
        private ProductConfig productConfig;
        private Panel pnlTableContainer;
        private Panel pnlPagination;
        private Panel pnlSearch;

        public InventoryPage()
        {
            productConfig = new ProductConfig();
            SetupLayout();
            LoadInventoryData();
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppColorConfig.ContentBackground;
            this.Padding = new Padding(20);

            // --- Header Title ---
            Label lblTitle = new Label
            {
                Text = "Inventory Management",
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                ForeColor = AppColorConfig.TextDark,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // --- Search Bar Container (Pill Style) ---
            pnlSearch = new Panel
            {
                Size = new Size(350, 45),
                Location = new Point(20, 70),
                BackColor = Color.FromArgb(180, 200, 235) // Muted blue from your photo
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(180, 200, 235),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(60, 60, 60),
                Text = "Search by id, name, category",
                Width = 300,
                Location = new Point(15, 12)
            };
            txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search by id, name, category") txtSearch.Text = ""; };
            txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) txtSearch.Text = "Search by id, name, category"; };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            pnlSearch.Controls.Add(txtSearch);

            // --- Table Container ---
            pnlTableContainer = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(this.Width - 40, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };

            // --- DataGridView (Styled exactly like ProductPage) ---
            dgvInventory = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = AppColorConfig.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                ReadOnly = true,
                GridColor = Color.FromArgb(174, 214, 241), // Matches ProductPage
                RowTemplate = { Height = 40 },
                ColumnHeadersHeight = 45,
                EnableHeadersVisualStyles = false
            };

            // Header Style (Blue variant from photo, using ProductPage fonts)
            dgvInventory.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(115, 160, 250), // Header Blue from photo
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // Cell Style (Matches ProductPage)
            dgvInventory.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.White,
                ForeColor = AppColorConfig.TextDark,
                SelectionBackColor = Color.FromArgb(230, 240, 255),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            dgvInventory.CellContentClick += DgvInventory_CellContentClick;

            // --- Pagination Placeholder ---
            pnlPagination = new Panel
            {
                Size = new Size(210, 50),
                BackColor = AppColorConfig.White,
                Location = new Point(pnlTableContainer.Right - 200, pnlTableContainer.Bottom + 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AddPaginationButtons();

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlTableContainer);
            pnlTableContainer.Controls.Add(dgvInventory);
            this.Controls.Add(pnlPagination);
        }

        private void AddPaginationButtons()
        {
            string[] buttons = { "<", "1", "2", "3", ">" };
            int x = 5;
            foreach (var b in buttons)
            {
                Button btn = new Button
                {
                    Text = b,
                    Size = new Size(35, 35),
                    Location = new Point(x, 7),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = (b == "2") ? Color.Orange : AppColorConfig.White,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                btn.FlatAppearance.BorderColor = Color.LightGray;
                pnlPagination.Controls.Add(btn);
                x += 40;
            }
        }

        private void LoadInventoryData()
        {
            DataTable dt = productConfig.GetAllProducts();
            dgvInventory.DataSource = dt;

            // Remove existing Action columns
            if (dgvInventory.Columns.Contains("Edit")) dgvInventory.Columns.Remove("Edit");
            if (dgvInventory.Columns.Contains("Delete")) dgvInventory.Columns.Remove("Delete");

            // Hide ID columns
            if (dgvInventory.Columns.Contains("category_id")) dgvInventory.Columns["category_id"].Visible = false;

            // Edit Column (Matches ProductPage)
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "action",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 60
            };
            btnEdit.DefaultCellStyle.BackColor = AppColorConfig.CardStaff;
            btnEdit.DefaultCellStyle.ForeColor = AppColorConfig.TextDark;
            dgvInventory.Columns.Add(btnEdit);

            // Delete Column (Matches ProductPage)
            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "action",
                Text = "delete",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 60
            };
            btnDelete.DefaultCellStyle.BackColor = AppColorConfig.HeaderPink;
            btnDelete.DefaultCellStyle.ForeColor = AppColorConfig.TextDark;
            dgvInventory.Columns.Add(btnDelete);

            // Column Header Renaming
            if (dgvInventory.Columns.Contains("id")) dgvInventory.Columns["id"].HeaderText = "Product Id";
            if (dgvInventory.Columns.Contains("product_name")) dgvInventory.Columns["product_name"].HeaderText = "Product Name";
            if (dgvInventory.Columns.Contains("category_name")) dgvInventory.Columns["category_name"].HeaderText = "Category Name";
            if (dgvInventory.Columns.Contains("stock_qty")) dgvInventory.Columns["stock_qty"].HeaderText = "stock";
            if (dgvInventory.Columns.Contains("price")) dgvInventory.Columns["price"].HeaderText = "price";
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvInventory.DataSource is DataTable dt)
            {
                string filter = txtSearch.Text;
                if (filter != "Search by id, name, category" && !string.IsNullOrWhiteSpace(filter))
                {
                    // Filters by Product Name or Category Name
                    dt.DefaultView.RowFilter = string.Format("product_name LIKE '%{0}%' OR category_name LIKE '%{0}%'", filter);
                }
                else
                {
                    dt.DefaultView.RowFilter = "";
                }
            }
        }

        private void DgvInventory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int id = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells["id"].Value);

            if (dgvInventory.Columns[e.ColumnIndex].Name == "Edit")
            {
                // This triggers the same Edit form as ProductPage
                // Note: You may need to ensure ProductForm is accessible from this namespace
                // using (ProductForm form = new ProductForm(productConfig, id)) 
                // { if (form.ShowDialog() == DialogResult.OK) LoadInventoryData(); }
            }
            else if (dgvInventory.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Delete this record?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (productConfig.DeleteProduct(id)) LoadInventoryData();
                }
            }
        }
    }
}