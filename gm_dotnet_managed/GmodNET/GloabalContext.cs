using System;
using System.Collections.Generic;
using GmodNET.API;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace GmodNET
{
    internal class GlobalContext
    {
        ILua lua;

        bool isServerSide;

        Dictionary<string, GmodNetModuleAssemblyLoadContext> module_contexts;

        CFuncManagedDelegate load_module_delegate;

        CFuncManagedDelegate unload_module_delegate;

        internal GlobalContext(ILua lua)
        { 
            this.lua = lua;

            lua.GetField(-10002, "SERVER");
            isServerSide = lua.GetBool(-1);

            load_module_delegate = (lua_state) =>
            {
                ILua lua = GmodInterop.GetLuaFromState(lua_state);

                try
                {
                    string module_name = lua.GetString(1);

                    lua.Pop(lua.Top());

                    if(String.IsNullOrEmpty(module_name))
                    {
                        lua.PrintToConsole("Unable to load module: module name is empty or null.");
                        return 0;
                    }

                    if(module_contexts.ContainsKey(module_name))
                    {
                        lua.PrintToConsole("Unable to load module: module with such name is already loaded.");
                        return 0;
                    }

                    GmodNetModuleAssemblyLoadContext module_context = new GmodNetModuleAssemblyLoadContext(module_name);

                    Assembly module_assembly = module_context.LoadFromAssemblyPath("garrysmod/lua/bin/Modules/" + module_name + "/" + module_name + ".dll");

                    Type[] module_types = module_assembly.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t)).ToArray();

                    List<IModule> modules = new List<IModule>();

                    foreach(Type t in module_types)
                    {
                        modules.Add((IModule)Activator.CreateInstance(t));
                    }

                    if(modules.Count == 0)
                    {
                        lua.PrintToConsole("Unable to load module: Module " + module_name + " does not contain any implementations of the IModule interface.");
                        return 0;
                    }

                    lua.PrintToConsole("Loading modules from " + module_name + ".");
                    lua.PrintToConsole("Number of the IModule interface implementations: " + modules.Count);

                    foreach(IModule m in modules)
                    {
                        lua.PrintToConsole("Loading class-module " + m.ModuleName + " version " + m.ModuleVersion + "...");
                        m.Load(lua, isServerSide, module_context);
                        lua.PrintToConsole("Class-module " + m.ModuleName + " was loaded.");
                    }

                    module_contexts.Add(module_name, module_context);

                    return 0;
                }
                catch (Exception e)
                {
                    lua.PrintToConsole("Unable to load module: exception was thrown");
                    lua.PrintToConsole(e.ToString());

                    return 0;
                }
            };

            unload_module_delegate = (lua_state) =>
            {
                ILua lua = GmodInterop.GetLuaFromState(lua_state);

                try
                {
                    string module_name = lua.GetString(1);

                    if(String.IsNullOrEmpty(module_name))
                    {
                        lua.PrintToConsole("Unable to unload module: module name is empty or null.");
                        return 0;
                    }

                    if(!module_contexts.ContainsKey(module_name))
                    {
                        lua.PrintToConsole("Unable to unload module: there is no loaded module with such name.");
                        return 0;
                    }

                    lua.PrintToConsole("Unloading module " + module_name + "...");

                    WeakReference context_weak_reference = new WeakReference(module_contexts[module_name]);

                    module_contexts[module_name].Unload();
                    module_contexts.Remove(module_name);

                    while(context_weak_reference.IsAlive)
                    {
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                    }

                    lua.PrintToConsole("Module was unloaded.");

                    return 0;
                }
                catch(Exception e)
                {
                    lua.PrintToConsole("Unable to unload module: exception was thrown");
                    lua.PrintToConsole(e.ToString());

                    return 0;
                }
            };

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushCFunction(Marshal.GetFunctionPointerForDelegate<CFuncManagedDelegate>(load_module_delegate));
            lua.SetField(-2, "dotnet_internal_load");
            lua.Pop(1);

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushCFunction(Marshal.GetFunctionPointerForDelegate<CFuncManagedDelegate>(unload_module_delegate));
            lua.SetField(-2, "dotnet_unload");
            lua.Pop(1);
        }

        ~GlobalContext()
        {
            
        }

        internal void OnNativeUnload()
        {
            foreach(KeyValuePair<string, GmodNetModuleAssemblyLoadContext> pair in module_contexts)
            {
                try
                {
                    pair.Value.Unload();
                }
                catch(Exception e)
                {
                    File.AppendAllText("managed_error.log", e.ToString());
                }
            }
        }
    }
}
