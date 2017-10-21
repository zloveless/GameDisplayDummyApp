// -----------------------------------------------------------------------------
//  <copyright file="Program.cs" author="Zack Loveless">
//      Copyright (c) Zachary Loveless. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------------

namespace GameLauncherDummy
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using GameLauncherDummy.Resources;

    public class Program
    {
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool DestroyIcon(IntPtr handle);

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;

        private static IntPtr _ConsoleHandle;

        private static NotifyIcon _trayIcon;
        private static IntPtr _iconPtr;

        public static void Main(string[] args)
        {
            Console.Title = StringResources.ApplicationTitle;

            _iconPtr = ImageResources.GearIcon.GetHicon();
            _trayIcon = new NotifyIcon
                            {
                                Text = StringResources.ApplicationTitle,
                                Icon = Icon.FromHandle(_iconPtr)
                            };

            
            
            var trayMenu = new ContextMenu();
            
            trayMenu.MenuItems.Add("Exit", _ExitApplicationCallback);

            _trayIcon.DoubleClick += _ExitApplicationCallback;
            _trayIcon.ContextMenu = trayMenu;
            _trayIcon.Visible = true;

            _ConsoleHandle = GetConsoleWindow();

            ShowWindow(_ConsoleHandle, SW_HIDE);

            WriteLog("Starting application...");

            _trayIcon.ShowBalloonTip(timeout: 8, tipTitle: @"Minimized to tray", tipText: @"Dummy application minimized to tray. Right click the icon to exit.", tipIcon: ToolTipIcon.Info);

            Application.Run();
        }
        
        private static void _ExitApplicationCallback(object sender, EventArgs args)
        {
            var result = MessageBox.Show(@"Are you sure you want to exit?", @"Confirmation", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

            if (result != DialogResult.OK)
            {
                return;
            }

            WriteLog("Terminating application...");
            ShowWindow(_ConsoleHandle, SW_SHOW);

            _trayIcon.Dispose();

            DestroyIcon(_iconPtr);

            Application.Exit();
        }

        private static void WriteLog(string formatString, params object[] args)
        {
            string message = string.Format(formatString, args);

            ConsoleColor c = Console.ForegroundColor;
            Console.Write(DateTime.Now.ToString("[yyyy-mm-dd HH:nn:ss] "));
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = c;
        }
    }
}
