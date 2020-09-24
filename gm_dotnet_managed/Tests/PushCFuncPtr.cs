using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Tests
{
    // Tests ILua.PushCFunction(delegate* unmanaged[Cdecl]<IntPtr, int>)
    public class PushCFuncPtr : ITest
    {
        static int random_number;

        static string random_string;

        static PushCFuncPtr()
        {
            random_number = new Random().Next(1, 500);

            random_string = Guid.NewGuid().ToString();
        }

        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
        static int TestFunc(IntPtr lua_state)
        {
            ILua lua = GmodInterop.GetLuaFromState(lua_state);

            lua.PushNumber(random_number);

            lua.PushString(random_string);

            return 2;
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new();

            try
            {
                unsafe
                {
                    lua.PushCFunction((delegate* unmanaged[Cdecl]<IntPtr, int>)(delegate* managed<IntPtr, int>)&TestFunc);
                    lua.MCall(0, 2);

                    string ret_string = lua.GetString(-1);
                    double ret_number = lua.GetNumber(-2);

                    lua.Pop(2);

                    if(ret_string != random_string)
                    {
                        throw new Exception("Returned string is invalid");
                    }

                    if(ret_number != random_number)
                    {
                        throw new Exception("Returned number is invalid");
                    }

                    taskCompletion.TrySetResult(true);
                }
            }
            catch(Exception  e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }
    }
}
