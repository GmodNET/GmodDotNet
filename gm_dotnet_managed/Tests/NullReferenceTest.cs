using System;
using System.Threading.Tasks;
using GmodNET.API;

namespace Tests
{
    // This test checks that CoreCLR can catch NullReferenceException (which is important on *nix systems since it uses SIGSEGV handling to detect)
    public class NullReferenceTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                unsafe
                {
                    int* ptr = null;
                    Console.WriteLine(*ptr);
                }
            }
            catch (NullReferenceException e)
            {
                // Null reference successfully caught
                taskCompletion.TrySetResult(true);
            }

            return taskCompletion.Task;
        }
    }
}