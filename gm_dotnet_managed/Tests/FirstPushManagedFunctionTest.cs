using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class FirstPushManagedFunctionTest : ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushManagedFunction((lua) =>
            {
                throw new Exception("Some very random exception");
            });
            lua.SetField(-2, "ManagedTestFunc");
            lua.Pop(1);

            return Task.FromResult(true);
        }
    }
}
