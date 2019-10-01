using System;
using System.IO;

namespace GmodNET
{
    public delegate void MainDelegate();
    public static class Startup
    {
        public static void Main()
        {
           File.WriteAllText("ManagedGeneratedFile.txt", "Managed code says hi!");
        }
    }
}
