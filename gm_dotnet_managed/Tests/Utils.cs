using System;
using System.Collections.Generic;
using System.Text;

namespace Tests
{
    static class Utils
    {
        public static void Print(this GmodNET.API.ILua lua, string message)
        {
            lua.PushSpecial(GmodNET.API.SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, "print");
            lua.PushString(message);
            lua.Call(1, 0);
            lua.Pop(1);
        }
    }
}
