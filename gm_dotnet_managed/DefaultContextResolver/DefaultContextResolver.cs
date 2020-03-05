using System;
using System.Runtime.Loader;
using System.Reflection;
using System.IO;
using System.Linq;

namespace GmodNET.Resolver
{
    public delegate void MainDelegate();

    public static class DefaultContextResolver
    {
        public static void Main()
        {
            AssemblyLoadContext.Default.Resolving += GmodNET_API_Resolving;
        }

        private static Assembly GmodNET_API_Resolving(AssemblyLoadContext context, AssemblyName asm_name)
        {
            Assembly ret = null;

            if(asm_name.Name == "GmodNET.API")
            {
                try
                {
                    DirectoryInfo info = new DirectoryInfo("garrysmod/lua/bin/gmodnet/API");
                    FileInfo file = info.GetFiles().First(f => f.Name == "GmodNET.API.dll");
                    ret = context.LoadFromAssemblyPath(file.FullName);
                }
                catch(Exception e)
                {
                    ret = null;
                }
            }

            return ret;
        }
    }
}
