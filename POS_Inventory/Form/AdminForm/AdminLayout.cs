using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm
{
    public partial class AdminLayout : System.Windows.Forms.Form
    {
        private Panel sidePanel;
        private Panel headerPanel;
        private Panel mainContentPanel;
        private System.Windows.Forms.Form activeForm = null;

        public AdminLayout()
        {
            InitializeComponent();
            SetupLayoutDesign();
            InitializeSidebar();
            LoadDashboardOverview();
        }

        private void SetupLayoutDesign()
        {
            this.Text = "Admin POS";
            // Set size to fit most laptop screens comfortably
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = AppColorConfix.ContentBackground;

            // Sidebar setup
            sidePanel = new Panel();
            sidePanel.Dock = DockStyle.Left;
            sidePanel.Width = 140;
            sidePanel.BackColor = AppColorConfix.SidebarRed;
            this.Controls.Add(sidePanel);

            // Header setup
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 50;
            headerPanel.BackColor = AppColorConfix.HeaderPink;
            this.Controls.Add(headerPanel);

            // Welcome Label
            Label lblWelcome = new Label();
            lblWelcome.Text = "Welcome Admin!";
            lblWelcome.ForeColor = AppColorConfix.TextDark;
            lblWelcome.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblWelcome.Location = new Point(15, 15);
            lblWelcome.AutoSize = true;
            headerPanel.Controls.Add(lblWelcome);

            // Header Links (Now Anchored to the Right side)
            AddHeaderLink("Logout", 60); // Offset from right
            AddHeaderLink("profile", 130); // Offset from right

            // Main Content Area
            mainContentPanel = new Panel();
            mainContentPanel.Dock = DockStyle.Fill;
            mainContentPanel.BackColor = AppColorConfix.ContentBackground;
            this.Controls.Add(mainContentPanel);
        }

        private void AddHeaderLink(string text, int rightOffset)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.AutoSize = true;
            lbl.Cursor = Cursors.Hand;
            lbl.ForeColor = AppColorConfix.TextDark;
            lbl.Font = new Font("Segoe UI", 9, FontStyle.Underline);

            // This math keeps them on the right regardless of window width
            lbl.Location = new Point(headerPanel.Width - rightOffset, 15);
            lbl.Anchor = AnchorStyles.Right | AnchorStyles.Top;

            headerPanel.Controls.Add(lbl);
            if (text == "Logout") lbl.Click += (s, e) => PerformLogout();
        }

        public void InitializeSidebar()
        {
           

            // "General" Header
            Label lblGeneral = new Label();
             lblGeneral.ForeColor = AppColorConfix.TextLight;
            lblGeneral.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblGeneral.Dock = DockStyle.Top;
            lblGeneral.Height = 30;
            lblGeneral.TextAlign = ContentAlignment.MiddleLeft;
            lblGeneral.Padding = new Padding(10, 0, 0, 0);
            sidePanel.Controls.Add(lblGeneral);

            // Menu Items List
            List<SidebarItem> menuItems = new List<SidebarItem>()
            {
                new SidebarItem { Title = "Dasboard", IconText = "📊" },
                new SidebarItem { Title = "Category", IconText = "📦" },
                new SidebarItem { Title = "Product", IconText = "🛍️" },
                new SidebarItem { Title = "Staff", IconText = "👥" },
                new SidebarItem { Title = "Report", IconText = "📈" },
                new SidebarItem { Title = "Logout", IconText = "🚪" }
            };

            // Add buttons from bottom to top (Last item in list becomes top button)
            for (int i = menuItems.Count - 1; i >= 0; i--)
            {
                sidePanel.Controls.Add(CreateMenuButton(menuItems[i]));
            }
        }

        private Button CreateMenuButton(SidebarItem item)
        {
            Button btn = new Button();
            // Combined Text + Emoji
            btn.Text = "  " + item.IconText + "  " + item.Title;
            btn.Dock = DockStyle.Top;
            btn.Height = 50;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = AppColorConfix.TextDark;
            btn.Font = new Font("Segoe UI", 9);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(10, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

            // Hover effects using AppColorConfix
            btn.MouseEnter += (s, e) => btn.BackColor = AppColorConfix.SidebarHover;
            btn.MouseLeave += (s, e) => btn.BackColor = AppColorConfix.SidebarRed;

            btn.Click += (s, e) => {
                if (item.Title == "Logout") PerformLogout();
                else if (item.Title == "Dasboard") LoadDashboardOverview();
            };

            return btn;
        }

        private void LoadDashboardOverview()
        {
            mainContentPanel.Controls.Clear();
            // Cards positioned using the new card colors
            Panel cardStaff = CreateDashboardCard("Total Staff", new Point(50, 50), AppColorConfix.CardStaff);
            Panel cardProduct = CreateDashboardCard("Quick Products", new Point(330, 50), AppColorConfix.CardProduct);

            mainContentPanel.Controls.Add(cardStaff);
            mainContentPanel.Controls.Add(cardProduct);
        }

        private Panel CreateDashboardCard(string text, Point location, Color bgColor)
        {
            Panel pnl = new Panel();
            pnl.Size = new Size(250, 160);
            pnl.Location = location;
            pnl.BackColor = bgColor;

            Label lbl = new Label();
            lbl.Text = text;
            lbl.ForeColor = AppColorConfix.TextDark;
            lbl.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleCenter;

            pnl.Controls.Add(lbl);
            return pnl;
        }

        private void PerformLogout()
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}