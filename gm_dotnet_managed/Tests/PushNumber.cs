using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This test creates 10 random double numbers, pushes them to GMOD, gets them back and compares
    public class PushNumber : ITest
    {
        TaskCompletionSource<bool> taskSource;

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            taskSource = new TaskCompletionSource<bool>();

            try
            {
                string LuaNumId = Guid.NewGuid().ToString();

                double[] Random_numbers = new double[10];

                Random rand = new Random();

                for(int i = 0; i < 10; i++)
                {
                    Random_numbers[i] = rand.NextDouble();
                }

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);

                for(int i = 0; i < 10; i++)
                {
                    lua.PushNumber(Random_numbers[i]);
                    lua.SetField(-2, LuaNumId + "Num" + i.ToString());
                }

                for(int i = 0; i < 10; i++)
                {
                    lua.GetField(-1, LuaNumId + "Num" + i.ToString());
                    double tmp = lua.GetNumber(-1);
                    lua.Pop(1);

                    if (tmp != Random_numbers[i])
                    { 
                        throw new PushNumberException(i, Random_numbers[i], tmp);
                    }
                }

                taskSource.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskSource.TrySetException(new Exception[] { e });
            }

            return taskSource.Task;
        }
    }

    public class PushNumberException : Exception
    {
        string message;

        public PushNumberException(int count, double expected, double recieved)
        { 
            message = "Number mismatch after " + count + " pushed values. Expected number " + expected + " but recieved " + recieved;
        }

        public override string Message => message;
    }
}
