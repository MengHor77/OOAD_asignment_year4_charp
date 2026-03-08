using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Windows.Forms; // For MessageBox if running WinForms

namespace POS_Inventory.Config
{
    internal class CategoryConfig
    {
        private readonly string connectionString = "server=localhost;port=3306;username=root;password=;database=pos_db;SslMode=none;ConnectionTimeout=30;";

        public CategoryConfig()
        {
            CreateCategoryTableIfNotExists();
        }

        private void CreateCategoryTableIfNotExists()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string sql = @"CREATE TABLE IF NOT EXISTS categories (
                                    id INT AUTO_INCREMENT PRIMARY KEY,
                                    category_name VARCHAR(100) NOT NULL UNIQUE,
                                    description TEXT
                                  ) ENGINE=InnoDB;";

                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    cmd.ExecuteNonQuery();

                    // Seed default category if table is empty
                    string checkEmpty = "SELECT COUNT(*) FROM categories";
                    MySqlCommand checkCmd = new MySqlCommand(checkEmpty, conn);

                    if (Convert.ToInt32(checkCmd.ExecuteScalar()) == 0)
                    {
                        string seedSql = @"INSERT INTO categories 
                                           (category_name, description) 
                                           VALUES ('General', 'Default category');";

                        MySqlCommand seedCmd = new MySqlCommand(seedSql, conn);
                        seedCmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Categories table created or already exists.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Show error in a message box for WinForms
                    MessageBox.Show("Category Setup Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw; // rethrow to see it in debugger
                }
            }
        }

        // CREATE CATEGORY
        public bool CreateCategory(string name, string description)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO categories (category_name, description) VALUES (@name, @desc)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@desc", description);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Create Category Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        // READ ALL CATEGORIES
        public DataTable GetAllCategories()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM categories ORDER BY id DESC";
                    MySqlDataAdapter sda = new MySqlDataAdapter(query, conn);
                    sda.Fill(dt);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching categories:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return dt;
            }
        }

        // GET CATEGORY BY ID
        public DataRow GetCategoryById(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                DataTable dt = new DataTable();
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM categories WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    MySqlDataAdapter sda = new MySqlDataAdapter(cmd);
                    sda.Fill(dt);

                    if (dt.Rows.Count > 0)
                        return dt.Rows[0];
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Get Category Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return null;
            }
        }

        // UPDATE CATEGORY
        public bool UpdateCategory(int id, string name, string description)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "UPDATE categories SET category_name=@name, description=@desc WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@desc", description);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Update Category Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }

        // DELETE CATEGORY
        public bool DeleteCategory(int id)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Check if category is used by products
                    string checkQuery = "SELECT COUNT(*) FROM products WHERE category_id=@id";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@id", id);

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (count > 0)
                    {
                        MessageBox.Show("Cannot delete category: used by products.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }

                    string query = "DELETE FROM categories WHERE id=@id";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@id", id);

                    return cmd.ExecuteNonQuery() > 0;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Delete Category Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
        }
    }
}