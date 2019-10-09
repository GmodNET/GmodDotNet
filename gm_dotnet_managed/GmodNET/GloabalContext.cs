using System;
using System.Collections.Generic;
using System.Text;
using GmodNET.API;
using System.Linq;
using System.Reflection;
using System.IO;

namespace GmodNET
{
    internal class GlobalContext
    {
        ILua lua;

        bool isServerSide;

        List<ModuleHolder> module_holders;

        CFuncManagedDelegate LuaLoadBridge;
        CFuncManagedDelegate LuaUnloadBridge;

        bool AreModulesWereLoaded;

        internal GlobalContext(ILua lua)
        { 
            this.lua = lua;

            AreModulesWereLoaded = false;

            LuaLoadBridge = (lua_state) =>
            {
                LoadAll();
                return 0;
            };
            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushCFunction(LuaLoadBridge);
            lua.SetField(-2, "gmod_net_load_all_function");
            lua.Pop(1);

            LuaUnloadBridge = (lua_state) =>
            {
                UnloadAll();
                return 0;
            };
            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushCFunction(LuaUnloadBridge);
            lua.SetField(-2, "gmod_net_unload_all_function");
            lua.Pop(1);

            lua.GetField(-10002, "SERVER");
            isServerSide = lua.GetBool(-1);

            if(isServerSide)
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "concommand");
                lua.GetField(-1, "Add");
                lua.PushString("gmod_net_load_all");
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "gmod_net_load_all_function");
                lua.Remove(-2);
                lua.PushNil();
                lua.PushString("Load all .NET managed modules by GmodNET");
                lua.PushNumber(0);
                lua.Call(5, 0);
                lua.Pop(2);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "concommand");
                lua.GetField(-1, "Add");
                lua.PushString("gmod_net_unload_all");
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "gmod_net_unload_all_function");
                lua.Remove(-2);
                lua.PushNil();
                lua.PushString("Unload all .NET managed modules by GmodNET");
                lua.PushNumber(0);
                lua.Call(5, 0);
                lua.Pop(2);
            }
            else
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "concommand");
                lua.GetField(-1, "Add");
                lua.PushString("gmod_net_load_all_cl");
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "gmod_net_load_all_function");
                lua.Remove(-2);
                lua.PushNil();
                lua.PushString("Load all .NET managed modules by GmodNET (client-side)");
                lua.PushNumber(0);
                lua.Call(5, 0);
                lua.Pop(2);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "concommand");
                lua.GetField(-1, "Add");
                lua.PushString("gmod_net_unload_all_cl");
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "gmod_net_unload_all_function");
                lua.Remove(-2);
                lua.PushNil();
                lua.PushString("Unload all .NET managed modules by GmodNET (client-side)");
                lua.PushNumber(0);
                lua.Call(5, 0);
                lua.Pop(2);
            }

            LoadAll();
        }

        private GlobalContext()
        {
            throw new NotImplementedException();
        }

        ~GlobalContext()
        {
            foreach(ModuleHolder m in module_holders)
            {
                m.context.Unload();
            }
        }

        void PrintToConsole(string msg)
        {
            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, "print");
            lua.PushString(msg);
            lua.Call(1, 0);
            lua.Pop(1);
        }

        void LoadAll()
        {
            if(AreModulesWereLoaded)
            {
                PrintToConsole("Unable to load .NET modules: modules are already loaded!");
                return;
            }
            PrintToConsole("Loading managed .NET modules...");

            module_holders = new List<ModuleHolder>();

            string[] module_directories = Directory.GetDirectories("garrysmod/lua/bin/Modules");

            List<string> module_names = new List<string>();

            foreach(string s in module_directories)
            {
                module_names.Add(s.Split(new char[] { '\\', '/' }).Last().Split('.')[0]);
            }

            foreach(string s in module_names)
            {
                var con = new ModuleAssemblyLoadContext(s);

                byte[] ass_im = File.ReadAllBytes("garrysmod/lua/bin/Modules/"+s+"/"+s+".dll");
                Assembly mod_ass = con.LoadFromStream(new MemoryStream(ass_im));

                Type[] module_types = mod_ass.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t)).ToArray();

                List<IModule> list_of_modules = new List<IModule>();

                foreach (Type t in module_types)
                { 
                    list_of_modules.Add((IModule)Activator.CreateInstance(t));
                }

                module_holders.Add(new ModuleHolder(con, list_of_modules));
            }

            foreach(ModuleHolder m in module_holders)
            {
                foreach(IModule mm in m.modules)
                {
                    PrintToConsole("Loading module " + mm.ModuleName + ". Version " + mm.ModuleVersion +".");

                    mm.Load(lua, Startup.ExtructLua);
                }
            }

            PrintToConsole("All managed modules were loaded.");

            AreModulesWereLoaded = true;
        }

        void UnloadAll()
        {
            if (!AreModulesWereLoaded)
            {
                PrintToConsole("Unable to unload .NET modules: modules were already unloaded!");
                return;
            }

            PrintToConsole("Unloading managed modules...");

            foreach(ModuleHolder m in module_holders)
            {
                foreach(IModule mm in m.modules)
                {
                    mm.Unload();
                }

                m.context.Unload();
            }

            module_holders = new List<ModuleHolder>();

            PrintToConsole("All managed modules were unloaded.");

            AreModulesWereLoaded = false;
        }
    }
}
