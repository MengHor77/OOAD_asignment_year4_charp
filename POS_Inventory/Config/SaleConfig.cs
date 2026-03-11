using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POS_Inventory.Config
{
    public class SaleConfig
    {
        private readonly string connectionString = "server=localhost;port=3306;username=root;password=;database=pos_db;SslMode=none;";

        public SaleConfig()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"CREATE TABLE IF NOT EXISTS sales(
                                id INT AUTO_INCREMENT PRIMARY KEY,
                                total DECIMAL(10,2),
                                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                              )";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public bool InsertSale(decimal total)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "INSERT INTO sales(total) VALUES(@total)";
                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@total", total);

                return cmd.ExecuteNonQuery() > 0;
            }
        }


        public decimal GetTotalAmountSaleToday()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COALESCE(SUM(total), 0) FROM sales WHERE DATE(created_at) = CURDATE()";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    return Convert.ToDecimal(cmd.ExecuteScalar());
                }
                catch { return 0; }
            }
        }

        public int GetTotalSaleCountToday()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM sales WHERE DATE(created_at) = CURDATE()";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
                catch { return 0; }
            }
        }

        // ✅ ADD THIS to SaleConfig.cs
        public DataTable GetSalesByDateRange(DateTime from, DateTime to)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    string query = @"SELECT id, total, created_at 
                             FROM sales 
                             WHERE created_at >= @from 
                             AND created_at <= @to 
                             ORDER BY created_at DESC";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@from", from);
                    cmd.Parameters.AddWithValue("@to", to);

                    MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GetSalesByDateRange Error: " + ex.Message);
                }
                return dt;
            }
        }

    }
}

    

