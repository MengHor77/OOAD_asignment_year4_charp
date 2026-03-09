using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace POS_Inventory.Config
{
    public class ProductConfig
    {
        private readonly string connectionString = "server=localhost;port=3306;username=root;password=;database=pos_db;SslMode=none;ConnectionTimeout=30;";

        public ProductConfig()
        {
            CreateProductTableIfNotExists();
        }

        private void CreateProductTableIfNotExists()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = @"CREATE TABLE IF NOT EXISTS products (
                                    id INT AUTO_INCREMENT PRIMARY KEY,
                                    product_name VARCHAR(100) NOT NULL,
                                    category_id INT,
                                    price DECIMAL(10,2) NOT NULL,
                                    stock_qty INT DEFAULT 0,
                                    created_by_user_id INT,
                                    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
                                  ) ENGINE=InnoDB;";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                 //   Console.WriteLine("Products table created or already exists.");
                }
                catch (Exception ex)
                {
                    // Show real error in MessageBox if WinForms, or console
                    Console.WriteLine("Product Setup Error: " + ex.Message);
                    throw; // rethrow to catch in debugger
                }
            }
        }

        public bool CreateProduct(string name, int categoryId, decimal price, int stock, int userId)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"INSERT INTO products
                                    (product_name, category_id, price, stock_qty, created_by_user_id)
                                    VALUES
                                    (@name, @catId, @price, @stock, @userId)";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@catId", categoryId);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@stock", stock);
                    cmd.Parameters.AddWithValue("@userId", userId);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Create Product Error: " + ex.Message);
                    return false;
                }
            }
        }

        public DataTable GetAllProducts()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();

                try
                {
                    conn.Open();

                    // We JOIN products (p) with categories (c) to get the name instead of just the ID
                    string query = @"SELECT 
                                p.id, 
                                p.product_name, 
                                c.category_name, 
                                p.price, 
                                p.stock_qty,
                                p.category_id
                             FROM products p
                             INNER JOIN categories c ON p.category_id = c.id
                             ORDER BY p.id DESC";

                    MySqlDataAdapter sda = new MySqlDataAdapter(query, conn);
                    sda.Fill(dt);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GetAllProducts Error: " + ex.Message);
                }

                return dt;
            }
        }

        public DataRow GetProductById(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();

                    string query = "SELECT * FROM products WHERE id=@id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                    sda.Fill(dt);

                    if (dt.Rows.Count > 0) return dt.Rows[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine("GetProductById Error: " + ex.Message);
                }
                return null;
            }
        }

        public bool UpdateProduct(int id, string name, int categoryId, decimal price, int stock)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = @"UPDATE products
                                     SET product_name=@name,
                                         category_id=@catId,
                                         price=@price,
                                         stock_qty=@stock
                                     WHERE id=@id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@catId", categoryId);
                    cmd.Parameters.AddWithValue("@price", price);
                    cmd.Parameters.AddWithValue("@stock", stock);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("UpdateProduct Error: " + ex.Message);
                    return false;
                }
            }
        }

        public bool DeleteProduct(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "DELETE FROM products WHERE id=@id";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("DeleteProduct Error: " + ex.Message);
                    return false;
                }
            }
        }
    }
}