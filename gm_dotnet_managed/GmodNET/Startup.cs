using System;
using System.IO;
using System.Runtime.InteropServices;
using static GmodNET.RuntimeServices;
using GmodNET.API;
using System.Collections.Generic;
using System.Linq;

namespace GmodNET
{
    // This delegate is needed for native interop and its signature nust match Startup.Main's one.
    internal delegate IntPtr MainDelegate(IntPtr lua_base, int maj_ver, int min_ver, int misc_ver, IntPtr param);

    // Startup class is ressponsible for bootstraping manged code 
    internal static class Startup
    {
        static List<GlobalContext> global_contexts = new List<GlobalContext>();

        internal delegate void CleanupDelegate();
        static List<CleanupDelegate> CleanupReturns = new List<CleanupDelegate>();

        static bool FirstRun = true;

        //Called by Garry's Mod. Responsible for initial configuration.
        internal static IntPtr Main(IntPtr lua_base, int maj_ver, int min_ver, int misc_ver, IntPtr param)
        {
            if(!((maj_ver == 0) && (min_ver == 4) && (misc_ver == 0)))
            {
                File.WriteAllText("GmodNETErrorLog.txt", "Version mismatch! \n");
                return IntPtr.Zero;
            }
            
            if(true)
            {
                unsafe
                {
                    Span<IntPtr> params_from_native_code = new Span<IntPtr>((void*)param, 54);

                    LuaInterop.top = CreateNativeCaller<Func<IntPtr, int>>(params_from_native_code[0]);

                    LuaInterop.push = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[1]);

                    LuaInterop.pop = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[2]);

                    LuaInterop.get_field = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[3]);

                    LuaInterop.set_field = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[4]);

                    LuaInterop.create_table = CreateNativeCaller<Action<IntPtr>>(params_from_native_code[5]);

                    LuaInterop.set_metatable = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[6]);

                    LuaInterop.get_metatable = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[7]);

                    LuaInterop.call = CreateNativeCaller<Action<IntPtr, int, int>>(params_from_native_code[8]);

                    LuaInterop.pcall = CreateNativeCaller<Func<IntPtr, int ,int, int, int>>(params_from_native_code[9]);

                    LuaInterop.equal = CreateNativeCaller<Func<IntPtr, int, int, int>>(params_from_native_code[10]);

                    LuaInterop.raw_equal = CreateNativeCaller<Func<IntPtr, int, int, int>>(params_from_native_code[11]);

                    LuaInterop.insert = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[12]);

                    LuaInterop.remove = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[13]);

                    LuaInterop.next = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[14]);

                    LuaInterop.throw_error = CreateNativeCaller<Action<IntPtr, IntPtr>>(params_from_native_code[15]);

                    LuaInterop.check_type = CreateNativeCaller<Action<IntPtr, int, int>>(params_from_native_code[16]);

                    LuaInterop.arg_error = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[17]);

                    LuaInterop.get_string = CreateNativeCaller<Func<IntPtr, int, IntPtr, IntPtr>>(params_from_native_code[18]);

                    LuaInterop.get_number = CreateNativeCaller<Func<IntPtr, int ,double>>(params_from_native_code[19]);

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

                    LuaInterop.get_type = CreateNativeCaller<Func<IntPtr, int ,int>>(params_from_native_code[33]);

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
                }
                FirstRun = false;
            }

            ILua lua = new Lua(lua_base);

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, "print");
            lua.PushString("GmodNET by Gleb Krasilich. Version " + 0 + "." + 4 + "." + 1 + " Nakhodka.");
            lua.Call(1, 0);
            lua.Pop(1);

            GlobalContext n_context = new GlobalContext(lua);

            global_contexts.Add(n_context);

            CleanupDelegate n_cleanup_delegate = () => CleanupRealization(n_context);


            CleanupReturns.Add(n_cleanup_delegate);

            return Marshal.GetFunctionPointerForDelegate<CleanupDelegate>(n_cleanup_delegate);
        }

        internal static void CleanupRealization(GlobalContext context)
        {
            context.OnNativeUnload();

            global_contexts.Remove(context);

            if (CleanupReturns.Count > 100)
            { 
                CleanupReturns.Remove(CleanupReturns.First());
            }
        }
    }
}
