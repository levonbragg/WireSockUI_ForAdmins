using Microsoft.Win32;
using System;
using System.Security.Principal;
using System.Threading;
using System.Windows.Forms;

namespace WireSockUI
{
    internal static class Global
    {
        public static string MainFolder =
            $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\WireSockUI";

        public static string ConfigsFolder = MainFolder + "\\Configs";
        public static Mutex AlreadyRunning;
        public static string RegistryKey = @"SOFTWARE\WireSockUI";

        public static int LimitNonAdmins;
        public static int AutoConnect;
        public static int AutoRun;
        public static int AutoMinimize;
        public static int AutoUpdate;
        public static int LogLevel;
        public static int UseAdapter;


        public static bool IsCurrentProcessElevated()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

    }

    
}