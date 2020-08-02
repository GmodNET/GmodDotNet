using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Tests
{
    //This is the test to push 1000 random bool values to lua and then read it.
    public class PushBool : ITest
    {
        TaskCompletionSource<bool> task_completion;
        string uuid;

        public PushBool()
        {
            uuid = Guid.NewGuid().ToString();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            task_completion = new TaskCompletionSource<bool>();

            Random rand = new Random();
            
            int[] random_numbers = new int[1000];
            for(int i = 0; i < 1000; i++)
            {
                random_numbers[i] = rand.Next(2);
            }

            bool[] random_bools = new bool[1000];
            for(int i = 0; i < 1000; i++)
            {
                if(random_numbers[i] == 0)
                {
                    random_bools[i] = false;
                }
                else
                {
                    random_bools[i] = true;
                }
            }


            try
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                for (int i = 0; i < 1000; i++)
                {
                    lua.PushBool(random_bools[i]);
                    lua.SetField(-2, this.uuid + "Bool" + i.ToString());
                }
                for (int i = 0; i < 1000; i++)
                {
                    lua.GetField(-1, this.uuid + "Bool" + i.ToString());
                    bool tmp = lua.GetBool(-1);
                    lua.Pop(1);

                    if(tmp != random_bools[i])
                    {
                        throw new PushBoolException(i, random_bools[i], tmp);
                    }
                }
                lua.Pop(1);

                task_completion.TrySetResult(true);
            }
            catch(Exception e)
            {
                task_completion.TrySetException(new Exception[] { e });
            }

            return task_completion.Task;
        }
    }

    public class PushBoolException : Exception
    {
        string message;
        public PushBoolException(int count, bool expected_value, bool recieved_value)
        { 
            message = "Bool mismatch on after " + (count) + " bool values. Expected " + expected_value.ToString() + " but recieved " + recieved_value.ToString();
        }

        public override string Message => this.message;
    }
}
