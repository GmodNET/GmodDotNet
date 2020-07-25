using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Loader;
using System.IO;
using System.Reflection;
using System.Linq;
using GmodNET.API;

namespace GmodNET
{
    internal class GmodNetModuleAssemblyLoadContext : ModuleAssemblyLoadContext
    {
        private AssemblyDependencyResolver resolver;
        private string module_name;
        private Func<ModuleAssemblyLoadContext, string, IntPtr> customNativeLibraryResolver;

        public override string ModuleName
        { 
            get
            {
                return module_name;
            }
        }

        public override Func<ModuleAssemblyLoadContext, string, IntPtr> CustomNativeLibraryResolver
        {
            get
            {
                return customNativeLibraryResolver;
            }
        }

        public override void SetCustomNativeLibraryResolver(Func<ModuleAssemblyLoadContext, string, IntPtr> resolver)
        {
            customNativeLibraryResolver = resolver;
        }
        
        internal GmodNetModuleAssemblyLoadContext(string module_name) : base(isCollectible: true)
        {
            this.module_name = module_name;
            resolver = new AssemblyDependencyResolver("garrysmod/lua/bin/Modules/"+module_name+"/"+module_name+".dll");
        }

        protected override System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyName)
        {
            if(assemblyName.Name == "GmodNET.API")
            {
                return null;
            }

            string path = resolver.ResolveAssemblyToPath(assemblyName);
            if (path == null || path == string.Empty)
            { 
                return null;
            }
            else
            {
                return this.LoadFromAssemblyPath(path);
            }
        }

        protected override IntPtr LoadUnmanagedDll (string unmanagedDllName)
        {
            string unmanaged_dep_path = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            if(unmanaged_dep_path == null)
            {
                return IntPtr.Zero;
            }
            else
            {
                return this.LoadUnmanagedDllFromPath(unmanaged_dep_path);
            }
        }
    }
}
