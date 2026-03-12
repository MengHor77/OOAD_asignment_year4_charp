using POS_Inventory.Component;
using POS_Inventory.Config;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

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
        private Pagination pagination;

        public InventoryPage()
        {
            productConfig = new ProductConfig();
            SetupLayout();
            // Start at page 1
            LoadPageData(1);
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppColorConfig.ContentBackground;
            this.Padding = new Padding(20);

            // --- Header Title ---
            Label lblTitle = new Label
            {
                Text = "📊 Inventory Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = AppColorConfig.TextDark,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // --- Search Bar Container ---
            pnlSearch = new Panel
            {
                Size = new Size(350, 45),
                Location = new Point(20, 70),
                BackColor = Color.FromArgb(180, 200, 235)
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

            // --- DataGridView ---
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
                GridColor = Color.FromArgb(174, 214, 241),
                RowTemplate = { Height = 40 },
                ColumnHeadersHeight = 45,
                EnableHeadersVisualStyles = false
            };

            dgvInventory.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(115, 160, 250),
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

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

            // --- Pagination Panel ---
            pnlPagination = new Panel
            {
                Size = new Size(210, 50),
                BackColor = AppColorConfig.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // Setup Pagination Object (10 items per page)
            pagination = new Pagination(pnlPagination, 10, LoadPageData);

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlTableContainer);
            pnlTableContainer.Controls.Add(dgvInventory);
            this.Controls.Add(pnlPagination);

            // Handle Dynamic Positioning
            this.Resize += (s, e) => UpdatePaginationPosition();
            UpdatePaginationPosition();
        }

        private void UpdatePaginationPosition()
        {
            pnlPagination.Left = pnlTableContainer.Left + pnlTableContainer.Width - pnlPagination.Width;
            pnlPagination.Top = pnlTableContainer.Bottom + 10;
        }

        private void LoadPageData(int pageNumber)
        {
            // 1. Get all data
            DataTable dtAll = productConfig.GetAllProducts();

            // 2. Apply Search Filter if active
            string filter = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(filter) && filter != "Search by id, name, category")
            {
                DataRow[] filteredRows = dtAll.Select(string.Format(
                    "CONVERT(id, 'System.String') LIKE '%{0}%' OR product_name LIKE '%{0}%' OR category_name LIKE '%{0}%'",
                    filter.Replace("'", "''")));

                if (filteredRows.Length > 0)
                    dtAll = filteredRows.CopyToDataTable();
                else
                    dtAll = dtAll.Clone(); // Empty table if no results
            }

            // 3. Perform Pagination on the (potentially filtered) data
            int pageSize = pagination.GetPageSize();
            int startIndex = (pageNumber - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, dtAll.Rows.Count);

            DataTable dtPage = dtAll.Clone();
            for (int i = startIndex; i < endIndex; i++)
                dtPage.ImportRow(dtAll.Rows[i]);

            dgvInventory.DataSource = dtPage;

            // 4. Refresh UI
            AddActionColumns();
            pagination.Bind(dtAll);
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            // Reset to page 1 whenever search changes to avoid "page out of bounds"
            LoadPageData(1);
        }

        private void AddActionColumns()
        {
            if (dgvInventory.Columns.Contains("Edit")) dgvInventory.Columns.Remove("Edit");
            if (dgvInventory.Columns.Contains("Delete")) dgvInventory.Columns.Remove("Delete");

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
            dgvInventory.Columns.Add(btnEdit);

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
            dgvInventory.Columns.Add(btnDelete);
        }

        private void DgvInventory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            int id = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells["id"].Value);

            if (dgvInventory.Columns[e.ColumnIndex].Name == "Edit")
            {
                using (InventoryEditForm editForm = new InventoryEditForm(productConfig, id))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadPageData(pagination.GetCurrentPage());
                    }
                }
            }
            else if (dgvInventory.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Delete this record?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (productConfig.DeleteProduct(id))
                        LoadPageData(pagination.GetCurrentPage());
                }
            }
        }
    }
}