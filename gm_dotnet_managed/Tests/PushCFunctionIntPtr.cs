using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for PushCFunction(IntPtr)
    public class PushCFunctionIntPtr : ITest
    {
        static string random;

        public PushCFunctionIntPtr()
        {
            random = Guid.NewGuid().ToString();
        }

        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
        static int TestFunc(IntPtr lua_state)
        {
            ILua lua = GmodInterop.GetLuaFromState(lua_state);

            lua.Pop(lua.Top());

            lua.PushString(random);

            return 1;
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                IntPtr func_int_ptr;

                unsafe
                {
                    func_int_ptr = (IntPtr)(delegate* unmanaged[Cdecl]<IntPtr, int>)&TestFunc;
                }

                lua.PushCFunction(func_int_ptr);

                lua.MCall(0, 1);

                string ret_string = lua.GetString(-1);

                lua.Pop(1);

                if(ret_string != random)
                {
                    throw new Exception("Return string is incorrect");
                }

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[]{ e });
            }

            return taskCompletion.Task;
        }
    }
}
