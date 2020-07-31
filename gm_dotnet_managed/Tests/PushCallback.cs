using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This test pushes managed function which takes and returnes 3 numbers to GM, register it with Global Table, and then calls it as Lua function
    public class PushCallback : ITest
    {
        TaskCompletionSource<bool> taskCompletion;
        GetILuaFromLuaStatePointer lua_extructor;
        CFuncManagedDelegate OnTickDelegate;
        CFuncManagedDelegate CallbackDelegate;
        string HookId;
        string FuncName;

        public PushCallback()
        { 
            taskCompletion = new TaskCompletionSource<bool>();

            HookId = Guid.NewGuid().ToString();
            FuncName = Guid.NewGuid().ToString();

            OnTickDelegate = OnTick;
            CallbackDelegate = Callback;
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            this.lua_extructor = lua_extructor;

            try
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);

                lua.PushCFunction(CallbackDelegate);
                lua.SetField(-2, FuncName);

                lua.GetField(-1, "hook");
                lua.GetField(-1, "Add");
                lua.PushString("Tick");
                lua.PushString(HookId);
                lua.PushCFunction(OnTickDelegate);
                lua.Call(3, 0);

                lua.Pop(2);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }

        int OnTick(IntPtr lua_state)
        {
            try
            {
                ILua lua = this.lua_extructor(lua_state);

                Random rand = new Random();

                double In1 = (double)rand.Next(100);
                double In2 = (double)rand.Next(100);
                double In3 = (double)rand.Next(100);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, this.FuncName);
                lua.PushNumber(In1);
                lua.PushNumber(In2);
                lua.PushNumber(In3);

                lua.Call(3, 3);

                double Out1 = lua.GetNumber(-3);
                double Out2 = lua.GetNumber(-2);
                double Out3 = lua.GetNumber(-1);

                lua.Pop(lua.Top());

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "hook");
                lua.GetField(-1, "Remove");
                lua.PushString("Tick");
                lua.PushString(HookId);
                lua.Call(2, 0);

                lua.Pop(lua.Top());

                if(!(In1 + 1 == Out1 && In2 + 1 == Out2 && In3 + 1 == Out3))
                {
                    throw new PushCallbackException();
                }

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return 0;
        }

        int Callback(IntPtr lua_state)
        {
            double FirstArg;
            double SecondArg;
            double ThirdArg;

            ILua lua = this.lua_extructor(lua_state);

            FirstArg = lua.GetNumber(1);
            SecondArg = lua.GetNumber(2);
            ThirdArg =lua.GetNumber(3);

            lua.Pop(lua.Top());

            double Res1 = FirstArg + 1;
            double Res2 = SecondArg + 1;
            double Res3 = ThirdArg + 1;

            lua.PushNumber(Res1);
            lua.PushNumber(Res2);
            lua.PushNumber(Res3);

            return 3;
        }
    }

    public class PushCallbackException : Exception
    {
        public override string Message => "PushCallbackTest failed";
    }
}
