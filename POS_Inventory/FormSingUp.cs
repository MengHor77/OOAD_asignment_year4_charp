using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory
{
    public partial class FormSingUp : System.Windows.Forms.Form
    {
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;
        private Button btnRegister;
        private Button btnTogglePass; // Added toggle
        private Label lblLoginLink;
        private Label lblTitle;
        private Panel cardPanel;

        private UserConfig userConfig = new UserConfig();

        public FormSingUp()
        {
            InitializeComponent();
            SetupDesign();
        }

        private void SetupDesign()
        {
            this.Size = new Size(400, 650);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = AppColorConfig.PrimaryBlue;

            cardPanel = new Panel();
            cardPanel.Size = new Size(340, 550);
            cardPanel.Location = new Point((this.ClientSize.Width - cardPanel.Width) / 2, 40);
            cardPanel.BackColor = AppColorConfig.Transparent;
            this.Controls.Add(cardPanel);

            lblTitle = new Label();
            lblTitle.Text = "Create Account";
            lblTitle.Font = new Font("Segoe UI", 22, FontStyle.Bold);
            lblTitle.ForeColor = AppColorConfig.White;
            lblTitle.Size = new Size(340, 50);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            cardPanel.Controls.Add(lblTitle);

            // Inputs
            txtUsername = CreateModernTextBox("Username", 80);
            txtEmail = CreateModernTextBox("Email Address", 150);

            // Password with Show/Hide
            txtPassword = CreateModernTextBox("Password", 220);
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Size = new Size(210, 30);

            btnTogglePass = new Button();
            btnTogglePass.Text = "show";
            btnTogglePass.Size = new Size(50, 25);
            btnTogglePass.Location = new Point(250, 245); // Aligned with txtPassword
            btnTogglePass.FlatStyle = FlatStyle.Flat;
            btnTogglePass.FlatAppearance.BorderSize = 0;
            btnTogglePass.ForeColor = AppColorConfig.White;
            btnTogglePass.Cursor = Cursors.Hand;
            btnTogglePass.Click += BtnTogglePass_Click;
            cardPanel.Controls.Add(btnTogglePass);

            txtConfirmPassword = CreateModernTextBox("Confirm Password", 290);
            txtConfirmPassword.UseSystemPasswordChar = true;

            // Register Button
            btnRegister = new Button();
            btnRegister.Text = "REGISTER";
            btnRegister.Size = new Size(260, 50);
            btnRegister.Location = new Point(40, 380);
            btnRegister.BackColor = AppColorConfig.White;
            btnRegister.ForeColor = AppColorConfig.LoginBtnText;
            btnRegister.FlatStyle = FlatStyle.Flat;
            btnRegister.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            btnRegister.Cursor = Cursors.Hand;
            btnRegister.Click += BtnRegister_Click;

            // Rounded Corners
            GraphicsPath path = new GraphicsPath();
            path.AddArc(0, 0, 25, 25, 180, 90);
            path.AddArc(btnRegister.Width - 25, 0, 25, 25, 270, 90);
            path.AddArc(btnRegister.Width - 25, btnRegister.Height - 25, 25, 25, 0, 90);
            path.AddArc(0, btnRegister.Height - 25, 25, 25, 90, 90);
            btnRegister.Region = new Region(path);
            cardPanel.Controls.Add(btnRegister);

            lblLoginLink = new Label();
            lblLoginLink.Text = "Already have an account? Login";
            lblLoginLink.ForeColor = AppColorConfig.PlaceholderWhite;
            lblLoginLink.Location = new Point(0, 460);
            lblLoginLink.Size = new Size(340, 20);
            lblLoginLink.TextAlign = ContentAlignment.MiddleCenter;
            lblLoginLink.Cursor = Cursors.Hand;
            lblLoginLink.Click += (s, e) => {
                new FormLogin().Show();
                this.Hide();
            };
            cardPanel.Controls.Add(lblLoginLink);

            Button btnExit = new Button() { Text = "✕", Size = new Size(30, 30), Location = new Point(360, 0), FlatStyle = FlatStyle.Flat, ForeColor = AppColorConfig.White };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatAppearance.MouseOverBackColor = Color.Red;
            btnExit.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnExit);
        }

        private void BtnTogglePass_Click(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            txtConfirmPassword.UseSystemPasswordChar = txtPassword.UseSystemPasswordChar;
            btnTogglePass.Text = txtPassword.UseSystemPasswordChar ? "show" : "hide";
        }

        private TextBox CreateModernTextBox(string placeholder, int y)
        {
            Label lblPlaceholder = new Label() { Text = placeholder, ForeColor = AppColorConfig.PlaceholderWhite, Location = new Point(40, y), AutoSize = true, Font = new Font("Segoe UI", 9) };
            cardPanel.Controls.Add(lblPlaceholder);

            TextBox tb = new TextBox();
            tb.Size = new Size(260, 30);
            tb.Location = new Point(40, y + 20);
            tb.Font = new Font("Segoe UI", 11);
            tb.BackColor = AppColorConfig.PrimaryBlue;
            tb.ForeColor = AppColorConfig.White;
            tb.BorderStyle = BorderStyle.None;

            Panel line = new Panel() { Size = new Size(260, 1), Location = new Point(40, y + 45), BackColor = AppColorConfig.White };
            cardPanel.Controls.Add(line);
            cardPanel.Controls.Add(tb);
            return tb;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            using (LinearGradientBrush brush = new LinearGradientBrush(this.ClientRectangle, AppColorConfig.GradientStart, AppColorConfig.GradientEnd, 45f))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
            base.OnPaint(e);
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            string user = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string pass = txtPassword.Text.Trim();
            string confirm = txtConfirmPassword.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("All fields are required!", "Warning");
                return;
            }

            if (pass != confirm)
            {
                MessageBox.Show("Passwords do not match!", "Error");
                return;
            }

            try
            {
                // Defaulting to 'Cashier' role for all SignUps
                if (userConfig.CreateCashier(user, email, pass))
                {
                    MessageBox.Show("Registration Successful!", "Success");
                    new FormLogin().Show();
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Username already exists or Registration failed.");
                }
            }
            catch (Exception ex) { MessageBox.Show("Error: " + ex.Message); }
        }
    }
}