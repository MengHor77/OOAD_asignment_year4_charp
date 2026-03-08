using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POS_Inventory
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // --- Initialize database tables ---
            try
            {
                // Create Users table and default admin
                var userConfig = new Config.UserConfig();

                // Create Categories table and default "General"
                var categoryConfig = new Config.CategoryConfig();

                // Create Products table (depends on users and categories)
                var productConfig = new Config.ProductConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Setup Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Stop app if database setup fails
            }

            // --- Then run login form ---
            Application.Run(new FormLogin());
        }
    }
}
