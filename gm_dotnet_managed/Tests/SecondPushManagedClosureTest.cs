using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Second test for PushManagedClosure (some args, some returns, 0 upvalues)
    public class SecondPushManagedClosureTest : ITest
    {
        string random1;
        string random2;
        double random3;

        public SecondPushManagedClosureTest()
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
                lua.PushManagedClosure(lua =>
                {
                    if(lua.Top() != 3)
                    {
                        throw new Exception("Closure execution stack has incorect number of items.");
                    }

                    string one = lua.GetString(1);
                    string two = lua.GetString(2);
                    double three = lua.GetNumber(3);

                    lua.Pop(3);

                    lua.PushString(one + three);
                    lua.PushString(three + two);

                    if(lua.Top() != 2)
                    {
                        throw new Exception("Closure execution stack has incorrect number of items after executtion.");
                    }

                    return 2;
                }, 0);

                lua.PushString(random1);
                lua.PushString(random2);
                lua.PushNumber(random3);

                lua.MCall(3, 2);

                string ret_1 = lua.GetString(-2);
                string ret_2 = lua.GetString(-1);

                lua.Pop(2);

                if(ret_1 != random1 + random3)
                {
                    throw new Exception("First return string is incorrect.");
                }

                if(ret_1 != random3 + random2)
                {
                    throw new Exception("Second return string is incorrect.");
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
