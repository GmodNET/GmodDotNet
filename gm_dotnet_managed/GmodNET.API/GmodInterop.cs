using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmodNET.API
{
    /// <summary>
    /// Provides helper methods for Garry's Mod interop.
    /// </summary>
    public static class GmodInterop
    {
        internal static Func<IntPtr, ILua> lua_extructor;

        /// <summary>
        /// Extructs an instance of <see cref="ILua"/> implementation from a pointer to the Garry's Mod native lua_state structure.
        /// </summary>
        /// <param name="lua_state">A pointer to the Garry's Mod native lua_state structure.</param>
        /// <returns>An implementation of <see cref="ILua"/> interface to work with given lua state.</returns>
        public static ILua GetLuaFromState(IntPtr lua_state)
        {
            return lua_extructor(lua_state);
        }
    }
}
