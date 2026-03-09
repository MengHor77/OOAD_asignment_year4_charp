using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Inventory
{
     public partial class InventoryEditForm : System.Windows.Forms.Form
    {
        private readonly ProductConfig _productConfig;
        private readonly CategoryConfig _categoryConfig;
        private readonly int _productId;

        private TextBox txtName, txtPrice, txtStock;
        private ComboBox cmbCategory;
        private Button btnSave, btnCancel;

        public InventoryEditForm(ProductConfig productConfig, int productId)
        {
            _productConfig = productConfig;
            _categoryConfig = new CategoryConfig();
            _productId = productId;

            SetupCustomControls(); 
            LoadCategories();
            LoadProductData();
        }

        private void SetupCustomControls()
        {
            this.Text = "Edit Product";
            this.Size = new Size(400, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            Label lblTitle = new Label
            {
                Text = "Update Product Details",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true,
                ForeColor = AppColorConfig.TextDark
            };

            CreateLabel("Product Name:", 70);
            txtName = CreateTextBox(100);

            CreateLabel("Category:", 150);
            cmbCategory = new ComboBox
            {
                Location = new Point(20, 180),
                Width = 340,
                Font = new Font("Segoe UI", 11),
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            CreateLabel("Price:", 230);
            txtPrice = CreateTextBox(260);

            CreateLabel("Stock Quantity:", 310);
            txtStock = CreateTextBox(340);

            btnSave = new Button
            {
                Text = "Save Changes",
                Location = new Point(20, 390),
                Size = new Size(160, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(115, 160, 250),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(200, 390),
                Size = new Size(160, 40),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                Font = new Font("Segoe UI", 10)
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblTitle, txtName, cmbCategory, txtPrice, txtStock, btnSave, btnCancel });
        }

        private void CreateLabel(string text, int y)
        {
            this.Controls.Add(new Label
            {
                Text = text,
                Location = new Point(20, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Gray
            });
        }

        private TextBox CreateTextBox(int y)
        {
            return new TextBox
            {
                Location = new Point(20, y),
                Width = 340,
                Font = new Font("Segoe UI", 11),
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private void LoadCategories()
        {
            DataTable dt = _categoryConfig.GetAllCategories();
            cmbCategory.DataSource = dt;
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
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                !decimal.TryParse(txtPrice.Text, out decimal price) ||
                !int.TryParse(txtStock.Text, out int stock))
            {
                MessageBox.Show("Please enter valid data.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int categoryId = Convert.ToInt32(cmbCategory.SelectedValue);

            if (_productConfig.UpdateProduct(_productId, txtName.Text, categoryId, price, stock))
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}