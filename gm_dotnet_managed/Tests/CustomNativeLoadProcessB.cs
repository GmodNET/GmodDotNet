using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Tests
{
    // A continuation of CustomNativeLoadProcessA. Tests that native resolution process can be reverted back to default.
    public class CustomNativeLoadProcessB : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                try
                {
                    IntPtr test_lib_handle = NativeLibrary.Load("SomeRandomLibraryName");
                }
                catch(DllNotFoundException e)
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
}
