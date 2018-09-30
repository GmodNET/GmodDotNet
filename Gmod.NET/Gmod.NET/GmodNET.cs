using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmodNET
{
    static class GmodNET
    {
        public static void Main()
        {
            //Print message
            lock (Lua.Api)
            {
                Lua.Api.PushSpecial(Lua.SpecialTables.Global);
                Lua.Api.GetField(-1, "print");
                Lua.Api.PushString("Hello, this your managed assembly!!!");
                Lua.Api.Call(1, 0);
                Lua.Api.Pop();
            }
        }
    }
}
