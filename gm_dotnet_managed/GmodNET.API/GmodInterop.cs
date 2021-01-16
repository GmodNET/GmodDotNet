using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;


namespace GmodNET.API
{
    /// <summary>
    /// Provides helper methods for better interoperability with Garry's Mod.
    /// </summary>
    public static class GmodInterop
    {
        internal unsafe static delegate* managed<IntPtr, ILua> lua_extractor;

        /// <summary>
        /// Gets an instance of <see cref="ILua"/> implementation from a pointer to the Garry's Mod's native lua_state structure.
        /// </summary>
        /// <param name="lua_state">A pointer to the Garry's Mod's native lua_state structure.</param>
        /// <returns>An implementation of <see cref="ILua"/> interface to work with given lua state.</returns>
        /// <remarks>
        /// This method is designed to be used with static functions annotated with <see cref="System.Runtime.InteropServices.UnmanagedCallersOnlyAttribute"/> attribute.
        /// See example (https://gist.github.com/GlebChili/444d8cb1d641d62f94495e546f681b89) for more info.
        /// </remarks>
        public static ILua GetLuaFromState(IntPtr lua_state)
        {
            unsafe
            {
                return lua_extractor(lua_state);
            }
        }

        /// <summary>
        /// Gets an upvlaue pseudo-index of the Lua closure.
        /// </summary>
        /// <param name="upvalue">A relative index of the upvalue.</param>
        /// <param name="managed_offset">Use upvalue offset for managed closures</param>
        /// <returns>A pseudo-index to access an upvalue.</returns>
        /// <remarks>
        /// Upvalues of Lua function closures can be accessed from C-like APIs using local state’s pseudo-indices calculated as follows: 
        /// the pseudo-index of the <c>n-th</c> upvalue is <c>-10002-n</c>. 
        /// For example, the pseudo-index of the first upvalue is <c>-10003</c>, the pseudo-index of the second upvalue is <c>-10004</c>, etc.
        /// 
        /// 
        /// If closure is created using <see cref="ILua.PushManagedClosure(Func{ILua, int}, byte)"/> 
        /// then first upvalue is an integer representation of the <see cref="GCHandle"/> for the underlying managed delegate. 
        /// Thus, to access actual upvalues we have to apply additional offset for managed closures, 
        /// i.e.the pseudo-index of the n-th upvalue is equal to <c>-10003–n</c> (index of <c>n+1-th</c> upvalue in the native case). 
        /// <paramref name="managed_offset"/> parameter is responsible for applying such managed offset.
        /// 
        /// 
        /// See an official Lua documentation about upvalues (https://www.lua.org/pil/27.3.3.html) for more info.
        /// </remarks>
        /// <example>
        /// See an example (https://gist.github.com/GlebChili/1be0fd80bf6d6ea8cca50f6d9699f462) for usage help.
        /// </example>
        public static int GetUpvalueIndex(byte upvalue, bool managed_offset = true)
        {
            if(managed_offset)
            {
                return (-10003 - upvalue);
            }
            else
            {
                return (-10002 - upvalue);
            }
        }
    }
}
