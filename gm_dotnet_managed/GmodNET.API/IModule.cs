using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Loader;

namespace GmodNET.API
{
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
        /// This method is called by GmodNET on module load and should be treated as module initializer.
        /// </summary>
        /// <param name="lua">Interface to communicate with Garry's Mod</param>
        /// <param name="is_serverside">Indicates weather module is loaded serverside</param>
        /// <param name="assembly_context">AseemblyLoadContext of this module.</param>
        public void Load(ILua lua, bool is_serverside, ModuleAssemblyLoadContext assembly_context);
        /// <summary>
        /// Called by GmodDotNet runtime on game/server quit and manual module unload.
        /// </summary>
        /// <param name="lua">Lua interface to interact with.</param>
        public void Unload(ILua lua);
    }
}
