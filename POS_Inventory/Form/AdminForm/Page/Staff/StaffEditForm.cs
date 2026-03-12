using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Staff
{
    public partial class StaffEditForm : System.Windows.Forms.Form
    {
        private int _userId;
        private string _currentUsername;
        private string _currentEmail;
        private UserConfig _userConfig;

        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPassword;
        private Button btnTogglePassword;
        private Button btnSave;
        private Button btnCancel;

        // We pass the current data from the DataGridView row to avoid calling GetUserById
        public StaffEditForm(UserConfig userConfig, int id, string username, string email)
        {
            this._userConfig = userConfig;
            this._userId = id;
            this._currentUsername = username;
            this._currentEmail = email;

            SetupFormLayout();

            // Fill fields with current data
            txtUsername.Text = _currentUsername;
            txtEmail.Text = _currentEmail;
        }

        private void SetupFormLayout()
        {
            this.Text = "Edit Staff Member";
            this.Size = new Size(400, 420);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.White;

            Label lblHeader = new Label { Text = "Update Staff Account", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 20), AutoSize = true };

            // Username
            Label lblUser = new Label { Text = "Username:", Location = new Point(20, 70), AutoSize = true };
            txtUsername = new TextBox { Location = new Point(20, 95), Width = 340, Font = new Font("Segoe UI", 11) };

            // Email
            Label lblEmail = new Label { Text = "Email:", Location = new Point(20, 150), AutoSize = true };
            txtEmail = new TextBox { Location = new Point(20, 175), Width = 340, Font = new Font("Segoe UI", 11) };

            // Password (Required by your UpdateUser method)
            Label lblPass = new Label {
                Text = "New Password (Required):",
                Location = new Point(20, 230),
                AutoSize = true };

            txtPassword = new TextBox
            {
                Location = new Point(20, 255),
                Width = 280,
                Font = new Font("Segoe UI", 11),
                UseSystemPasswordChar = true
            };

            btnTogglePassword = new Button
            {
                Text = "Show",
                Location = new Point(310, 255), 
                Size = new Size(60, 32),     
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            btnTogglePassword.Click += BtnTogglePassword_Click;

            // Save Button
            btnSave = new Button
            {
                Text = "Save Changes",
                Size = new Size(160, 45),
                Location = new Point(20, 310),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 123, 255),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;

            // Cancel Button
            btnCancel = new Button { Text = "Cancel", Size = new Size(160, 45), Location = new Point(200, 310), FlatStyle = FlatStyle.Flat, BackColor = Color.Gainsboro };
            btnCancel.Click += (s, e) => this.Close();

            this.Controls.AddRange(new Control[] { lblHeader, lblUser, txtUsername, lblEmail, txtEmail, lblPass, txtPassword, btnTogglePassword, btnSave, btnCancel });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Your current UpdateUser method requires a password to be set.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Calling your existing: public bool UpdateUser(int id, string username, string email, string password)
            bool success = _userConfig.UpdateUser(
                _userId,
                txtUsername.Text.Trim(),
                txtEmail.Text.Trim(),
                txtPassword.Text
            );

            if (success)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Update failed. Make sure the username is unique.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnTogglePassword_Click(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar)
            {
                txtPassword.UseSystemPasswordChar = false;
                btnTogglePassword.Text = "Hide";
            }
            else
            {
                txtPassword.UseSystemPasswordChar = true;
                btnTogglePassword.Text = "Show";
            }
        }
    }
}