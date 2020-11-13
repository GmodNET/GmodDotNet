using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // First test for PushManagedClosure (0 upvalues)
    public class FirstPushManagedClosureTest : ITest
    {
        string random;

        public FirstPushManagedClosureTest()
        {
            random = Guid.NewGuid().ToString();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                lua.PushManagedClosure(l =>
                {
                    if(l.Top() != 0)
                    {
                        throw new Exception("Closure execution stack is non-empty.");
                    }
                    l.PushString(random);
                    return 1;
                }, 0);
                lua.MCall(0, 1);
                string ret_string = lua.GetString(-1);
                lua.Pop(1);

                if (ret_string != random)
                {
                    throw new Exception("Return string is incorrect.");
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
