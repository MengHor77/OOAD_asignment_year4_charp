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

        // Collapse variables
        private Timer sidebarTimer;
        private bool isCollapsed = false;
        private const int ExpandedWidth = 140;
        private const int CollapsedWidth = 50;

        public AdminLayout()
        {
            InitializeComponent();
            SetupCollapseTimer();
            SetupLayoutDesign();
            InitializeSidebar();
            LoadDashboardOverview();
        }

        private void SetupCollapseTimer()
        {
            sidebarTimer = new Timer();
            sidebarTimer.Interval = 10;
            sidebarTimer.Tick += SidebarTimer_Tick;
        }

        private void SetupLayoutDesign()
        {
            this.Text = "Admin POS";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = AppColorConfix.ContentBackground;

            sidePanel = new Panel();
            sidePanel.Dock = DockStyle.Left;
            sidePanel.Width = ExpandedWidth;
            sidePanel.BackColor = AppColorConfix.SidebarRed;
            this.Controls.Add(sidePanel);

            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 50;
            headerPanel.BackColor = AppColorConfix.HeaderPink;
            this.Controls.Add(headerPanel);

            Label btnMenu = new Label();
            btnMenu.Text = "≡";
            btnMenu.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            btnMenu.ForeColor = AppColorConfix.TextDark;
            btnMenu.Cursor = Cursors.Hand;
            btnMenu.Size = new Size(30, 30);
            btnMenu.Location = new Point(10, 10);
            btnMenu.Click += (s, e) => sidebarTimer.Start();
            headerPanel.Controls.Add(btnMenu);

            Label lblWelcome = new Label();
            lblWelcome.Text = "Welcome Admin!";
            lblWelcome.ForeColor = AppColorConfix.TextDark;
            lblWelcome.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblWelcome.Location = new Point(50, 15);
            lblWelcome.AutoSize = true;
            headerPanel.Controls.Add(lblWelcome);

            AddHeaderLink("Logout", 60);
            AddHeaderLink("profile", 130);

            mainContentPanel = new Panel();
            mainContentPanel.Dock = DockStyle.Fill;
            mainContentPanel.BackColor = AppColorConfix.ContentBackground;
            this.Controls.Add(mainContentPanel);
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                sidePanel.Width += 10;
                if (sidePanel.Width >= ExpandedWidth)
                {
                    isCollapsed = false;
                    sidebarTimer.Stop();
                    RefreshButtonAppearance(); // Fix alignment after expand
                }
            }
            else
            {
                sidePanel.Width -= 10;
                if (sidePanel.Width <= CollapsedWidth)
                {
                    isCollapsed = true;
                    sidebarTimer.Stop();
                    RefreshButtonAppearance(); // Fix alignment after collapse
                }
            }
        }

        // --- NEW METHOD TO CENTER ICONS ---
        private void RefreshButtonAppearance()
        {
            foreach (Control ctrl in sidePanel.Controls)
            {
                if (ctrl is Button btn)
                {
                    if (isCollapsed)
                    {
                        btn.TextAlign = ContentAlignment.MiddleCenter;
                        btn.Padding = new Padding(0);
                        // Only show the emoji
                        if (btn.Tag is SidebarItem item) btn.Text = item.IconText;
                    }
                    else
                    {
                        btn.TextAlign = ContentAlignment.MiddleLeft;
                        btn.Padding = new Padding(10, 0, 0, 0);
                        // Show Emoji + Text
                        if (btn.Tag is SidebarItem item) btn.Text = "    " + item.IconText + "    " + item.Title;
                    }
                }
            }
        }

        private void AddHeaderLink(string text, int rightOffset)
        {
            Label lbl = new Label();
            lbl.Text = text;
            lbl.AutoSize = true;
            lbl.Cursor = Cursors.Hand;
            lbl.ForeColor = AppColorConfix.TextDark;
            lbl.Font = new Font("Segoe UI", 9, FontStyle.Underline);
            lbl.Location = new Point(headerPanel.Width - rightOffset, 15);
            lbl.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            headerPanel.Controls.Add(lbl);
            if (text == "Logout") lbl.Click += (s, e) => PerformLogout();
        }

        public void InitializeSidebar()
        {
            sidePanel.Controls.Clear();

            List<SidebarItem> menuItems = new List<SidebarItem>()
            {
                new SidebarItem { Title = "Dasboard", IconText = "📊" },
                new SidebarItem { Title = "Category", IconText = "📦" },
                new SidebarItem { Title = "Product", IconText = "🛍️" },
                new SidebarItem { Title = "Staff", IconText = "👥" },
                new SidebarItem { Title = "Report", IconText = "📈" },
                new SidebarItem { Title = "Logout", IconText = "🚪" }
            };

            for (int i = menuItems.Count - 1; i >= 0; i--)
            {
                sidePanel.Controls.Add(CreateMenuButton(menuItems[i]));
            }

            Label lblGeneral = new Label();
            lblGeneral.ForeColor = AppColorConfix.TextLight;
            lblGeneral.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            lblGeneral.Dock = DockStyle.Top;
            lblGeneral.Height = 30;
            lblGeneral.TextAlign = ContentAlignment.MiddleLeft;
            lblGeneral.Padding = new Padding(10, 0, 0, 0);
            sidePanel.Controls.Add(lblGeneral);

            Label lblBrand = new Label();
            lblBrand.Text = "Admin POS";
            lblBrand.ForeColor = AppColorConfix.TextDark;
            lblBrand.BackColor = AppColorConfix.BrandBlue;
            lblBrand.TextAlign = ContentAlignment.MiddleCenter;
            lblBrand.Dock = DockStyle.Top;
            lblBrand.Height = 50;
            lblBrand.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            sidePanel.Controls.Add(lblBrand);
        }

        private Button CreateMenuButton(SidebarItem item)
        {
            Button btn = new Button();
            btn.Tag = item; // Save the item data here so we can access it during collapse
            btn.Text = "    " + item.IconText + "    " + item.Title;
            btn.Dock = DockStyle.Top;
            btn.Height = 50;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.ForeColor = AppColorConfix.TextDark;
            btn.Font = new Font("Segoe UI", 10);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(10, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

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