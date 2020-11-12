using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Fifth test for PushManagedClosure (2 upvalues and 1 arg);
    public class FifthPushManagedClosureTest : ITest
    {
        string random1;
        string random2;
        double random3;

        public FifthPushManagedClosureTest()
        {
            random1 = Guid.NewGuid().ToString();
            random2 = Guid.NewGuid().ToString();
            random3 = new Random().NextDouble();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                int stack_state = lua.Top();

                lua.PushString(random1);
                lua.PushString(random2);
                lua.PushManagedClosure(lua =>
                {
                    if (lua.Top() != 1)
                    {
                        throw new Exception("Managed closure execution stack has incorrect number of items");
                    }

                    double num = lua.GetNumber(1);

                    lua.Pop(1);

                    string first = lua.GetString(GmodInterop.GetUpvalueIndex(1));
                    string second = lua.GetString(GmodInterop.GetUpvalueIndex(2));

                    lua.PushString(first + num + second);

                    return 1;
                }, 2);

                if(lua.Top() != stack_state + 1)
                {
                    throw new Exception("Wrong number of items left on the stack");
                }

                lua.PushNumber(random3);

                lua.MCall(1, 1);

                string ret = lua.GetString(-1);
                lua.Pop(1);

                if(ret != random1 + random3 + random2)
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
