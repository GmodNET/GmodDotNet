using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Tests usage of Managed Function delegates as metamethods
    public class MetaMethodsPushManagedFunctionTest : ITest
    {
        int custom_type_id;
        long random_integer;

        public MetaMethodsPushManagedFunctionTest()
        {
            random_integer = new Random().Next(1, int.MaxValue);
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                int stack_state = lua.Top();

                custom_type_id = lua.CreateMetaTable(Guid.NewGuid().ToString());
                lua.PushManagedFunction(lua =>
                {
                    long value = (long)lua.GetUserType(1, custom_type_id);
                    lua.Pop(1);
                    lua.PushString((value * 2).ToString());
                    return 1;
                });
                lua.SetField(-2, "__call");
                lua.Pop(1);

                lua.PushUserType((IntPtr)random_integer, custom_type_id);
                lua.MCall(0, 1);
                string ret_string = lua.GetString(-1);
                lua.Pop(1);

                lua.PushMetaTable(custom_type_id);
                lua.PushNil();
                lua.SetField(-2, "__call");
                lua.Pop(1);

                if((stack_state - lua.Top()) != 0)
                {
                    throw new Exception("Lua stack has some values left");
                }

                if(ret_string != (random_integer * 2).ToString())
                {
                    throw new Exception("Return string is incorrect");
                }

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
