using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This test checks that lua.Pop() and lua.Top() work properly
    public class PopTopTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                int initial_number_of_values = lua.Top();

                Random rand = new Random();

                int count = rand.Next(5, 16);

                for(int i = 0; i < count; i++)
                {
                    lua.PushNumber(1);
                }

                int first_get = lua.Top();

                lua.Pop(1);

                int second_get = lua.Top();

                if(first_get != initial_number_of_values + count || second_get != initial_number_of_values + count - 1)
                {
                    throw new PopTopException("Test failed");
                }

                lua.Pop(second_get - initial_number_of_values);

                int last_get = lua.Top();

                if(last_get != initial_number_of_values)
                {
                    throw new PopTopException("Test failed on last check");
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

    public class PopTopException : Exception
    {
        string message;

        public PopTopException(string mess)
        {
            message = mess;
        }

        public override string Message => message;
    }
}
