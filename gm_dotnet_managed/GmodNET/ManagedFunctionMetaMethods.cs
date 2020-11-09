using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using GmodNET.API;

namespace GmodNET
{
    internal static class ManagedFunctionMetaMethods
    {
        internal static readonly string ManagedFunctionIdField = "gmodnet-managed-delegate-type-id-fe74e198-5ff8-44d4-be32-8abc4a996fd0";

        internal static IntPtr NativeDelegateExecutor = IntPtr.Zero;

        [UnmanagedCallersOnly]
        internal static int ManagedDelegateExecutor(IntPtr lua_state)
        {
            ILua lua = GmodInterop.GetLuaFromState(lua_state);

            try
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, ManagedFunctionIdField);
                int managed_delegate_type_id = (int)lua.GetNumber(-1);
                lua.Pop(2);

                IntPtr managed_delegate_handle = lua.GetUserType(1, managed_delegate_type_id);

                Func<ILua, int> managed_delegate = (Func<ILua, int>)GCHandle.FromIntPtr(managed_delegate_handle).Target;

                return Math.Max(0, managed_delegate(lua));
            }
            catch (Exception e)
            {
                lua.Pop(lua.Top());
                lua.PushString(".NET Exception: " + e.ToString());
                return -1;
            }
        }

        [UnmanagedCallersOnly(CallConvs = new Type[] { typeof(CallConvCdecl) })]
        internal static int ManagedDelegateGC(IntPtr lua_state)
        {
            ILua lua = GmodInterop.GetLuaFromState(lua_state);

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, ManagedFunctionIdField);
            int managed_delegate_type_id = (int)lua.GetNumber(-1);
            lua.Pop(2);

            IntPtr managed_delegate_handle = lua.GetUserType(1, managed_delegate_type_id);

            GCHandle.FromIntPtr(managed_delegate_handle).Free();

            return 0;
        }
    }
}
