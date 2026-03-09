using System;
using System.Drawing;
using System.Windows.Forms;

namespace POS_Inventory.Form.POSForm
{
    public partial class ItemsOrder : UserControl
    {
        // Controls
        private Label lblItemName;
        private Label lblQty;
        private Label btnMinus;
        private Label btnPlus;
        private PictureBox picDelete;

        // Constructor
        public ItemsOrder()
        {
            InitializeComponent();
            SetupOrderRowDesign();
        }

        // Setup the design of the order row
        private void SetupOrderRowDesign()
        {
            // UserControl properties
            this.Size = new Size(298, 45);
            this.BackColor = Color.FromArgb(170, 160, 230);
            this.Margin = new Padding(0, 8, 0, 8);

            // Delete icon
            picDelete = new PictureBox
            {
                Size = new Size(20, 20),
                Location = new Point(270, 12),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = SystemIcons.Error.ToBitmap(),
                Cursor = Cursors.Hand
            };

            // Plus button
            btnPlus = new Label
            {
                Text = "+",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(235, 10),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            // Quantity label
            lblQty = new Label
            {
                Text = "1",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Location = new Point(165, 13),
                AutoSize = true
            };

            // Minus button
            btnMinus = new Label
            {
                Text = "—", // use a proper dash
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(200, 12),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            // Item name label
            lblItemName = new Label
            {
                Text = "Item",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                Location = new Point(10, 12),
                AutoSize = false,
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Add controls to UserControl
            this.Controls.Add(picDelete);
            this.Controls.Add(btnPlus);
            this.Controls.Add(lblQty);
            this.Controls.Add(btnMinus);
            this.Controls.Add(lblItemName);
        }

        // Properties for external access
        public string ItemName
        {
            get => lblItemName.Text;
            set => lblItemName.Text = value;
        }

        public int Quantity
        {
            get => int.Parse(lblQty.Text);
            set => lblQty.Text = value.ToString();
        }
    }
}