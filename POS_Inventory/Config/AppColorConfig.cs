using System;
using System.Drawing;

namespace POS_Inventory.Config
{
    internal class AppColorConfig
    {
        // --- LOGIN & FORM COLORS ---
        public static Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        public static Color GradientStart = Color.FromArgb(52, 152, 219);
        public static Color GradientEnd = Color.FromArgb(142, 68, 173);
        public static Color LoginBtnText = Color.RoyalBlue;
        public static Color ErrorRed = Color.Red;
        public static Color White = Color.White;
        public static Color Transparent = Color.Transparent;
        public static Color PlaceholderWhite = Color.FromArgb(200, 255, 255, 255);

        // --- SIDEBAR & HEADER ---
        public static Color SidebarRed = Color.FromArgb(193, 119, 114);
        public static Color HeaderPink = Color.FromArgb(204, 155, 151);
        public static Color BrandBlue = Color.FromArgb(127, 185, 218);

        // --- BACKGROUNDS ---
        public static Color ContentBackground = Color.White;

        // --- DASHBOARD CARDS (Normal Background) ---
        public static Color CardStaff = Color.FromArgb(174, 192, 225);
        public static Color CardProduct = Color.FromArgb(112, 160, 255);
        public static Color CardPOSProduct = Color.FromArgb(123, 164, 224);
        public static Color CardAdmin = Color.FromArgb(127, 185, 218);          // ✅ NEW
        public static Color CardLowStock = Color.FromArgb(255, 249, 230);       // ✅ NEW

        // --- DASHBOARD CARDS (Hover Background) ---
        public static Color CardStaffHover = Color.FromArgb(140, 165, 205);     // ✅ NEW
        public static Color CardAdminHover = Color.FromArgb(90, 155, 195);      // ✅ NEW
        public static Color CardProductHover = Color.FromArgb(70, 125, 220);    // ✅ NEW
        public static Color CardLowStockHover = Color.FromArgb(255, 236, 153);  // ✅ NEW

        // --- LOW STOCK ALERT COLORS ---
        public static Color LowStockBorder = Color.FromArgb(255, 200, 0);       // ✅ NEW
        public static Color LowStockText = Color.FromArgb(180, 120, 0);         // ✅ NEW

        // --- BUTTONS ---
        public static Color BtnSave = Color.FromArgb(40, 167, 69);
        public static Color BtnCancel = Color.Gray;

        // --- GENERAL GRAYS ---
        public static Color Gray = Color.FromArgb(128, 128, 128);
        public static Color GrayLight = Color.FromArgb(235, 232, 232);
        public static Color GrayDark = Color.FromArgb(100, 100, 100);
        public static Color LightBlue = Color.FromArgb(50, 137, 230);

        // --- HOVER & SELECTION ---
        public static Color SidebarHover = ColorTranslator.FromHtml("#76BFEC");
        public static Color SidebarActive = Color.FromArgb(80, 255, 255, 255);

        // --- TEXT ---
        public static Color TextDark = Color.FromArgb(40, 40, 40);
        public static Color TextLight = Color.White;
    }
}