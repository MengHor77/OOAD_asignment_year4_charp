using System;
using System.Drawing; 
using System.Windows.Forms;

namespace POS_Inventory.Form.AdminForm
{
    internal class SidebarItem
    {
        public string Title { get; set; }
        public string IconText { get; set; }
        public Image Icon { get; set; } 
        public System.Windows.Forms.Form TargetForm { get; set; }
    }
}