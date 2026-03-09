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

            Label lblTitle = new Label
            {
                Text = "Product Inventory",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = AppColorConfig.TextDark,
                Location = new Point(20, 20),
                AutoSize = true
            };

            btnAdd = new Button
            {
                Text = "+ Add New Product",
                BackColor = AppColorConfig.BrandBlue,
                ForeColor = AppColorConfig.TextDark,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(160, 40),
                Location = new Point(20, 70),
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            pnlTableContainer = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(this.Width - 40, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = AppColorConfig.White
            };

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
                GridColor = Color.FromArgb(240, 240, 240),
                RowTemplate = { Height = 40 },
                ColumnHeadersHeight = 45,
                EnableHeadersVisualStyles = false
            };

            dgvProduct.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.CardProduct,
                ForeColor = AppColorConfig.TextLight,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvProduct.CellContentClick += DgvProduct_CellContentClick;

            pnlTableContainer.Controls.Add(dgvProduct);
            this.Controls.Add(pnlTableContainer);
            this.Controls.Add(btnAdd);
            this.Controls.Add(lblTitle);
        }

        private void LoadData()
        {
            DataTable dt = productConfig.GetAllProducts();
            dgvProduct.DataSource = dt;

            if (dgvProduct.Columns.Contains("Edit")) dgvProduct.Columns.Remove("Edit");
            if (dgvProduct.Columns.Contains("Delete")) dgvProduct.Columns.Remove("Delete");

            // Add Action Buttons with Theme Colors
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn
            {
                Name = "Edit",
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
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 60
            };
            btnDelete.DefaultCellStyle.BackColor = AppColorConfig.HeaderPink;
            btnDelete.DefaultCellStyle.ForeColor = AppColorConfig.TextDark;
            dgvProduct.Columns.Add(btnDelete);

            if (dgvProduct.Columns.Contains("id")) dgvProduct.Columns["id"].HeaderText = "ID";
            if (dgvProduct.Columns.Contains("product_name")) dgvProduct.Columns["product_name"].HeaderText = "Product Name";
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