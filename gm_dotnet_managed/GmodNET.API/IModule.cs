using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Loader;

namespace GmodNET.API
{
    /// <summary>
    /// Interface which every GmodDotNet module must implement. Module will be initialized with parameterless constructor and then Load method will be called.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Name of the module.
        /// </summary>
        /// <value>
        /// Name of the module.
        /// </value>
        public string ModuleName
        {
            get;
        }

        /// <summary>
        /// Version of the module.
        /// </summary>
        /// <value>
        /// Version of the module.
        /// </value>
        public string ModuleVersion
        {
            get;
        }

        /// <summary>
        /// This method is called by GmodDotNet runtime on module load and should be treated as module's entry point.
        /// </summary>
        /// <param name="lua">An instance of the implementation of <see cref="ILua"/> interface to communicate with Garry's Mod.</param>
        /// <param name="is_serverside"><c>True</c> if module is being loaded into the serverside environment, <c>False</c> otherwise.</param>
        /// <param name="assembly_context">An instance of <see cref="ModuleAssemblyLoadContext"/> in which module will be loaded into.</param>
        public void Load(ILua lua, bool is_serverside, ModuleAssemblyLoadContext assembly_context);

        /// <summary>
        /// Called by GmodDotNet runtime on game/server quit or manual module unload.
        /// </summary>
        /// <param name="lua">An instance of the implementation of <see cref="ILua"/> interface to communicate with Garry's Mod.</param>
        public void Unload(ILua lua);
    }
}
