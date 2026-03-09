using System;
using System.Drawing;
using System.Windows.Forms;

namespace POS_Inventory.Form.POSForm
{
    public partial class ItemsOrder : UserControl
    {
        private Label lblItemName, lblQty, btnMinus, btnPlus;
        private PictureBox picDelete;

        public ItemsOrder()
        {
            SetupOrderRowDesign();
        }

        private void SetupOrderRowDesign()
        {
            this.Size = new Size(310, 45);
            this.BackColor = Color.FromArgb(170, 160, 230);
            this.Margin = new Padding(0, 5, 0, 5);

            picDelete = new PictureBox { Size = new Size(20, 20), Location = new Point(220, 12), SizeMode = PictureBoxSizeMode.Zoom, Image = SystemIcons.Error.ToBitmap(), Cursor = Cursors.Hand };
            btnPlus = new Label { Text = "+", ForeColor = Color.White, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(190, 10), AutoSize = true, Cursor = Cursors.Hand };
            lblQty = new Label { Text = "1", ForeColor = Color.White, Font = new Font("Segoe UI", 10), Location = new Point(165, 13), AutoSize = true };
            btnMinus = new Label { Text = "—", ForeColor = Color.White, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(140, 12), AutoSize = true, Cursor = Cursors.Hand };
            lblItemName = new Label { Text = "Item", ForeColor = Color.White, Font = new Font("Segoe UI", 10), Location = new Point(10, 12), AutoSize = false, Size = new Size(120, 25), TextAlign = ContentAlignment.MiddleLeft };

            this.Controls.Add(picDelete);
            this.Controls.Add(btnPlus);
            this.Controls.Add(lblQty);
            this.Controls.Add(btnMinus);
            this.Controls.Add(lblItemName);
        }

        public string ItemName { get => lblItemName.Text; set => lblItemName.Text = value; }
        public int Quantity { get => int.Parse(lblQty.Text); set => lblQty.Text = value.ToString(); }
    }
}