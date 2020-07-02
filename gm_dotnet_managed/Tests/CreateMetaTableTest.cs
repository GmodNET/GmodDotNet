using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.CreateMetaTable and ILua.PushMetaTable
    public class CreateMetaTableTest : ITest
    {
        CFuncManagedDelegate ToStringImpl;
        string RandomString;

        GetILuaFromLuaStatePointer lua_extructor;

        int NewTypeId;

        public CreateMetaTableTest()
        {
            RandomString = Guid.NewGuid().ToString();

            NewTypeId = 0;

            ToStringImpl = (lua_state) =>
            {
                ILua lua = this.lua_extructor(lua_state);

                lua.PushString(this.RandomString);

                return 1;
            };
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                this.lua_extructor = lua_extructor;

                this.NewTypeId = lua.CreateMetaTable("TestType1");

                lua.PushCFunction(this.ToStringImpl);

                lua.SetField(-2, "__tostring");

                lua.Pop(1);


                //Create new table to test newly created metatable

                lua.CreateTable();

                lua.PushMetaTable(this.NewTypeId);

                lua.SetMetaTable(-2);
                /*
                if(!lua.IsType(-1, this.NewTypeId))
                {
                    throw new Exception("Received type id is invalid");
                }

                string received_type_name = lua.GetTypeName(lua.GetType(-1));

                if(received_type_name != "TestType1")
                {
                    throw new Exception("Received type name is invalid");
                }
                */

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);

                lua.GetField(-1, "tostring");

                lua.Push(-3);

                lua.MCall(1, 1);

                if(lua.GetString(-1) != this.RandomString)
                {
                    throw new Exception("Metatable method __tostring returned incorrect string");
                }

                lua.Pop(3);

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
