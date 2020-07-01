using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Loader;
using System.IO;
using System.Reflection;
using System.Linq;

namespace GmodNET
{
    internal class ModuleAssemblyLoadContext : AssemblyLoadContext
    {
        private AssemblyDependencyResolver resolver;
        private string module_name;

        internal string ModuleName
        { 
            get
            {
                return module_name;
            }
        }
        
        internal ModuleAssemblyLoadContext(string module_name) : base(isCollectible: true)
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
