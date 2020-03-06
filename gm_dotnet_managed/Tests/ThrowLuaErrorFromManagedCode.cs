using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This test checks if we are able to throw Lua errors from managed code.
    public class ThrowLuaErrorFromManagedCode : ITest
    {
        string error_message;
        CFuncManagedDelegate function_to_throw_error;
        TaskCompletionSource<bool> taskCompletion;
        GetILuaFromLuaStatePointer lua_extructor;

        public ThrowLuaErrorFromManagedCode()
        {
            taskCompletion = new TaskCompletionSource<bool>();
            error_message = Guid.NewGuid().ToString();

            function_to_throw_error = (lua_state) =>
            {
                ILua lua = lua_extructor(lua_state);

                lua.PushString(error_message);

                return -1;
            };
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            this.lua_extructor = lua_extructor;

            try
            {
                lua.PushCFunction(function_to_throw_error);
                lua.PCall(0, 0, 0);
                string received_string = lua.GetString(-1);
                lua.Pop(1);

                if(received_string != error_message)
                {
                    throw new ThrowLuaErrorFromManagedCodeException("Error message is invalid");
                }
                else
                {
                    taskCompletion.TrySetResult(true);
                }
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }
    }

    public class ThrowLuaErrorFromManagedCodeException : Exception
    {
        public ThrowLuaErrorFromManagedCodeException(string message) : base(message)
        {

        }
    }
}
