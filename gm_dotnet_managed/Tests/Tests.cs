using System;
using System.Runtime.Loader;
using GmodNET.API;
using System.Diagnostics;
using System.IO;

namespace Tests
{
    public class Tests : IModule
    {
        public string ModuleName => "Test suit for Gmod.NET";

        public string ModuleVersion => "1.0.0";

        ILua lua;

        bool isServerSide;

        GetILuaFromLuaStatePointer getter;

        AssemblyLoadContext current_load_context;

        CFuncManagedDelegate on_tick;

        bool WasServerQuitTrigered;

        public Tests()
        {
            WasServerQuitTrigered = false;
        }

        public void Load(ILua LuaInterface, bool is_serverside, GetILuaFromLuaStatePointer del, AssemblyLoadContext assembly_context)
        {
            this.lua = LuaInterface;
            this.isServerSide = is_serverside;
            this.getter = del;
            this.current_load_context = assembly_context;

            on_tick = (state) =>
            {
                ILua lua = getter(state);

                if(!this.WasServerQuitTrigered)
                {
                    File.WriteAllText("tests-success.txt", "Success!");

                    lua.Print("Terminating game process");

                    lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                    lua.GetField(-1, "engine");
                    lua.GetField(-1, "CloseServer");

                    lua.Call(0, 0);

                    lua.Pop(lua.Top());

                    this.WasServerQuitTrigered = true;
                }

                return 0;
            };

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, "hook");
            lua.GetField(-1, "Add");
            lua.PushString("Tick");
            lua.PushString("main_on_tick_listener");
            lua.PushCFunction(on_tick);
            lua.Call(3, 0);
            
            lua.Pop(lua.Top());
        }

        public void Unload()
        {
            
        }
    }
}
