using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    class ConsoleTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();
            try
            {
                // THIS IS BAD
                Task asyncTask = Task.Run(() =>
                {
                    Console.WriteLine("~Async started~");
                    Console.WriteLine("that's a string");
                    Console.WriteLine('a');
                    Console.WriteLine(12345);
                    Console.WriteLine(1.23456789);
                    Console.WriteLine((float)12.123456789);
                    Console.WriteLine((decimal)23.456789);
                    Console.WriteLine(true);
                    Console.WriteLine((object)null);
                    Console.WriteLine("~Async ended~");
                });

                Console.WriteLine("~Sync started~");
                Console.WriteLine("that's a string");
                Console.WriteLine('a');
                Console.WriteLine(12345);
                Console.WriteLine(1.23456789);
                Console.WriteLine((float)12.123456789);
                Console.WriteLine((decimal)23.456789);
                Console.WriteLine(true);
                Console.WriteLine((object)null);
                Console.WriteLine("~Sync ended~");

                asyncTask.Wait();
                taskCompletion.TrySetResult(true);
            }
            catch (Exception e)
            {
                taskCompletion.TrySetException(e);
            }
            return taskCompletion.Task;
        }
    }
}
