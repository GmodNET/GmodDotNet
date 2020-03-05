using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Loader;

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
        /// Name of the module.
        /// </summary>
        public string ModuleName
        {
            get;
        }
        /// <summary>
        /// Module version.
        /// </summary>
        public string ModuleVersion
        {
            get;
        }
        /// <summary>
        /// This method is called by GmodNET on module load and should be treated as module constructor.
        /// </summary>
        /// <param name="lua">Interface to communicate with Garry's Mod</param>
        /// <param name="is_serverside">Indicates weather module is loaded serverside</param>
        /// <param name="lua_extructor">This property gets a delegate which helps to get ILua from lua_state native pointer. Needed for ILua.PushCFunction functionality.</param>
        /// <param name="assembly_context">AseemblyLoadContext of this module.</param>
        public void Load(ILua lua, bool is_serverside, GetILuaFromLuaStatePointer lua_extructor, AssemblyLoadContext assembly_context);
        /// <summary>
        /// Will be called by GmodNET on module unload. In this method module must cleanup and unregister with Garry's Mod native/lua data.
        /// </summary>
        public void Unload();
    }
}
