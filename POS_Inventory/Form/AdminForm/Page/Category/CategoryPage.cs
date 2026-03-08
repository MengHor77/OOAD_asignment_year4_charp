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
        private TextBox txtCategoryName;
        private Button btnAdd, btnUpdate, btnDelete, btnClear;
        private int selectedId = -1;

        public CategoryPage()
        {
            InitializeComponent(); // This calls the code in .Designer.cs
            SetupLayout();
            LoadData();
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // --- 1. TOP INPUT PANEL ---
            Panel pnlInputs = new Panel { Dock = DockStyle.Top, Height = 150, BackColor = Color.FromArgb(245, 245, 245) };

            Label lblTitle = new Label
            {
                Text = "Category Management",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 10),
                AutoSize = true
            };

            Label lblName = new Label {
                Text = "Category Name", 
                Location = new Point(20, 50), 
                AutoSize = true,
                Padding = new Padding(0, 20, 0, 0),// left, top, right, bottom
            };
            txtCategoryName = new TextBox { Location = new Point(20, 70), Width = 300, Font = new Font("Segoe UI", 12) };

            // Buttons
            btnAdd = CreateBtn("Add New", Color.FromArgb(40, 167, 69), 20, 110);
            btnUpdate = CreateBtn("Update", Color.FromArgb(255, 193, 7), 130, 110);
            btnDelete = CreateBtn("Delete", Color.FromArgb(220, 53, 69), 240, 110);
            btnClear = CreateBtn("Clear", Color.Gray, 350, 110);

            // Events
            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearForm();

            pnlInputs.Controls.AddRange(new Control[] { lblTitle, lblName, txtCategoryName, btnAdd, btnUpdate, btnDelete, btnClear });

            // --- 2. GRID VIEW ---
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
            this.Controls.Add(pnlInputs);
        }

        private Button CreateBtn(string text, Color backColor, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(100, 35),
                Location = new Point(x, y),
                BackColor = backColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // --- DATABASE LOGIC (CRUD) ---

        private void LoadData()
        {
            try
            {
                // Replace this with your actual DB helper:
                // dgvCategory.DataSource = dbHelper.GetDataTable("SELECT id, category_name FROM categories");

                // Temporary Dummy Data for testing UI
                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("Category Name");
                dt.Rows.Add(1, "Food");
                dt.Rows.Add(2, "Drinks");
                dgvCategory.DataSource = dt;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void DgvCategory_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvCategory.Rows[e.RowIndex];
                selectedId = Convert.ToInt32(row.Cells["ID"].Value);
                txtCategoryName.Text = row.Cells["Category Name"].Value.ToString();
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCategoryName.Text)) return;
            // DB logic: INSERT INTO categories (category_name) VALUES (@name)
            MessageBox.Show("Category Added Successfully!");
            LoadData();
            ClearForm();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            // DB logic: UPDATE categories SET category_name = @name WHERE id = @id
            MessageBox.Show("Category Updated!");
            LoadData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            var confirm = MessageBox.Show("Delete this category?", "Confirm", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                // DB logic: DELETE FROM categories WHERE id = @id
                LoadData();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            txtCategoryName.Clear();
            selectedId = -1;
        }
    }
}