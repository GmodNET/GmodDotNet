using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This test creates a new table, populates it with some data, and tries to get the data back
    public class CreateTable : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                string table_name = Guid.NewGuid().ToString();
                string field_name = Guid.NewGuid().ToString();

                Random rand = new Random();
                double rand_num = rand.NextDouble();

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.CreateTable();
                lua.PushNumber(rand_num);
                lua.SetField(-2, field_name);
                lua.SetField(-2, table_name);

                lua.Pop(1);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, table_name);
                if(lua.GetType(-1) != (int)TYPES.TABLE)
                {
                    throw new CreateTableException("Type check failed");
                }
                lua.GetField(-1, field_name);
                double get_num = lua.GetNumber(-1);
                if(get_num != rand_num)
                {
                    throw new CreateTableException("Wrong number recieved");
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

    public class CreateTableException : Exception
    {
        string message;

        public CreateTableException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
