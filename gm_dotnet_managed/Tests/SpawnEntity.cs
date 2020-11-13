using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // This test tries to create an entity (physical prop) and checks if it is valid
    public class SpawnEntity : ITest
    {
        TaskCompletionSource<bool> taskCompletion;
        string hook_id;
        GetILuaFromLuaStatePointer lua_extructor;

        Func<ILua, int> spawn_func;

        public SpawnEntity()
        {
            taskCompletion = new TaskCompletionSource<bool>();

            hook_id = Guid.NewGuid().ToString();

            spawn_func = SpawnFunc;
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext _)
        {
            this.lua_extructor = lua_extructor;

            try
            {
                lua.PushManagedFunction(this.spawn_func);
                lua.MCall(0, 0);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }

        int SpawnFunc(ILua lua)
        {
            try
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "ents");
                lua.GetField(-1, "Create");
                lua.PushString("prop_physics");
                lua.MCall(1, 1);

                if(lua.GetType(-1) != (int)TYPES.ENTITY)
                {
                    throw new SpawnEntityException("ents.Create returned a value which type is not Entity");
                }

                lua.GetField(-1, "SetModel");
                lua.Push(-2);
                lua.PushString("models/props_c17/oildrum001.mdl");
                lua.MCall(2, 0);

                lua.GetField(-1, "SetPos");
                lua.Push(-2);
                lua.PushVector(new System.Numerics.Vector3(0));
                lua.MCall(2, 0);

                lua.GetField(-1, "Spawn");
                lua.Push(-2);
                lua.MCall(1, 0);

                lua.GetField(-1, "IsValid");
                lua.Push(-2);
                lua.MCall(1, 1);
                if(!lua.GetBool(-1))
                {
                    throw new SpawnEntityException("Entity is not valid");
                }

                lua.Pop(lua.Top());

                taskCompletion.TrySetResult(true);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return 0;
        }
    }

    public class SpawnEntityException : Exception
    {
        string message;

        public SpawnEntityException(string message)
        {
            this.message = message;
        }

        public override string Message => message;
    }
}
