using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Product
{
    public partial class ProductPage : UserControl
    {
        private DataGridView dgvProduct;
        private Button btnAdd;
        private ProductConfig productConfig;
        private Panel pnlTableContainer;
        private Panel pnlPagination;

        public ProductPage()
        {
            productConfig = new ProductConfig();
            SetupLayout();
            LoadData();
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppColorConfig.ContentBackground;
            this.Padding = new Padding(20);

            // --- Header Label ---
            Label lblTitle = new Label
            {
                Text = "📊 Product Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = AppColorConfig.TextDark,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // --- Add Button ---
            btnAdd = new Button
            {
                Text = "Add new product",
                BackColor = AppColorConfig.BrandBlue,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(20, 70),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            // --- Table Container ---
            pnlTableContainer = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(this.Width - 40, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };

            // --- DataGridView Styling ---
            dgvProduct = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = AppColorConfig.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                ReadOnly = true,
                GridColor = Color.FromArgb(174, 214, 241),
                RowTemplate = { Height = 40 },
                ColumnHeadersHeight = 45,
                EnableHeadersVisualStyles = false
            };

            dgvProduct.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.CardProduct,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvProduct.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.White,
                ForeColor = AppColorConfig.TextDark,
                SelectionBackColor = Color.FromArgb(230, 240, 255),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            dgvProduct.CellContentClick += DgvProduct_CellContentClick;

            // --- Pagination Placeholder ---
            pnlPagination = new Panel
            {
                Size = new Size(210, 50),
                BackColor = AppColorConfig.White,
                Location = new Point(pnlTableContainer.Right - 200, pnlTableContainer.Bottom + 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            AddPaginationButtons();

            this.Controls.Add(pnlPagination);
            pnlTableContainer.Controls.Add(dgvProduct);
            this.Controls.Add(pnlTableContainer);
            this.Controls.Add(btnAdd);
            this.Controls.Add(lblTitle);
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

        private void LoadData()
        {
            DataTable dt = productConfig.GetAllProducts();
            dgvProduct.DataSource = dt;

            // Remove existing Action columns to avoid duplication
            if (dgvProduct.Columns.Contains("Edit")) dgvProduct.Columns.Remove("Edit");
            if (dgvProduct.Columns.Contains("Delete")) dgvProduct.Columns.Remove("Delete");

            // Hide the ID columns we don't want the user to see
            if (dgvProduct.Columns.Contains("category_id")) dgvProduct.Columns["category_id"].Visible = false;

            // Add Action Buttons
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
            dgvProduct.Columns.Add(btnEdit);

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
            dgvProduct.Columns.Add(btnDelete);

            // Rename Headers for Display
            if (dgvProduct.Columns.Contains("id")) dgvProduct.Columns["id"].HeaderText = "Id";
            if (dgvProduct.Columns.Contains("product_name")) dgvProduct.Columns["product_name"].HeaderText = "Product Name";
            if (dgvProduct.Columns.Contains("category_name")) dgvProduct.Columns["category_name"].HeaderText = "Category";
            if (dgvProduct.Columns.Contains("price")) dgvProduct.Columns["price"].HeaderText = "Price";
            if (dgvProduct.Columns.Contains("stock_qty")) dgvProduct.Columns["stock_qty"].HeaderText = "Stock";
        }

        private void DgvProduct_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int id = Convert.ToInt32(dgvProduct.Rows[e.RowIndex].Cells["id"].Value);

            if (dgvProduct.Columns[e.ColumnIndex].Name == "Edit")
            {
                using (ProductForm form = new ProductForm(productConfig, id))
                {
                    if (form.ShowDialog() == DialogResult.OK) LoadData();
                }
            }
            else if (dgvProduct.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Delete this product?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (productConfig.DeleteProduct(id)) LoadData();
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (ProductForm form = new ProductForm(productConfig))
            {
                if (form.ShowDialog() == DialogResult.OK) LoadData();
            }
        }
    }
}