using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.POSForm
{
    public partial class LayoutPos : System.Windows.Forms.Form
    {
        private Panel pnlTopNav, pnlSideNav, pnlMainContent, pnlSearchContainer, pnlOrderSidebar;
        private TextBox txtSearch;
        private Label lblLogo, lblSystemName, lblCashierName, lblLogout, lblOrderTitle, lblTotal;
        private FlowLayoutPanel flowProductGrid, flowItemsOrder;
        private Button btnSubmit;

        private ProductConfig _productRepo = new ProductConfig();

        public LayoutPos()
        {
            InitializeComponent();
            LayoutDesign();
            LoadProductsFromDatabase();
        }

        private void LayoutDesign()
        {
            this.Text = "POS System";
            this.Size = new Size(1100, 750);//1100, 750
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppColorConfig.White;

            // --- 1. Top Navigation ---
            pnlTopNav = new Panel {
                Dock = DockStyle.Top,
                Height = 70, 
                BackColor = AppColorConfig.HeaderPink,
               // Padding = new Padding(0, 100, 0, 0)//Padding = new Padding(left, top, right, bottom)
            };
            this.Controls.Add(pnlTopNav);

            lblLogo = new Label {
                Text = "Logo",
                ForeColor = AppColorConfig.TextLight,
                Location = new Point(20, 25),
                AutoSize = true
            };
            pnlTopNav.Controls.Add(lblLogo);

            lblSystemName = new Label {
                Text = "Pos System",
                ForeColor = AppColorConfig.TextLight,
                Font = new Font("Segoe UI", 18),
                Location = new Point(70, 18), 
                AutoSize = true };
            pnlTopNav.Controls.Add(lblSystemName);

            pnlSearchContainer = new Panel {
                Size = new Size(350, 40),
                Location = new Point(260, 15),
                BackColor = Color.FromArgb(240, 240, 240)
            };
            pnlTopNav.Controls.Add(pnlSearchContainer);

            txtSearch = new TextBox { 
                Text = "search by name..",
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(240, 240, 240),
                Size = new Size(300, 30),
                Location = new Point(35, 10) };
            pnlSearchContainer.Controls.Add(txtSearch);

            lblCashierName = new Label { 
                Text = "Cashier Name",
                ForeColor = AppColorConfig.TextLight,
                Location = new Point(pnlTopNav.Width - 140, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = true
            };
            pnlTopNav.Controls.Add(lblCashierName);

            // --- 2. Side Navigation ---
            pnlSideNav = new Panel { 
                Dock = DockStyle.Left, Width = 70,
                BackColor = AppColorConfig.SidebarRed
            };
            this.Controls.Add(pnlSideNav);

            lblLogout = new Label {
                Text = "logout",
                ForeColor = AppColorConfig.TextLight,
                Location = new Point(10, 600),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left,
                Cursor = Cursors.Hand,
                AutoSize = true };
            lblLogout.Click += (s, e) => PerformLogout();
            pnlSideNav.Controls.Add(lblLogout);

            // --- 3. Main Content Wrapper ---
            pnlMainContent = new Panel {
                Dock = DockStyle.Fill,
                BackColor = AppColorConfig.CardStaff,
                Padding = new Padding(80, 90, 20, 20)
            };//left, top , right , buttom 
      
            this.Controls.Add(pnlMainContent);

            // --- 4. Order Sidebar (Right) ---
            pnlOrderSidebar = new Panel {
                Dock = DockStyle.Right,
                Width = 348,
                BackColor = AppColorConfig.CardProduct,
                Padding = new Padding(10, 10, 10, 20) //left, top , right , buttom 
            };

            lblOrderTitle = new Label { 
                Text = "Items Orders",
                ForeColor = AppColorConfig.TextLight,
                Font = new Font("Segoe UI", 11, FontStyle.Bold), 
                Dock = DockStyle.Top, 
                Height = 30 
            };

            Panel pnlOrderBottom = new Panel {
                Dock = DockStyle.Bottom, 
                Height = 110
            };
            lblTotal = new Label {
                Text = "total Amount: 0.00$",
                ForeColor = AppColorConfig.TextLight,
                BackColor = Color.FromArgb(100, 255, 255, 255),
                TextAlign = ContentAlignment.MiddleCenter, 
                Dock = DockStyle.Top,
                Height = 45 
            };
            btnSubmit = new Button {
                Text = "SUBMIT",
                FlatStyle = FlatStyle.Flat,
                ForeColor = AppColorConfig.TextLight,
                BackColor = AppColorConfig.BtnSave,
                Dock = DockStyle.Bottom,
                Height = 45 
            
            };
            pnlOrderBottom.Controls.Add(lblTotal);
            pnlOrderBottom.Controls.Add(btnSubmit);

            flowItemsOrder = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent 
            };

            // IMPORTANT ORDER: Add Bottom and Top first, then the Fill panel
            pnlOrderSidebar.Controls.Add(flowItemsOrder);
            pnlOrderSidebar.Controls.Add(pnlOrderBottom);
            pnlOrderSidebar.Controls.Add(lblOrderTitle);

            // Add Sidebar to Main Content first
            pnlMainContent.Controls.Add(pnlOrderSidebar);

            // --- 5. Product Grid ---
            flowProductGrid = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(5),
                BackColor = Color.Transparent
            };

            // Add Grid to Main Content last so it Fills the remaining space
            pnlMainContent.Controls.Add(flowProductGrid);
            flowProductGrid.BringToFront();
        }

        private void LoadProductsFromDatabase()
        {
            DataTable dt = _productRepo.GetAllProducts();
            flowProductGrid.Controls.Clear();

            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    CardProductPOS card = new CardProductPOS();
                    card.ProductID = Convert.ToInt32(row["id"]);
                    card.ProductName = row["product_name"].ToString();
                    card.CategoryName = row["category_name"].ToString();
                    card.Price = Convert.ToDecimal(row["price"]);
                    card.ProductPrice = row["price"].ToString();
                    card.Stock = Convert.ToInt32(row["stock_qty"]);

                    card.Click += (s, e) => AddProductToOrder(card.ProductName, card.Price);
                    flowProductGrid.Controls.Add(card);
                }
            }
        }

        private void AddProductToOrder(string name, decimal price)
        {
            ItemsOrder item = new ItemsOrder();
            item.ItemName = name;
            item.Quantity = 1;
            flowItemsOrder.Controls.Add(item);
        }

        private void PerformLogout()
        {
            // 1. Ask the user for confirmation
            if (MessageBox.Show("Are you sure you want to logout?", "Logout",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // 2. Hide the current POS/Admin form so it disappears immediately
                this.Hide();

                // 3. Create and show the Login form
                FormLogin login = new FormLogin();

                // Use Show() instead of ShowDialog() if you want the app 
                // to treat the new login form as the primary window now.
                login.Show();

                // 4. Close the current form to free up memory
                this.Close();
            }
        }
    }
}