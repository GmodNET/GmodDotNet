using GmodNET.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    // Tests that PushManagedFunction works with hooks.
    public class HookPushManagedFunctionTest : ITest
    {
        string hook_id;
        int counter;
        TaskCompletionSource<bool> taskCompletion;

        public HookPushManagedFunctionTest()
        {
            hook_id = Guid.NewGuid().ToString();
            counter = 0;
            taskCompletion = new TaskCompletionSource<bool>();
        }

        public Task<bool> Start(ILua lua, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            try
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "hook");
                lua.GetField(-1, "Add");
                lua.PushString("Tick");
                lua.PushString(hook_id);
                lua.PushManagedFunction(l =>
                {
                    try
                    {
                        if(counter < 33)
                        {
                            counter++;
                        }
                        else
                        {
                            l.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                            l.GetField(-1, "hook");
                            l.GetField(-1, "Remove");
                            l.PushString("Tick");
                            l.PushString(hook_id);
                            l.MCall(2, 0);
                            l.Pop(2);

                            if(counter == 33)
                            {
                                taskCompletion.TrySetResult(true);
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        taskCompletion.TrySetException(new Exception[] { e });
                    }

                    return 0;
                });
                lua.MCall(3, 0);
                lua.Pop(2);
            }
            catch(Exception e)
            {
                taskCompletion.TrySetException(new Exception[] { e });
            }

            return taskCompletion.Task;
        }
    }
}
