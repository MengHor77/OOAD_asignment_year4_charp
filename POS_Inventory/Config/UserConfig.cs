using System;
using System.Data;
using MySql.Data.MySqlClient;
using BCrypt.Net;

namespace POS_Inventory.Config
{
    internal class UserConfig
    {
        // Consider moving this to App.config or a secure Settings file later
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

                    // 1. Create Table
                    string sql = @"CREATE TABLE IF NOT EXISTS users (
                                    id INT AUTO_INCREMENT PRIMARY KEY,
                                    username VARCHAR(50) NOT NULL UNIQUE,
                                    email VARCHAR(100),
                                    password VARCHAR(255) NOT NULL, 
                                    role VARCHAR(20) NOT NULL
                                  );";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // 2. Seed Admin if not exists
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
                catch (Exception ex)
                {
                    Console.WriteLine("Setup Error: " + ex.Message);
                }
            }
        }

        public DataTable ValidateLogin(string identifier, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    // Allows login via username OR email
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
                                // BCrypt verification
                                if (BCrypt.Net.BCrypt.Verify(password, dbHashedPassword))
                                {
                                    return tempDt;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Database Error: " + ex.Message);
                }

                return new DataTable(); // Return empty table if login fails
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

        public DataTable GetAllCashiers()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                conn.Open();
                string query = "SELECT id, username, email, role FROM users WHERE role = 'Cashier'";

                using (MySqlDataAdapter sda = new MySqlDataAdapter(query, conn))
                {
                    sda.Fill(dt);
                    return dt;
                }
            }
        }

        public bool UpdateCashier(int id, string username, string email, string password)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string hashedPass = BCrypt.Net.BCrypt.HashPassword(password);
                string query = "UPDATE users SET username=@user, email=@email, password=@pass WHERE id=@id AND role='Cashier'";

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

        public bool DeleteCashier(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM users WHERE id=@id AND role='Cashier'";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    return cmd.ExecuteNonQuery() > 0;
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
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
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
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
                catch { return 0; }
            }
        }
    }
}