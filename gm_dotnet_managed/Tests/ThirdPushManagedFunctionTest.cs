using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Third test for PushManagedFunction. Tries to work with multiple args and returns.
    public class ThirdPushManagedFunctionTest : ITest
    {
        string random_string_1;
        string random_string_2;
        double random_number;

        public ThirdPushManagedFunctionTest()
        {
            random_string_1 = Guid.NewGuid().ToString();
            random_string_2 = Guid.NewGuid().ToString();
            random_number = new Random().NextDouble();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                lua.PushManagedFunction((lua) =>
                {
                    int stack_items = lua.Top();
                    if(stack_items != 3)
                    {
                        throw new Exception("The number of items on the execution stack is incorrect");
                    }

                    string first = lua.GetString(1);
                    string second = lua.GetString(2);
                    double third = lua.GetNumber(3);

                    lua.Pop(3);

                    lua.PushString(first + third);
                    lua.PushString(third + second);

                    return 2;
                });

                lua.PushString(random_string_1);
                lua.PushString(random_string_2);
                lua.PushNumber(random_number);

                lua.MCall(3, 2);

                string ret_1 = lua.GetString(-2);
                string ret_2 = lua.GetString(-1);

                lua.Pop(2);

                if(ret_1 != random_string_1 + random_number)
                {
                    throw new Exception("First return string is incorrect");
                }

                if(ret_2 != random_number + random_string_2)
                {
                    throw new Exception("Second return string is incorrect");
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
