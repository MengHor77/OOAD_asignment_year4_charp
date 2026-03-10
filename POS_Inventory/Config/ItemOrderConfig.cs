using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

namespace POS_Inventory.Config
{
    public class ItemOrderConfig
    {
        private readonly string connectionString = "server=localhost;port=3306;username=root;password=;database=pos_db;SslMode=none;";

        public ItemOrderConfig()
        {
            CreateTable();
        }

        private void CreateTable()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"CREATE TABLE IF NOT EXISTS item_orders(
                                id INT AUTO_INCREMENT PRIMARY KEY,
                                product_name VARCHAR(100),
                                price DECIMAL(10,2),
                                qty INT,
                                created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                              )";

                MySqlCommand cmd = new MySqlCommand(sql, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public bool InsertItem(string name, decimal price, int qty)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO item_orders(product_name,price,qty)
                                 VALUES(@name,@price,@qty)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@qty", qty);

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public DataTable GetItems()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();

                conn.Open();
                string query = "SELECT * FROM item_orders";

                MySqlDataAdapter da = new MySqlDataAdapter(query, conn);
                da.Fill(dt);

                return dt;
            }
        }

        public void ClearItems()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                string query = "DELETE FROM item_orders";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateQty(string name, int qty)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = @"UPDATE item_orders 
                         SET qty=@qty 
                         WHERE product_name=@name";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@qty", qty);
                cmd.Parameters.AddWithValue("@name", name);

                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteItem(string name)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();

                string query = "DELETE FROM item_orders WHERE product_name=@name";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                cmd.Parameters.AddWithValue("@name", name);

                cmd.ExecuteNonQuery();
            }
        }



    }
}
