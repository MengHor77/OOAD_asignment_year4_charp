using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace POS_Inventory.Form.POSForm
{
    public partial class CardProductPOS : UserControl
    {
        private Label lblProductName, lblCategory, lblStock, lblPrice;
        public int ProductID { get; set; }
        public decimal Price { get; set; }

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nL, int nT, int nR, int nB, int nW, int nH);

        public CardProductPOS()
        {
            SetupCardDesign();
        }

        private void SetupCardDesign()
        {
            this.Size = new Size(191, 240);
            this.BackColor = Color.FromArgb(150, 145, 200);
            this.Padding = new Padding(10);
            this.Margin = new Padding(10);
            this.Cursor = Cursors.Hand;

            lblProductName = new Label { Text = "Item Name", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.White, Dock = DockStyle.Top, Height = 40, TextAlign = ContentAlignment.MiddleCenter };
            lblStock = new Label { Text = "stock: 0", Font = new Font("Segoe UI", 9), ForeColor = Color.WhiteSmoke, Dock = DockStyle.Bottom, Height = 20, TextAlign = ContentAlignment.MiddleCenter };
            lblPrice = new Label { Text = "$0.00", Font = new Font("Segoe UI", 12, FontStyle.Bold), ForeColor = Color.Yellow, Dock = DockStyle.Bottom, Height = 25, TextAlign = ContentAlignment.MiddleCenter };
            lblCategory = new Label { Text = "Category", Font = new Font("Segoe UI", 9), ForeColor = Color.Gainsboro, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter };

            // Adding in reverse order for correct Docking layering
            this.Controls.Add(lblCategory);
            this.Controls.Add(lblProductName);
            this.Controls.Add(lblPrice);
            this.Controls.Add(lblStock);

            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));

            foreach (Control c in this.Controls)
            {
                c.Click += (s, e) => this.InvokeOnClick(this, e);
            }
        }

        public string ProductName { get => lblProductName.Text; set => lblProductName.Text = value; }
        public string CategoryName { get => lblCategory.Text; set => lblCategory.Text = "Category: " + value; }
        public string ProductPrice { set => lblPrice.Text = "$" + value; }
        public int Stock { set => lblStock.Text = "stock: " + value; }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }
    }
}