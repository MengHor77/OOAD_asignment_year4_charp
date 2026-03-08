using POS_Inventory.Config;
using POS_Inventory.Form.AdminForm.Page.Dashboard;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory;
using POS_Inventory.Form.AdminForm.Page.Category;
using POS_Inventory.Form.AdminForm.Page.Inventory;
using POS_Inventory.Form.AdminForm.Page.Product;
using POS_Inventory.Form.AdminForm.Page.Staff;
using POS_Inventory.Form.AdminForm.Page.Report;

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
        private const int ExpandedWidth = 180;
        private const int CollapsedWidth = 80;

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
            // --- 0. FORM PROPERTIES ---
            this.DoubleBuffered = true;
            this.Text = "Admin POS";
            this.Size = new Size(1100, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.BackColor = AppColorConfig.ContentBackground;

            // --- 1. INITIALIZE THE "SLOT" (Main Content Panel) ---
            mainContentPanel = new Panel();
            mainContentPanel.Dock = DockStyle.Fill;
            mainContentPanel.BackColor = AppColorConfig.ContentBackground;
            mainContentPanel.Padding = new Padding(0);

            // --- 2. SIDE PANEL (Docked Left) ---
            sidePanel = new Panel();
            sidePanel.Dock = DockStyle.Left;
            sidePanel.Width = ExpandedWidth;
            sidePanel.BackColor = AppColorConfig.SidebarRed;

            // --- 3. HEADER PANEL (Docked Top) ---
            headerPanel = new Panel();
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Height = 50;
            headerPanel.BackColor = AppColorConfig.HeaderPink;

            // --- 4. ADD TO FORM & SET Z-ORDER (CRITICAL FOR RESIZING) ---
            // We add them in this specific order, then adjust depth
            this.Controls.Add(mainContentPanel);
            this.Controls.Add(headerPanel);
            this.Controls.Add(sidePanel);

            sidePanel.BringToFront();
            headerPanel.BringToFront();
            mainContentPanel.SendToBack(); // Sends the slot to the back so it fills remaining space

            // --- 5. HEADER UI ELEMENTS ---
            Label btnMenu = new Label();
            btnMenu.Text = "≡";
            btnMenu.Font = new Font("Segoe UI", 18, FontStyle.Bold);
            btnMenu.ForeColor = AppColorConfig.TextDark;
            btnMenu.Cursor = Cursors.Hand;
            btnMenu.Size = new Size(45, 45);
            btnMenu.TextAlign = ContentAlignment.MiddleCenter;
            btnMenu.Location = new Point(2, 2);
            btnMenu.Click += (s, e) => sidebarTimer.Start();

            btnMenu.MouseEnter += (s, e) => btnMenu.BackColor = AppColorConfig.SidebarHover;
            btnMenu.MouseLeave += (s, e) => btnMenu.BackColor = Color.Transparent;
            headerPanel.Controls.Add(btnMenu);

            Label lblWelcome = new Label();
            lblWelcome.Text = "Welcome Admin!";
            lblWelcome.ForeColor = AppColorConfig.TextDark;
            lblWelcome.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            lblWelcome.Location = new Point(60, 15);
            lblWelcome.AutoSize = true;
            headerPanel.Controls.Add(lblWelcome);

            AddHeaderLink("Logout", 60);
            AddHeaderLink("profile", 130);
            // --- Add this at the very bottom of SetupLayoutDesign ---
            this.Load += (s, e) => {
                sidePanel.BringToFront();
                headerPanel.BringToFront();
                mainContentPanel.SendToBack();
            };
        }

        public void InitializeSidebar()
        {
            sidePanel.Controls.Clear();

            // 1. Branding Panel (Container)
            pnlSidebarBranding = new Panel();
            pnlSidebarBranding.Dock = DockStyle.Top;
            pnlSidebarBranding.Height = 50;
            pnlSidebarBranding.BackColor = AppColorConfig.SidebarHover;

            lblAdminTitle = new Label();
            lblAdminTitle.Text = isCollapsed ? "Admin" : "Admin POS";
            lblAdminTitle.ForeColor = AppColorConfig.TextDark;
            lblAdminTitle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            lblAdminTitle.Dock = DockStyle.Fill;
            lblAdminTitle.TextAlign = ContentAlignment.MiddleCenter;

            pnlSidebarBranding.Controls.Add(lblAdminTitle);
            sidePanel.Controls.Add(pnlSidebarBranding);

            // 2. Menu Items
            List<SidebarItem> menuItems = new List<SidebarItem>()
            {
                new SidebarItem { Title = "Dasboard", IconText = "📊" },
                new SidebarItem { Title = "Category", IconText = "📦" },
                new SidebarItem { Title = "Product", IconText = "🛍️" },
                new SidebarItem { Title = "Staff", IconText = "👥" },
                new SidebarItem { Title = "Report", IconText = "📈" },
                new SidebarItem { Title = "Logout", IconText = "🚪" }
            };

            // 3. Add buttons in reverse order because of DockStyle.Top stack
            for (int i = menuItems.Count - 1; i >= 0; i--)
            {
                sidePanel.Controls.Add(CreateMenuButton(menuItems[i]));
            }

            // 4. Ensure branding stays at the very top
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
            btn.ForeColor = AppColorConfig.TextDark;
            btn.Font = new Font("Segoe UI", 10);
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(10, 0, 0, 0);
            btn.Cursor = Cursors.Hand;

            btn.MouseEnter += (s, e) => btn.BackColor = AppColorConfig.SidebarHover;
            btn.MouseLeave += (s, e) => btn.BackColor = AppColorConfig.SidebarRed;
            btn.Click += (s, e) => {
                if (item.Title == "Logout") PerformLogout();
                else NavigateToPage(item.Title);
            };

            return btn;
        }

        private void NavigateToPage(string title)
        {
            // 1. Clear the current page from the main panel
            mainContentPanel.Controls.Clear();

            // 2. Identify which page to load based on the button title
            UserControl pageToLoad = null;

            switch (title)
            {
                case "Dasboard":
                    pageToLoad = new DashboardPage();
                    break;

                case "Category":
                    pageToLoad = new CategoryPage();
                    break;

                case "Product":
                    // Adjust the class name if your Product page is named differently
                    pageToLoad = new InventoryPage();
                    break;

                case "Staff":
                    pageToLoad = new StaffPage();
                    break;

                case "Report":
                    pageToLoad = new ReportPage();
                    break;

                default:
                    // Fallback if page isn't created yet
                    Label lbl = new Label
                    {
                        Text = title + " Page coming soon...",
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Font = new Font("Segoe UI", 12)
                    };
                    mainContentPanel.Controls.Add(lbl);
                    return;
            }

            // 3. Configure the page to fill the panel and add it
            if (pageToLoad != null)
            {
                pageToLoad.Dock = DockStyle.Fill;
                mainContentPanel.Controls.Add(pageToLoad);
            }
        }
        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            this.SuspendLayout();

            if (isCollapsed)
            {
                sidePanel.Width += 10;

                if (sidePanel.Width >= ExpandedWidth)
                {
                    sidebarTimer.Stop();
                    isCollapsed = false;
                    RefreshButtonAppearance();
                }
            }
            else
            {
                sidePanel.Width -= 10;

                if (sidePanel.Width <= CollapsedWidth)
                {
                    sidebarTimer.Stop();
                    isCollapsed = true;
                    RefreshButtonAppearance();
                }
            }

            this.ResumeLayout(true);
        }
        private void RefreshButtonAppearance()
        {
            if (lblAdminTitle != null)
            {
                lblAdminTitle.Text = isCollapsed ? "Admin" : "Admin POS";
                lblAdminTitle.Font = new Font("Segoe UI", isCollapsed ? 10 : 11, FontStyle.Bold);
            }

            foreach (Control ctrl in sidePanel.Controls)
            {
                if (ctrl is Button btn && btn.Tag is SidebarItem item)
                {
                    if (isCollapsed)
                    {
                        btn.TextAlign = ContentAlignment.MiddleCenter;
                        btn.Padding = new Padding(0);
                        btn.Text = item.IconText;
                    }
                    else
                    {
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
            lbl.ForeColor = AppColorConfig.TextDark;
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
            dashboard.Dock = DockStyle.Fill;
            mainContentPanel.Controls.Add(dashboard);
        }

        private void PerformLogout()
        {
            if (MessageBox.Show("Are you sure you want to logout?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                 FormLogin loginForm = new FormLogin();

                 loginForm.Show();

                 this.Close();
            }
        }
    }
}