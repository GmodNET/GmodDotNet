using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Fourth test for PushManagedClosure (1 upvalue)
    public class FourthPushManagedClosureTest : ITest
    {
        string random;

        public FourthPushManagedClosureTest()
        {
            random = Guid.NewGuid().ToString();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                int lua_state = lua.Top();

                lua.PushString(random);
                lua.PushManagedClosure(lua =>
                {
                    if (lua.Top() != 0)
                    {
                        throw new Exception("Managed closure execution stack is non-empty");
                    }

                    string upvalue = lua.GetString(GmodInterop.GetUpvalueIndex(1));

                    lua.PushString(upvalue + upvalue);

                    return 1;
                }, 1);

                if(lua.Top() != lua_state + 1)
                {
                    throw new Exception("There is incorrect number of items on the Lua stack");
                }

                lua.MCall(0, 1);

                string ret = lua.GetString(-1);
                lua.Pop(1);

                if(ret != random + random)
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
