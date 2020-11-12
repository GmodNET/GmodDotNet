using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Third test for PushManagedClosure (throw an exception, 0 upvalues).
    public class ThirdPushManagedClosureTest : ITest
    {
        string error_message;

        public ThirdPushManagedClosureTest()
        {
            error_message = Guid.NewGuid().ToString();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                lua.PushManagedClosure(lua =>
                {
                    throw new Exception(error_message);
                }, 0);

                lua.MCall(0, 0);

                throw new Exception("MCall hasn't caught an exception");
            }
            catch(Exception e)
            {
                if(e is GmodLuaException && e.ToString().Contains(error_message))
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
