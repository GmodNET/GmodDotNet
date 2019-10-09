using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using static GmodNET.RuntimeServices;
using GmodNET.API;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GmodNET
{
    // This delegate is needed for native interop and its signature nust match Startup.Main's one.
    internal delegate IntPtr MainDelegate(IntPtr lua_base, int maj_ver, int min_ver, int misc_ver, IntPtr param);

    // Startup class is ressponsible for bootstraping manged code 
    internal static class Startup
    {
        delegate void CleanupDelegate();
        static CleanupDelegate CleanupReturn;

        static List<ModuleHolder> module_holders;

        static ILua Global_Lua;

        static CFuncManagedDelegate LuaLoadBridge;
        static CFuncManagedDelegate LuaUnloadBridge;

        static bool AreModulesWereLoaded = false;

        //Called by Garry's Mod. Responsible for initial configuration.
        internal static IntPtr Main(IntPtr lua_base, int maj_ver, int min_ver, int misc_ver, IntPtr param)
        {
            if(!((maj_ver == VersionInfo.maj_ver) && (min_ver == VersionInfo.min_ver) && (misc_ver == VersionInfo.misc_ver)))
            {
                File.WriteAllText("GmodNETErrorLog.txt", "Version mismatch! \n");
                return IntPtr.Zero;
            }

            unsafe
            {
                Span<IntPtr> params_from_native_code = new Span<IntPtr>((void*)param, 46);

                LuaInterop.top = CreateNativeCaller<Func<IntPtr, int>>(params_from_native_code[0]);

                LuaInterop.push = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[1]);

                LuaInterop.pop = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[2]);

                LuaInterop.get_field = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[3]);

                LuaInterop.set_field = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[4]);

                LuaInterop.create_table = CreateNativeCaller<Action<IntPtr>>(params_from_native_code[5]);

                LuaInterop.set_metatable = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[6]);

                LuaInterop.get_metatable = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[7]);

                LuaInterop.call = CreateNativeCaller<Action<IntPtr, int, int>>(params_from_native_code[8]);

                LuaInterop.pcall = CreateNativeCaller<Func<IntPtr, int ,int, int, int>>(params_from_native_code[9]);

                LuaInterop.equal = CreateNativeCaller<Func<IntPtr, int, int, int>>(params_from_native_code[10]);

                LuaInterop.raw_equal = CreateNativeCaller<Func<IntPtr, int, int, int>>(params_from_native_code[11]);

                LuaInterop.insert = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[12]);

                LuaInterop.remove = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[13]);

                LuaInterop.next = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[14]);

                LuaInterop.throw_error = CreateNativeCaller<Action<IntPtr, IntPtr>>(params_from_native_code[15]);

                LuaInterop.check_type = CreateNativeCaller<Action<IntPtr, int, int>>(params_from_native_code[16]);

                LuaInterop.arg_error = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[17]);

                LuaInterop.get_string = CreateNativeCaller<Func<IntPtr, int, IntPtr, IntPtr>>(params_from_native_code[18]);

                LuaInterop.get_number = CreateNativeCaller<Func<IntPtr, int ,double>>(params_from_native_code[19]);

                LuaInterop.get_bool = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[20]);

                LuaInterop.get_c_function = CreateNativeCaller<Func<IntPtr, int, IntPtr>>(params_from_native_code[21]);

                LuaInterop.push_nil = CreateNativeCaller<Action<IntPtr>>(params_from_native_code[22]);

                LuaInterop.push_string = CreateNativeCaller<Action<IntPtr, IntPtr, uint>>(params_from_native_code[23]);

                LuaInterop.push_number = CreateNativeCaller<Action<IntPtr, double>>(params_from_native_code[24]);

                LuaInterop.push_bool = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[25]);

                LuaInterop.push_c_function = CreateNativeCaller<Action<IntPtr, IntPtr>>(params_from_native_code[26]);

                LuaInterop.push_c_closure = CreateNativeCaller<Action<IntPtr, IntPtr, int>>(params_from_native_code[27]);

                LuaInterop.reference_create = CreateNativeCaller<Func<IntPtr, int>>(params_from_native_code[28]);

                LuaInterop.reference_free = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[29]);

                LuaInterop.reference_push = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[30]);

                LuaInterop.push_special = CreateNativeCaller<Action<IntPtr, int>>(params_from_native_code[31]);

                LuaInterop.is_type = CreateNativeCaller<Func<IntPtr, int, int, int>>(params_from_native_code[32]);

                LuaInterop.get_type = CreateNativeCaller<Func<IntPtr, int ,int>>(params_from_native_code[33]);

                LuaInterop.get_type_name = CreateNativeCaller<Func<IntPtr, int, IntPtr, IntPtr>>(params_from_native_code[34]);

                LuaInterop.obj_len = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[35]);

                LuaInterop.get_angle = CreateNativeCaller<Action<IntPtr, IntPtr, int>>(params_from_native_code[36]);

                LuaInterop.get_vector = CreateNativeCaller<Action<IntPtr, IntPtr, int>>(params_from_native_code[37]);

                LuaInterop.push_angle = CreateNativeCaller<Action<IntPtr, float, float, float>>(params_from_native_code[38]);

                LuaInterop.push_vector = CreateNativeCaller<Action<IntPtr, float, float, float>>(params_from_native_code[39]);

                LuaInterop.set_state = CreateNativeCaller<Action<IntPtr, IntPtr>>(params_from_native_code[40]);

                LuaInterop.create_metatable = CreateNativeCaller<Func<IntPtr, IntPtr, int>>(params_from_native_code[41]);

                LuaInterop.push_metatable = CreateNativeCaller<Func<IntPtr, int, int>>(params_from_native_code[42]);

                LuaInterop.push_user_type = CreateNativeCaller<Action<IntPtr, IntPtr, int>>(params_from_native_code[43]);

                LuaInterop.set_user_type = CreateNativeCaller<Action<IntPtr, int, IntPtr>>(params_from_native_code[44]);

                LuaInterop.get_iluabase_from_the_lua_state = CreateNativeCaller<Func<IntPtr, IntPtr>>(params_from_native_code[45]);
            }

            ILua lua = new Lua(lua_base);

            Global_Lua = lua;

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, "print");
            lua.PushString("GmodNET by Gleb Krasilich. Version " + VersionInfo.maj_ver + "." + VersionInfo.min_ver + "." + VersionInfo.misc_ver + ".");
            lua.Call(1, 0);
            lua.Pop(1);

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

            bool isServerSide;
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

            CleanupReturn = () => { CleanupRealization(lua); };
            return Marshal.GetFunctionPointerForDelegate<CleanupDelegate>(CleanupReturn);
        }

        internal static void CleanupRealization(ILua l)
        {
            
        }

        static void LoadAll()
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

                    mm.Load(Global_Lua, ExtructLua);
                }
            }

            PrintToConsole("All managed modules were loaded.");

            AreModulesWereLoaded = true;
        }

        static void UnloadAll()
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

        static ILua ExtructLua(IntPtr lua_state)
        { 
            IntPtr tmp_ptr = LuaInterop.get_iluabase_from_the_lua_state(lua_state);

            return new Lua(tmp_ptr);
        }

        static void PrintToConsole(string msg)
        {
            Global_Lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            Global_Lua.GetField(-1, "print");
            Global_Lua.PushString(msg);
            Global_Lua.Call(1, 0);
            Global_Lua.Pop(1);
        }
    }

    internal class ModuleHolder
    {
        internal ModuleAssemblyLoadContext context;
        internal List<IModule> modules;

        internal ModuleHolder(ModuleAssemblyLoadContext context, List<IModule> modules)
        {
            this.context = context;
            this.modules = modules;
        }
    }
}
