using System;
using System.Diagnostics;
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
            lua.MCall(1, 0);
            lua.Pop(1);
        }

        public static void Log(this GmodNET.API.ILua lua, string message, bool isError = false)
        {
            string timed_message = "[" + DateTime.Now.ToString() + "] " + message;

            string info_suffix = "::error::";

            if(isError)
            {
                timed_message = info_suffix + timed_message;
            }

            lua.Print(timed_message);
            File.AppendAllText("tests-log.txt", timed_message + Environment.NewLine);
        }
    }
}
