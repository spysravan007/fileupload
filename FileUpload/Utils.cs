using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload
{
    static class Shell32
    {
        private const int MaxPath = 256;
        [StructLayout(LayoutKind.Sequential)]
        public struct Shfileinfo
        {
            private const int Namesize = 80;
            public readonly IntPtr hIcon;
            private readonly int iIcon;
            private readonly uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxPath)]
            private readonly string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Namesize)]
            private readonly string szTypeName;
        };
        public const uint ShgfiIcon = 0x000000100;     // get icon
        public const uint ShgfiLinkoverlay = 0x000008000;     // put a link overlay on icon
        public const uint ShgfiLargeicon = 0x000000000;     // get large icon
        public const uint ShgfiSmallicon = 0x000000001;     // get small icon
        public const uint ShgfiUsefileattributes = 0x000000010;     // use passed dwFileAttribute
        public const uint FileAttributeNormal = 0x00000080;
        [DllImport("Shell32.dll")]
        public static extern IntPtr SHGetFileInfo(
            string pszPath,
            uint dwFileAttributes,
            ref Shfileinfo psfi,
            uint cbFileInfo,
            uint uFlags
            );
    }
    static class User32
    {
        /// <summary>
        /// Provides access to function required to delete handle. This method is used internally
        /// and is not required to be called separately.
        /// </summary>
        /// <param name="hIcon">Pointer to icon handle.</param>
        /// <returns>N/A</returns>
        [DllImport("User32.dll")]
        public static extern int DestroyIcon(IntPtr hIcon);
    }
}
