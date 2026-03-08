using System;
using System.Drawing;

namespace POS_Inventory.Config
{
    internal class AppColorConfig
    {
        // --- YOUR OLD COLORS (Preserved) ---
        public static Color PrimaryBlue = Color.FromArgb(41, 128, 185);
        public static Color GradientStart = Color.FromArgb(52, 152, 219);
        public static Color GradientEnd = Color.FromArgb(142, 68, 173);
        public static Color LoginBtnText = Color.RoyalBlue;
        public static Color ErrorRed = Color.Red;
        public static Color White = Color.White;
        public static Color Transparent = Color.Transparent;
        public static Color PlaceholderWhite = Color.FromArgb(200, 255, 255, 255);

        // --- NEW DASHBOARD THEME (From your Screenshot) ---

        // Sidebar & Header
        public static Color SidebarRed = Color.FromArgb(193, 119, 114);    // Muted Red-Brown
        public static Color HeaderPink = Color.FromArgb(204, 155, 151);     // Soft Pink-Grey
        public static Color BrandBlue = Color.FromArgb(127, 185, 218);      // Light Blue Logo Box

        // Backgrounds
        public static Color ContentBackground = Color.FromArgb(87, 153, 196); // Ocean Blue

        // Dashboard Cards
        public static Color CardStaff = Color.FromArgb(174, 192, 225);     // Light Blue Card
        public static Color CardProduct = Color.FromArgb(112, 160, 255);   // Vibrant Blue Card

        // Hover & Selection
        public static Color SidebarHover = ColorTranslator.FromHtml("#76BFEC");// Semi-transparent Dark
        public static Color SidebarActive = Color.FromArgb(80, 255, 255, 255); // Semi-transparent White

        // Text Colors
        public static Color TextDark = Color.FromArgb(40, 40, 40);         // Nearly Black
        public static Color TextLight = Color.White;
    }
}