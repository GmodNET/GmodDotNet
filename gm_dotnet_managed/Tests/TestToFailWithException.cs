using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class TestToFailWithException : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();
            taskCompletion.TrySetException(new Exception[] {new Exception("This exception will always occur!")});
            return taskCompletion.Task;
        }
    }
}
