using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using POS_Inventory.Config;
using POS_Inventory.Form.AdminForm;
 using POS_Inventory.Form.POSForm; 

namespace POS_Inventory
{
    public partial class FormLogin : System.Windows.Forms.Form
    {
        private TextBox txtUsername;
        private TextBox txtPassword;
        private ComboBox cmbRole;
        private Button btnLogin;
        private Button btnTogglePass;
        private Label lblForgotPassword;
        private Label lblGoToSignUp;
        private Label lblTitle;
        private Panel cardPanel;

        UserConfig userConfig = new UserConfig();

        public FormLogin()
        {
            InitializeComponent();
            formLogindesign();
        }

        public void formLogindesign()
        {
            this.Size = new Size(400, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;

            cardPanel = new Panel();
            cardPanel.Size = new Size(340, 580);
            cardPanel.Location = new Point((this.ClientSize.Width - cardPanel.Width) / 2, 40);
            cardPanel.BackColor = Color.Transparent;
            this.Controls.Add(cardPanel);

            lblTitle = new Label();
            lblTitle.Text = "Welcome Back!";
            lblTitle.Font = new Font("Segoe UI", 26, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Size = new Size(340, 60);
            lblTitle.Location = new Point(0, 20);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            cardPanel.Controls.Add(lblTitle);

            txtUsername = CreateModernTextBox("username or Email", 120);
            cardPanel.Controls.Add(txtUsername);

            txtPassword = CreateModernTextBox("password", 210);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Size = new Size(200, 30);
            cardPanel.Controls.Add(txtPassword);

            btnTogglePass = new Button();
            btnTogglePass.Text = "show";
            btnTogglePass.Font = new Font("Segoe UI", 9);
            btnTogglePass.Size = new Size(50, 25);
            btnTogglePass.Location = new Point(250, 240);
            btnTogglePass.FlatStyle = FlatStyle.Flat;
            btnTogglePass.FlatAppearance.BorderSize = 0;
            btnTogglePass.ForeColor = Color.FromArgb(200, 255, 255, 255);
            btnTogglePass.Cursor = Cursors.Hand;
            btnTogglePass.Click += BtnTogglePass_Click;
            cardPanel.Controls.Add(btnTogglePass);

            Label lblRole = new Label()
            {
                Text = "Select Role:",
                ForeColor = Color.White,
                Location = new Point(40, 310),
                AutoSize = true,
                Font = new Font("Segoe UI", 10)
            };
            cmbRole = new ComboBox();
            cmbRole.Items.AddRange(new string[] { "Admin", "Cashier" });
            cmbRole.SelectedIndex = 0;
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.Size = new Size(260, 30);
            cmbRole.Location = new Point(40, 340);
            cardPanel.Controls.Add(lblRole);
            cardPanel.Controls.Add(cmbRole);

            btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.Size = new Size(260, 60);
            btnLogin.Location = new Point(40, 430);
            btnLogin.BackColor = Color.White;
            btnLogin.ForeColor = Color.FromArgb(60, 100, 210);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Font = new Font("Segoe UI", 16, FontStyle.Bold);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;

            GraphicsPath path = new GraphicsPath();
            int radius = 20;
            path.AddArc(0, 0, radius, radius, 180, 90);
            path.AddArc(btnLogin.Width - radius, 0, radius, radius, 270, 90);
            path.AddArc(btnLogin.Width - radius, btnLogin.Height - radius, radius, radius, 0, 90);
            path.AddArc(0, btnLogin.Height - radius, radius, radius, 90, 90);
            btnLogin.Region = new Region(path);
            cardPanel.Controls.Add(btnLogin);

            lblForgotPassword = new Label();
            lblForgotPassword.Text = "forgot password?";
            lblForgotPassword.ForeColor = Color.FromArgb(200, 255, 255, 255);
            lblForgotPassword.Location = new Point(0, 510);
            lblForgotPassword.Size = new Size(340, 20);
            lblForgotPassword.TextAlign = ContentAlignment.MiddleCenter;
            lblForgotPassword.Cursor = Cursors.Hand;
            lblForgotPassword.Click += (s, e) => MessageBox.Show("Contact Admin.");
            cardPanel.Controls.Add(lblForgotPassword);

            lblGoToSignUp = new Label();
            lblGoToSignUp.Text = "Don't have an account? Create one";
            lblGoToSignUp.ForeColor = Color.White;
            lblGoToSignUp.Location = new Point(0, 545);
            lblGoToSignUp.Size = new Size(340, 20);
            lblGoToSignUp.TextAlign = ContentAlignment.MiddleCenter;
            lblGoToSignUp.Cursor = Cursors.Hand;
            lblGoToSignUp.Click += LblGoToSignUp_Click;
            cardPanel.Controls.Add(lblGoToSignUp);

            Button btnExit = new Button() { Text = "✕", Size = new Size(30, 30), Location = new Point(360, 0), FlatStyle = FlatStyle.Flat, ForeColor = Color.White, BackColor = Color.FromArgb(50, 0, 0, 0) };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatAppearance.MouseOverBackColor = Color.Red;

            btnExit.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnExit);
        }

        private TextBox CreateModernTextBox(string labelText, int y)
        {
            Label lbl = new Label();
            lbl.Text = labelText;
            lbl.ForeColor = Color.White;
            lbl.Font = new Font("Segoe UI", 11);
            lbl.Location = new Point(40, y);
            lbl.AutoSize = true;
            cardPanel.Controls.Add(lbl);

            TextBox tb = new TextBox();
            tb.Size = new Size(260, 35);
            tb.Location = new Point(40, y + 28);
            tb.Font = new Font("Segoe UI", 12);
            tb.BackColor = Color.FromArgb(40, 120, 180);
            tb.ForeColor = Color.White;
            tb.BorderStyle = BorderStyle.None;

            Panel line = new Panel() { Size = new Size(260, 2), Location = new Point(40, y + 65), BackColor = Color.White };
            cardPanel.Controls.Add(line);
            cardPanel.Controls.Add(tb);
            return tb;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                Color.FromArgb(50, 150, 220), Color.FromArgb(120, 60, 160), 90f))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            base.OnPaint(e);
        }

        private void BtnTogglePass_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            btnTogglePass.Text = txtPassword.UseSystemPasswordChar ? "show" : "hide";
        }

        private void LblGoToSignUp_Click(object sender, EventArgs e)
        {
            FormSingUp signUpForm = new FormSingUp();
            signUpForm.Show();
            this.Hide();
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            string identifier = txtUsername.Text.Trim();
            string pass = txtPassword.Text.Trim();
            string selectedRole = cmbRole.SelectedItem?.ToString();

            if (string.IsNullOrEmpty(identifier) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("Please enter your credentials.", "Warning");
                return;
            }

            try
            {
                DataTable result = userConfig.ValidateLogin(identifier, pass);
                if (result != null && result.Rows.Count > 0)
                {
                    string dbRole = result.Rows[0]["role"].ToString();

                    // Check if selected role matches database role
                    if (dbRole.Equals(selectedRole, StringComparison.OrdinalIgnoreCase))
                    {
                        if (dbRole == "Admin")
                        {
                            AdminLayout adminForm = new AdminLayout();
                            adminForm.Show();
                            this.Hide();
                        }
                        else if (dbRole == "Cashier")
                        {
                            
                             LayoutPos cashierForm = new LayoutPos();
                            cashierForm.Show();
                            this.Hide();
                        }
                    }
                    else
                    {
                        MessageBox.Show($"Access Denied. You are registered as {dbRole}, not {selectedRole}.", "Role Mismatch");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Username/Email or Password", "Login Failed");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
    }
}