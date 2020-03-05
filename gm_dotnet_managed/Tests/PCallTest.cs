using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This test calls function with a pcall.
    public class PCallTest : ITest
    {
        TaskCompletionSource<bool> taskCompletion;

        public PCallTest()
        {
            taskCompletion = new TaskCompletionSource<bool>();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            try
            {
                Random rand = new Random();

                int random_number = rand.Next(1, 500);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "tonumber");
                lua.PushString(random_number.ToString());
                if(lua.PCall(1, 1, 0) != 0)
                {
                    throw new PCallTest_NoError_Exception("PCall returned nonzero error code");
                }
                double recieved_num = lua.GetNumber(-1);
                if(recieved_num != (double)random_number)
                {
                    throw new PCallTest_NoError_Exception("Recieved value is invalid");
                }
                lua.Pop(2);

                string random_error_msg = Guid.NewGuid().ToString();

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "assert");
                lua.PushBool(false);
                lua.PushString(random_error_msg);
                if(lua.PCall(2, 0, 0) == 0)
                {
                    throw new PCallTest_Exception("assert(false, ...) didn't throw an error");
                }
                string res_err_msg = lua.GetString(-1);
                if(res_err_msg != random_error_msg)
                {
                    throw new PCallTest_Exception("Received error message is invalid");
                }
                lua.Pop(2);

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }
    }

    public class PCallTest_NoError_Exception : Exception
    {
        string message;

        public PCallTest_NoError_Exception(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }

    public class PCallTest_Exception : Exception
    {
        string message;

        public PCallTest_Exception(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
