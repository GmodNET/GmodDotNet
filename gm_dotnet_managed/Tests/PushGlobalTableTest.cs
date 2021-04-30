using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.PushGlobalTable
    public class PushGlobalTableTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                string random_key1 = Guid.NewGuid().ToString();
                string random_key2 = Guid.NewGuid().ToString();
                string random_key3 = Guid.NewGuid().ToString();

                string random1 = Guid.NewGuid().ToString();
                string random2 = Guid.NewGuid().ToString();
                string random3 = Guid.NewGuid().ToString();

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.PushString(random1);
                lua.SetField(-2, random_key1);

                lua.PushString(random2);
                lua.SetField(-2, random_key2);

                lua.PushString(random3);
                lua.SetField(-2, random_key3);

                lua.Pop(lua.Top());

                lua.PushGlobalTable();

                lua.GetField(-1, random_key1);
                if(lua.GetString(-1) != random1)
                {
                    throw new Exception("First random string is invalid");
                }
                lua.Pop(1);

                lua.GetField(-1, random_key2);
                if (lua.GetString(-1) != random2)
                {
                    throw new Exception("Second random string is invalid");
                }
                lua.Pop(1);

                lua.GetField(-1, random_key3);
                if (lua.GetString(-1) != random3)
                {
                    throw new Exception("Third random string is invalid");
                }
                lua.Pop(1);

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
