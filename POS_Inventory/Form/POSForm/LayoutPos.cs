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
        private Label  lblSystemName, lblCashierName, lblOrderTitle, lblTotal;
        private FlowLayoutPanel flowProductGrid, flowItemsOrder;
        private Button btnSubmit, btnLogout;
        private DataTable productTable;
        private readonly  ProductConfig _productRepo = new ProductConfig();
        [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
        private static extern Int32 SendMessage(IntPtr hWnd, int msg, int wParam, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string lParam);

        private readonly  ItemOrderConfig _orderRepo = new ItemOrderConfig();
        private readonly  SaleConfig _saleRepo = new SaleConfig();


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
                Size = new Size(500, 40),
                Location = new Point(260, 15),
                BackColor = Color.FromArgb(240, 240, 240)
            };

            pnlSearchContainer.Paint += (s, e) =>
            {
                using (System.Drawing.Drawing2D.GraphicsPath gp = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    int radius = 20;
                    gp.AddArc(0, 0, radius, radius, 180, 90);
                    gp.AddArc(pnlSearchContainer.Width - radius, 0, radius, radius, 270, 90);
                    gp.AddArc(pnlSearchContainer.Width - radius, pnlSearchContainer.Height - radius, radius, radius, 0, 90);
                    gp.AddArc(0, pnlSearchContainer.Height - radius, radius, radius, 90, 90);
                    gp.CloseAllFigures();
                    pnlSearchContainer.Region = new Region(gp);
                    e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    using (System.Drawing.Drawing2D.GraphicsPath border = (System.Drawing.Drawing2D.GraphicsPath)gp.Clone())
                    {
                        e.Graphics.DrawPath(new Pen(Color.FromArgb(200, 200, 200), 1), border);
                    }
                }
            };

            pnlTopNav.Controls.Add(pnlSearchContainer);

            // Inside LayoutDesign, replace your txtSearch setup with this:
            txtSearch = new TextBox
            {
                Text = "search by name..",
                ForeColor = Color.Gray, // Set initial color here
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(240, 240, 240),
                Size = new Size(450, 30),
                Location = new Point(35, 10)
            };

            // 1. Create a spacer panel
            Panel spacer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 50, // This is your gap size
                BackColor = Color.Transparent
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
            pnlSideNav = new Panel
            {
                Dock = DockStyle.Left,
                Width = 80, // Slightly wider to fit a button better
                BackColor = AppColorConfig.SidebarRed,
                Padding = new Padding(5, 0, 5, 20) // Adds padding so button doesn't touch edges
            };
            this.Controls.Add(pnlSideNav);

            // Create the Button instead of the Label
            btnLogout = new Button
            {
                Text = "Logout",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold), // Slightly smaller font for narrow sidebar
                ForeColor = AppColorConfig.White,
                BackColor = Color.FromArgb(200, 0, 0), // A darker red so it's visible on the sidebar
                Dock = DockStyle.Bottom,
                Height = 45,
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;

            // Apply rounding and click event
            btnLogout.Resize += (s, e) => ApplyRounding(btnLogout, 15);
            btnLogout.Click += (s, e) => PerformLogout();

            pnlSideNav.Controls.Add(btnLogout);

            // --- 3. Main Content Wrapper ---
            pnlMainContent = new Panel {
                Dock = DockStyle.Fill,
                BackColor = AppColorConfig.ContentBackground,
                Padding = new Padding(80, 90, 0, 20)
            };//left, top , right , buttom 
      
            this.Controls.Add(pnlMainContent);

            // --- 4. Order Sidebar (Right) ---
            pnlOrderSidebar = new Panel {
                Dock = DockStyle.Right,
                Width = 370,
                BackColor = AppColorConfig.GrayLight,
                Padding = new Padding(10, 10, 0, 20) //left, top , right , buttom 
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
                Height = 110,
                Padding =new Padding(0,0,30,0),//(left, top, right, bottom)
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
            lblTotal.Resize += (s, e) => ApplyRounding(lblTotal, 20);


            btnSubmit = new Button {
                Text = "SUBMIT",
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = AppColorConfig.White,
                BackColor = AppColorConfig.BtnSave,
                Dock = DockStyle.Bottom,
                Height = 45 ,

            };
            btnSubmit.FlatAppearance.BorderSize = 0; // Required for clean rounding

            btnSubmit.Resize += (s, e) => ApplyRounding(btnSubmit, 20);
            btnSubmit.Click += BtnSubmit_Click;

            pnlOrderBottom.Controls.Add(lblTotal);
            pnlOrderBottom.Controls.Add(btnSubmit);

            flowItemsOrder = new FlowLayoutPanel {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.Transparent 
            };

            // IMPORTANT ORDER: Add Bottom and Top first, then the Fill panel
            pnlOrderSidebar.Controls.Add(flowItemsOrder);
            pnlOrderSidebar.Controls.Add(spacer);
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

        private void UpdateTotal()
        {
            decimal total = 0;

            foreach (ItemsOrder item in flowItemsOrder.Controls)
            {
                decimal price = item.Price;
                int qty = item.Quantity;

                total += price * qty;
            }

            lblTotal.Text = "Total Amount: " + total.ToString("0.00") + "$";
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

        private void ApplyRounding(Control control, int radius)
        {
            if (control.Width <= 0 || control.Height <= 0) return;

            using (System.Drawing.Drawing2D.GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddArc(0, 0, radius, radius, 180, 90);
                path.AddArc(control.Width - radius, 0, radius, radius, 270, 90);
                path.AddArc(control.Width - radius, control.Height - radius, radius, radius, 0, 90);
                path.AddArc(0, control.Height - radius, radius, radius, 90, 90);
                path.CloseAllFigures();

                control.Region = new Region(path);
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
        private void AddProductToOrder(string name, decimal price)
        {
            foreach (ItemsOrder item in flowItemsOrder.Controls)
            {
                if (item.ItemName == name)
                {
                    item.Quantity++;
                    UpdateTotal();
                    return;
                }
            }

            ItemsOrder newItem = new ItemsOrder();
            newItem.ItemName = name;
            newItem.Price = price;
            newItem.Quantity = 1;

            newItem.OnQuantityChanged += UpdateTotal;
            newItem.OnItemDeleted += UpdateTotal;
            newItem.UnitPrice = price; // use the price passed as parameter
            flowItemsOrder.Controls.Add(newItem);

            _orderRepo.InsertItem(name, price, 1);

            UpdateTotal();
        }

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            if (flowItemsOrder.Controls.Count == 0)
            {
                MessageBox.Show("No items in order yet! Please add products before submitting.",
                                "No Items",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            decimal total = 0;

            DataTable dt = _orderRepo.GetItems();

            foreach (DataRow row in dt.Rows)
            {
                decimal price = Convert.ToDecimal(row["price"]);
                int qty = Convert.ToInt32(row["qty"]);

                total += price * qty;
            }

            bool success = _saleRepo.InsertSale(total);

            if (success)
            {
                MessageBox.Show("Sale completed successfully!", "Success");

                flowItemsOrder.Controls.Clear();
                _orderRepo.ClearItems();

                lblTotal.Text = "Total Amount: 0.00$";
            }
        }


    }
}