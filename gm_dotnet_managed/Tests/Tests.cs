using System;
using System.Runtime.Loader;
using GmodNET.API;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    // Test runner. Upon success of all tests 'File.WriteAllText("tests-success.txt", "Success!");' should be called. Information should be logged to "tests-log.txt" file
    public class Tests : IModule
    {
        public string ModuleName => "Test suit for Gmod.NET";

        public string ModuleVersion => "1.0.0";

        ILua lua;

        bool isServerSide;

        GetILuaFromLuaStatePointer lua_extructor;

        ModuleAssemblyLoadContext current_load_context;

        CFuncManagedDelegate OnTickDelegate;

        bool WasServerQuitTrigered;

        bool IsEverythingSuccessful;

        string OnTickIdentifier;

        List<ITest> ListOfTests;

        DateTime tests_start_time;

        Tuple<ITest, Task<bool>> current_test;

        public Tests()
        {
            WasServerQuitTrigered = false;
            IsEverythingSuccessful = false;
        }

        public void Load(ILua lua, bool is_serverside, GetILuaFromLuaStatePointer lua_extructor, ModuleAssemblyLoadContext assembly_context)
        {
            this.lua = lua;
            this.isServerSide = is_serverside;
            this.lua_extructor = lua_extructor;
            this.current_load_context = assembly_context;
            this.OnTickDelegate = this.OnTick;
            this.OnTickIdentifier = Guid.NewGuid().ToString();
            current_test = null;

            if(isServerSide)
            {
                try
                {
                lua.Log("Loading test runner");

                //Register OnTick with Garry's Mod
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "hook");
                lua.GetField(-1, "Add");
                lua.PushString("Tick");
                lua.PushString(this.OnTickIdentifier);
                lua.PushCFunction(this.OnTickDelegate);
                lua.Call(3, 0);
                lua.Pop(2);

                //Get the list of tests
                ListOfTests = new List<ITest>();
                if(typeof(Tests).Assembly.GetTypes().Any(type => typeof(ITest).IsAssignableFrom(type) && type != typeof(ITest)))
                {
                    foreach(Type t in typeof(Tests).Assembly.GetTypes().Where(type => type != typeof(ITest) && typeof(ITest).IsAssignableFrom(type)))
                    {
                        ListOfTests.Add((ITest)Activator.CreateInstance(t));
                    }
                }

                lua.Log("There are " +ListOfTests.Count + " tests to run:");
                foreach(ITest test in ListOfTests)
                {
                    lua.Log(test.GetType().ToString());
                }

                IsEverythingSuccessful = true;
                tests_start_time = DateTime.Now;
                lua.Log("Test runner was loaded!");
                }
                catch (Exception e)
                {
                    lua.Log("Test runner FAILED to start: " + e.GetType().ToString() + ". Exception message: " + e.Message);
                }
            }
        }

        public void Unload()
        {
            
        }

        int OnTick(IntPtr lua_state)
        {
            ILua lua = lua_extructor(lua_state);

            if(current_test == null)
            {
                if (ListOfTests.Count == 0 && !WasServerQuitTrigered)
                {
                    if (IsEverythingSuccessful)
                    { 
                        lua.Log("All tests were completed successfully!");
                        lua.Log("Test run time is " + DateTime.Now.Subtract(tests_start_time).TotalSeconds + " seconds");
                        File.WriteAllText("tests-success.txt", "Success!");

                        lua.Log("Shutting down game...");
                        lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                        lua.GetField(-1, "engine");
                        lua.GetField(-1, "CloseServer");
                        lua.Call(0, 0);

                        WasServerQuitTrigered = true;
                    }
                    else
                    {
                        lua.Log("There are no more tests to run. Some tests have failed. Check log.");
                        lua.Log("Test run time is " + DateTime.Now.Subtract(tests_start_time).TotalSeconds + " seconds");

                        lua.Log("Shutting down game...");
                        lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                        lua.GetField(-1, "engine");
                        lua.GetField(-1, "CloseServer");
                        lua.Call(0, 0);

                        WasServerQuitTrigered = true;
                    }
                }
                else if (ListOfTests.Count != 0)
                {
                    ITest cur_test_inst = ListOfTests.First();
                    ListOfTests.RemoveAt(0);

                    lua.Log("Starting test " + cur_test_inst.GetType().ToString());

                    Task<bool> cur_test_promise = cur_test_inst.Start(lua, this.lua_extructor, current_load_context);

                    current_test = new Tuple<ITest, Task<bool>>(cur_test_inst, cur_test_promise);
                }
            }
            else
            {
                if(current_test.Item2.IsCompleted)
                {
                    ITest curr_test_inst = current_test.Item1;
                    Task<bool> curr_test_promise = current_test.Item2;

                    current_test = null;

                    if(curr_test_promise.IsCompletedSuccessfully)
                    {
                        if(curr_test_promise.Result)
                        {
                           lua.Log("Test " + curr_test_inst.GetType().ToString() + " was completed successfully");
                        }
                        else
                        {
                            lua.Log("FAILED TEST " + curr_test_inst.GetType().ToString() +". An exception was not thrown");
                            this.IsEverythingSuccessful = false;
                        }
                    }
                    else if(curr_test_promise.IsFaulted)
                    {
                        lua.Log("FAILED TEST " + curr_test_inst.GetType().ToString() + ". List of exceptions:");
                        foreach (Exception e in curr_test_promise.Exception.InnerExceptions)
                        {
                            lua.Log(e.GetType().ToString() + " - " + e.Message);
                        }
                        this.IsEverythingSuccessful = false;
                    }
                }
            }

            return 0;
        }
    }
}
