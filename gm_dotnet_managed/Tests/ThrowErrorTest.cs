using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This tst checks lua.ThrowError and lua.PCall
    public class ThrowErrorTest : ITest
    {
        TaskCompletionSource<bool> taskCompletion;
        string errorMessage;
        CFuncManagedDelegate errorThrowerDelegate;
        GetILuaFromLuaStatePointer lua_extructor;

        public ThrowErrorTest()
        {
            taskCompletion = new TaskCompletionSource<bool>();
            errorMessage = Guid.NewGuid().ToString();
            errorThrowerDelegate = ErrorThrower;
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            this.lua_extructor = lua_extructor;

            try
            {
                lua.PushCFunction(errorThrowerDelegate);
                if(lua.PCall(0, 0, 0) == 0)
                {
                    throw new ThrowErrorTestException("PCall error code is 0");
                }
                string received_string = lua.GetString(-1);
                lua.Pop(1);

                if(received_string != errorMessage)
                {
                    throw new ThrowErrorTestException("Received string is incorrect");
                }

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }

        int ErrorThrower(IntPtr lua_state)
        {
            ILua lua = lua_extructor(lua_state);

            lua.ThrowError(errorMessage);

            return 0;
        }
    }

    public class ThrowErrorTestException : Exception
    {
        string message;

        public ThrowErrorTestException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
