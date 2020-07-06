using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.GetTable and ILua.RawGet
    public class GetTableAndRawGet : ITest
    {
        string RandomString1;
        string RandomString2;

        int TypeId;

        CFuncManagedDelegate indexDelegate;

        GetILuaFromLuaStatePointer lua_extructor;

        public GetTableAndRawGet()
        {
            TypeId = -1;

            RandomString1 = Guid.NewGuid().ToString();
            RandomString2 = Guid.NewGuid().ToString();

            indexDelegate = (lua_State) =>
            {
                ILua lua = this.lua_extructor(lua_State);

                lua.Pop(lua.Top());

                lua.PushString(this.RandomString1);

                return 1;
            };
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                this.lua_extructor = lua_extructor;
                 
                // Create new metatable and populate it
                this.TypeId = lua.CreateMetaTable("GetTableAndRawGetMetaTable");
                lua.PushCFunction(this.indexDelegate);
                lua.SetField(-2, "__index");
                lua.Pop(1);

                // Create a test table
                lua.CreateTable();
                lua.PushString(this.RandomString2);
                lua.SetField(-2, "TestVal");
                lua.PushMetaTable(this.TypeId);
                lua.SetMetaTable(-2);

                // Test GetTable
                lua.PushString("TestVal");
                lua.GetTable(-2);
                string receivedString1 = lua.GetString(-1);
                if(receivedString1 != this.RandomString2)
                {
                    throw new Exception("GetTable returned invalid string on existing key");
                }
                lua.Pop(1);

                lua.PushString("ArbitraryString");
                lua.GetTable(-2);
                string receivedString11 = lua.GetString(-1);
                if(receivedString11 != this.RandomString1)
                {
                    throw new Exception("GetTable returned invalid string on non-existing key");
                }
                lua.Pop(1);

                // Test RawGet
                lua.PushString("TestVal");
                lua.RawGet(-2);
                string receivedString2 = lua.GetString(-1);
                if(receivedString2 != this.RandomString2)
                {
                    throw new Exception("RawGet returned invalid string on existing key");
                }
                lua.Pop(1);

                lua.PushString("ArbitraryString");
                lua.RawGet(-2);
                int received_type = lua.GetType(-1);
                if(received_type != (int)TYPES.NIL)
                {
                    throw new Exception("RawGet didn't return NIL on non-existing key");
                }
                lua.Pop(1);

                lua.Pop(lua.Top());

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
