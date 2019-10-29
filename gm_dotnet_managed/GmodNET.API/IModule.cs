using System;
using System.Collections.Generic;
using System.Text;

namespace GmodNET.API
{
    /// <summary>
    /// Delegate which binds to the function which derives ILua interface from the lua_state native pointer.
    /// </summary>
    /// <param name="lua_state_pointer">Pointer to the native lua_state</param>
    /// <returns>ILua instance</returns>
    public delegate ILua GetILuaFromLuaStatePointer(IntPtr lua_state_pointer);
    /// <summary>
    /// Interface which every GmodNET module must implement. Module will be created with parameterless constructor and then Load method will be called.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Name of the module
        /// </summary>
        public string ModuleName
        {
            get;
        }
        /// <summary>
        /// Module version
        /// </summary>
        public string ModuleVersion
        {
            get;
        }
        /// <summary>
        /// Return true if module should be loaded by server only and false otherwise
        /// </summary>
        public bool IsServerSideOnly
        {
            get;
        }
        /// <summary>
        /// Return true if this module should be loaded clientside only after signature validation.
        /// </summary>
        public bool UseClientsideModuleVerification
        {
            get;
        }
        /// <summary>
        /// This method is called by GmodNET on module load and should be treated as module constructor
        /// </summary>
        /// <param name="LuaInterface">Interface to communicate with Garry's Mod</param>
        /// <param name="is_serverside">Indicates weather module is loaded serverside</param>
        /// <param name="del">This property will get a delegate which help to get ILua from lua_state pointer. Needed for ILua.PushCFunction functionality.</param>
        public void Load(ILua LuaInterface, bool is_serverside, GetILuaFromLuaStatePointer del);
        /// <summary>
        /// Will be called by GmodNET on module unload. In this method module must cleanup and unregister with Garry's Mod native/lua data
        /// </summary>
        public void Unload();
        /// <summary>
        /// May given version of the module be loaded clientside. Called only when UseClientsideModuleVerification is true.
        /// </summary>
        /// <param name="version"></param>
        /// <returns>Is version allowed</returns>
        public bool AllowVersion(string version);
    }
}
