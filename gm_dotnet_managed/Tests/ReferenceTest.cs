using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.ReferenceCreate, ILua.ReferencePush and ILua.ReferenceFree
    public class ReferenceTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                string random_string = Guid.NewGuid().ToString();

                lua.PushString(random_string);

                int reference = lua.ReferenceCreate();

                if(lua.GetType(-1) == (int)TYPES.STRING && lua.GetString(-1) == random_string)
                {
                    throw new ReferenceTestException("String wasn't poped from the stack");
                }

                lua.ReferencePush(reference);

                if(lua.GetType(-1) == (int)TYPES.STRING && lua.GetString(-1) != random_string)
                {
                    throw new ReferenceTestException("Reference wasn't pushed to the stack");
                }

                lua.Pop(1);

                lua.ReferenceFree(reference);

                lua.ReferencePush(reference);

                if(lua.GetType(-1) == (int)TYPES.STRING && lua.GetString(-1) == random_string)
                {
                   throw new ReferenceTestException("Reference wasn't freed");
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

    public class ReferenceTestException : Exception
    {
        public ReferenceTestException(string message) : base(message)
        {

        }
    }
}
