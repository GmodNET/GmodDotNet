using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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

        public static void Log(this GmodNET.API.ILua lua, string message)
        {
            string timed_message = "[" + DateTime.Now.ToString() + "] " + message;
            lua.Print(timed_message);
            File.AppendAllText("tests-log.txt", timed_message + Environment.NewLine);
        }
    }
}
