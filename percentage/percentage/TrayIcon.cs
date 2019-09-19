using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace percentage
{
    class TrayIcon
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern bool DestroyIcon(IntPtr handle);

        private const string _iconFont = "Segoe UI";
        private const int _iconFontSize = 28;

        private string _batteryPercentage;
        private NotifyIcon _notifyIcon;

        public TrayIcon()
        {
            ContextMenu contextMenu = new ContextMenu();
            MenuItem menuItem = new MenuItem();

            _notifyIcon = new NotifyIcon();

            // initialize contextMenu
            contextMenu.MenuItems.AddRange(new MenuItem[] { menuItem });

            // initialize menuItem
            menuItem.Index = 0;
            menuItem.Text = "E&xit";
            menuItem.Click += new System.EventHandler(menuItem_Click);

            _notifyIcon.ContextMenu = contextMenu;

            _batteryPercentage = "?";

            _notifyIcon.Visible = true;

            Timer timer = new Timer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = 1000; // in miliseconds
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            PowerStatus powerStatus = SystemInformation.PowerStatus;
            _batteryPercentage = $"{(int)(powerStatus.BatteryLifePercent * 100)}";

            using (Font font = new Font(_iconFont, _iconFontSize))
            using (Bitmap bitmap = new Bitmap(DrawText(_batteryPercentage, font)))
            {
                IntPtr intPtr = bitmap.GetHicon();
                try
                {
                    using (Icon icon = Icon.FromHandle(intPtr))
                    {
                        _notifyIcon.Icon = icon;
                        _notifyIcon.Text = _batteryPercentage;
                    }
                }
                finally
                {
                    DestroyIcon(intPtr);
                }
            }
        }

        private void menuItem_Click(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            Application.Exit();
        }

        private Image DrawText(string text, Font font)
        {
            SizeF textSize = GetImageSize(text, font);
            Image image = new Bitmap((int) textSize.Width, (int) textSize.Height);
            using (Brush brush = new SolidBrush(Color.White))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                // paint the background
                graphics.Clear(Color.Transparent);
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.TextRenderingHint = TextRenderingHint.SystemDefault;
                graphics.DrawString(text, font, brush, 0, 0);
                graphics.Save();
            }

            return image;
        }

        private static SizeF GetImageSize(string text, Font font)
        {
            using (Image image = new Bitmap(1, 1))
            using (Graphics graphics = Graphics.FromImage(image))
            {
                return graphics.MeasureString(text, font);
            }
        }
    }
}
