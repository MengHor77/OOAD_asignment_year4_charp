using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Product
{
    public partial class ProductForm : System.Windows.Forms.Form
    {
        private ProductConfig _productConfig;
        private int _productId;
        private TextBox txtName, txtPrice, txtStock;
        private ComboBox cmbCategory;
        private Button btnSave;

        public ProductForm(ProductConfig config, int id = -1)
        {
            _productConfig = config;
            _productId = id;
            SetupForm();
            LoadCategories();
            if (_productId != -1) LoadProductData();
        }

        private void SetupForm()
        {
            this.Size = new Size(400, 480);
            this.Text = _productId == -1 ? "Add Product" : "Edit Product";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = AppColorConfig.White;

            int y = 30;
            CreateLabel("Product Name", 30, y);
            txtName = CreateTextBox(30, y + 25);

            y += 70;
            CreateLabel("Category", 30, y);
            cmbCategory = new ComboBox { Location = new Point(30, y + 25), Width = 320, DropDownStyle = ComboBoxStyle.DropDownList };
            this.Controls.Add(cmbCategory);

            y += 70;
            CreateLabel("Price", 30, y);
            txtPrice = CreateTextBox(30, y + 25);

            y += 70;
            CreateLabel("Stock Quantity", 30, y);
            txtStock = CreateTextBox(30, y + 25);

            y += 80;
            btnSave = new Button
            {
                Text = "Save Changes",
                Size = new Size(320, 45),
                Location = new Point(30, y),
                BackColor = AppColorConfig.BrandBlue,
                ForeColor = AppColorConfig.TextDark,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
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
                decimal price = decimal.Parse(txtPrice.Text);
                int stock = int.Parse(txtStock.Text);
                int catId = Convert.ToInt32(cmbCategory.SelectedValue);

                bool result = (_productId == -1)
                    ? _productConfig.CreateProduct(txtName.Text, catId, price, stock, 1)
                    : _productConfig.UpdateProduct(_productId, txtName.Text, catId, price, stock);

                if (result) { this.DialogResult = DialogResult.OK; this.Close(); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
    }
}