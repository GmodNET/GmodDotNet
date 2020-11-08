using System.Runtime.InteropServices;

namespace GmodNET.SourceSDK
{
    public static class Tier0
    {
        /// <summary>
        /// Prints white colored message to game console
        /// </summary>
        /// <param name="message">Text to print</param>
        [DllImport("tier0", CallingConvention = CallingConvention.Cdecl)]
        public static extern void Msg([MarshalAs(UnmanagedType.LPStr)] string message);
    }
}
