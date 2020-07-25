using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Loader;

namespace GmodNET.API
{
    public abstract class ModuleAssemblyLoadContext : AssemblyLoadContext
    {
        public abstract string ModuleName {get; }

        public ModuleAssemblyLoadContext() : base(isCollectible: true){ }
    }
}
