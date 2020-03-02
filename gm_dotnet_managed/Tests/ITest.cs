using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GmodNET.API;

namespace Tests
{
    public interface ITest
    {
        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor);
    }
}
