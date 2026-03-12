using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;
using POS_Inventory.Component; // Make sure Pagination class is here

namespace POS_Inventory.Form.AdminForm.Page.Product
{
    public partial class ProductPage : UserControl
    {
        private DataGridView dgvProduct;
        private Button btnAdd;
        private ProductConfig productConfig;
        private Panel pnlTableContainer;
        private Panel pnlPagination;
        private Pagination pagination;

        public ProductPage()
        {
            productConfig = new ProductConfig();
            SetupLayout();

            this.Load += (s, e) =>
            {
                pnlTableContainer.Width = this.ClientSize.Width - 40;
            };


            LoadPageData(1); // Load first page on initialization

        }

        private void SetupLayout()
        {
            // --- UserControl Setup ---
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
                Height = 400,
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

            pnlTableContainer.Controls.Add(dgvProduct);

            // --- Pagination Panel ---
            pnlPagination = new Panel
            {
                Size = new Size(210, 50),
                BackColor = AppColorConfig.White
            };

            // Align pagination like Category page
            void UpdatePaginationPosition()
            {
                pnlPagination.Left = pnlTableContainer.Left + pnlTableContainer.Width - pnlPagination.Width;
                pnlPagination.Top = pnlTableContainer.Bottom + 10;
            }


            // Update on resize
            this.Resize += (s, e) =>
            {
                pnlTableContainer.Width = this.ClientSize.Width - 40;
                UpdatePaginationPosition();
            };
            // --- Add Controls ---
            this.Controls.Add(lblTitle);
            this.Controls.Add(btnAdd);
            this.Controls.Add(pnlTableContainer);
            this.Controls.Add(pnlPagination);

            // Initial position
            UpdatePaginationPosition();


            // --- Pagination Object ---
            pagination = new Pagination(pnlPagination, 10, LoadPageData); // page size = 10
        }        // --- Load one page of data ---
        private void LoadPageData(int pageNumber)
        {
            DataTable dtAll = productConfig.GetAllProducts();
            int pageSize = pagination.GetPageSize();
            int startIndex = (pageNumber - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, dtAll.Rows.Count);

            DataTable dtPage = dtAll.Clone();
            for (int i = startIndex; i < endIndex; i++)
                dtPage.ImportRow(dtAll.Rows[i]);

            dgvProduct.DataSource = dtPage;

            AddActionColumns();
            pagination.Bind(dtAll); // refresh pagination buttons
        }

        private void AddActionColumns()
        {
            if (dgvProduct.Columns.Contains("Edit")) dgvProduct.Columns.Remove("Edit");
            if (dgvProduct.Columns.Contains("Delete")) dgvProduct.Columns.Remove("Delete");

            if (dgvProduct.Columns.Contains("category_id")) dgvProduct.Columns["category_id"].Visible = false;

            var btnEdit = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "Action",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 60
            };
            btnEdit.DefaultCellStyle.BackColor = AppColorConfig.CardStaff;
            btnEdit.DefaultCellStyle.ForeColor = AppColorConfig.TextDark;
            dgvProduct.Columns.Add(btnEdit);

            var btnDelete = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "Action",
                Text = "Delete",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 60
            };
            btnDelete.DefaultCellStyle.BackColor = AppColorConfig.HeaderPink;
            btnDelete.DefaultCellStyle.ForeColor = AppColorConfig.TextDark;
            dgvProduct.Columns.Add(btnDelete);

            // Rename headers
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
                    if (form.ShowDialog() == DialogResult.OK) LoadPageData(pagination.GetCurrentPage());
                }
            }
            else if (dgvProduct.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Delete this product?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (productConfig.DeleteProduct(id)) LoadPageData(pagination.GetCurrentPage());
                }
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (ProductForm form = new ProductForm(productConfig))
            {
                if (form.ShowDialog() == DialogResult.OK) LoadPageData(pagination.GetCurrentPage());
            }
        }
    }
}