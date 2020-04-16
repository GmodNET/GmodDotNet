using System;
using System.Collections.Generic;
using System.Text;

namespace GmodNET.API
{
    /// <summary>
    /// Static class with helper methods for interoperability with Garry's Mod
    /// </summary>
    public static class InteropMethods
    {
        /// <summary>
        /// Get lua pseudoindex for a CClosure upvalue.
        /// </summary>
        /// <param name="upvalue_index">Relative index of the upvalue.</param>
        /// <returns>Lua global pseudoindex for the upvalue.</returns>
        public static int GetUpValuePseudoIndex(int upvalue_index)
        {
            return (-10002 - upvalue_index);
        }
    }
}
