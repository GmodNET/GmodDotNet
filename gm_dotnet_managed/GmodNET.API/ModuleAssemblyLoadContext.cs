using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Loader;
using System.Reflection;

namespace GmodNET.API
{
    /// <summary>
    /// An extension of <see cref="AssemblyLoadContext" /> class with Gmod.NET specific features.
    /// </summary>
    public abstract class ModuleAssemblyLoadContext : AssemblyLoadContext
    {
        /// <summary>
        /// Get associated module name or module's absolute path.
        /// </summary>
        public virtual string ModuleName => moduleName;

        string moduleName;

        /// <summary>
        /// Get current custom native library resolver delegate.
        /// </summary>
        public abstract Func<ModuleAssemblyLoadContext, string, IntPtr> CustomNativeLibraryResolver {get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleAssemblyLoadContext" /> class with a value that indicates whether unloading is enabled.
        /// </summary>
        /// <param name="isCollectible"><c>true</c> to enable <see cref="AssemblyLoadContext.Unload()"/>; otherwise, <c>false</c>. </param>
        /// <param name="module_name">The name or an absolute path of the corresponding module.</param>
        public ModuleAssemblyLoadContext(string module_name, bool isCollectible) : base(isCollectible: isCollectible)
        { 
            moduleName = module_name;
        }

        /// <summary>
        /// Sets <paramref name="resolver"/> as new custom native library resolution middleware for the current instance of <see cref="ModuleAssemblyLoadContext"/>.
        /// </summary>
        /// <param name="resolver">
        /// A new custom native library resolver delegate. This delegate takes an instance of <see cref="ModuleAssemblyLoadContext"/> in which native resolution process
        /// is taking place and the name of the native library to resolve as parameters. The delegate should return an OS handle (as IntPtr) for the loaded native library.
        /// If the delegate returnes <see cref="IntPtr.Zero"/> then the instance of <see cref="ModuleAssemblyLoadContext"/> will try to resolve the native library using the
        /// default resolution process.
        /// Can be set to null to return the instance of <see cref="ModuleAssemblyLoadContext"/> to the default native library resolution process.
        /// </param>
        /// <example>
        /// <code>
        /// public void Load(ILua lua, bool is_serverside, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        /// {
        ///     assembly_context.SetCustomNativeLibraryResolver((context, library_name) =>
        ///     {
        ///         if(library_name == "some_special_library_for_which_we_want_to_customize_resolution_process")
        ///         {
        ///             return System.Runtime.InteropServices.NativeLibrary.Load("path/to/that/special/library");
        ///         }
        ///         else
        ///         {
        ///             // By returning IntPtr.Zero here we allow GmodNET runtime to use the default resolution process
        ///             // for libraries with different names.
        ///             return IntPtr.Zero;
        ///         }
        ///     });
        /// }
        /// </code>
        /// </example>
        public abstract void SetCustomNativeLibraryResolver(Func<ModuleAssemblyLoadContext, string, IntPtr> resolver);
    }
}
