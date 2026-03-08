using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Category
{
    public partial class CategoryPage : UserControl
    {
        private DataGridView dgvCategory;
        private Button btnAdd;
        private CategoryConfig categoryConfig;
        private Panel pnlTableContainer;
        private Panel pnlPagination;

        public CategoryPage()
        {
            categoryConfig = new CategoryConfig();
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
                Text = "Category Management",
                Font = new Font("Segoe UI", 16, FontStyle.Regular),
                ForeColor = Color.Black,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // --- Add Button (Rounded style) ---
            btnAdd = new Button
            {
                Text = "Add new category",
                BackColor = Color.FromArgb(180, 190, 230), // Soft purple/blue
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(150, 40),
                Location = new Point(20, 70),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            // --- Table Container (To provide margins) ---
            pnlTableContainer = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(this.Width - 40, 300),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };

            // --- DataGridView Styling ---
            dgvCategory = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.FromArgb(174, 214, 241),
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

            // Header Style
            dgvCategory.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(120, 160, 255), // Bright blue header
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // Cell Style
            dgvCategory.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.White,
                ForeColor = Color.Black,
                SelectionBackColor = Color.FromArgb(230, 240, 255),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            dgvCategory.CellContentClick += DgvCategory_CellContentClick;

            // --- Pagination Placeholder ---
            pnlPagination = new Panel
            {
                Size = new Size(200, 50),
                BackColor = Color.White,
                Location = new Point(this.Width - 250, 450),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            // Adding dummy pagination buttons to match photo
            AddPaginationButtons();

            this.Controls.Add(pnlPagination);
            pnlTableContainer.Controls.Add(dgvCategory);
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
                    Size = new Size(30, 30),
                    Location = new Point(x, 10),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = (b == "2") ? Color.Orange : Color.White // Highlight active page
                };
                btn.FlatAppearance.BorderColor = Color.LightGray;
                pnlPagination.Controls.Add(btn);
                x += 35;
            }
        }

        private void LoadData()
        {
            DataTable dt = categoryConfig.GetAllCategories();
            dgvCategory.DataSource = dt;

            // Clear existing columns to re-add buttons at the end
            if (dgvCategory.Columns.Contains("Edit")) dgvCategory.Columns.Remove("Edit");
            if (dgvCategory.Columns.Contains("Delete")) dgvCategory.Columns.Remove("Delete");

            // Edit Column
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "action",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 60
            };
            btnEdit.DefaultCellStyle.BackColor = Color.FromArgb(120, 150, 230);
            btnEdit.DefaultCellStyle.ForeColor = Color.White;
            dgvCategory.Columns.Add(btnEdit);

            // Delete Column
            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "",
                Text = "delete",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 60
            };
            btnDelete.DefaultCellStyle.BackColor = Color.FromArgb(230, 160, 150);
            btnDelete.DefaultCellStyle.ForeColor = Color.White;
            dgvCategory.Columns.Add(btnDelete);

            // Formatting column headers
            if (dgvCategory.Columns.Contains("id")) dgvCategory.Columns["id"].HeaderText = "Id";
            if (dgvCategory.Columns.Contains("category_name")) dgvCategory.Columns["category_name"].HeaderText = "Category Name";
            if (dgvCategory.Columns.Contains("description")) dgvCategory.Columns["description"].HeaderText = "Desscription"; // Matching your typo
        }

        private void DgvCategory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(dgvCategory.Rows[e.RowIndex].Cells["id"].Value);

            if (dgvCategory.Columns[e.ColumnIndex].Name == "Edit")
            {
                using (CategoryForm form = new CategoryForm(categoryConfig, id))
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
            using (CategoryForm form = new CategoryForm(categoryConfig))
            {
                form.ShowDialog();
                LoadData();
            }
        }

    }
}