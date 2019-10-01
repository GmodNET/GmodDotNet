using System;
using System.IO;
using System.Threading;

namespace GmodDotNet
{
    //Entry point class of the GmodDotNet
    static class GmodDotNet
    {
        // Entry point of the GmodDotNet
        static void Main()
        {
            File.WriteAllText("GmodDotNetLog.txt", "GmodDotNet is loaded");
        }
    }
}
