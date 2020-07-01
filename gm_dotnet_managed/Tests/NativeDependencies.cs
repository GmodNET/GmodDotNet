using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NSec.Cryptography;

namespace Tests
{
    // This test checks GmodNET runtime's ability to load module's native library dependencies: this test tries to use functions from NSec Crypto library, which requires
    // libsodium native library
    public class NativeDependencies : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                var blake2_hasher = HashAlgorithm.Blake2b_512;
                var hash = blake2_hasher.Hash(Encoding.UTF8.GetBytes("Test"));

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
