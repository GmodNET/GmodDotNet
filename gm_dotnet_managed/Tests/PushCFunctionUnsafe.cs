using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This test pushes Managed C Function delegate without safe wrapper and runs it
    public class PushCFunctionUnsafe : ITest
    {
        double random_double;
        GetILuaFromLuaStatePointer lua_extructor;
        TaskCompletionSource<bool> taskCompletion;
        CFuncManagedDelegate func_to_push;

        public PushCFunctionUnsafe()
        {
            Random rand = new Random();
            random_double = rand.NextDouble();
            taskCompletion = new TaskCompletionSource<bool>();
            func_to_push = (lua_state_pointer) =>
            {
                ILua lua =lua_extructor(lua_state_pointer);

                lua.PushNumber(this.random_double);

                return 1;
            };
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            this.lua_extructor = lua_extructor;

            try
            {
                lua.PushCFunction(func_to_push, false);
                lua.MCall(0, 1);
                double received_double = lua.GetNumber(-1);
                lua.Pop(1);

                if(received_double != random_double)
                {
                    throw new PushCFunctionUnsafeException("Received double is invalid");
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

    public class PushCFunctionUnsafeException : Exception
    {
        public PushCFunctionUnsafeException(string message) : base(message)
        {
            
        }
    }
}
