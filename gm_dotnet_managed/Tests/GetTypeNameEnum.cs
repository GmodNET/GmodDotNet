using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Tests ILua.GetTypeName(TYPES) method
    public class GetTypeNameEnum : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                if(lua.GetTypeName(TYPES.NIL) != "nil")
                {
                    throw new Exception("GetTypeName returned incorrect name for the type NIL");
                }

                if(lua.GetTypeName(TYPES.STRING) != "string")
                {
                    throw new Exception("GetTypeName returned incorrect name for the type STRING");
                }

                if(lua.GetTypeName(TYPES.NUMBER) != "number")
                {
                    throw new Exception("GetTypeName returned incorrect name for the type NUMBER");
                }

                if(lua.GetTypeName(TYPES.BOOL) != "bool")
                {
                    throw new Exception("GetTypeName returned incorrect name for the type BOOL");
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
