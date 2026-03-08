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
            flowCards.Padding = new Padding(0, 20, 0, 0); // left, top, right, bottom

            // Flex behavior
            flowCards.FlowDirection = FlowDirection.LeftToRight;
            flowCards.WrapContents = true;                // flex to next row if needed
            flowCards.AutoSize = true;                    // grow with content
            flowCards.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowCards.Padding = new Padding(0);

            // Anchor to left and top to expand properly
            flowCards.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Add directly to the UserControl
            this.Controls.Add(flowCards);
        }
        private void LoadDashboardCards()
        {
            flowCards.Controls.Clear();

            UserConfig userConfig = new UserConfig();

            int totalCashier = userConfig.GetTotalCashier();
            int totalAdmin = userConfig.GetTotalAdmin();

            Panel cardCashier = CreateDashboardCard("Total Cashier", "Amount: " + totalCashier, AppColorConfig.CardStaff);
            Panel cardAdmin = CreateDashboardCard("Total Admin", "Amount: " + totalAdmin, AppColorConfig.CardStaff);
            Panel cardProduct = CreateDashboardCard("Quick Product", "", AppColorConfig.CardProduct);
            Panel cardSales = CreateDashboardCard("Sale Today", "", Color.LightBlue);

            Panel[] cards = { cardCashier, cardAdmin, cardProduct, cardSales };

            flowCards.Controls.AddRange(cards);

            // Adjust flowCards width to fit all cards
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
            pnl.Margin = new Padding(0, 0, 15, 0);
            pnl.BorderStyle = BorderStyle.FixedSingle;
            pnl.Margin = new Padding(0, 20, 15, 0); // left, top, right, bottom

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