using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Tests
{
    // This test pushes different values on stack and ckecks their types
    public class CheckType : ITest
    {
        TaskCompletionSource<bool> taskCompletion;
        Func<ILua, int> test_func;

        public CheckType()
        {
            taskCompletion = new TaskCompletionSource<bool>();

            test_func = (lua_state) =>
            {
                return 0;
            };
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            try
            {
                Random rand = new Random();

                int curr_type;
                string curr_name;

                lua.PushNumber(rand.NextDouble());
                curr_type = lua.GetType(-1);
                curr_name = lua.GetTypeName(curr_type);
                if(curr_type != (int)TYPES.NUMBER  || curr_name != "number")
                {
                    throw new CheckTypeException("number");
                }
                lua.Pop(1);


                lua.PushString(Guid.NewGuid().ToString());
                curr_type = lua.GetType(-1);
                curr_name = lua.GetTypeName(curr_type);
                if(curr_type != (int)TYPES.STRING || curr_name != "string")
                {
                    throw new CheckTypeException("string");
                }
                lua.Pop(1);


                lua.PushBool(true);
                curr_type = lua.GetType(-1);
                curr_name = lua.GetTypeName(curr_type);
                if(curr_type != (int)TYPES.BOOL || curr_name != "bool")
                {
                    throw new CheckTypeException("bool");
                }
                lua.Pop(1);


                lua.PushNil();
                curr_type = lua.GetType(-1);
                curr_name = lua.GetTypeName(curr_type);
                if(curr_type != (int)TYPES.NIL || curr_name != "nil")
                {
                    throw new CheckTypeException("nil");
                }
                lua.Pop(1);


                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                curr_type = lua.GetType(-1);
                curr_name = lua.GetTypeName(curr_type);
                if(curr_type != (int)TYPES.TABLE || curr_name != "table")
                {
                    throw new CheckTypeException("table");
                }
                lua.Pop(1);


                lua.PushVector(new Vector3((float)rand.NextDouble()));
                curr_type = lua.GetType(-1);
                curr_name = lua.GetTypeName(curr_type);
                if (curr_type != (int)TYPES.Vector || curr_name != "vector")
                { 
                    throw new CheckTypeException("vector");
                }
                lua.Pop(1);


                lua.PushAngle(new Vector3((float)rand.NextDouble()));
                curr_type = lua.GetType(-1);
                curr_name = lua.GetTypeName(curr_type);
                if(curr_type != (int)TYPES.ANGLE || curr_name != "angle")
                {
                    throw new CheckTypeException("angle");
                }
                lua.Pop(1);


                lua.PushManagedFunction(this.test_func);
                curr_type = lua.GetType(-1);
                curr_name = lua.GetTypeName(curr_type);
                if(curr_type != (int)TYPES.FUNCTION || curr_name != "function")
                {
                    throw new CheckTypeException("function");
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

    public class CheckTypeException : Exception
    {
        string message;

        public CheckTypeException(string TypeName)
        {
            message = "Test failed on type " + TypeName;
        }

        public override string Message => message;
    }
}
