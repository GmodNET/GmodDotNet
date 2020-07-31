using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.IsType(TYPES) method
    public class IsTypeEnum : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                lua.PushNumber(1);

                lua.PushString("I love Dima. Dima is the best.");

                lua.PushBool(true);

                lua.PushNil();

                if(!lua.IsType(-4, TYPES.NUMBER))
                {
                    throw new Exception("IsType returned false on NUMBER type");
                }

                if(!lua.IsType(-3, TYPES.STRING))
                {
                    throw new Exception("IsType returned false on STRING type");
                }

                if(!lua.IsType(-2, TYPES.BOOL))
                {
                    throw new Exception("IsType returned false on BOOL type");
                }

                if(!lua.IsType(-1, TYPES.NIL))
                {
                    throw new Exception("IsType returned false on NIL type");
                }

                lua.Pop(4);

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
