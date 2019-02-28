using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gmod.NET
{
    static class Util
    {
        public static void ConsolePrint(string msg)
        {
            lock (Lua.Engine)
            {
                Lua.Engine.PushSpecial(SpecialTables.SPECIAL_GLOB);
                Lua.Engine.GetField(-1, "print");
                Lua.Engine.PushString(msg);
                Lua.Engine.Call(1, 0);
                Lua.Engine.Pop(1);
            }
        }
    }
}
