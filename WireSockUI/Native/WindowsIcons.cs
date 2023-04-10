﻿using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace WireSockUI.Native
{
    /// <summary>
    /// Native API class to retrieve specific size icons from icongroups in Windows resources
    /// </summary>
    /// 


    internal class WindowsIcons
    {
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        private static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)] string lpFileName);

        [DllImport("kernel32")]
        private static extern IntPtr FindResource(IntPtr hModule, int lpName, int lpType);

        [DllImport("kernel32", SetLastError = true)]
        private static extern IntPtr LoadResource(IntPtr hModule, IntPtr hResInfo);

        [DllImport("kernel32")]
        private static extern IntPtr LockResource(IntPtr hResData);

        [DllImport("user32")]
        private static extern int LookupIconIdFromDirectoryEx(byte[] presbits, bool fIcon, int cxDesired, int cyDesired, uint Flags);

        [DllImport("user32")]
        private static extern IntPtr CreateIconFromResourceEx(byte[] pbIconBits, uint cbIconBits, bool fIcon, uint dwVersion, int cxDesired, int cyDesired, uint uFlags);

        [DllImport("kernel32", SetLastError = true)]
        private static extern uint SizeofResource(IntPtr hModule, IntPtr hResInfo);

        const int RT_GROUP_ICON = 14;
        const int RT_ICON = 0x00000003;

        /*
         * Icons of interest
         *  New: 2
         *  Open: 3
         *  Disk Save: 28
         *  Disk Delete: 31
         *  Disk network: 33
         *  Delete: 89
         *  Green Shield: 106
         *  Yellow Shield: 107
         *  Settings: 114
         *  Icon Archive: 174
         *  Disk Lock Open: 1030
         *  Disk Lock Closed: 1031
         *  Disk Lock Warning: 1032
         */
        public enum Icons
        {
            Addtunnel = 33,
            NewTunnel = 2,
            OpenTunnel = 3,
            DeleteTunnel = 31,
            InactiveState = 32,
            ActiveState = 33,
            Settings = 114,
            ConnectedTunnel = 1031,
            DisconnectedTunnel = 1030,
            ConnectingTunnel = 1032,
            ProcessList = 150,
            Refresh = 1401,
            Activated = 106
        }

        private static Icon GetIconFromGroup(string file, int groupId, int size)
        {
            IntPtr hLibrary = LoadLibrary(file);

            if (hLibrary != IntPtr.Zero)
            {
                IntPtr hResource = FindResource(hLibrary, groupId, RT_GROUP_ICON);

                IntPtr hMem = LoadResource(hLibrary, hResource);

                IntPtr lpResourcePtr = LockResource(hMem);
                uint sz = SizeofResource(hLibrary, hResource);
                byte[] lpResource = new byte[sz];
                Marshal.Copy(lpResourcePtr, lpResource, 0, (int)sz);

                int nID = LookupIconIdFromDirectoryEx(lpResource, true, size, size, 0x0000);

                hResource = FindResource(hLibrary, nID, RT_ICON);

                hMem = LoadResource(hLibrary, hResource);

                lpResourcePtr = LockResource(hMem);
                sz = SizeofResource(hLibrary, hResource);
                lpResource = new byte[sz];
                Marshal.Copy(lpResourcePtr, lpResource, 0, (int)sz);

                IntPtr hIcon = CreateIconFromResourceEx(lpResource, sz, true, 0x00030000, size, size, 0);

                return Icon.FromHandle(hIcon);
            }

            return null;
        }

        /// <summary>
        /// Get specific size icon (in pixels) from the Icons enum
        /// </summary>
        /// <param name="icon"><see cref="Icons"/></param>
        /// <param name="size">Icon size/width in pixels</param>
        /// <returns><see cref="Icon"/> or null</returns>
        /// <exception cref="FileNotFoundException">Windows ImageRes resource could not be located.</exception>
        public static Icon GetWindowsIcon(Icons icon, int size)
        {
            // Windows 11
            String library = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "SystemResources", "imageres.dll.mun");

            if (!File.Exists(library))
            {
                library = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "imageres.dll");
            }

            if (!File.Exists(library))
                throw new FileNotFoundException("Unable to locate imageres.dll for Windows Icons");

            return GetIconFromGroup(library, (int)icon, size);
        }
    }
}