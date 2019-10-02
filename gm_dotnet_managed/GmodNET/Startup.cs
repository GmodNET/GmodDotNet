using System;
using System.IO;

namespace GmodNET
{
    internal delegate void MainDelegate();
    internal static class Startup
    {
        internal static void Main()
        {
           File.WriteAllText("ManagedGeneratedFile.txt", "Managed code says hi!");
        }
    }
}
