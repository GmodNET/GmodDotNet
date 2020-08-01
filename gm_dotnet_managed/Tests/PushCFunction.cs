using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This tests pushes a function to Garry's Mod which counts ticks and stops after 60.
    public class PushCFunction : ITest
    {
        GetILuaFromLuaStatePointer lua_extructor;
        CFuncManagedDelegate TickCounterDelegate;
        int ticks;
        string HookIdentifier;
        TaskCompletionSource<bool> taskSource;

        public PushCFunction()
        { 
            ticks = 0;
            HookIdentifier = Guid.NewGuid().ToString();
            taskSource = new TaskCompletionSource<bool>();
            TickCounterDelegate = TickCounter;
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            this.lua_extructor = lua_extructor;

            try
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "hook");
                lua.GetField(-1, "Add");
                lua.PushString("Tick");
                lua.PushString(this.HookIdentifier);
                lua.PushCFunction(this.TickCounterDelegate);
                lua.Call(3, 0);
                lua.Pop(2);
            }
            catch(Exception e)
            {
                taskSource.TrySetException(new Exception[] { e });
            }

            return taskSource.Task;
        }

        int TickCounter(IntPtr lua_state)
        {
            try
            {
                this.ticks += 1;

                if (this.ticks == 60)
                { 
                    ILua lua = lua_extructor(lua_state);

                    lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                    lua.GetField(-1, "hook");
                    lua.GetField(-1, "Remove");
                    lua.PushString("Tick");
                    lua.PushString(this.HookIdentifier);
                    lua.Call(2, 0);
                    lua.Pop(2);

                    taskSource.TrySetResult(true);
                }
            }
            catch(Exception e)
            {
                taskSource.TrySetException(new Exception[] { e });
            }
            
            return 0;
        }
    }
}
