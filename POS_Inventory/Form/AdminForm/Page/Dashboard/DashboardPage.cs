using System;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Dashboard
{
    public partial class DashboardPage : UserControl
    {
        private FlowLayoutPanel flowCards;

        public DashboardPage()
        {
            InitializeComponent();
            SetupPage();
            LoadDashboardCards();
        }

        private void SetupPage()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppColorConfig.ContentBackground;
            this.Padding = new Padding(20);

            flowCards = new FlowLayoutPanel();
            flowCards.Dock = DockStyle.Fill;
            flowCards.AutoScroll = true;
            flowCards.BackColor = Color.Transparent;
            flowCards.FlowDirection = FlowDirection.LeftToRight;
            flowCards.WrapContents = true; // IMPORTANT: Allows cards to move to next line when sidebar grows

            this.Controls.Add(flowCards);
        }

        private void LoadDashboardCards()
        {
            flowCards.Controls.Clear();

            Panel cardStaff = CreateDashboardCard("Total Staff", AppColorConfig.CardStaff);
            Panel cardProduct = CreateDashboardCard("Quick Products", AppColorConfig.CardProduct);
            Panel cardReports = CreateDashboardCard("Monthly Sales", Color.LightBlue);

            flowCards.Controls.Add(cardStaff);
            flowCards.Controls.Add(cardProduct);
            flowCards.Controls.Add(cardReports);
        }

        private Panel CreateDashboardCard(string text, Color bgColor)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(250, 160);
            pnl.BackColor = bgColor;
            pnl.Margin = new Padding(0, 0, 20, 20);

            Label lbl = new Label();
            lbl.Text = text;
            lbl.ForeColor = AppColorConfig.TextDark;
            lbl.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;

            pnl.Controls.Add(lbl);
            return pnl;
        }
    }
}