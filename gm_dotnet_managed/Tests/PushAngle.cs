using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace Tests
{
    // This test pushes random angle and gets it back
    public class PushAngle : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                Random rand = new Random();

                string field_id = Guid.NewGuid().ToString();

                float x = (float)rand.NextDouble() + rand.Next(-100, 101);
                float y = (float)rand.NextDouble() + rand.Next(-100, 101);
                float z = (float)rand.NextDouble() + rand.Next(-100, 101);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);

                lua.PushAngle(new Vector3(x, y ,z));
                lua.SetField(-2, field_id);

                lua.GetField(-1, field_id);
                Vector3 recieved_vec = lua.GetAngle(-1);
                lua.Pop(2);
                
                if(recieved_vec != new Vector3(x, y, z))
                {
                    throw new PushAngleException(recieved_vec, new Vector3(x, y, z));
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

    public class PushAngleException : Exception
    {
        string message;

        public PushAngleException(Vector3 expected, Vector3 recieved)
        {
            message = "Expected angle: (" + expected.X + ", " + expected.Y + ", " + expected.Z 
                + "). Recieved angle: (" + recieved.X + ", " + recieved.Y + ", " + recieved.Z + ")";
        }

        public override string Message => message;
    }
}
