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
                        else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        {
                            return NativeLibrary.Load("libdl.so");
                        }
                        else if(RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        {
                            return NativeLibrary.Load("libdl.dylib");
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

                if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    LoadLibraryA("Kernel32");
                }
                else if(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    dlopen("libdl", 0);
                }
                else
                {
                    throw new PlatformNotSupportedException("The current OS platform is not supported by GmodNET.Tests");
                }

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            { 
                taskCompletion.TrySetException(new Exception[] { e });
            }

            assembly_context.SetCustomNativeLibraryResolver(null);

            return taskCompletion.Task;
        }

        [DllImport("SomeRandomLibraryName", CharSet = CharSet.Ansi)]
        public static extern IntPtr LoadLibraryA(string lib_name);

        [DllImport("SomeRandomLibraryName")]
        public static extern IntPtr dlopen(string lib_name, int flag);
    }
}
