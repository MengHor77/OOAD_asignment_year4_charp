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
        private Label btnDelete;
        public event Action OnQuantityChanged;
        public event Action OnItemDeleted;
        private Label lblPricePerUnit;

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
        private void SetupOrderRowDesign()
        {
            // --- UserControl properties ---
            this.Size = new Size(330, 60);
            this.BackColor = Color.LightGray; // temporary background
            this.Margin = new Padding(0, 8, 0, 8);

            int centerY = this.Height / 2;

            // --- Apply rounded corners to UserControl ---
            this.Load += (s, e) => ApplyRounding(this, 15);
            this.Resize += (s, e) => ApplyRounding(this, 15);

            // --- Item Name Label ---
            lblItemName = new Label
            {
                Text = "iphone",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                AutoSize = false,
                Size = new Size(150, 25),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(10, centerY - 20)
            };

            // --- Price Per Unit Label ---
            lblPricePerUnit = new Label
            {
                Text = "30$/ unit",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                AutoSize = false,
                Size = new Size(80, 20),
                TextAlign = ContentAlignment.MiddleLeft,
                Location = new Point(10, centerY + 8)
            };

            int circleSize = 30; // Width = Height for perfect circle
            int circleRadius = circleSize / 2;

            // --- Plus Button (circular) ---
            btnPlus = new Label
            {
                Text = "+",
                ForeColor = Color.White,
                BackColor = AppColorConfig.CardPOSProduct,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(circleSize, circleSize),
                Cursor = Cursors.Hand,
                Location = new Point(180, centerY - circleRadius)
            };
            btnPlus.MouseEnter += (s, e) => btnPlus.BackColor = AppColorConfig.LightBlue;
            btnPlus.MouseLeave += (s, e) => btnPlus.BackColor = AppColorConfig.CardPOSProduct;
            btnPlus.Click += (s, e) =>
            {
                int qty = int.Parse(lblQty.Text);
                qty++;
                lblQty.Text = qty.ToString();
                new ItemOrderConfig().UpdateQty(ItemName, qty);
                OnQuantityChanged?.Invoke();
            };
            ApplyRounding(btnPlus, circleRadius); // perfect circle

            // --- Quantity Label ---
            lblQty = new Label
            {
                Text = "3",
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = false,
                Size = new Size(20, circleSize),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(220, centerY - circleRadius)
            };

            // --- Minus Button (circular) ---
            btnMinus = new Label
            {
                Text = "-",
                ForeColor = Color.White,
                BackColor = AppColorConfig.CardPOSProduct,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(circleSize, circleSize),
                Cursor = Cursors.Hand,
                Location = new Point(250, centerY - circleRadius)
            };
            btnMinus.MouseEnter += (s, e) => btnMinus.BackColor = AppColorConfig.LightBlue ;
            btnMinus.MouseLeave += (s, e) => btnMinus.BackColor = AppColorConfig.CardPOSProduct; 
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
            ApplyRounding(btnMinus, circleRadius); // perfect circle

            // --- Delete Button (X circular) ---
            btnDelete = new Label
            {
                Size = new Size(circleSize, circleSize),
                BackColor = AppColorConfig.CardPOSProduct,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Location = new Point(290, centerY - circleRadius)
            };
            btnDelete.MouseEnter += (s, e) => btnDelete.BackColor = AppColorConfig.LightBlue;
            btnDelete.MouseLeave += (s, e) => btnDelete.BackColor = AppColorConfig.CardPOSProduct;

            btnDelete.Paint += (s, e) =>
            {
                using (var b = new SolidBrush(Color.White))
                {
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    e.Graphics.DrawString("X", new Font("Segoe UI", 10, FontStyle.Bold), b, new PointF(7, 5));
                }
            };
            btnDelete.Click += (s, e) =>
            {
                new ItemOrderConfig().DeleteItem(ItemName);
                this.Parent.Controls.Remove(this);
                OnItemDeleted?.Invoke();
            };
            ApplyRounding(btnDelete, circleRadius); // perfect circle

            // --- Add controls ---
            this.Controls.Add(lblItemName);
            this.Controls.Add(lblPricePerUnit);
            this.Controls.Add(btnPlus);
            this.Controls.Add(lblQty);
            this.Controls.Add(btnMinus);
            this.Controls.Add(btnDelete);
        }

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

 
        public decimal UnitPrice
        {
            get => Price;
            set
            {
                Price = value;
                lblPricePerUnit.Text = "$" + value.ToString("0.00");
            }
        }

    }
}