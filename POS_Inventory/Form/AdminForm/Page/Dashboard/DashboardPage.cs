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

            // --- Title ---
            lblTitle = new Label();
            lblTitle.Text = "Dashboard Management";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.Black;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 10);
            this.Controls.Add(lblTitle);

            // --- FlowLayoutPanel for dashboard cards ---
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
            int totalSaleCountToday = saleConfig.GetTotalSaleCountToday();

            Panel cardCashier = CreateDashboardCard("Total Cashier", "Amount: " + totalCashier.ToString(), AppColorConfig.CardStaff);
            Panel cardAdmin = CreateDashboardCard("Total Admin", "Amount: " + totalAdmin.ToString(), AppColorConfig.CardStaff);
            Panel cardLowStock = CreateDashboardCard("Alert Low Stock", "Items: " + totalLowStock.ToString(), Color.LightBlue);
            Panel cardTotalAmountSaleToday = CreateDashboardCard("Total Sale Amount Today", "$" + totalAmountSaleToday.ToString("0.00"), AppColorConfig.CardProduct);

            Panel[] cards = { cardCashier, cardAdmin, cardLowStock, cardTotalAmountSaleToday };

            flowCards.Controls.AddRange(cards);

            int totalWidth = 0;
            foreach (Panel card in cards)
                totalWidth += card.Width + card.Margin.Right;

            flowCards.Width = totalWidth;
        }

        private Panel CreateDashboardCard(string title, string value, Color bgColor)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(250, 120);
            pnl.BackColor = bgColor;
            pnl.BorderStyle = BorderStyle.FixedSingle;
            pnl.Margin = new Padding(0, 20, 15, 0);

            Label lblTitle = new Label();
            lblTitle.Text = title;
            lblTitle.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblTitle.ForeColor = Color.Black;
            lblTitle.AutoSize = false;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Dock = DockStyle.Top;
            lblTitle.Height = 50;

            Label lblValue = new Label();
            lblValue.Text = value;
            lblValue.Font = new Font("Segoe UI", 11, FontStyle.Regular);
            lblValue.ForeColor = Color.Black;
            lblValue.Dock = DockStyle.Fill;
            lblValue.TextAlign = ContentAlignment.TopCenter;

            pnl.Controls.Add(lblValue);
            pnl.Controls.Add(lblTitle);

            return pnl;
        }
    }
}