using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Tests
{
    // Tests ILua.GetCFunction
    public class GetCFunctionTest : ITest
    {
        int random_int;
        CFuncManagedDelegate test_func;
        TaskCompletionSource<bool> taskCompletion;
        GetILuaFromLuaStatePointer lua_extructor;

        public GetCFunctionTest()
        {
            random_int = new Random().Next(0, 100);
            taskCompletion = new TaskCompletionSource<bool>();
            test_func = (lua_state) =>
            {
                return random_int;
            };
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            this.lua_extructor = lua_extructor;

            try
            {
                lua.PushCFunction(test_func, false);
                IntPtr pointer_to_test_func = lua.GetCFunction(-1);
                lua.Pop(1);

                CFuncManagedDelegate received_func = Marshal.GetDelegateForFunctionPointer<CFuncManagedDelegate>(pointer_to_test_func);
                int new_int = received_func(IntPtr.Zero);

                if(new_int != random_int)
                {
                    throw new GetCFunctionTestException("Return number is invalid");
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

    public class GetCFunctionTestException : Exception
    {
        public GetCFunctionTestException(string message) : base(message)
        {

        }
    }
}
