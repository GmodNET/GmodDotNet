using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This test creates metatable and checks if it works
    public class CreateMetaTable : ITest
    {
        TaskCompletionSource<bool> taskCompletion;
        string global_field_id;
        string to_str_msg;
        Func<ILua, int> MetaToStringDelegate;

        GetILuaFromLuaStatePointer lua_extructor;

        public CreateMetaTable()
        {
            taskCompletion = new TaskCompletionSource<bool>();

            global_field_id = Guid.NewGuid().ToString();

            to_str_msg = Guid.NewGuid().ToString();

            MetaToStringDelegate = MetaToString;
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            this.lua_extructor = lua_extructor;

            try
            {
                lua.CreateTable();
                lua.CreateTable();
                lua.PushManagedFunction(MetaToStringDelegate);
                lua.SetField(-2, "__tostring");
                lua.SetMetaTable(-2);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "tostring");
                lua.Push(-3);
                lua.MCall(1, 1);

                string get_val = lua.GetString(-1);

                lua.Pop(2);

                if(get_val != to_str_msg)
                {
                    throw new CreateMetaTableException("Recieved string is incorrect");
                }

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }

        int MetaToString(ILua lua)
        {
            lua.Pop(lua.Top());

            lua.PushString(to_str_msg);

            return 1;
        }
    }

    public class CreateMetaTableException : Exception
    {
        string message;

        public CreateMetaTableException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
