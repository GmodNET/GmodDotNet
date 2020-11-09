using GmodNET.API;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Second PushManagedFunction test. Throwing exception in managed function.
    public class SecondPushManagedFunctionTest : ITest
    {
        string random_string;

        public SecondPushManagedFunctionTest()
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
                    throw new Exception(random_string);
                });
                lua.MCall(0, 0);

                throw new Exception("The managed exception was not caught by MCall.");
            }
            catch (Exception e)
            {
                if (e is GmodLuaException && e.Message.Contains(random_string))
                {
                    taskCompletion.TrySetResult(true);
                }
                else
                {
                    taskCompletion.TrySetException(new Exception[] { e });
                }
            }

            return taskCompletion.Task;
        }
    }
}
