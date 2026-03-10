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
        private DataTable productTable;
        private ProductConfig _productRepo = new ProductConfig();
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lParam);
        public LayoutPos()
        {
            InitializeComponent();
            LayoutDesign();
            LoadProductsFromDatabase();
            SetSearchPlaceholder();
            lblCashierName.Text = "User: " + UserSession.Username;

            this.ActiveControl = lblSystemName;

        }

        private void LayoutDesign()
        {
            this.Text = "POS System";
            this.Size = new Size(1340, 750); 
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = AppColorConfig.White;

            // --- 1. Top Navigation ---
            pnlTopNav = new Panel {
                Dock = DockStyle.Top,
                Height = 70, 
                BackColor = AppColorConfig.HeaderPink,
            };
            this.Controls.Add(pnlTopNav);

            PictureBox picLogo = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(20, 15),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Properties.Resources.logo
            };

            pnlTopNav.Controls.Add(picLogo);

            lblSystemName = new Label {
                Text = "Pos System",
                ForeColor = AppColorConfig.TextLight,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Location = new Point(70, 18), 
                AutoSize = true };
            pnlTopNav.Controls.Add(lblSystemName);

            pnlSearchContainer = new Panel {
                Size = new Size(350, 40),
                Location = new Point(260, 15),
                BackColor = Color.FromArgb(240, 240, 240)
            };
            pnlTopNav.Controls.Add(pnlSearchContainer);

            // Inside LayoutDesign, replace your txtSearch setup with this:
            txtSearch = new TextBox
            {
                Text = "search by name..",
                ForeColor = Color.Gray, // Set initial color here
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(240, 240, 240),
                Size = new Size(300, 30),
                Location = new Point(35, 10)
            };

            // Use ONLY these event assignments
            txtSearch.Enter += TxtSearch_Enter;
            txtSearch.Leave += TxtSearch_Leave;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            pnlSearchContainer.Controls.Add(txtSearch);

            lblCashierName = new Label {
                Text = "",
                ForeColor = AppColorConfig.TextLight,
                Location = new Point(pnlTopNav.Width - 140, 25),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                AutoSize = true,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),

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
                BackColor = AppColorConfig.ContentBackground,
                Padding = new Padding(80, 90, 20, 20)
            };//left, top , right , buttom 
      
            this.Controls.Add(pnlMainContent);

            // --- 4. Order Sidebar (Right) ---
            pnlOrderSidebar = new Panel {
                Dock = DockStyle.Right,
                Width = 348,
                BackColor = AppColorConfig.GrayLight,
                Padding = new Padding(10, 10, 10, 20) //left, top , right , buttom 
            };

            lblOrderTitle = new Label { 
                Text = "Items Orders",
                ForeColor = AppColorConfig.GrayDark,
                Font = new Font("Segoe UI", 11, FontStyle.Bold), 
                Dock = DockStyle.Top, 
                Height = 30 
            };

            Panel pnlOrderBottom = new Panel {
                Dock = DockStyle.Bottom, 
                Height = 110
            };
            lblTotal = new Label {
                Text = "Total Amount: 0.00$",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = AppColorConfig.White,
                BackColor = AppColorConfig.LightBlue,
                TextAlign = ContentAlignment.MiddleCenter, 
                Dock = DockStyle.Top,
                Height = 45 
            };
            btnSubmit = new Button {
                Text = "SUBMIT",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = AppColorConfig.White,
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
        private void SetSearchPlaceholder()
        {
            // 0x1501 is the Windows message for EM_SETCUEBANNER
            SendMessage(txtSearch.Handle, 0x1501, 0, "Search by name...");
        }
        private void TxtSearch_Enter(object sender, EventArgs e)
        {
            if (txtSearch.Text == "search by name..")
            {
                txtSearch.Text = "";
                txtSearch.ForeColor = Color.Black;
            }
        }



        private void TxtSearch_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                txtSearch.Text = "search by name..";
                txtSearch.ForeColor = Color.Gray;
            }
        }


        private void LoadProductsFromDatabase()
        {
            productTable = _productRepo.GetAllProducts();   // store products
            flowProductGrid.Controls.Clear();

            if (productTable != null)
            {
                foreach (DataRow row in productTable.Rows)
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

        private void DisplayProducts(DataTable dt)
        {
            flowProductGrid.Controls.Clear();

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
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (productTable == null) return;

            string search = txtSearch.Text.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(search) || search == "search by name..")
            {
                DisplayProducts(productTable);
                return;
            }

            DataRow[] filteredRows = productTable.Select(
                $"Convert(id,'System.String') LIKE '%{search}%' OR " +
                $"product_name LIKE '%{search}%' OR " +
                $"category_name LIKE '%{search}%'"
            );

            DataTable filteredTable = productTable.Clone();

            foreach (DataRow row in filteredRows)
            {
                filteredTable.ImportRow(row);
            }

            DisplayProducts(filteredTable);
        }

    }
}