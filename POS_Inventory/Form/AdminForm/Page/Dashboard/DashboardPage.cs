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

            // Title
            lblTitle = new Label();
            lblTitle.Text = "Dashboard Management";
            lblTitle.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTitle.ForeColor = Color.Black;
            lblTitle.AutoSize = true;
            lblTitle.Location = new Point(20, 10);

            this.Controls.Add(lblTitle);

            // Flow panel for cards
            flowCards = new FlowLayoutPanel();
            flowCards.Location = new Point(20, 50);
            flowCards.Size = new Size(900, 200);
            flowCards.BackColor = Color.Transparent;
            flowCards.FlowDirection = FlowDirection.LeftToRight;
            flowCards.WrapContents = false;
            flowCards.AutoScroll = false;
            flowCards.Padding = new Padding(0);

            this.Controls.Add(flowCards);
        }

        private void LoadDashboardCards()
        {
            flowCards.Controls.Clear();

            Panel cardStaff = CreateDashboardCard("Total Staff", "amount : 10", AppColorConfig.CardStaff);
            Panel cardProduct = CreateDashboardCard("Quick Product", "", AppColorConfig.CardProduct);
            Panel cardSales = CreateDashboardCard("Sale Today", "", Color.LightBlue);

            flowCards.Controls.Add(cardStaff);
            flowCards.Controls.Add(cardProduct);
            flowCards.Controls.Add(cardSales);
        }

        private Panel CreateDashboardCard(string title, string value, Color bgColor)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(250, 120);
            pnl.BackColor = bgColor;
            pnl.Margin = new Padding(0, 0, 20, 0);
            pnl.BorderStyle = BorderStyle.FixedSingle;

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