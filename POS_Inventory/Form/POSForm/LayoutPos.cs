using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System;
using System.Drawing;
using System.Windows.Forms;

namespace POS_Inventory.Form.POSForm
{
    public partial class LayoutPos : System.Windows.Forms.Form
    {
        // Define UI Components
        private Panel pnlTopNav;
        private Panel pnlSideNav;
        private Panel pnlMainContent;
        private Panel pnlSearchContainer;
        private TextBox txtSearch;
        private Label lblLogo;
        private Label lblSystemName;
        private Label lblCashierName;
        private Label lblLogout;
        private PictureBox picSearchIcon;

        public LayoutPos()
        {
            InitializeComponent();
            ApplyCustomDesign();
        }

        private void ApplyCustomDesign()
        {
            // --- Form Basic Setup ---
            this.Text = "LayoutPos"; // Matches image_11cfbe.png
            this.Size = new Size(1000, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            // --- 1. Top Navigation Panel (Purple) ---
            pnlTopNav = new Panel();
            pnlTopNav.Dock = DockStyle.Top;
            pnlTopNav.Height = 70;
            pnlTopNav.BackColor = Color.FromArgb(147, 131, 206); // Purple shade from image_12a9b7.png
            this.Controls.Add(pnlTopNav);

            // Logo Label
            lblLogo = new Label();
            lblLogo.Text = "Logo";
            lblLogo.ForeColor = Color.White;
            lblLogo.Font = new Font("Segoe UI", 10);
            lblLogo.Location = new Point(20, 25);
            lblLogo.AutoSize = true;
            pnlTopNav.Controls.Add(lblLogo);

            // System Name
            lblSystemName = new Label();
            lblSystemName.Text = "Pos System";
            lblSystemName.ForeColor = Color.White;
            lblSystemName.Font = new Font("Segoe UI", 18, FontStyle.Regular);
            lblSystemName.Location = new Point(70, 18);
            lblSystemName.AutoSize = true;
            pnlTopNav.Controls.Add(lblSystemName);

            // Search Container (Greyish bar)
            pnlSearchContainer = new Panel();
            pnlSearchContainer.Size = new Size(350, 40);
            pnlSearchContainer.Location = new Point(260, 15);
            pnlSearchContainer.BackColor = Color.FromArgb(211, 211, 211); // Grey from image_12a9b7.png
            pnlTopNav.Controls.Add(pnlSearchContainer);

            // Search Icon Placeholder (using a Label for simplicity)
            Label lblSearchIcon = new Label();
            lblSearchIcon.Text = "🔍"; // Search icon
            lblSearchIcon.Font = new Font("Segoe UI", 12);
            lblSearchIcon.Location = new Point(5, 8);
            lblSearchIcon.Size = new Size(25, 25);
            pnlSearchContainer.Controls.Add(lblSearchIcon);

            // Search TextBox
            txtSearch = new TextBox();
            txtSearch.Text = "search by name..";
            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.BackColor = Color.FromArgb(211, 211, 211);
            txtSearch.Font = new Font("Segoe UI", 11);
            txtSearch.ForeColor = Color.DimGray;
            txtSearch.Size = new Size(300, 30);
            txtSearch.Location = new Point(35, 10);
            pnlSearchContainer.Controls.Add(txtSearch);

            // Cashier Name
            lblCashierName = new Label();
            lblCashierName.Text = "Cashier name";
            lblCashierName.ForeColor = Color.White;
            lblCashierName.Font = new Font("Segoe UI", 11);
            lblCashierName.Location = new Point(pnlTopNav.Width - 140, 25);
            lblCashierName.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCashierName.AutoSize = true;
            pnlTopNav.Controls.Add(lblCashierName);

            // --- 2. Side Navigation Panel (Purple) ---
            pnlSideNav = new Panel();
            pnlSideNav.Dock = DockStyle.Left;
            pnlSideNav.Width = 70;
            pnlSideNav.BackColor = Color.FromArgb(147, 131, 206); // Same purple
            this.Controls.Add(pnlSideNav);

            // Logout Label (Placed at the bottom)
            lblLogout = new Label();
            lblLogout.Text = "logout";
            lblLogout.ForeColor = Color.White;
            lblLogout.Font = new Font("Segoe UI", 10);
            lblLogout.Location = new Point(10, pnlSideNav.Height - 100);
            lblLogout.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            lblLogout.Cursor = Cursors.Hand;
            lblLogout.AutoSize = true;
            lblLogout.Click += (s, e) => {
                // Logic for logout, e.g., show login form
                this.Close();
            };
            pnlSideNav.Controls.Add(lblLogout);

            // --- 3. Main Content Panel (Light Blue) ---
            pnlMainContent = new Panel();
            pnlMainContent.Dock = DockStyle.Fill;
            pnlMainContent.BackColor = Color.FromArgb(119, 158, 254); // Light Blue from image_12a9b7.png
            this.Controls.Add(pnlMainContent);
        }
    }
}
