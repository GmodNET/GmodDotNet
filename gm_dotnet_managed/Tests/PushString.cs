using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    //This test generates five random strings, pushes them on stack, pops, and compares
    public class PushString : ITest
    {
        string FieldName;
        TaskCompletionSource<bool> taskCompletion;

        public PushString()
        {
            FieldName = Guid.NewGuid().ToString();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            taskCompletion = new TaskCompletionSource<bool>();

            string[] TestStrings = new string[5];
            for(int i = 0; i < 5; i++)
            {
                TestStrings[i] = Guid.NewGuid().ToString();
            }

            try
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);

                for(int i = 0; i < 5; i++)
                {
                    lua.PushString(TestStrings[i]);
                    lua.SetField(-2, FieldName + "Str" + i.ToString());
                }

                for(int i = 0; i < 5; i++)
                {
                    lua.GetField(-1, FieldName + "Str" + i.ToString());
                    string tmp = lua.GetString(-1);
                    lua.Pop(1);

                    if(tmp != TestStrings[i])
                    {
                        throw new PushStringException(i, TestStrings[i], tmp);
                    }
                }

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }
    }

    public class PushStringException : Exception
    {
        string message;

        public PushStringException(int count, string expected, string recieved)
        {
            message = "Fail after " + count + " pushed strings. Expected " + expected + " but recieved " + recieved;
        }

        public override string Message => message;
    }
}
