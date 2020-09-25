using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class FailingTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new();

            taskCompletion.TrySetException(new Exception[] { new Exception("Test exception") });

            return taskCompletion.Task;
        }
    }
}
