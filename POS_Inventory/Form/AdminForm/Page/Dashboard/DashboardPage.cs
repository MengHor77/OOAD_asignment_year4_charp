using System;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config; 

namespace POS_Inventory.Form.AdminForm.Page.Dashboard
{
    public partial class DashboardPage : UserControl
    {
        public DashboardPage()
        {
            InitializeComponent();
            SetupPage();
            LoadDashboardCards();
        }

        private void SetupPage()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppColorConfix.ContentBackground;
        }

        private void LoadDashboardCards()
        {
            // Clear existing controls if any
            this.Controls.Clear();

            // Create and add your cards
            Panel cardStaff = CreateDashboardCard("Total Staff", new Point(50, 50), AppColorConfix.CardStaff);
            Panel cardProduct = CreateDashboardCard("Quick Products", new Point(330, 50), AppColorConfix.CardProduct);

            this.Controls.Add(cardStaff);
            this.Controls.Add(cardProduct);
        }

        private Panel CreateDashboardCard(string text, Point location, Color bgColor)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(250, 160);
            pnl.Location = location;
            pnl.BackColor = bgColor;

            // Optional: Add rounded corners feel or border if your config supports it

            Label lbl = new Label();
            lbl.Text = text;
            lbl.ForeColor = AppColorConfix.TextDark;
            lbl.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;

            pnl.Controls.Add(lbl);
            return pnl;
        }
    }
}