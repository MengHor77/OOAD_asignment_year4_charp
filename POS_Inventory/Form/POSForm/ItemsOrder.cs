using System;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;


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
        public event Action OnQuantityChanged;
        public event Action OnItemDeleted;

        // Constructor
        public ItemsOrder()
        {
            InitializeComponent();
            SetupOrderRowDesign();
            this.Load += (s, e) => ApplyRounding(this, 15);
            // Ensure rounding stays correct if the sidebar width changes
            this.Resize += (s, e) => ApplyRounding(this, 15);

        }


        private void ApplyRounding(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0) return;

            using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();

                control.Region = new Region(path);
            }
        }



        // Setup the design of the order row
        private void SetupOrderRowDesign()
        {
            // UserControl properties
            this.Size = new Size(298, 60);
            this.BackColor = AppColorConfig.CardPOSProduct;
            this.Margin = new Padding(0, 8, 0, 8);

            int centerY = this.Height / 2;

            // --- Item Name Label ---
            lblItemName = new Label
            {
                Text = "Item",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                AutoSize = false,
                Size = new Size(120, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(10, 0) // Y will adjust below
            };
            lblItemName.Top = centerY - lblItemName.Height / 2;

            // --- Minus Button ---
            btnMinus = new Label
            {
                Text = "_", // proper dash
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Cursor = Cursors.Hand,
                Location = new Point(165, 0) // X position
            };
            btnMinus.Top = centerY - btnMinus.Height / 2;

            btnMinus.Click += (s, e) =>
            {
                int qty = int.Parse(lblQty.Text);

                if (qty > 1)
                {
                    qty--;
                    lblQty.Text = qty.ToString();

                    new ItemOrderConfig().UpdateQty(ItemName, qty);

                    OnQuantityChanged?.Invoke();
                }
            };



            // --- Quantity Label ---
            lblQty = new Label
            {
                Text = "1",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(190, 0)
            };
            lblQty.Top = centerY - lblQty.Height / 2;

            // --- Plus Button ---
            btnPlus = new Label
            {
                Text = "+",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Cursor = Cursors.Hand,
                Location = new Point(220, 0)
            };
            btnPlus.Top = centerY - btnPlus.Height / 2;

            btnPlus.Click += (s, e) =>
            {
                int qty = int.Parse(lblQty.Text);
                qty++;
                lblQty.Text = qty.ToString();

                new ItemOrderConfig().UpdateQty(ItemName, qty);

                OnQuantityChanged?.Invoke();
            };


            // --- Delete Icon ---
            picDelete = new PictureBox
            {
                Size = new Size(20, 20),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = SystemIcons.Error.ToBitmap(), // replace with your icon if needed
                Cursor = Cursors.Hand,
                Location = new Point(260, 0)
            };
            picDelete.Top = centerY - picDelete.Height / 2;

            picDelete.Click += (s, e) =>
            {
                new ItemOrderConfig().DeleteItem(ItemName);

                this.Parent.Controls.Remove(this);

                OnItemDeleted?.Invoke();
            };


            // --- Add controls to UserControl ---
            this.Controls.Add(lblItemName);
            this.Controls.Add(btnMinus);
            this.Controls.Add(lblQty);
            this.Controls.Add(btnPlus);
            this.Controls.Add(picDelete);
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
        public decimal Price { get; set; }

    }
}