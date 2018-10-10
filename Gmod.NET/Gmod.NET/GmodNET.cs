/* GmodNET.Main() is an entry point of the managed code of the GmodDotNet. Main() will be called by native module as soon as it
 * will successfully init mono domain, load assembly and image.
 * 
 * Authors: Gleb Krasilich
 * 2018*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GmodNET
{
    static class GmodNET
    {
        //Entry point of the GmodDotNet
        static void Main()
        {
            //Test function to import
            int TestFunc()
            {
                //Print to console
                lock(Lua.Api)
                {
                    //Push global table on stack
                    Lua.Api.PushSpecial(Lua.SpecialTables.Global);
                    //Get print function
                    Lua.Api.GetField(-1, "print");
                    //Push message to print on stack
                    Lua.Api.PushString("You called TestFunc!");
                    //Call print function
                    Lua.Api.Call(1, 0);
                    //Pop global table from the stack
                    Lua.Api.Pop();
                }
                //Our function doesn't return anything to Lua stack
                return 0;
            }

            //Print welcome message
            lock(Lua.Api)
            {
                Lua.Api.PushSpecial(Lua.SpecialTables.Global);
                Lua.Api.GetField(-1, "print");
                Lua.Api.PushString("GmodDotNet loaded!");
                Lua.Api.Call(1, 0);
                Lua.Api.Pop();
            }

            //Register TestFunc with garry's mod and make it callable by lua as testFunc()
            lock(Lua.Api)
            {
                //Push global table on stack
                Lua.Api.PushSpecial(Lua.SpecialTables.Global);
                //Create delegate for the TestFunc
                Lua.CFunc del = new Lua.CFunc(TestFunc);
                //Push del on stack
                Lua.Api.PushCFunction(del);
                //Add TestFunc to Garry's Mod Lua global table as testFunc
                Lua.Api.SetField(-2, "testFunc");
                //Clean the stack by poping global table
                Lua.Api.Pop();
            }
        }
    }
}
