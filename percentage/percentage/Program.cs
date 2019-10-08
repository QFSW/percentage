using System;
using System.Windows.Forms;

namespace Percentage
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using TrayIcon trayIcon = new TrayIcon();
            Application.Run();
        }
    }
}
