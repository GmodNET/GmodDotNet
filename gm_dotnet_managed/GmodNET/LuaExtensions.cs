using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GmodNET
{
    internal static class LuaExtensions
    {
        internal static void PrintToConsole(this ILua lua, string message)
        {
            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, "print");
            lua.PushString(message);
            lua.MCall(1, 0);
            lua.Pop(1);
        }
    }
}
