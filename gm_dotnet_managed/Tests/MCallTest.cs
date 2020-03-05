using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This is a test for ILua.MCall
    public class MCallTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                Random rand = new Random();

                int random_number = rand.Next(1, 500);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "tonumber");
                lua.PushString(random_number.ToString());
                try
                {
                    lua.MCall(1, 1);
                }
                catch(GmodLuaException e)
                {
                    throw new MCallTestException("Exception was thrown by MCall, but it is not expected.");
                }
                int received_number = (int)lua.GetNumber(-1);
                if(received_number != random_number)
                {
                    throw new MCallTestException("Numbers mismatch.");
                }
                lua.Pop(2);

                string random_error_message = Guid.NewGuid().ToString();

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "assert");
                lua.PushBool(false);
                lua.PushString(random_error_message);
                try
                {
                    lua.MCall(2, 0);
                }
                catch(GmodLuaException e)
                {
                    if(e.Message != random_error_message)
                    {
                        throw new MCallTestException("Error message is invalid");
                    }

                    taskCompletion.TrySetResult(true);
                }
                catch(Exception e)
                {
                    throw new MCallTestException("Wrong exception type was thrown");
                }
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }
    }

    public class MCallTestException : Exception
    {
        public MCallTestException(string message) : base(message)
        {

        }
    }
}
