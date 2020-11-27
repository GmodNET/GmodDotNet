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
        /// See [example](https://gist.github.com/GlebChili/444d8cb1d641d62f94495e546f681b89) for more info.
        /// </remarks>
        public static ILua GetLuaFromState(IntPtr lua_state)
        {
            unsafe
            {
                return lua_extractor(lua_state);
            }
        }

        /// <summary>
        /// Get an upvlaue pseudo-index of the Lua closure.
        /// </summary>
        /// <param name="upvalue">A relative index of the upvalue.</param>
        /// <param name="managed_offset">Use upvalue offset for managed closures</param>
        /// <returns>A pseudo-index to access upvalue.</returns>
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
