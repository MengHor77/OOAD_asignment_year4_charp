using System;
using System.Data;
using MySql.Data.MySqlClient;
using BCrypt.Net;

namespace POS_Inventory.Config
{
    public class UserConfig
    {
        private readonly string connectionString = "server=localhost;port=3306;username=root;password=;database=pos_db;SslMode=none;ConnectionTimeout=30;";

        public UserConfig()
        {
            CreateUserTableIfNotExists();
        }

        private void CreateUserTableIfNotExists()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string sql = @"CREATE TABLE IF NOT EXISTS users (
                                    id INT AUTO_INCREMENT PRIMARY KEY,
                                    username VARCHAR(50) NOT NULL UNIQUE,
                                    email VARCHAR(100),
                                    password VARCHAR(255) NOT NULL, 
                                    role VARCHAR(20) NOT NULL,
                                    status VARCHAR(20) DEFAULT 'Active',
                                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                                  );";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string checkAdmin = "SELECT COUNT(*) FROM users WHERE username=@username";
                    using (MySqlCommand checkCmd = new MySqlCommand(checkAdmin, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@username", "admin");
                        if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                        {
                            string hashedPass = BCrypt.Net.BCrypt.HashPassword("admin123");
                            string seedSql = "INSERT INTO users (username, email, password, role) VALUES ('admin', 'admin@gmail.com', @pass, 'Admin');";
                            using (MySqlCommand seedCmd = new MySqlCommand(seedSql, conn))
                            {
                                seedCmd.Parameters.AddWithValue("@pass", hashedPass);
                                seedCmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Setup Error: " + ex.Message); }
            }
        }

        public DataTable ValidateLogin(string identifier, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, username, email, password, role FROM users WHERE username=@id OR email=@id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", identifier);
                        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                        {
                            DataTable tempDt = new DataTable();
                            sda.Fill(tempDt);
                            if (tempDt.Rows.Count > 0)
                            {
                                string dbHashedPassword = tempDt.Rows[0]["password"].ToString();
                                if (BCrypt.Net.BCrypt.Verify(password, dbHashedPassword)) return tempDt;
                            }
                        }
                    }
                }
                catch (Exception ex) { throw new Exception("Database Error: " + ex.Message); }
                return new DataTable();
            }
        }

        public bool CreateCashier(string username, string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string hashedPass = BCrypt.Net.BCrypt.HashPassword(password);
                string query = "INSERT INTO users (username, email, password, role) VALUES (@user, @email, @pass, 'Cashier')";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@pass", hashedPass);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public DataTable GetAllUsers()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    string query = "SELECT id, username, email, created_at, role, status FROM users ORDER BY id DESC";
                    using (MySqlDataAdapter sda = new MySqlDataAdapter(query, conn))
                    {
                        sda.Fill(dt);
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                return dt;
            }
        }

        public DataTable GetAllCashiers()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                conn.Open();
                string query = "SELECT id, username, email, created_at, role, status FROM users WHERE role = 'Cashier'";
                using (MySqlDataAdapter sda = new MySqlDataAdapter(query, conn))
                {
                    sda.Fill(dt);
                    return dt;
                }
            }
        }

        // FIX: Removed role restriction so Admin can be updated
        public bool UpdateUser(int id, string username, string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string hashedPass = BCrypt.Net.BCrypt.HashPassword(password);
                // Updated query to allow updating any ID regardless of role
                string query = "UPDATE users SET username=@user, email=@email, password=@pass WHERE id=@id";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@user", username);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@pass", hashedPass);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // FIX: Added protection so Admin role cannot be deleted
        public bool DeleteUser(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                // This query only deletes if the user exists AND their role is NOT 'Admin'
                string query = "DELETE FROM users WHERE id=@id AND role != 'Admin'";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected == 0)
                    {
                        // Optional: You could throw an error or return false to tell the UI 
                        // that Admin accounts are protected.
                        return false;
                    }
                    return true;
                }
            }
        }

        public int GetTotalCashier()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE role='Cashier'";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn)) return Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch { return 0; }
            }
        }

        public int GetTotalAdmin()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM users WHERE role='Admin'";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn)) return Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch { return 0; }
            }
        }

        // 1. Add this to get data for a specific user to fill the Edit Form
        public DataTable GetUserById(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    string query = "SELECT id, username, email, role, status FROM users WHERE id=@id";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (MySqlDataAdapter sda = new MySqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                    }
                }
                catch (Exception ex) { Console.WriteLine("Error: " + ex.Message); }
                return dt;
            }
        }

        
        }


}
