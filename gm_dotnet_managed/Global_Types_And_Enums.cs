using System;
using System.Runtime.InteropServices;

namespace Gmod.NET
{
    /// <summary>
    /// Delegate which represents lua stack function. Pointer derived from this delegate can be passed to Lua engine as CFunc.
    /// </summary>
    /// <param name="ptr">Lua stack pointer. This argument is required by calling convention and shouldn't be used.</param>
    /// <returns>Number of return values pushed on stack</returns>
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate int GarrysModFunc(IntPtr ptr);

    /// <summary>
    /// Enumeration of global tables available in Garry's mod lua engine
    /// </summary>
    public enum SpecialTables
    {
        /// <summary>
        /// Global table.
        /// </summary>
        SPECIAL_GLOB = 0,
        /// <summary>
        /// Environment table
        /// </summary>
        SPECIAL_ENV = 1,
        /// <summary>
        /// Registry table
        /// </summary>
        SPECIAL_REG = 2
    }

    /// <summary>
    /// Lua and Garry's mod types' IDs
    /// </summary>
    public enum LuaTypes
    {
        NONE = -1,
        NIL,
        BOOL,
        LIGHTUSERDATA,
        NUMBER,
        STRING,
        TABLE,
        FUNCTION,
        USERDATA,
        THREAD,

        // GMod Types
        ENTITY,
        VECTOR,
        ANGLE,
        PHYSOBJ,
        SAVE,
        RESTORE,
        DAMAGEINFO,
        EFFECTDATA,
        MOVEDATA,
        RECIPIENTFILTER,
        USERCMD,
        SCRIPTEDVEHICLE,
        MATERIAL,
        PANEL,
        PARTICLE,
        PARTICLEEMITTER,
        TEXTURE,
        USERMSG,
        CONVAR,
        IMESH,
        MATRIX,
        SOUND,
        PIXELVISHANDLE,
        DLIGHT,
        VIDEO,
        FILE,
        LOCOMOTION,
        PATH,
        NAVAREA,
        SOUNDHANDLE,
        NAVLADDER,
        PARTICLESYSTEM,
        PROJECTEDTEXTURE,
        PHYSCOLLIDE,

        COUNT
    }
}
