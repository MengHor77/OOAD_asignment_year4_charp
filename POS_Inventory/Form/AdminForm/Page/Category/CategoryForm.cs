using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Category
{
    public partial class CategoryForm : System.Windows.Forms.Form
    {
        private TextBox txtName, txtDescription;
        private Button btnSave, btnCancel;
        private CategoryConfig categoryConfig;
        private int categoryId = -1;

        public CategoryForm(CategoryConfig config, int id = -1)
        {
            categoryConfig = config;
            categoryId = id;
            InitializeComponent();
            SetupLayout();

            if (categoryId != -1)
                LoadCategory();
        }

        private void SetupLayout()
        {
            this.Size = new Size(400, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = categoryId == -1 ? "Add Category" : "Edit Category";

            Label lblName = new Label { Text = "Category Name", Location = new Point(20, 20), AutoSize = true };
            txtName = new TextBox { Location = new Point(20, 45), Width = 340, Font = new Font("Segoe UI", 10) };

            Label lblDesc = new Label { Text = "Description", Location = new Point(20, 80), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(20, 105), Width = 340, Height = 60, Multiline = true, Font = new Font("Segoe UI", 10) };

            btnSave = new Button
            {
                Text = "Save",
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(60, 180),
                Size = new Size(100, 35),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(220, 180),
                Size = new Size(100, 35),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblName, txtName, lblDesc, txtDescription, btnSave, btnCancel });
        }

        private void LoadCategory()
        {
            DataRow row = categoryConfig.GetCategoryById(categoryId);
            if (row != null)
            {
                txtName.Text = row["category_name"].ToString();
                txtDescription.Text = row["description"].ToString();
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            string name = txtName.Text.Trim();
            string desc = txtDescription.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                MessageBox.Show("Category name is required!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success = false;
            if (categoryId == -1)
                success = categoryConfig.CreateCategory(name, desc);
            else
                success = categoryConfig.UpdateCategory(categoryId, name, desc);

            if (success)
            {
                MessageBox.Show("Category saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
        }
    }
}