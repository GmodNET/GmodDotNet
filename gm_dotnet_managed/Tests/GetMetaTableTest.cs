using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    //Test for ILua.GetMetaTable
    public class GetMetaTableTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                lua.PushAngle(new System.Numerics.Vector3(0));
                if(!lua.GetMetaTable(-1))
                {
                    throw new GetMetaTableTestException("GetMetaTable returned false");
                }
                lua.GetField(-1, "IsZero");
                lua.Push(-3);
                lua.MCall(1, 1);
                bool received_bool = lua.GetBool(-1);
                lua.Pop(3);
                if(!received_bool)
                {
                    throw new GetMetaTableTestException("Meta function returned False but must return True");
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

    public class GetMetaTableTestException : Exception
    {
        public GetMetaTableTestException(string message) : base(message)
        {

        }
    }
}
