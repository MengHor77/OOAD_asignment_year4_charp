using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

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
    }
}
