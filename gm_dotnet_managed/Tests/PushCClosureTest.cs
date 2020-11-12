using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Tests
{
    // Test for ILua.PushCClosure
    public class PushCClosureTest : ITest
    {
        static string string_to_add;
        GetILuaFromLuaStatePointer lua_extructor;
        IntPtr closure_ptr;

        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
        static int ClosureTestFunc(IntPtr lua_state)
        {
            ILua lua = GmodInterop.GetLuaFromState(lua_state);

            lua.Pop(lua.Top());

            string upvalue = lua.GetString(GmodInterop.GetUpvalueIndex(1, false));

            lua.PushString(upvalue + string_to_add);

            return 1;
        }

        public PushCClosureTest()
        {
            string_to_add = Guid.NewGuid().ToString();

            unsafe
            {
                closure_ptr = (IntPtr)(delegate* unmanaged[Cdecl]<IntPtr, int>)&ClosureTestFunc;
            }
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            this.lua_extructor = lua_extructor;

            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                string closure_upvalue = Guid.NewGuid().ToString();

                lua.PushString(closure_upvalue);

                lua.PushCClosure(closure_ptr, 1);

                lua.MCall(0, 1);

                string received_string = lua.GetString(-1);

                lua.Pop(1);

                if(received_string != (closure_upvalue + string_to_add))
                {
                    throw new PushCClosureTestException("Received string is invalid");
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

    public class PushCClosureTestException : Exception
    {
        public PushCClosureTestException(string message) : base(message)
        {

        }
    }
}
