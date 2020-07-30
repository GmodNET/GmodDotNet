using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Tests
{
    // This test checks GmodNET runtime's ability to load module's native library dependencies: this test tries to use functions from NSec Crypto library, which requires
    // libsodium native library
    public class NativeDependencies : ITest
    {
        [DllImport("gmodTestLib.dll", EntryPoint = "hello")]
        public static extern int NativeTestFunc(int x);

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                int random_int = new Random().Next(0, 1000);
                int result = NativeTestFunc(random_int);

                if(result != random_int + 3)
                {
                    throw new Exception("NativeTestFunc returned invalid value");
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
}
