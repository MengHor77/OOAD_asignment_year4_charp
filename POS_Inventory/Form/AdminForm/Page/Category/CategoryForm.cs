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
            // InitializeComponent(); // Only keep this if you have a Designer.cs file
            SetupLayout();

            if (categoryId != -1)
                LoadCategory();
        }

        private void SetupLayout()
        {
            // Increased height to 380 to fit the larger buttons properly
            this.Size = new Size(400, 380);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = AppColorConfig.White;
            this.Text = categoryId == -1 ? "Add Category" : "Edit Category";
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // --- Category Name ---
            Label lblName = new Label { Text = "Category Name", Location = new Point(30, 20), AutoSize = true, Font = new Font("Segoe UI", 9), ForeColor = AppColorConfig.TextDark };
            txtName = new TextBox { Location = new Point(30, 45), Width = 320, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };

            // --- Description ---
            Label lblDesc = new Label { Text = "Description", Location = new Point(30, 85), AutoSize = true, Font = new Font("Segoe UI", 9), ForeColor = AppColorConfig.TextDark };
            txtDescription = new TextBox
            {
                Location = new Point(30, 110),
                Width = 320,
                Height = 100,
                Multiline = true,
                Font = new Font("Segoe UI", 10),
                BorderStyle = BorderStyle.FixedSingle
            };

            // --- Buttons (Matching ProductForm Style) ---
            int buttonY = 240;

            btnSave = new Button
            {
                Text = "Save Changes",
                Size = new Size(155, 45),
                Location = new Point(30, buttonY),
                BackColor = AppColorConfig.BtnSave,
                ForeColor = AppColorConfig.TextDark,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(155, 45),
                Location = new Point(195, buttonY), // Side-by-side with 10px gap
                BackColor = AppColorConfig.BtnCancel,
                ForeColor = AppColorConfig.TextDark,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            // Add all controls to form
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
            try
            {
                if (categoryId == -1)
                    success = categoryConfig.CreateCategory(name, desc);
                else
                    success = categoryConfig.UpdateCategory(categoryId, name, desc);

                if (success)
                {
                    // This tells the calling page that the save was successful
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving category: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}