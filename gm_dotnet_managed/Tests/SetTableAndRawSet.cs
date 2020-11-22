using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for the ILua.SetTable and ILua.RawSet methods
    public class SetTableAndRawSet : ITest
    {
        int type_id;
        GetILuaFromLuaStatePointer lua_extructor;
        string random1;
        string random2;

        Func<ILua, int> newIndexImpl;

        public SetTableAndRawSet()
        {
            type_id = -1;

            random1 = Guid.NewGuid().ToString();
            random2 = Guid.NewGuid().ToString();

            newIndexImpl = (lua) =>
            {
                lua.Pop(lua.Top());

                return 0;
            };
        }
        
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                this.lua_extructor = lua_extructor;

                // Create new type
                this.type_id = lua.CreateMetaTable("SetTableAndRawSetTestType");
                lua.PushManagedFunction(this.newIndexImpl);
                lua.SetField(-2, "__newindex");
                lua.Pop(1);

                // Create test table
                lua.CreateTable();
                lua.PushString(random1);
                lua.SetField(-2, "Val");
                lua.PushMetaTable(this.type_id);
                lua.SetMetaTable(-2);

                // Test SetTable
                lua.PushString("Val");
                lua.PushString(random1 + random1);
                lua.SetTable(-3);
                lua.GetField(-1, "Val");
                string received_string = lua.GetString(-1);
                lua.Pop(1);
                if(received_string != random1 + random1)
                {
                    throw new Exception("SetTable didn't set a value for an existing key");
                }

                lua.PushString("ArbitraryKey");
                lua.PushString("ArbitraryString");
                lua.SetTable(-3);
                lua.GetField(-1, "ArbitraryKey");
                int received_type = lua.GetType(-1);
                lua.Pop(1);
                if(received_type != (int)TYPES.NIL)
                {
                    throw new Exception("SetTable ignored overriden __newindex");
                }

                lua.PushString("Val2");
                lua.PushString(random2);
                lua.RawSet(-3);
                lua.GetField(-1, "Val2");
                string received_string2 = lua.GetString(-1);
                lua.Pop(1);
                if(received_string2 != random2)
                {
                    throw new Exception("RawSet didn't set a value to a key");
                }

                lua.Pop(1);

                lua.PushMetaTable(type_id);
                lua.PushNil();
                lua.SetField(-2, "__newindex");
                lua.Pop(1);

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }
    }
}
