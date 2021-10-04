using System;
using System.Collections.Generic;
using GmodNET.API;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.CompilerServices;

namespace GmodNET
{
    internal class GlobalContext
    {
        ILua lua;

        bool isServerSide;

        Dictionary<string, Tuple<GmodNetModuleAssemblyLoadContext, List<GCHandle>>> module_contexts;

        internal GlobalContext(ILua lua)
        { 
            this.lua = lua;

            lua.GetField(-10002, "SERVER");
            isServerSide = lua.GetBool(-1);

            module_contexts = new Dictionary<string, Tuple<GmodNetModuleAssemblyLoadContext, List<GCHandle>>>();

            int managed_func_type_id = lua.CreateMetaTable("ManagedFunction");
            unsafe
            {
                lua.PushCFunction(&ManagedFunctionMetaMethods.ManagedDelegateGC);
            }
            lua.SetField(-2, "__gc");
            lua.Pop(1);

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushNumber(managed_func_type_id);
            lua.SetField(-2, ManagedFunctionMetaMethods.ManagedFunctionIdField);
            lua.Pop(1);

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.CreateTable();

            lua.PushManagedFunction(LoadModule);
            lua.SetField(-2, "load");

            lua.PushManagedFunction(UnloadModule);
            lua.SetField(-2, "unload");

            lua.SetField(-2, "dotnet");
            lua.Pop(1);
        }

        int LoadModule(ILua lua)
        {
            try
            {
                string module_name = lua.GetString(1);

                lua.Pop(lua.Top());

                if (String.IsNullOrEmpty(module_name))
                {
                    throw new ArgumentException("Module name is null or empty");
                }

                if (module_contexts.ContainsKey(module_name))
                {
                    throw new ArgumentException($"Module with name {module_name} is already loaded");
                }

                if (Path.IsPathRooted(module_name) && !EnvironmentChecks.IsDevelopmentEnvironemt())
                {
                    throw new ArgumentException("An absolute path to module was given and Development environment is not enabled. " +
                        "Either use a short name of the module in game's lua/bin/Modules folder or enable Development environment.");
                }

                if (!Path.IsPathRooted(module_name) && (module_name.Contains("/") || module_name.Contains("\\")))
                {
                    throw new ArgumentException("Module name can't be a relative path.");
                }

                GmodNetModuleAssemblyLoadContext module_context = new GmodNetModuleAssemblyLoadContext(module_name);

                string module_dll_path;
                if(Path.IsPathRooted(module_name))
                {
                    module_dll_path = module_name;
                }
                else
                {
                    module_dll_path = Path.GetFullPath("garrysmod/lua/bin/Modules/" + module_name + "/" + module_name + ".dll");
                }

                Assembly module_assembly = module_context.LoadFromAssemblyPath(module_dll_path);

                Type[] module_types = module_assembly.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t)).ToArray();

                List<IModule> modules = new List<IModule>();

                List<GCHandle> gc_handles = new List<GCHandle>();

                foreach (Type t in module_types)
                {
                    IModule current_module = (IModule)Activator.CreateInstance(t);
                    modules.Add(current_module);
                    gc_handles.Add(GCHandle.Alloc(current_module));
                }

                if (modules.Count == 0)
                {
                    throw new Exception($"Module {module_name} does not contain any implementations of the IModule interface.");
                }

                lua.PrintToConsole("Loading modules from " + module_name + ".");
                lua.PrintToConsole("Number of the IModule interface implementations: " + modules.Count);

                foreach (IModule m in modules)
                {
                    lua.PrintToConsole("Loading class-module " + m.ModuleName + " version " + m.ModuleVersion + "...");
                    m.Load(lua, isServerSide, module_context);
                    lua.PrintToConsole("Class-module " + m.ModuleName + " was loaded.");
                }

                module_contexts.Add(module_name, Tuple.Create(module_context, gc_handles));

                lua.PushBool(true);

                return 1;
            }
            catch (Exception e)
            {
                lua.PrintToConsole("Unable to load module: exception was thrown");
                lua.PrintToConsole(e.ToString());

                lua.PushBool(false);

                return 1;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        WeakReference<GmodNetModuleAssemblyLoadContext> UnloadHelper(string module_name)
        {
            WeakReference<GmodNetModuleAssemblyLoadContext> context_weak_reference =
                 new WeakReference<GmodNetModuleAssemblyLoadContext>(module_contexts[module_name].Item1);

            try
            {
                foreach (GCHandle h in module_contexts[module_name].Item2)
                {
                    ((IModule)h.Target).Unload(lua);
                    h.Free();
                }

                module_contexts[module_name].Item1.Unload();
            }
            catch(Exception e)
            {
                throw new AggregateException($"Unable to unload module (inner exception: {e.ToString()}). Module resources can't be freed. " +
                    $"Memory leak could occur. Game restart may be required.", e);
            }
            finally
            {
                module_contexts.Remove(module_name);
            }

            return context_weak_reference;
        }

        int UnloadModule(ILua lua)
        {
            try
            {
                string module_name = lua.GetString(1);

                if (String.IsNullOrEmpty(module_name))
                {
                    throw new Exception("Module name is empty or null");
                }

                if (!module_contexts.ContainsKey(module_name))
                {
                    throw new Exception($"There is no loaded module with name { module_name }");
                }

                lua.PrintToConsole($"Unloading module { module_name } ...");

                WeakReference<GmodNetModuleAssemblyLoadContext> context_weak_reference = UnloadHelper(module_name);

                for(int i = 0; context_weak_reference.TryGetTarget(out _); i++)
                {
                    lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                    lua.GetField(-1, "collectgarbage");
                    lua.MCall(0, 0);
                    lua.Pop(1);

                    GC.Collect();
                    GC.WaitForPendingFinalizers();

                    if(i >= 300)
                    {
                        throw new Exception($"Module {module_name} can't be unloaded: there are remaining references or background threads still executing. " +
                            $"Module resources can't be freed. Memory leak could occur. Game restart may be required.");
                    }
                }

                lua.PrintToConsole($"Module {module_name} was unloaded.");

                lua.PushBool(true);

                return 1;
            }
            catch (Exception e)
            {
                lua.PrintToConsole("Unable to unload module: exception was thrown");
                lua.PrintToConsole(e.ToString());

                lua.PushBool(false);

                return 1;
            }
        }

        internal void OnNativeUnload(ILua lua)
        {
            try
            {
                List<string> module_names = new List<string>();

                foreach (var p in module_contexts)
                {
                    module_names.Add(p.Key);
                }

                foreach(string name in module_names)
                {
                    try
                    {
                        WeakReference<GmodNetModuleAssemblyLoadContext> weak_reference = UnloadHelper(name);

                        for (int i = 0; weak_reference.TryGetTarget(out _); i++)
                        {
                            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                            lua.GetField(-1, "collectgarbage");
                            lua.MCall(0, 0);
                            lua.Pop(1);

                            GC.Collect();
                            GC.WaitForPendingFinalizers();

                            if (i >= 300)
                            {
                                throw new Exception($"Module {name} can't be unloaded: there are remaining references or background threads still executing. " +
                                    $"Module resources can't be freed. Memory leak could occur. Game restart may be required.");
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        lua.PrintToConsole($"Exception was thrown while unloading .NET module {name}");
                        lua.PrintToConsole(e.ToString());
                    }
                }
            }
            catch(Exception e)
            {
                lua.PrintToConsole("Critiacal error occured on .NET modules unload");
                lua.PrintToConsole(e.ToString());
            }
        }
    }
}
