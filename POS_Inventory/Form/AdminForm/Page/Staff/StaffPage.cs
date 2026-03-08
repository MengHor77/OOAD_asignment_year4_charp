using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using POS_Inventory.Config;

namespace POS_Inventory.Form.AdminForm.Page.Staff
{
    public partial class StaffPage : UserControl
    {
        private DataGridView dgvStaff;
        private TextBox txtFullName, txtUsername, txtPassword;
        private ComboBox cmbRole;
        private Button btnAdd, btnUpdate, btnDelete, btnClear, btnTogglePass;
        private int selectedId = -1;

        public StaffPage()
        {
            InitializeComponent();
            SetupLayout();
            LoadStaffData();
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.White;

            // --- 1. INPUT PANEL (TOP) ---
            Panel pnlInputs = new Panel { Dock = DockStyle.Top, Height = 180, BackColor = Color.FromArgb(242, 242, 242), Padding = new Padding(15) };

            Label lblTitle = new Label { Text = "Staff Management", Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(20, 10), AutoSize = true };

            // Row 1: Name and Username
            AddLabelAndControl(pnlInputs, "Full Name:", txtFullName = new TextBox { Width = 200 }, 20, 50);
            AddLabelAndControl(pnlInputs, "Username:", txtUsername = new TextBox { Width = 150 }, 240, 50);

            // Row 2: Password and Role
            AddLabelAndControl(pnlInputs, "Password:", txtPassword = new TextBox { Width = 150, UseSystemPasswordChar = true }, 20, 105);

            btnTogglePass = new Button { Text = "👁", Size = new Size(30, 26), Location = new Point(175, 124), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
            btnTogglePass.Click += (s, e) => {
                txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
                btnTogglePass.Text = txtPassword.UseSystemPasswordChar ? "👁" : "🔒";
            };
            pnlInputs.Controls.Add(btnTogglePass);

            AddLabelAndControl(pnlInputs, "Role:", cmbRole = new ComboBox { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList }, 240, 105);
            cmbRole.Items.AddRange(new string[] { "Admin", "Cashier" });
            cmbRole.SelectedIndex = 1;

            // Buttons
            btnAdd = CreateBtn("Add Staff", Color.SeaGreen, 420, 120);
            btnUpdate = CreateBtn("Update", Color.Orange, 530, 120);
            btnDelete = CreateBtn("Delete", Color.Crimson, 640, 120);
            btnClear = CreateBtn("Clear", Color.Gray, 750, 120);

            btnAdd.Click += BtnAdd_Click;
            btnUpdate.Click += BtnUpdate_Click;
            btnDelete.Click += BtnDelete_Click;
            btnClear.Click += (s, e) => ClearForm();

            pnlInputs.Controls.AddRange(new Control[] { lblTitle, btnAdd, btnUpdate, btnDelete, btnClear });
            this.Controls.Add(pnlInputs);

            // --- 2. GRID VIEW ---
            dgvStaff = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                ReadOnly = true,
                RowHeadersVisible = false
            };
            dgvStaff.CellClick += DgvStaff_CellClick;
            this.Controls.Add(dgvStaff);
            dgvStaff.BringToFront();
        }

        private void AddLabelAndControl(Panel p, string labelText, Control ctrl, int x, int y)
        {
            Label lbl = new Label { Text = labelText, Location = new Point(x, y), AutoSize = true, Font = new Font("Segoe UI", 9) };
            ctrl.Location = new Point(x, y + 20);
            p.Controls.Add(lbl);
            p.Controls.Add(ctrl);
        }

        private Button CreateBtn(string text, Color color, int x, int y)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(100, 35),
                Location = new Point(x, y),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        // --- DATABASE OPERATIONS (MOCKUP) ---

        private void LoadStaffData()
        {
            // SQL: SELECT id, full_name, username, role FROM users
            DataTable dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("Full Name");
            dt.Columns.Add("Username");
            dt.Columns.Add("Role");

            dt.Rows.Add(1, "System Admin", "admin", "Admin");
            dt.Rows.Add(2, "Jane Doe", "jane_cashier", "Cashier");

            dgvStaff.DataSource = dt;
        }

        private void DgvStaff_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = dgvStaff.Rows[e.RowIndex];
                selectedId = Convert.ToInt32(row.Cells["ID"].Value);
                txtFullName.Text = row.Cells["Full Name"].Value.ToString();
                txtUsername.Text = row.Cells["Username"].Value.ToString();
                cmbRole.SelectedItem = row.Cells["Role"].Value.ToString();
                txtPassword.Text = "********"; // Don't show real password in grid for security
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please fill in Username and Password.");
                return;
            }
            // Logic: Password Hashing -> SQL INSERT
            MessageBox.Show($"Staff member '{txtFullName.Text}' created successfully!");
            LoadStaffData();
            ClearForm();
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            MessageBox.Show("Staff information updated.");
            LoadStaffData();
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (selectedId == -1) return;
            if (MessageBox.Show("Are you sure you want to delete this user?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                LoadStaffData();
                ClearForm();
            }
        }

        private void ClearForm()
        {
            txtFullName.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            cmbRole.SelectedIndex = 1;
            selectedId = -1;
        }
    }
}