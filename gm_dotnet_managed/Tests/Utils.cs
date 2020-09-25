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
            lua.Call(1, 0);
            lua.Pop(1);
        }

        public static void Log(this GmodNET.API.ILua lua, string message, bool isError = false, Exception exception = null)
        {
            string timed_message = "[" + DateTime.Now.ToString() + "] " + message;

            string info_suffix = "::error";

            if(exception != null)
            {
                StackTrace st = new(exception);
                StackFrame sf = st.GetFrame(0);

                info_suffix += " file=" + sf.GetFileName();
                info_suffix += ",line=" + sf.GetFileLineNumber();
            }

            info_suffix += "::";

            if(isError)
            {
                timed_message = info_suffix + timed_message;
            }

            lua.Print(timed_message);
            File.AppendAllText("tests-log.txt", timed_message + Environment.NewLine);
        }
    }
}
