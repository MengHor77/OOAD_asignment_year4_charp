using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using POS_Inventory.Config;
using POS_Inventory.Form.AdminForm;

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
            this.Size = new Size(400, 600);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = AppColorConfix.PrimaryBlue;

            cardPanel = new Panel();
            cardPanel.Size = new Size(340, 480);
            cardPanel.Location = new Point((this.ClientSize.Width - cardPanel.Width) / 2, 60);
            cardPanel.BackColor = AppColorConfix.Transparent;
            this.Controls.Add(cardPanel);

            lblTitle = new Label();
            lblTitle.Text = "Welcome Back!";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = AppColorConfix.White;
            lblTitle.Size = new Size(340, 50);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            cardPanel.Controls.Add(lblTitle);

            // Placeholder updated to show Username or Email
            txtUsername = CreateModernTextBox("Username or Email", 100);
            cardPanel.Controls.Add(txtUsername);

            txtPassword = CreateModernTextBox("Password", 170);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Size = new Size(210, 30);
            cardPanel.Controls.Add(txtPassword);

            btnTogglePass = new Button();
            btnTogglePass.Text = "show";
            btnTogglePass.Size = new Size(50, 25);
            btnTogglePass.Location = new Point(250, 175);
            btnTogglePass.FlatStyle = FlatStyle.Flat;
            btnTogglePass.FlatAppearance.BorderSize = 0;
            btnTogglePass.ForeColor = AppColorConfix.White;
            btnTogglePass.Cursor = Cursors.Hand;
            btnTogglePass.Click += BtnTogglePass_Click;
            cardPanel.Controls.Add(btnTogglePass);

            Label lblRole = new Label() { Text = "Select Role:", ForeColor = AppColorConfix.White, Location = new Point(40, 230), AutoSize = true };
            cmbRole = new ComboBox();
            cmbRole.Items.AddRange(new string[] { "Admin", "Cashier" });
            cmbRole.SelectedIndex = 0;
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.Size = new Size(260, 30);
            cmbRole.Location = new Point(40, 250);
            cardPanel.Controls.Add(lblRole);
            cardPanel.Controls.Add(cmbRole);

            btnLogin = new Button();
            btnLogin.Text = "LOGIN";
            btnLogin.Size = new Size(260, 50);
            btnLogin.Location = new Point(40, 320);
            btnLogin.BackColor = AppColorConfix.White;
            btnLogin.ForeColor = AppColorConfix.LoginBtnText;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;

            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, 25, 25, 180, 90);
            path.AddArc(btnLogin.Width - 25, 0, 25, 25, 270, 90);
            path.AddArc(btnLogin.Width - 25, btnLogin.Height - 25, 25, 25, 0, 90);
            path.AddArc(0, btnLogin.Height - 25, 25, 25, 90, 90);
            btnLogin.Region = new Region(path);
            cardPanel.Controls.Add(btnLogin);

            lblForgotPassword = new Label();
            lblForgotPassword.Text = "forgot password?";
            lblForgotPassword.ForeColor = AppColorConfix.PlaceholderWhite;
            lblForgotPassword.Location = new Point(0, 390);
            lblForgotPassword.Size = new Size(340, 20);
            lblForgotPassword.TextAlign = ContentAlignment.MiddleCenter;
            lblForgotPassword.Cursor = Cursors.Hand;
            lblForgotPassword.Click += (s, e) => MessageBox.Show("Contact Admin.");
            cardPanel.Controls.Add(lblForgotPassword);

            Button btnExit = new Button() { Text = "✕", Size = new Size(30, 30), Location = new Point(360, 0), FlatStyle = FlatStyle.Flat, ForeColor = AppColorConfix.White };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatAppearance.MouseOverBackColor = Color.Red;
            btnExit.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnExit);
        }

        private TextBox CreateModernTextBox(string placeholder, int y)
        {
            TextBox tb = new TextBox();
            tb.Size = new Size(260, 30);
            tb.Location = new Point(40, y + 5);
            tb.Font = new Font("Segoe UI", 12);
            tb.BackColor = AppColorConfix.PrimaryBlue;
            tb.ForeColor = AppColorConfix.White;
            tb.BorderStyle = BorderStyle.None;
            // Note: In a real app, you'd add logic here to handle the placeholder text disappearance

            Panel line = new Panel() { Size = new Size(260, 2), Location = new Point(40, y + 35), BackColor = AppColorConfix.White };
            cardPanel.Controls.Add(line);
            return tb;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle,
                AppColorConfix.GradientStart, AppColorConfix.GradientEnd, 45f))
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
                if (result.Rows.Count > 0)
                {
                    string dbRole = result.Rows[0]["role"].ToString();
                    if (dbRole.Equals(selectedRole, StringComparison.OrdinalIgnoreCase))
                    {
                        if (dbRole == "Admin")
                        {
                            AdminLayout adminForm = new AdminLayout();
                            adminForm.Show();
                            this.Hide();
                        }
                    }
                    else { MessageBox.Show("Role Mismatch"); }
                }
                else { MessageBox.Show("Invalid Username/Email or Password"); }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
    }
}