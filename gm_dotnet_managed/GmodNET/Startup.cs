﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using static GmodNET.RuntimeServices;
using GmodNET.API;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GmodNET
{
    // Startup class is ressponsible for bootstraping managed code 
    internal static class Startup
    {
        static List<GlobalContext> global_contexts = new List<GlobalContext>();

        internal delegate void CleanupDelegate(IntPtr lua_pointer);
        static List<CleanupDelegate> CleanupReturns = new List<CleanupDelegate>();

        static bool FirstRun = true;

        //Called by Garry's Mod. Responsible for initial configuration.
        [UnmanagedCallersOnly]
        internal static unsafe IntPtr Main(IntPtr lua_base, IntPtr native_version_string, int version_string_length, IntPtr param, 
                                           IntPtr native_delegate_executor_ptr, IntPtr* managed_delegate_executor_ptr)
        {
            try
            {
                string full_assembly_version = FileVersionInfo.GetVersionInfo(typeof(Startup).Assembly.Location).ProductVersion;
                string friendly_version = full_assembly_version.Split("+")[0];
                string version_codename = full_assembly_version.Split("+")[1].Split(".")[1];

               
                string native_version = Encoding.UTF8.GetString((byte*)native_version_string.ToPointer(), version_string_length);

                if (native_version != full_assembly_version)
                {
                    throw new Exception($"GmodNET version does not match with the version of dotnet_loader. Managed version: {full_assembly_version}; " +
                        $"Native version: {native_version}");
                }

                ILua lua = new Lua(lua_base);

                if (FirstRun)
                {
                    Span<IntPtr> params_from_native_code = new Span<IntPtr>((void*)param, 55);

                    LuaInterop.top = CreateNativeCaller<Func<IntPtr, int>>(params_from_native_code[0]);

                    LuaInterop.push = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[1]);

                    LuaInterop.pop = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[2]);

                    LuaInterop.get_field = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[3]);

                    LuaInterop.set_field = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[4]);

                    LuaInterop.create_table = CreateNativeCaller<Action<IntPtr>>(params_from_native_code[5]);

                    LuaInterop.set_metatable = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[6]);

                    LuaInterop.get_metatable = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[7]);

                    LuaInterop.call = CreateNativeCaller<Action<IntPtr, int, int>>(params_from_native_code[8]);

                    LuaInterop.pcall = CreateNativeCaller<Func<IntPtr, int, int, int, int>>(params_from_native_code[9]);

                    LuaInterop.equal = CreateNativeCaller<Func<IntPtr, int, int, int>>(params_from_native_code[10]);

                    LuaInterop.raw_equal = CreateNativeCaller<Func<IntPtr, int, int, int>>(params_from_native_code[11]);

                    LuaInterop.insert = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[12]);

                    LuaInterop.remove = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[13]);

                    LuaInterop.next = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[14]);

                    LuaInterop.throw_error = CreateNativeCaller<Action<IntPtr, IntPtr>>(params_from_native_code[15]);

                    LuaInterop.check_type = CreateNativeCaller<Action<IntPtr, int, int>>(params_from_native_code[16]);

                    LuaInterop.arg_error = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[17]);

                    LuaInterop.get_string = CreateNativeCaller<Func<IntPtr, int, IntPtr, IntPtr>>(params_from_native_code[18]);

                    LuaInterop.get_number = CreateNativeCaller<Func<IntPtr, int, double>>(params_from_native_code[19]);

                    LuaInterop.get_bool = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[20]);

                    LuaInterop.get_c_function = CreateNativeCaller<Func<IntPtr, int, IntPtr>>(params_from_native_code[21]);

                    LuaInterop.push_nil = CreateNativeCaller<Action<IntPtr>>(params_from_native_code[22]);

                    LuaInterop.push_string = CreateNativeCaller<Action<IntPtr, IntPtr, uint>>(params_from_native_code[23]);

                    LuaInterop.push_number = CreateNativeCaller<Action<IntPtr, double>>(params_from_native_code[24]);

                    LuaInterop.push_bool = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[25]);

                    LuaInterop.push_c_function = CreateNativeCaller<Action<IntPtr, IntPtr>>(params_from_native_code[26]);

                    LuaInterop.push_c_closure = CreateNativeCaller<Action<IntPtr, IntPtr, int>>(params_from_native_code[27]);

                    LuaInterop.reference_create = CreateNativeCaller<Func<IntPtr, int>>(params_from_native_code[28]);

                    LuaInterop.reference_free = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[29]);

                    LuaInterop.reference_push = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[30]);

                    LuaInterop.push_special = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[31]);

                    LuaInterop.is_type = CreateNativeCaller<Func<IntPtr, int, int, int>>(params_from_native_code[32]);

                    LuaInterop.get_type = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[33]);

                    LuaInterop.get_type_name = CreateNativeCaller<Func<IntPtr, int, IntPtr, IntPtr>>(params_from_native_code[34]);

                    LuaInterop.obj_len = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[35]);

                    LuaInterop.get_angle = CreateNativeCaller<Action<IntPtr, IntPtr, int>>(params_from_native_code[36]);

                    LuaInterop.get_vector = CreateNativeCaller<Action<IntPtr, IntPtr, int>>(params_from_native_code[37]);

                    LuaInterop.push_angle = CreateNativeCaller<Action<IntPtr, float, float, float>>(params_from_native_code[38]);

                    LuaInterop.push_vector = CreateNativeCaller<Action<IntPtr, float, float, float>>(params_from_native_code[39]);

                    LuaInterop.set_state = CreateNativeCaller<Action<IntPtr, IntPtr>>(params_from_native_code[40]);

                    LuaInterop.create_metatable = CreateNativeCaller<Func<IntPtr, IntPtr, int>>(params_from_native_code[41]);

                    LuaInterop.push_metatable = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[42]);

                    LuaInterop.push_user_type = CreateNativeCaller<Action<IntPtr, IntPtr, int>>(params_from_native_code[43]);

                    LuaInterop.set_user_type = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[44]);

                    LuaInterop.get_user_type = CreateNativeCaller<Func<IntPtr, int, int, IntPtr>>(params_from_native_code[45]);

                    LuaInterop.get_iluabase_from_the_lua_state = CreateNativeCaller<Func<IntPtr, IntPtr>>(params_from_native_code[46]);

                    LuaInterop.get_table = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[47]);

                    LuaInterop.set_table = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[48]);

                    LuaInterop.raw_get = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[49]);

                    LuaInterop.raw_set = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[50]);

                    LuaInterop.push_user_data = CreateNativeCaller<Action<IntPtr, IntPtr>>(params_from_native_code[51]);

                    LuaInterop.check_string = CreateNativeCaller<Func<IntPtr, int, IntPtr, IntPtr>>(params_from_native_code[52]);

                    LuaInterop.check_number = CreateNativeCaller<Func<IntPtr, int, double>>(params_from_native_code[53]);

                    LuaInterop.push_c_function_safe = CreateNativeCaller<Action<IntPtr, IntPtr, IntPtr>>(params_from_native_code[54]);

                    GmodInterop.lua_extractor = &LuaInterop.ExtructLua;

                    ManagedFunctionMetaMethods.NativeDelegateExecutor = native_delegate_executor_ptr;

                    Console.SetOut(new GameConsoleWriter(lua));

                    FirstRun = false;
                }

                *managed_delegate_executor_ptr = (IntPtr)(delegate* unmanaged<IntPtr, int>)&ManagedFunctionMetaMethods.ManagedDelegateExecutor;

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "print");
                lua.PushString(
                    "   _____       _   _   ______   _______ \n"+
                    "  / ____|     | \\ | | |  ____| |__   __|\n"+
                    " | |  __      |  \\| | | |__       | |   \n"+
                    " | | |_ |     | . ` | |  __|      | |   \n"+
                    " | |__| |  _  | |\\  | | |____     | |   \n"+
                    "  \\_____| (_) |_| \\_| |______|    |_|   \n");
                lua.MCall(1, 0);
                lua.Pop(1);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "print");
                lua.PushString("GmodNET by Gleb Krasilich and GmodNET team. Version " + friendly_version + " codename " + version_codename);
                lua.MCall(1, 0);
                lua.Pop(1);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "print");
                lua.PushString("(full build version: " + full_assembly_version + ")");
                lua.MCall(1, 0);
                lua.Pop(1);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "print");
                lua.PushString(RuntimeInformation.FrameworkDescription);
                lua.MCall(1, 0);
                lua.Pop(1);

                if (EnvironmentChecks.IsDevelopmentEnvironemt())
                {
                    lua.PushGlobalTable();
                    lua.GetField(-1, "print");
                    lua.PushString("WARNING: GmodDotNet is running in Development environment. .NET modules can be loaded from any location. " +
                        "DO NOT use untrusted Lua scripts or join untrusted servers while Development mode is on!");
                    lua.MCall(1, 0);
                    lua.Pop(1);
                }

                GlobalContext n_context = new GlobalContext(lua);

                global_contexts.Add(n_context);

                CleanupDelegate n_cleanup_delegate = (lua_pointer) => CleanupRealization(n_context, lua_pointer);


                CleanupReturns.Add(n_cleanup_delegate);

                return Marshal.GetFunctionPointerForDelegate<CleanupDelegate>(n_cleanup_delegate);
            }
            catch(Exception e)
            {
                File.WriteAllText("managed_error.log", e.ToString());
                *managed_delegate_executor_ptr = (IntPtr)(delegate* unmanaged<IntPtr, int>)&ManagedFunctionMetaMethods.ManagedDelegateExecutor;
                return IntPtr.Zero;
            }
        }

        internal static void CleanupRealization(GlobalContext context, IntPtr lua_pointer)
        {
            ILua lua = new Lua(lua_pointer);

            context.OnNativeUnload(lua);

            global_contexts.Remove(context);

            if (CleanupReturns.Count > 100)
            { 
                CleanupReturns.Remove(CleanupReturns.First());
            }
        }
    }
}
