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
        private UserConfig userConfig;
        private Panel pnlTableContainer;
        private Panel pnlPagination;
        private Panel pnlSearch;
        private TextBox txtSearch;

        public StaffPage()
        {
            userConfig = new UserConfig();
            SetupLayout();
            LoadStaffData();
        }

        private void SetupLayout()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = AppColorConfig.ContentBackground;
            this.Padding = new Padding(20);

            // --- 1. Header Title ---
            Label lblTitle = new Label
            {
                Text = "📊 Staff Management",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = AppColorConfig.TextDark,
                Location = new Point(20, 20),
                AutoSize = true
            };

            // --- 2. Search Bar (Pill Style) ---
            pnlSearch = new Panel
            {
                Size = new Size(350, 45),
                Location = new Point(20, 70),
                BackColor = Color.FromArgb(180, 200, 235) // Muted blue from reference
            };

            txtSearch = new TextBox
            {
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(180, 200, 235),
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.FromArgb(60, 60, 60),
                Text = "Search by name, role",
                Width = 300,
                Location = new Point(15, 12)
            };

            // Placeholder Logic
            txtSearch.Enter += (s, e) => { if (txtSearch.Text == "Search by name, role") txtSearch.Text = ""; };
            txtSearch.Leave += (s, e) => { if (string.IsNullOrWhiteSpace(txtSearch.Text)) txtSearch.Text = "Search by name, role"; };
            txtSearch.TextChanged += TxtSearch_TextChanged;

            pnlSearch.Controls.Add(txtSearch);

            // --- 3. Table Container ---
            pnlTableContainer = new Panel
            {
                Location = new Point(20, 130),
                Size = new Size(this.Width - 40, 400),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.Transparent
            };

            // --- 4. DataGridView ---
            dgvStaff = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = AppColorConfig.White,
                BorderStyle = BorderStyle.None,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                RowHeadersVisible = false,
                ReadOnly = true,
                GridColor = Color.FromArgb(174, 214, 241),
                RowTemplate = { Height = 40 },
                ColumnHeadersHeight = 45,
                EnableHeadersVisualStyles = false
            };

            // Header Style
            dgvStaff.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.CardProduct,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Alignment = DataGridViewContentAlignment.MiddleCenter
            };

            // Cell Style
            dgvStaff.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = AppColorConfig.White,
                ForeColor = AppColorConfig.TextDark,
                SelectionBackColor = Color.FromArgb(230, 240, 255),
                SelectionForeColor = Color.Black,
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Padding = new Padding(5)
            };

            dgvStaff.CellContentClick += DgvStaff_CellContentClick;

            // --- 5. Pagination Panel ---
            pnlPagination = new Panel
            {
                Size = new Size(210, 50),
                BackColor = AppColorConfig.White,
                Location = new Point(pnlTableContainer.Right - 200, pnlTableContainer.Bottom + 10),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            AddPaginationButtons();

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(pnlSearch);
            this.Controls.Add(pnlTableContainer);
            pnlTableContainer.Controls.Add(dgvStaff);
            this.Controls.Add(pnlPagination);
        }

        private void AddPaginationButtons()
        {
            string[] buttons = { "<", "1", "2", "3", ">" };
            int x = 5;
            foreach (var b in buttons)
            {
                Button btn = new Button
                {
                    Text = b,
                    Size = new Size(35, 35),
                    Location = new Point(x, 7),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = (b == "2") ? Color.Orange : AppColorConfig.White,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                btn.FlatAppearance.BorderColor = Color.LightGray;
                pnlPagination.Controls.Add(btn);
                x += 40;
            }
        }

        private void LoadStaffData()
        {
            DataTable dt = userConfig.GetAllUsers();
            dgvStaff.DataSource = dt;

            // Remove existing Action columns to prevent duplicates
            if (dgvStaff.Columns.Contains("Edit")) dgvStaff.Columns.Remove("Edit");
            if (dgvStaff.Columns.Contains("Delete")) dgvStaff.Columns.Remove("Delete");

            // Renaming
            if (dgvStaff.Columns.Contains("id")) dgvStaff.Columns["id"].HeaderText = "No";
            if (dgvStaff.Columns.Contains("username")) dgvStaff.Columns["username"].HeaderText = "Staff Name";
            if (dgvStaff.Columns.Contains("role")) dgvStaff.Columns["role"].HeaderText = "Role";
            if (dgvStaff.Columns.Contains("status")) dgvStaff.Columns["status"].HeaderText = "Status";
            if (dgvStaff.Columns.Contains("created_at")) dgvStaff.Columns["created_at"].HeaderText = "Created At";

            // Hide Sensitive Info
            if (dgvStaff.Columns.Contains("email")) dgvStaff.Columns["email"].Visible = false;
            if (dgvStaff.Columns.Contains("password")) dgvStaff.Columns["password"].Visible = false;

            // Add Action Buttons
            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn
            {
                Name = "Edit",
                HeaderText = "action",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 60
            };
            btnEdit.DefaultCellStyle.BackColor = AppColorConfig.CardStaff;
            btnEdit.DefaultCellStyle.ForeColor = AppColorConfig.TextDark;
            dgvStaff.Columns.Add(btnEdit);

            DataGridViewButtonColumn btnDelete = new DataGridViewButtonColumn
            {
                Name = "Delete",
                HeaderText = "action",
                Text = "delete",
                UseColumnTextForButtonValue = true,
                FlatStyle = FlatStyle.Flat,
                Width = 60
            };
            btnDelete.DefaultCellStyle.BackColor = AppColorConfig.HeaderPink;
            btnDelete.DefaultCellStyle.ForeColor = AppColorConfig.TextDark;
            dgvStaff.Columns.Add(btnDelete);
        }

        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            if (dgvStaff.DataSource is DataTable dt)
            {
                string filterText = txtSearch.Text.Trim().Replace("'", "''");
                if (filterText != "Search by name, role" && !string.IsNullOrWhiteSpace(filterText))
                {
                    dt.DefaultView.RowFilter = string.Format("username LIKE '%{0}%' OR role LIKE '%{0}%'", filterText);
                }
                else
                {
                    dt.DefaultView.RowFilter = "";
                }
            }
        }

        private void DgvStaff_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ignore clicks on header rows or empty space
            if (e.RowIndex < 0) return;

            // 1. Extract data directly from the selected row
            // Note: Make sure these column names match exactly what is in your database/DataTable
            int userId = Convert.ToInt32(dgvStaff.Rows[e.RowIndex].Cells["id"].Value);
            string username = dgvStaff.Rows[e.RowIndex].Cells["username"].Value?.ToString() ?? "";
            string email = dgvStaff.Rows[e.RowIndex].Cells["email"].Value?.ToString() ?? "";
            string role = dgvStaff.Rows[e.RowIndex].Cells["role"].Value?.ToString() ?? "";

            // --- Edit Logic ---
            if (dgvStaff.Columns[e.ColumnIndex].Name == "Edit")
            {
                // We pass the data we already have into the Form constructor
                // This prevents the "GetUserById" error since we don't need to call the database again
                using (StaffEditForm editForm = new StaffEditForm(userConfig, userId, username, email))
                {
                    if (editForm.ShowDialog() == DialogResult.OK)
                    {
                        LoadStaffData(); // Refresh table to show changes
                        MessageBox.Show("Staff member updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            // --- Delete Logic ---
            else if (dgvStaff.Columns[e.ColumnIndex].Name == "Delete")
            {
                // Security check: Prevent accidental deletion of Admin via UI
                if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Security Protection: Admin accounts cannot be deleted.", "System", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (MessageBox.Show("Are you sure you want to delete this staff member?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Uses your existing DeleteUser(id) method from UserConfig
                    if (userConfig.DeleteUser(userId))
                    {
                        LoadStaffData();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete user. The account may be protected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}