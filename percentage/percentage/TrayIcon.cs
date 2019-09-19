using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Forms;

namespace Percentage
{
    class TrayIcon : IDisposable
    {
        private const string _iconFont = "Tahoma";

        private readonly NotifyIcon _percentageIcon = new NotifyIcon();
        private readonly Timer _timer = new Timer();

        public TrayIcon()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();

            // initialize contextMenu
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem });

            // initialize menuItem
            menuItem.Index = 0;
            menuItem.Text = "E&xit";
            menuItem.Click += new EventHandler(MenuItemClick);

            _percentageIcon.ContextMenu = contextMenu;
            _percentageIcon.Visible = true;

            _timer.Tick += new EventHandler(TimerTick);
            _timer.Interval = 500;
            _timer.Start();
        }

        private void TimerTick(object sender, EventArgs e)
        {
            PowerStatus powerStatus = SystemInformation.PowerStatus;
            int battery = (int)(powerStatus.BatteryLifePercent * 100);
            string batteryText = battery.ToString();

            int iconFontSize;
            Point point;
            if (battery == 100)
            {
                iconFontSize = 11;
                point = new Point(-4, 1);
            }
            else if (battery >= 10)
            {
                iconFontSize = 14;
                point = new Point(-2, -1);
            }
            else
            {
                iconFontSize = 14;
                point = new Point(1, -1);
            }

            using (Font font = new Font(_iconFont, iconFontSize, GraphicsUnit.Pixel))
            using (Bitmap bitmap = new Bitmap(DrawText(batteryText, font, point)))
            {
                IntPtr intPtr = bitmap.GetHicon();
                using (Icon icon = Icon.FromHandle(intPtr))
                {
                    _percentageIcon.Icon = icon;
                    _percentageIcon.Text = $"{battery}%";
                }
            }
        }

        private void MenuItemClick(object sender, EventArgs e)
        {
            _percentageIcon.Visible = false;
            Application.Exit();
        }

        private Image DrawText(string text, Font font, Point point)
        {
            Image image = new Bitmap(16, 16);
            using (Brush brush = new SolidBrush(Color.White))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                graphics.Clear(Color.Transparent);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
                graphics.DrawString(text, font, brush, point);
                graphics.Save();
            }

            return image;
        }

        public void Dispose()
        {
            _percentageIcon.Dispose();
            _timer.Dispose();
        }
    }
}
