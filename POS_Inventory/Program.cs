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
                 var userConfig = new Config.UserConfig();

                 var categoryConfig = new Config.CategoryConfig();

                 var productConfig = new Config.ProductConfig();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Setup Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;  
            }

             Application.Run(new FormLogin());
        }
    }
}
