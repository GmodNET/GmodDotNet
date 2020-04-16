using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.Remove
    public class RemoveTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                Random rand = new Random();

                int first = rand.Next(1, 10000000);
                int second = rand.Next(1, 10000000);
                int third = rand.Next(1, 10000000);

                lua.PushNumber(first);
                lua.PushNumber(second);
                lua.PushNumber(third);

                lua.Remove(-2);

                int first_received = (int)lua.GetNumber(-2);
                int second_received = (int)lua.GetNumber(-1);

                lua.Pop(2);

                if(first_received != first || second_received != third)
                {
                    throw new RemoveTestException("Received numbers are invalid");
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

    public class RemoveTestException : Exception
    {
        public RemoveTestException(string message) : base(message)
        {

        }
    }
}
