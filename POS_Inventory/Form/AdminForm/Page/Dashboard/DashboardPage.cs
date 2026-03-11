using System;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Dashboard
{
    public partial class DashboardPage : UserControl
    {
        private FlowLayoutPanel flowCards;
        private Label lblTitle;

        public DashboardPage()
        {
            InitializeComponent();
            SetupPage();
            LoadDashboardCards();
        }

        private void SetupPage()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            lblTitle = new Label();
            lblTitle.Text = "📊 Dashboard Management";
            lblTitle.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            lblTitle.ForeColor = AppColorConfig.TextDark;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 12);
            this.Controls.Add(lblTitle);

            flowCards = new FlowLayoutPanel();
            flowCards.Location = new Point(20, 50);
            flowCards.Height = 200;
            flowCards.BackColor = Color.Transparent;
            flowCards.FlowDirection = FlowDirection.LeftToRight;
            flowCards.WrapContents = true;
            flowCards.AutoSize = true;
            flowCards.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowCards.Padding = new Padding(0);
            flowCards.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.Controls.Add(flowCards);
        }

        private void LoadDashboardCards()
        {
            flowCards.Controls.Clear();

            UserConfig userConfig = new UserConfig();
            SaleConfig saleConfig = new SaleConfig();
            ProductConfig productConfig = new ProductConfig();

            int totalCashier = userConfig.GetTotalCashier();
            int totalAdmin = userConfig.GetTotalAdmin();
            int totalLowStock = productConfig.GetTotalLowStock();
            decimal totalAmountSaleToday = saleConfig.GetTotalAmountSaleToday();

            Panel cardCashier = CreateDashboardCard(
                "Total Cashier",
                "👤  " + totalCashier.ToString(),
                AppColorConfig.CardStaff,
                AppColorConfig.CardStaffHover,
                false
            );

            Panel cardAdmin = CreateDashboardCard(
                "Total Admin",
                "🛡️  " + totalAdmin.ToString(),
                AppColorConfig.CardAdmin,
                AppColorConfig.CardAdminHover,
                false
            );

            Panel cardLowStock = CreateDashboardCard(
                "Alert Low Stock",
                "⚠️  Items: " + totalLowStock.ToString(),
                AppColorConfig.CardLowStock,
                AppColorConfig.CardLowStockHover,
                true
            );

            Panel cardTotalAmountSaleToday = CreateDashboardCard(
                "Total Sale Today",
                "💰  $" + totalAmountSaleToday.ToString("0.00"),
                AppColorConfig.CardProduct,
                AppColorConfig.CardProductHover,
                false
            );

            Panel[] cards = { cardCashier, cardAdmin, cardLowStock, cardTotalAmountSaleToday };

            flowCards.Controls.AddRange(cards);

            int totalWidth = 0;
            foreach (Panel card in cards)
                totalWidth += card.Width + card.Margin.Right;

            flowCards.Width = totalWidth;
        }

        private Panel CreateDashboardCard(string title, string value, Color bgColor, Color hoverColor, bool isLowStock)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(220, 110);
            pnl.BackColor = bgColor;
            pnl.BorderStyle = BorderStyle.None;
            pnl.Margin = new Padding(0, 20, 20, 0);
            pnl.Cursor = Cursors.Hand;

            // --- Yellow left border stripe for Low Stock card ---
            if (isLowStock)
            {
                Panel leftBorder = new Panel();
                leftBorder.Size = new Size(6, 110);
                leftBorder.Location = new Point(0, 0);
                leftBorder.BackColor = AppColorConfig.LowStockBorder;
                pnl.Controls.Add(leftBorder);
            }

            // --- Rounded corners via Paint ---
            pnl.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using (System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 12;
                    gp.AddArc(0, 0, radius, radius, 180, 90);
                    gp.AddArc(pnl.Width - radius, 0, radius, radius, 270, 90);
                    gp.AddArc(pnl.Width - radius, pnl.Height - radius, radius, radius, 0, 90);
                    gp.AddArc(0, pnl.Height - radius, radius, radius, 90, 90);
                    gp.CloseAllFigures();
                    pnl.Region = new Region(gp);
                }
            };

            // --- Title label ---
            Label lblCardTitle = new Label();
            lblCardTitle.Text = title;
            lblCardTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblCardTitle.ForeColor = isLowStock ? AppColorConfig.LowStockText : AppColorConfig.TextLight;
            lblCardTitle.AutoSize = false;
            lblCardTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblCardTitle.Dock = DockStyle.Top;
            lblCardTitle.Height = 40;
            lblCardTitle.Cursor = Cursors.Hand;

            // --- Value label ---
            Label lblCardValue = new Label();
            lblCardValue.Text = value;
            lblCardValue.Font = new Font("Segoe UI", 13, FontStyle.Bold);
            lblCardValue.ForeColor = isLowStock ? AppColorConfig.LowStockText : AppColorConfig.TextLight;
            lblCardValue.Dock = DockStyle.Fill;
            lblCardValue.TextAlign = ContentAlignment.MiddleCenter;
            lblCardValue.Cursor = Cursors.Hand;

            pnl.Controls.Add(lblCardValue);
            pnl.Controls.Add(lblCardTitle);

            // --- Hover effect ---
            EventHandler enterHandler = (s, e) => pnl.BackColor = hoverColor;
            EventHandler leaveHandler = (s, e) => pnl.BackColor = bgColor;

            pnl.MouseEnter += enterHandler;
            pnl.MouseLeave += leaveHandler;
            lblCardTitle.MouseEnter += enterHandler;
            lblCardTitle.MouseLeave += leaveHandler;
            lblCardValue.MouseEnter += enterHandler;
            lblCardValue.MouseLeave += leaveHandler;

            return pnl;
        }
    }
}