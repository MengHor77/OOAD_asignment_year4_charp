using POS_Inventory.Config;
using POS_Inventory.Form.AdminForm.Page.Dashboard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace POS_Inventory.Form.AdminForm
{
    public partial class AdminLayout : System.Windows.Forms.Form
    {
        private Panel sidePanel;
        private Panel headerPanel;
        private Panel mainContentPanel;

        // References for the Branding Area at the Top
        private Panel pnlSidebarBranding;
        private Label lblAdminTitle;

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
            LoadDashboardPage();
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

            // 1. Initialize Side Panel
            sidePanel = new Panel();
            sidePanel.Dock = DockStyle.Left;
            sidePanel.Width = ExpandedWidth;
            sidePanel.BackColor = AppColorConfix.SidebarRed;
            this.Controls.Add(sidePanel);

            // 2. Initialize Header Panel
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 50;
            headerPanel.BackColor = AppColorConfix.HeaderPink;
            this.Controls.Add(headerPanel);

            // 3. Initialize Main Content Panel
            mainContentPanel = new Panel();
            mainContentPanel.Dock = DockStyle.Fill;
            mainContentPanel.BackColor = AppColorConfix.ContentBackground;
            this.Controls.Add(mainContentPanel);

            // Z-ORDER: Sidebar and Header must be on top
            mainContentPanel.SendToBack();
            sidePanel.BringToFront();
            headerPanel.BringToFront();

            // 4. Header UI Elements (Hamburger Button)
            Label btnMenu = new Label();
            btnMenu.Text = "≡";
            btnMenu.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            btnMenu.ForeColor = AppColorConfix.TextDark;
            btnMenu.Cursor = Cursors.Hand;
            btnMenu.AutoSize = false;
            btnMenu.Size = new Size(45, 45);
            btnMenu.TextAlign = ContentAlignment.MiddleCenter;
            btnMenu.Location = new Point(2, 2);
            btnMenu.Click += (s, e) => sidebarTimer.Start();

            btnMenu.MouseEnter += (s, e) => btnMenu.BackColor = AppColorConfix.SidebarHover;
            btnMenu.MouseLeave += (s, e) => btnMenu.BackColor = Color.Transparent;
            headerPanel.Controls.Add(btnMenu);

            Label lblWelcome = new Label();
            lblWelcome.Text = "Welcome Admin!";
            lblWelcome.ForeColor = AppColorConfix.TextDark;
            lblWelcome.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblWelcome.Location = new Point(60, 15);
            lblWelcome.AutoSize = true;
            headerPanel.Controls.Add(lblWelcome);

            AddHeaderLink("Logout", 60);
            AddHeaderLink("profile", 130);
        }

        public void InitializeSidebar()
        {
            sidePanel.Controls.Clear();

            // 1. Create the Top Branding Panel first
            pnlSidebarBranding = new Panel();
            pnlSidebarBranding.Dock = DockStyle.Top;
            pnlSidebarBranding.Height = 50; // Aligns with header
            pnlSidebarBranding.BackColor = AppColorConfix.SidebarHover;

            lblAdminTitle = new Label();
            lblAdminTitle.Text = isCollapsed ? "AP" : "Admin POS";
            lblAdminTitle.ForeColor = AppColorConfix.TextDark;
            lblAdminTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblAdminTitle.Dock = DockStyle.Fill;
            lblAdminTitle.TextAlign = ContentAlignment.MiddleCenter;

            pnlSidebarBranding.Controls.Add(lblAdminTitle);
            sidePanel.Controls.Add(pnlSidebarBranding);

            // 2. Define Menu Items in order
            List<SidebarItem> menuItems = new List<SidebarItem>()
    {
        new SidebarItem { Title = "Dasboard", IconText = "📊" },
        new SidebarItem { Title = "Category", IconText = "📦" },
        new SidebarItem { Title = "Product", IconText = "🛍️" },
        new SidebarItem { Title = "Staff", IconText = "👥" },
        new SidebarItem { Title = "Report", IconText = "📈" },
        new SidebarItem { Title = "Logout", IconText = "🚪" }
    };

            // 3. Add buttons in reverse order for correct Docking stack
            for (int i = menuItems.Count - 1; i >= 0; i--)
            {
                sidePanel.Controls.Add(CreateMenuButton(menuItems[i]));
            }

            // 4. Force Branding to stay at the absolute top
            pnlSidebarBranding.SendToBack();
        }
        private Button CreateMenuButton(SidebarItem item)
        {
            Button btn = new Button();
            btn.Tag = item;
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
                else NavigateToPage(item.Title);
            };

            return btn;
        }

        private void NavigateToPage(string title)
        {
            mainContentPanel.Controls.Clear();
            if (title == "Dasboard") LoadDashboardPage();
            else
            {
                Label lbl = new Label { Text = title + " Page coming soon...", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 12) };
                mainContentPanel.Controls.Add(lbl);
            }
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (isCollapsed)
            {
                sidePanel.Width += 10;
                if (sidePanel.Width >= ExpandedWidth)
                {
                    sidePanel.Width = ExpandedWidth;
                    isCollapsed = false;
                    sidebarTimer.Stop();
                    RefreshButtonAppearance();
                }
            }
            else
            {
                sidePanel.Width -= 10;
                if (sidePanel.Width <= CollapsedWidth)
                {
                    sidePanel.Width = CollapsedWidth;
                    isCollapsed = true;
                    sidebarTimer.Stop();
                    RefreshButtonAppearance();
                }
            }
        }

        private void RefreshButtonAppearance()
        {
            // 1. Update Top Branding Text (Admin POS)
            if (lblAdminTitle != null)
            {
                if (isCollapsed)
                {
                    // Show short version when sidebar is 50px
                    lblAdminTitle.Text = "AP";
                    lblAdminTitle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
                }
                else
                {
                    // Show full title when sidebar is 140px
                    lblAdminTitle.Text = "Admin POS";
                    lblAdminTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                }
            }

            // 2. Update Sidebar Buttons (Dashboard, Category, etc.)
            foreach (Control ctrl in sidePanel.Controls)
            {
                if (ctrl is Button btn && btn.Tag is SidebarItem item)
                {
                    if (isCollapsed)
                    {
                        // Minimized: Show only Icon
                        btn.TextAlign = ContentAlignment.MiddleCenter;
                        btn.Padding = new Padding(0);
                        btn.Text = item.IconText;
                    }
                    else
                    {
                        // Expanded: Show Icon + Text
                        btn.TextAlign = ContentAlignment.MiddleLeft;
                        btn.Padding = new Padding(10, 0, 0, 0);
                        btn.Text = "    " + item.IconText + "    " + item.Title;
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

        private void LoadDashboardPage()
        {
            mainContentPanel.Controls.Clear();
            DashboardPage dashboard = new DashboardPage();
            mainContentPanel.Controls.Add(dashboard);
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