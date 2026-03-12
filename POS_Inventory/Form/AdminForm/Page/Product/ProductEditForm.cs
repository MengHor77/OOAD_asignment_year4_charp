using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Product
{
    public partial class ProductEditForm : System.Windows.Forms.Form
    {
        private readonly ProductConfig _productConfig;
        private readonly int _productId;
        private TextBox txtName, txtPrice, txtStock;
        private ComboBox cmbCategory;
        private Button btnSave, btnCancel;

        public ProductEditForm(ProductConfig config, int id = -1)
        {
            _productConfig = config;
            _productId = id;
            SetupForm();
            LoadCategories();
            if (_productId != -1) LoadProductData();
        }

        private void SetupForm()
        {
            this.Size = new Size(400, 450); // Increased height slightly for buttons
            this.Text = _productId == -1 ? "Add Product" : "Edit Product";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = AppColorConfig.White;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int y = 30;
            CreateLabel("Product Name", 30, y);
            txtName = CreateTextBox(30, y + 25);

            y += 70;
            CreateLabel("Category", 30, y);
            cmbCategory = new ComboBox { Location = new Point(30, y + 25), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(cmbCategory);

            y += 70;
            CreateLabel("Price", 30, y);
            txtPrice = CreateTextBox(30, y + 25);

            y += 70;
            CreateLabel("Stock Quantity", 30, y);
            txtStock = CreateTextBox(30, y + 25);

            y += 80;
            // --- Save Button ---
            btnSave = new Button
            {
                Text = "Save Changes",
                Size = new Size(155, 45),
                Location = new Point(30, y),
                BackColor = AppColorConfig.BtnSave,
                ForeColor = AppColorConfig.TextDark,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            // --- Cancel Button ---
            btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(155, 45),
                Location = new Point(195, y), 
                BackColor = AppColorConfig.BtnCancel,
                ForeColor = AppColorConfig.TextDark,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);
        }

        private void CreateLabel(string text, int x, int y)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = AppColorConfig.TextDark
            });
        }

        private TextBox CreateTextBox(int x, int y)
        {
            TextBox tb = new TextBox { Location = new Point(x, y), Width = 320, Font = new Font("Segoe UI", 10) };
            this.Controls.Add(tb);
            return tb;
        }

        private void LoadCategories()
        {
            CategoryConfig catConfig = new CategoryConfig();
            cmbCategory.DataSource = catConfig.GetAllCategories();
            cmbCategory.DisplayMember = "category_name";
            cmbCategory.ValueMember = "id";
        }

        private void LoadProductData()
        {
            DataRow row = _productConfig.GetProductById(_productId);
            if (row != null)
            {
                txtName.Text = row["product_name"].ToString();
                txtPrice.Text = row["price"].ToString();
                txtStock.Text = row["stock_qty"].ToString();
                cmbCategory.SelectedValue = row["category_id"];
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtName.Text)) throw new Exception("Name is required.");
                if (!decimal.TryParse(txtPrice.Text, out decimal price)) throw new Exception("Invalid Price.");
                if (!int.TryParse(txtStock.Text, out int stock)) throw new Exception("Invalid Stock.");

                int catId = Convert.ToInt32(cmbCategory.SelectedValue);

                bool result = (_productId == -1)
                    ? _productConfig.CreateProduct(txtName.Text, catId, price, stock, 1)
                    : _productConfig.UpdateProduct(_productId, txtName.Text, catId, price, stock);

                if (result)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}