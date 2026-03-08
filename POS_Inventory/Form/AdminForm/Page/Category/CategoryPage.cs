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

        public CategoryPage()
        {
            InitializeComponent();
            categoryConfig = new CategoryConfig();
            SetupLayout();
            LoadData();
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // --- Top Panel ---
            Panel pnlTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = Color.White };
            Label lblTitle = new Label
            {
                Text = "Category Management",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            btnAdd = new Button
            {
                Text = "Add New",
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(100, 35),
                Location = new Point(400, 12),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(btnAdd);

            // --- DataGridView ---
            dgvCategory = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true
            };
            dgvCategory.CellClick += DgvCategory_CellClick;

            this.Controls.Add(dgvCategory);
            this.Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            DataTable dt = categoryConfig.GetAllCategories();

            // Add Action Buttons column
            if (!dt.Columns.Contains("Action"))
                dt.Columns.Add("Action", typeof(string));

            foreach (DataRow row in dt.Rows)
                row["Action"] = "Edit";

            dgvCategory.DataSource = dt;

            // Add Button Column for Edit
            if (!dgvCategory.Columns.Contains("btnEdit"))
            {
                DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn
                {
                    Name = "btnEdit",
                    HeaderText = "Action",
                    Text = "Edit",
                    UseColumnTextForButtonValue = true
                };
                dgvCategory.Columns.Add(btnCol);
            }

            dgvCategory.Columns["id"].HeaderText = "ID";
            dgvCategory.Columns["category_name"].HeaderText = "Category Name";
            dgvCategory.Columns["description"].HeaderText = "Description";
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            using (CategoryForm form = new CategoryForm(categoryConfig))
            {
                form.ShowDialog();
                LoadData();
            }
        }

        private void DgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dgvCategory.Columns[e.ColumnIndex].Name == "btnEdit")
            {
                int id = Convert.ToInt32(dgvCategory.Rows[e.RowIndex].Cells["id"].Value);
                using (CategoryForm form = new CategoryForm(categoryConfig, id))
                {
                    form.ShowDialog();
                    LoadData();
                }
            }
        }
    }
}