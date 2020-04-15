using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GmodNET.API;

namespace Tests
{
    // The ILua.Push test
    public class PushTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                string first_string = Guid.NewGuid().ToString();
                string second_string = Guid.NewGuid().ToString();

                int number_of_items_on_stack = lua.Top();

                lua.PushString(first_string);
                lua.PushString(second_string);
                lua.Push(-2);

                string received_string = lua.GetString(-1);
                string another_received_string = lua.GetString(-2);

                lua.Pop(3);

                if(received_string != first_string || another_received_string != second_string)
                {
                    throw new PushTestException("Received strings are invalid");
                }

                if(lua.Top() != number_of_items_on_stack)
                {
                    throw new PushTestException("Not all strings were poped from the stack");
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

    public class PushTestException : Exception
    {
        public PushTestException(string message) : base(message)
        {

        }
    }
}
