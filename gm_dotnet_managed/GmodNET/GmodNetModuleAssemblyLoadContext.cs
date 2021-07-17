using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Loader;
using System.IO;
using System.Reflection;
using System.Linq;
using GmodNET.API;
using System.Runtime.InteropServices;

namespace GmodNET
{
    internal class GmodNetModuleAssemblyLoadContext : ModuleAssemblyLoadContext
    {
        private AssemblyDependencyResolver resolver;
        private string module_name;
        private Func<ModuleAssemblyLoadContext, string, IntPtr> customNativeLibraryResolver;
        private List<IntPtr> native_libray_handles;

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
        
        internal GmodNetModuleAssemblyLoadContext(string module_name) : base(module_name: module_name, isCollectible: true)
        {
            this.module_name = module_name;

            string path_for_resolver;
            if (Path.IsPathRooted(module_name))
            {
                path_for_resolver = module_name;
            }
            else
            {
                path_for_resolver = "garrysmod/lua/bin/Modules/" + module_name + "/" + module_name + ".dll";
            }
            resolver = new AssemblyDependencyResolver(path_for_resolver);
            customNativeLibraryResolver = null;
            native_libray_handles = new List<IntPtr>();

            this.Unloading += (context) =>
            {
                foreach(IntPtr h in native_libray_handles)
                {
                    NativeLibrary.Free(h);
                }
                native_libray_handles.Clear();
            };
        }

        protected override System.Reflection.Assembly Load(System.Reflection.AssemblyName assemblyName)
        {
            if(assemblyName.Name == "GmodNET.API")
            {
                return null;
            }

            string path = resolver.ResolveAssemblyToPath(assemblyName);
            if (string.IsNullOrEmpty(path))
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
            IntPtr lib_pointer = IntPtr.Zero;

            if(customNativeLibraryResolver != null)
            {
                lib_pointer = customNativeLibraryResolver(this, unmanagedDllName);
            }

            if(lib_pointer != IntPtr.Zero)
            {
                return lib_pointer;
            }
            else
            {
                string unmanaged_dep_path = resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

                if(String.IsNullOrEmpty(unmanaged_dep_path))
                {
                    return IntPtr.Zero;
                }
                else
                {
                    try
                    {
                        IntPtr lib_handle = NativeLibrary.Load(unmanaged_dep_path);
                        native_libray_handles.Add(lib_handle);
                        return lib_handle;
                    }
                    catch
                    {
                        return IntPtr.Zero;
                    }
                }
            }
        }
    }
}
