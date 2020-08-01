using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Tests
{
    // Tests that we can customize native library resolution process from within a module
    public class CustomNativeLoadProcessA : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                assembly_context.SetCustomNativeLibraryResolver((context, lib_name) =>
                {
                    if(lib_name == "SomeRandomLibraryName")
                    {
                        if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        {
                            return NativeLibrary.Load("Kernel32");
                        }
                        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            return NativeLibrary.Load("libdl");
                        }
                        else
                        {
                            return IntPtr.Zero;
                        }
                    }
                    else
                    {
                        return IntPtr.Zero;
                    }
                });

                IntPtr test_lib_handle = NativeLibrary.Load("SomeRandomLibraryName");

                if(test_lib_handle == IntPtr.Zero)
                {
                    throw new Exception("Native library handle is zero");
                }

                assembly_context.SetCustomNativeLibraryResolver(null);

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            { 
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }
    }
}
