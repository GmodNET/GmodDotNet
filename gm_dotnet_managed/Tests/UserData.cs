using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Test for ILua's PushUserData, PushUserType, GetUserType, SetUserType, and GetType IsType for userdata
    public class UserData : ITest
    {
        int Type_Id;

        int RandomInt;
        int RandomInt2;
        int RandomInt3;

        public UserData()
        {
            Type_Id = -1;

            Random rand = new Random();

            RandomInt = rand.Next(0, 10000);

            RandomInt2 = rand.Next(0, 10000);

            RandomInt3 = rand.Next(0, 10000);
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor)
        {
            TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

            try
            {
                /*
                //Test PushUserData
                lua.PushUserData((IntPtr)this.RandomInt);

                if(lua.GetUserType(-1, (int)TYPES.USERDATA) != (IntPtr)this.RandomInt)
                {
                    throw new Exception("GetUserType returned incorrect pointer after PushUserData");
                }

                lua.Pop(1);
                */

                //Test PushUserType
                this.Type_Id = lua.CreateMetaTable("UserDataTestType");
                lua.Pop(1);

                lua.PushUserType((IntPtr)RandomInt2, this.Type_Id);

                if(lua.GetUserType(-1, this.Type_Id) != (IntPtr)this.RandomInt2)
                {
                    throw new Exception("GetUserType returned incorrect pointer after PushUserType");
                }

                //Test SetUserType
                lua.SetUserType(-1, (IntPtr)this.RandomInt3);
                if(lua.GetUserType(-1, this.Type_Id) != (IntPtr)this.RandomInt3)
                {
                    throw new Exception("GetUserType returned incorrect pointer after SetUserType");
                }

                //Additional test for GetType and IsType
                if(lua.GetType(-1) != this.Type_Id)
                {
                    throw new Exception("GetType returned incorrect type id on usertype");
                }

                if(!lua.IsType(-1, this.Type_Id))
                {
                    throw new Exception("IsType returned false on usertype");
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
