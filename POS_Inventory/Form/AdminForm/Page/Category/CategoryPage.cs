using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;
using POS_Inventory.Component;


namespace POS_Inventory.Form.AdminForm.Page.Category
{
    public partial class CategoryPage : UserControl
    {
        private DataGridView dgvCategory;
        private Button btnAdd;
        private CategoryConfig categoryConfig;
        private Panel pnlTableContainer;
        private Panel pnlPagination;
        private Pagination pagination;

        public CategoryPage()
        {
            categoryConfig = new CategoryConfig();
            SetupLayout();
            LoadData();
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
                Text = "📊 Category Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = AppColorConfig.TextDark,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // --- Add Button ---
            btnAdd = new Button
            {
                Text = "Add new category",
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

            pnlTableContainer.Width = this.ClientSize.Width - 40;

            // --- DataGridView ---
            dgvCategory = new DataGridView
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

            dgvCategory.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.CardProduct,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            dgvCategory.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.White,
                ForeColor = AppColorConfig.TextDark,
                SelectionBackColor = Color.FromArgb(230, 240, 255),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            dgvCategory.CellContentClick += DgvCategory_CellContentClick;

            // --- Pagination Panel ---
            pnlPagination = new Panel
            {
                Size = new Size(210, 50),
                BackColor = AppColorConfig.White,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            // --- Add Controls ---
            pnlTableContainer.Controls.Add(dgvCategory);
            this.Controls.Add(pnlTableContainer);
            this.Controls.Add(btnAdd);
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlPagination);
            UpdatePaginationPosition();
            this.Resize += (s, e) => UpdatePaginationPosition();
            pagination = new Pagination(pnlPagination, 10, LoadPageData);
        }

        void UpdatePaginationPosition()
        {
            pnlPagination.Left = pnlTableContainer.Left + pnlTableContainer.Width - pnlPagination.Width;
            pnlPagination.Top = pnlTableContainer.Bottom + 10;
        }

        private void LoadPageData(int pageNumber)
        {
            DataTable dtAll = categoryConfig.GetAllCategories();
            int pageSize = pagination.GetPageSize();
            int startIndex = (pageNumber - 1) * pageSize;
            int endIndex = Math.Min(startIndex + pageSize, dtAll.Rows.Count);

            DataTable dtPage = dtAll.Clone();
            for (int i = startIndex; i < endIndex; i++)
                dtPage.ImportRow(dtAll.Rows[i]);

            dgvCategory.DataSource = dtPage;

            AddActionColumns();          // add Edit/Delete buttons
            pagination.Bind(dtAll);      // refresh pagination buttons
        }

        private void LoadData()
        {
            LoadPageData(1); // just loads page 1 using pagination
        }

        private void AddActionColumns()
        {
            if (dgvCategory.Columns.Contains("Edit")) dgvCategory.Columns.Remove("Edit");
            if (dgvCategory.Columns.Contains("Delete")) dgvCategory.Columns.Remove("Delete");

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
            dgvCategory.Columns.Add(btnEdit);

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
            dgvCategory.Columns.Add(btnDelete);

            // Fix column headers
            if (dgvCategory.Columns.Contains("id")) dgvCategory.Columns["id"].HeaderText = "Id";
            if (dgvCategory.Columns.Contains("category_name")) dgvCategory.Columns["category_name"].HeaderText = "Category Name";
            if (dgvCategory.Columns.Contains("description")) dgvCategory.Columns["description"].HeaderText = "Description";
        }


        private void DgvCategory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(dgvCategory.Rows[e.RowIndex].Cells["id"].Value);

            if (dgvCategory.Columns[e.ColumnIndex].Name == "Edit")
            {
                using (CategoryEditForm form = new CategoryEditForm(categoryConfig, id))
                {
                    form.ShowDialog();
                    LoadData();
                }
            }

            if (dgvCategory.Columns[e.ColumnIndex].Name == "Delete")
            {
                if (MessageBox.Show("Delete this category?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    if (categoryConfig.DeleteCategory(id)) LoadData();
                }
            }
        }
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (CategoryEditForm form = new CategoryEditForm(categoryConfig))
            {
                form.ShowDialog();
                LoadData();
            }
        }

    }
}