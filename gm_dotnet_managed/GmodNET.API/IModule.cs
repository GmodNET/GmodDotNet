using System;
using System.Collections.Generic;
using System.Text;

namespace GmodNET.API
{
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
        /// This method is called by GmodNET on module load and should be treated as module constructor
        /// </summary>
        /// <param name="LuaInterface">Interface to communicate with Garry's Mod</param>
        /// <param name="GetILuaFromLuaStatePointerMethod">This property will get a delegate which help to get ILua from lua_state pointer. 
        /// Needed for ILua.PushCFunction functionality.</param>
        public void Load(ILua LuaInterface, Func<IntPtr, ILua> GetILuaFromLuaStatePointerMethod);
        /// <summary>
        /// Will be called by GmodNET on module unload. In this method module must cleanup and unregister with Garry's Mod native/lua data
        /// </summary>
        public void Unload();
    }
}
