using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Checks that ILua.GetUserType returns IntPtr.Zero when iType param is not equal to userdata's type id
    public class GetUserTypeOnTypeMismatch : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                int type1 = lua.CreateMetaTable(Guid.NewGuid().ToString());
                int type2 = lua.CreateMetaTable(Guid.NewGuid().ToString());
                lua.Pop(2);

                lua.PushUserType((IntPtr)1, type1);

                if(lua.GetUserType(-1, type2) != IntPtr.Zero)
                {
                    throw new Exception("GetUserType returned non-zero pointer in the first test.");
                }

                if (lua.GetUserType(-1, type1) != (IntPtr)1)
                {
                    throw new Exception("GetUserType returned invalid pointer in the second test.");
                }

                lua.Pop(1);

                lua.PushUserType((IntPtr)2, type2);

                if(lua.GetUserType(-1, type1) != IntPtr.Zero)
                {
                    throw new Exception("GetUserType returned non-zero pointer in the third test.");
                }

                if(lua.GetUserType(-1, type2) != (IntPtr)2)
                {
                    throw new Exception("GetUserType returned invalid pointer in the fourth test.");
                }

                lua.Pop(1);

                lua.PushString(Guid.NewGuid().ToString());

                if(lua.GetUserType(-1, type1) != IntPtr.Zero)
                {
                    throw new Exception("GetUserType returned invalid pointer in the fifth test.");
                }

                lua.Pop(1);

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
