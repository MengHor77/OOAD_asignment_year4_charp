using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace POS_Inventory.Component
{
    public class Pagination
    {
        private Panel pnlPagination;
        private int currentPage;
        private int pageSize;
        private int totalPages;
        private Action<int> onPageChanged; // callback to load page data

        public Pagination(Panel panel, int pageSize, Action<int> onPageChanged)
        {
            this.pnlPagination = panel;
            this.pageSize = pageSize;
            this.onPageChanged = onPageChanged;
            this.currentPage = 1;
        }

        // Call this whenever you update your data
        public void Bind(DataTable dt)
        {
            int totalRows = dt.Rows.Count;
            totalPages = (int)Math.Ceiling(totalRows / (double)pageSize);
            RenderButtons();
        }

        private void RenderButtons()
        {
            pnlPagination.Controls.Clear();
            int x = 5;

            // Previous
            Button btnPrev = new Button
            {
                Text = "<",
                Size = new Size(35, 35),
                Location = new Point(x, 7),
                FlatStyle = FlatStyle.Flat,
                BackColor = (currentPage == 1) ? Color.LightGray : Color.White,
                Enabled = currentPage > 1
            };
            btnPrev.Click += (s, e) => { currentPage--; PageChanged(); };
            pnlPagination.Controls.Add(btnPrev);
            x += 40;

            // Page numbers
            for (int i = 1; i <= totalPages; i++)
            {
                Button btnPage = new Button
                {
                    Text = i.ToString(),
                    Size = new Size(35, 35),
                    Location = new Point(x, 7),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = (i == currentPage) ? Color.Orange : Color.White
                };
                int page = i;
                btnPage.Click += (s, e) => { currentPage = page; PageChanged(); };
                pnlPagination.Controls.Add(btnPage);
                x += 40;
            }

            // Next
            Button btnNext = new Button
            {
                Text = ">",
                Size = new Size(35, 35),
                Location = new Point(x, 7),
                FlatStyle = FlatStyle.Flat,
                BackColor = (currentPage == totalPages) ? Color.LightGray : Color.White,
                Enabled = currentPage < totalPages
            };
            btnNext.Click += (s, e) => { currentPage++; PageChanged(); };
            pnlPagination.Controls.Add(btnNext);
        }

        private void PageChanged()
        {
            onPageChanged?.Invoke(currentPage);
            RenderButtons(); // refresh buttons
        }

        public int GetCurrentPage() => currentPage;
        public int GetPageSize() => pageSize;
    }
}
