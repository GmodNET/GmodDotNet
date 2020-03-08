using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using GmodNET.API;

namespace GmodNET
{
    [SuppressUnmanagedCodeSecurity]
    internal static class LuaInterop
    {
        static internal Func<IntPtr, int> top;

        static internal Action<IntPtr, int> push;

        static internal Action<IntPtr, int> pop;

        static internal Action<IntPtr, int, IntPtr> get_field;

        static internal Action<IntPtr, int, IntPtr> set_field;

        static internal Action<IntPtr> create_table;

        static internal Action<IntPtr, int> set_metatable;

        static internal Func<IntPtr, int, int> get_metatable;

        static internal Action<IntPtr, int, int> call;

        static internal Func<IntPtr, int ,int, int, int> pcall;

        static internal Func<IntPtr, int, int, int> equal;

        static internal Func<IntPtr, int, int, int> raw_equal;

        static internal Action<IntPtr, int> insert;

        static internal Action<IntPtr, int> remove;

        static internal Func<IntPtr, int, int> next;

        static internal Action<IntPtr, IntPtr> throw_error;

        static internal Action<IntPtr, int, int> check_type;

        static internal Action<IntPtr, int, IntPtr> arg_error;

        static internal Func<IntPtr, int, IntPtr, IntPtr> get_string;

        static internal Func<IntPtr, int ,double> get_number;

        static internal Func<IntPtr, int, int> get_bool;

        static internal Func<IntPtr, int, IntPtr> get_c_function;

        static internal Action<IntPtr> push_nil;

        static internal Action<IntPtr, IntPtr, uint> push_string;

        static internal Action<IntPtr, double> push_number;

        static internal Action<IntPtr, int> push_bool;

        static internal Action<IntPtr, IntPtr> push_c_function;

        static internal Action<IntPtr, IntPtr, int> push_c_closure;

        static internal Func<IntPtr, int> reference_create;

        static internal Action<IntPtr, int> reference_free;

        static internal Action<IntPtr, int> reference_push;

        static internal Action<IntPtr, int> push_special;

        static internal Func<IntPtr, int, int, int> is_type;

        static internal Func<IntPtr, int ,int> get_type;

        static internal Func<IntPtr, int, IntPtr, IntPtr> get_type_name;

        static internal Func<IntPtr, int, int> obj_len;

        static internal Action<IntPtr, IntPtr, int> get_angle;

        static internal Action<IntPtr, IntPtr, int> get_vector;

        static internal Action<IntPtr, float, float, float> push_angle;

        static internal Action<IntPtr, float, float, float> push_vector;

        static internal Action<IntPtr, IntPtr> set_state;

        static internal Func<IntPtr, IntPtr, int> create_metatable;

        static internal Func<IntPtr, int, int> push_metatable;

        static internal Action<IntPtr, IntPtr, int> push_user_type;

        static internal Action<IntPtr, int, IntPtr> set_user_type;

        static internal Func<IntPtr, int, int, IntPtr> get_user_type;

        static internal Func<IntPtr, IntPtr> get_iluabase_from_the_lua_state;

        static internal Action<IntPtr, int> get_table;

        static internal Action<IntPtr, int> set_table;

        static internal Action<IntPtr, int> raw_get;

        static internal Action<IntPtr, int> raw_set;

        static internal Action<IntPtr, IntPtr> push_user_data;

        static internal Func<IntPtr, int, IntPtr, IntPtr> check_string;

        static internal Func<IntPtr, int, double> check_number;

        static internal Action<IntPtr, IntPtr, IntPtr> push_c_function_safe;

        internal static ILua ExtructLua(IntPtr lua_state)
        { 
            IntPtr tmp_ptr = LuaInterop.get_iluabase_from_the_lua_state(lua_state);

            return new Lua(tmp_ptr);
        }
    }
}
