using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // First test for PushManagedFunction
    public class FirstPushManagedFunctionTest : ITest
    {
        string random_string;

        public FirstPushManagedFunctionTest()
        {
            random_string = Guid.NewGuid().ToString();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                lua.PushManagedFunction((lua) =>
                {
                    lua.PushString(random_string);
                    return 1;
                });
                lua.MCall(0, 1);
                string ret_string = lua.GetString(-1);
                lua.Pop(1);

                if(ret_string != random_string)
                {
                    throw new Exception("Return string is invalid");
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
