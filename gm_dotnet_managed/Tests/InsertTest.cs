using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.Insert
    public class InsertTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                Random rand = new Random();

                int first = rand.Next(0, 10000000);
                int second = rand.Next(0, 10000000);
                int third = rand.Next(0, 10000000);

                lua.PushNumber(first);
                lua.PushNumber(second);
                lua.PushNumber(third);

                lua.Insert(-2);

                int received_first = (int)lua.GetNumber(-3);
                int received_second = (int)lua.GetNumber(-2);
                int received_third = (int)lua.GetNumber(-1);

                lua.Pop(3);

                if(!(received_first == first && received_second == third && received_third == second))
                {
                    throw new InsertTestException("Received numbers are invalid");
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

    public class InsertTestException : Exception
    {
        public InsertTestException(string message) : base(message)
        {

        }
    }
}
