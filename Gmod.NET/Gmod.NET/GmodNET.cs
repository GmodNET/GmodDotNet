/* GmodNET.Main() is an entry point of the managed code of the GmodDotNet. Main() will be called by native module as soon as it
 * will successfully init mono domain, load assembly and image.
 * 
 * Authors: Gleb Krasilich
 * 2018*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GmodNET.Math;

namespace GmodNET
{
    static class GmodNET
    {
        //Print to console function
        static void Print(string msg)
        {
            lock (Lua.Api)
            {
                Lua.Api.PushSpecial(Lua.SpecialTables.Global);
                Lua.Api.GetField(-1, "print");
                Lua.Api.PushString(msg);
                Lua.Api.Call(1, 0);
                Lua.Api.Pop();
            }
        }
        //Entry point of the GmodDotNet
        static void Main()
        {
            //Test GetNumber
            Lua.Api.PushSpecial(Lua.SpecialTables.Global);
            Lua.Api.PushNumber(2018.000000009);
            Lua.Api.SetField(-2, "testNumber");
            Lua.Api.Pop();

            Print("Number pushed");

            Lua.Api.PushSpecial(Lua.SpecialTables.Global);
            Lua.Api.GetField(-1, "testNumber");
            double ret_num = Lua.Api.GetNumber();
            Lua.Api.Pop(2);
            Print("Your test number: " + ret_num);
        }
    }
}
