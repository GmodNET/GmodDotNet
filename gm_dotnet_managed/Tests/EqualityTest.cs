using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua.Equal and ILua.RawEqual
    public class EqualityTest : ITest
    {
        GetILuaFromLuaStatePointer lua_extructor;

        Func<ILua, int> eq_func;

        public EqualityTest()
        {
            eq_func = (lua) =>
            {
                lua.Pop(lua.Top());

                lua.PushBool(true);

                return 1;
            };
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            this.lua_extructor = lua_extructor;

            try
            {
                // Create metatable
                lua.CreateTable();
                lua.PushManagedFunction(this.eq_func);
                lua.SetField(-2, "__eq");

                // Create first table to compare
                lua.CreateTable();
                lua.PushNumber(1);
                lua.SetField(-2, "A");
                lua.Push(-2);
                lua.SetMetaTable(-2);

                // Create second table to compare
                lua.CreateTable();
                lua.PushNumber(2);
                lua.SetField(-2, "A");
                lua.Push(-3);
                lua.SetMetaTable(-2);

                // Get compare results
                bool equal_result = lua.Equal(-1 , -2);
                bool raw_equal_result = lua.RawEqual(-1, -2);

                lua.Pop(3);

                if(!equal_result)
                {
                    throw new EqualityTestException("ILua.Equal returned false but must return true");
                }

                if(raw_equal_result)
                {
                    throw new EqualityTestException("ILua.RawEqual returned true but must return false");
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

    public class EqualityTestException : Exception
    {
        public EqualityTestException(string message) : base(message)
        {

        }
    }
}
