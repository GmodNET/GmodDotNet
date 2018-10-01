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
           lock(Lua.Api)
            {
                Lua.Api.PushSpecial(Lua.SpecialTables.Global);
                Lua.Api.GetField(-1, "print");
                Lua.Api.PushString("Message from your managed code!");
                Lua.Api.Call(1, 0);
                Lua.Api.Pop(1);
            }
        }
    }
}
