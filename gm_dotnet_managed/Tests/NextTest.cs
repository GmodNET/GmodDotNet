using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.Next
    public class NextTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                string key = Guid.NewGuid().ToString();
                string value = Guid.NewGuid().ToString();

                lua.CreateTable();
                lua.PushString(value);
                lua.SetField(-2, key);

                lua.PushNil();
                lua.Next(-2);

                string received_key = lua.GetString(-2);
                string received_value = lua.GetString(-1);
                lua.Pop(3);

                if(received_key != key || received_value != value)
                {
                    throw new NextTestException("Received key value pair is invalid");
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

    public class NextTestException : Exception
    {
        public NextTestException(string message) : base(message)
        {

        }
    }
}
