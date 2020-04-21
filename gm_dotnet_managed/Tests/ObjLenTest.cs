using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.ObjLen
    public class ObjLenTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                string random_string = Guid.NewGuid().ToString();

                Random rand = new Random();
                int num_of_additions = rand.Next(5, 11);

                for(int i = 0; i < num_of_additions; i++)
                {
                    random_string += Guid.NewGuid().ToString();
                }

                lua.PushString(random_string);

                int received_len = lua.ObjLen(-1);

                lua.Pop(1);

                if(received_len != random_string.Length)
                {
                    throw new ObjLenTestException("Received length of the string is invalid");
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

    public class ObjLenTestException : Exception
    {
        public ObjLenTestException(string message) : base(message)
        {

        }
    }
}
