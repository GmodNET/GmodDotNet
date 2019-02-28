using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Gmod.NET
{
    /// <summary>
    /// Main class of the Gmod.NET
    /// </summary>
    static class Gmod
    {
        //Modules storage
        static List<IModule> Modules = new List<IModule>();
        //Entry point of the Gmod.NET
        static void Main()
        {
            // Print welcome message
            Util.ConsolePrint("Gmod.NET loaded! Version: " + Version.Major + "." + Version.Minor + "." + Version.Misc + " " +
            Version.Name);
            // Create GmodNET table and fill it with entries
            lock (Lua.Engine)
            {
                Lua.Engine.PushSpecial(SpecialTables.SPECIAL_GLOB);
                Lua.Engine.CreateTable();
                Lua.Engine.PushFunction(UnloadDelegate);
                Lua.Engine.SetField(-2, "UnloadModules");
                Lua.Engine.PushFunction(ReloadDelegate);
                Lua.Engine.SetField(-2, "ReloadModules");
                Lua.Engine.SetField(-2, "GmodNET");
                Lua.Engine.Pop(1);
            }
            // Register command line commands
            // Register dotnet_unload
            lock (Lua.Engine)
            {
                Lua.Engine.PushSpecial(SpecialTables.SPECIAL_GLOB);
                Lua.Engine.GetField(-1, "concommand");
                Lua.Engine.GetField(-1, "Add");
                // name
                Lua.Engine.PushString("dotnet_unload");
                //Callback
                Lua.Engine.PushFunction(UnloadDelegateConsole);
                //autoComplete
                Lua.Engine.PushNil();
                //helpText
                Lua.Engine.PushString("Unload all Gmod.NET modules");
                //Flags
                Lua.Engine.PushNumber(0);
                //Make call
                Lua.Engine.Call(5, 0);
                //Pop tables
                Lua.Engine.Pop(2);
            }

            // Run LoadMoudules
            LoadModules();
        }

        static void LoadModules()
        {

        }

        static void UnloadModules()
        {
            foreach (IModule m in Modules)
            {
                m.CleanUp();
            }
            Modules.Clear();
        }

        static void ReloadMoules()
        {
            UnloadModules();
            LoadModules();
        }

        // Delegate to expose Unload() to Garry's mod lua
        static GarrysModFunc UnloadDelegate = (ptr) =>
        {
            Util.ConsolePrint("Unloading NET Modules...");
            UnloadModules();
            Util.ConsolePrint("NET Modules were unloaded!");
            return 0;
        };
        // Delgate to expose Reload() to Garry's mod lua
        static GarrysModFunc ReloadDelegate = (ptr) =>
        {
            Util.ConsolePrint("Reloading NET Modules...");
            ReloadMoules();
            Util.ConsolePrint("NET Modiles were reloaded!");
            return 0;
        };

        static GarrysModFunc UnloadDelegateConsole = (ptr) =>
        {
            lock(Lua.Engine)
            {
                // Pop arguments
                Lua.Engine.Pop(4);
                Lua.Engine.PushSpecial(SpecialTables.SPECIAL_GLOB);
                Lua.Engine.GetField(-1, "GmodNET");
                Lua.Engine.GetField(-1, "UnloadModules");
                Lua.Engine.Call(0, 0);
                Lua.Engine.Pop(2);
            }

            return 0;
        };
    }
}
