using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using GmodNET.API;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using NSec.Cryptography;
using System.Text.Encodings;

namespace GmodNET
{
    internal struct ManagedModuleInfoForClient
    {
        public string Name {get; set; }
        public string MinVersion {get; set; }
        public string MaxVersion {get; set; }
        public string PublicKey {get; set; }
    }
    internal struct KeyPair
    {
        public string PublicKey {get; set; }
        public string PrivateKey {get; set; }
    }
    internal struct ModuleSignature
    {
        public string Version {get; set; }
        public string Signature {get; set; }
    }

    internal class GlobalContext
    {
        ILua lua;

        bool isServerSide;

        List<ModuleHolder> module_holders;
        List<ManagedModuleInfoForClient> modules_for_client;

        CFuncManagedDelegate LuaLoadBridge;
        CFuncManagedDelegate LuaUnloadBridge;

        BaseVerificationHandler base_verification_handler;

        bool AreModulesWereLoaded;

        CFuncManagedDelegate ServerListOfModulesListener;
        CFuncManagedDelegate ClientContinueLoad;

        internal GlobalContext(ILua lua)
        { 
            module_holders = new List<ModuleHolder>();
            modules_for_client = new List<ManagedModuleInfoForClient>();

            this.lua = lua;

            AreModulesWereLoaded = false;

            lua.GetField(-10002, "SERVER");
            isServerSide = lua.GetBool(-1);

            LuaLoadBridge = (lua_state) =>
            {
                ILua lua = LuaInterop.ExtructLua(lua_state);

                LoadAll(lua);

                lua.Pop(lua.Top());

                return 0;
            };

            LuaUnloadBridge = (lua_state) =>
            {
                ILua lua = LuaInterop.ExtructLua(lua_state);

                UnloadAll(lua);

                lua.Pop(lua.Top());

                return 0;
            };

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushCFunction(LuaLoadBridge);
            lua.SetField(-2, "gmod_net_load_all_function");
            lua.Pop(lua.Top());

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushCFunction(LuaUnloadBridge);
            lua.SetField(-2, "gmod_net_unload_all_function");
            lua.Pop(lua.Top());

            if(isServerSide)
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "concommand");
                lua.GetField(-1, "Add");
                lua.PushString("gmod_net_load_all");
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "gmod_net_load_all_function");
                lua.Remove(-2);
                lua.PushNil();
                lua.PushString("Load all .NET managed modules by GmodNET");
                lua.PushNumber(0);
                lua.Call(5, 0);
                lua.Pop(2);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "concommand");
                lua.GetField(-1, "Add");
                lua.PushString("gmod_net_unload_all");
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "gmod_net_unload_all_function");
                lua.Remove(-2);
                lua.PushNil();
                lua.PushString("Unload all .NET managed modules by GmodNET");
                lua.PushNumber(0);
                lua.Call(5, 0);
                lua.Pop(2);
            }
            else
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "concommand");
                lua.GetField(-1, "Add");
                lua.PushString("gmod_net_load_all_cl");
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "gmod_net_load_all_function");
                lua.Remove(-2);
                lua.PushNil();
                lua.PushString("Load all .NET managed modules by GmodNET (client-side)");
                lua.PushNumber(0);
                lua.Call(5, 0);
                lua.Pop(2);

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "concommand");
                lua.GetField(-1, "Add");
                lua.PushString("gmod_net_unload_all_cl");
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "gmod_net_unload_all_function");
                lua.Remove(-2);
                lua.PushNil();
                lua.PushString("Unload all .NET managed modules by GmodNET (client-side)");
                lua.PushNumber(0);
                lua.Call(5, 0);
                lua.Pop(2);
            }

            if(isServerSide)
            {
                base_verification_handler = new BaseVerificationHandler();

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "util");
                lua.GetField(-1, "AddNetworkString");
                lua.PushString("gmodnet_verify_request");
                lua.Call(1, 1);
                lua.Pop(1);

                lua.GetField(-1, "AddNetworkString");
                lua.PushString("gmodnet_verify_response");
                lua.Call(1, 1);
                lua.Pop(lua.Top());

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "util");
                lua.GetField(-1, "AddNetworkString");
                lua.PushString("gmodnet_request_clientside_modules");
                lua.Call(1, 1);
                lua.Pop(1);

                lua.GetField(-1, "AddNetworkString");
                lua.PushString("gmodnet_request_clientside_modules_response");
                lua.Call(1, 1);
                lua.Pop(lua.Top());

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "net");
                lua.GetField(-1, "Receive");
                lua.PushString("gmodnet_verify_request");
                lua.PushCFunction(base_verification_handler.NetMessageListener);
                lua.Call(2, 0);
                lua.Pop(lua.Top());

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "hook");
                lua.GetField(-1, "Add");
                lua.PushString("Tick");
                lua.PushString("BaseVerificationHandler_tick_listener");
                lua.PushCFunction(base_verification_handler.OnServerTickDelegate);
                lua.Call(3, 0);
                lua.Pop(lua.Top());
            }

            if(isServerSide)
            {
                ServerListOfModulesListener = (lua_state) =>
                {
                    ILua lua = LuaInterop.ExtructLua(lua_state);

                    int player_reference = lua.ReferenceCreate();

                    lua.Pop(lua.Top());

                    lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                    lua.GetField(-1, "net");
                    lua.GetField(-1, "Start");
                    lua.PushString("gmodnet_request_clientside_modules_response");
                    lua.Call(1, 1);
                    lua.Pop(1);

                    lua.GetField(-1, "WriteDouble");
                    lua.PushNumber(modules_for_client.Count);
                    lua.Call(1, 0);

                    foreach(ManagedModuleInfoForClient info in modules_for_client)
                    {
                        lua.GetField(-1, "WriteString");
                        lua.PushString(JsonSerializer.Serialize<ManagedModuleInfoForClient>(info));
                        lua.Call(1, 0);
                    }

                    lua.GetField(-1, "Send");
                    lua.ReferencePush(player_reference);
                    lua.Call(1, 0);

                    lua.ReferenceFree(player_reference);

                    lua.Pop(lua.Top());

                    return 0;
                };

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "net");
                lua.GetField(-1, "Receive");
                lua.PushString("gmodnet_request_clientside_modules");
                lua.PushCFunction(ServerListOfModulesListener);
                lua.Call(2, 0);

                lua.Pop(lua.Top());
            }

            if(!isServerSide)
            {
                ClientContinueLoad = (lua_state) =>
                {
                    ILua lua = LuaInterop.ExtructLua(lua_state);

                    if(AreModulesWereLoaded)
                    {
                        lua.Pop(lua.Top());
                        return 0;
                    }

                    lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                    lua.GetField(-1, "net");
                    lua.GetField(-1, "ReadDouble");
                    lua.Call(0, 1);
                    int number_of_entries = (int)lua.GetNumber(-1);
                    lua.Pop(1);

                    for(int i = 0; i < number_of_entries; i++)
                    {
                        try
                        {
                            lua.GetField(-1, "ReadString");
                            lua.Call(0, 1);
                            string local_msg = lua.GetString(-1);
                            lua.Pop(1);

                            ManagedModuleInfoForClient module_info = JsonSerializer.Deserialize<ManagedModuleInfoForClient>(local_msg);

                            modules_for_client.Add(module_info);
                        }
                        catch(Exception e)
                        {
                            PrintToConsole(lua, "Server returned invalid .NET module info: " + e.GetType().ToString() + " " + e.Message);
                        }
                    }

                    lua.Pop(lua.Top());

                    DirectoryInfo modules_directory = new DirectoryInfo("garrysmod/lua/bin/Modules");

                    DirectoryInfo[] proper_module_directories = modules_directory.GetDirectories().Where((d) => modules_for_client
                    .Any((m) => m.Name == d.Name)).ToArray();

                    foreach(DirectoryInfo d in proper_module_directories)
                    {
                        ManagedModuleInfoForClient info = modules_for_client.First((m) => m.Name == d.Name);

                        try
                        {
                            if(info.PublicKey == null || info.PublicKey == String.Empty)
                            {
                                ModuleAssemblyLoadContext context = new ModuleAssemblyLoadContext(d.Name); 

                                Assembly module_asm = context.LoadFromAssemblyPath(d.FullName + "/" + d.Name + ".dll");

                                List<IModule> local_list_of_modules = new List<IModule>();

                                Type[] classes_of_modules = module_asm.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t)).ToArray();

                                foreach(Type t in classes_of_modules)
                                {
                                    local_list_of_modules.Add((IModule)Activator.CreateInstance(t));
                                }

                                module_holders.Add(new ModuleHolder(context, local_list_of_modules));
                            }
                            else
                            {
                                static byte[] hex_converter(string s)
                                {
                                    int len = s.Length;
                                    if(len % 2 != 0)
                                    {
                                        throw new Exception("hex string is of odd length");
                                    }

                                    byte[] res = new byte[len / 2];

                                    for(int i = 0; i < len; i += 2)
                                    {
                                        res[i/2] = Convert.ToByte(s.Substring(i, 2), 16);
                                    }

                                    return res;
                                }

                                static bool is_in_interval(string version, string first)
                                {
                                    string[] first_split = first.Split('.');
                                    string[] version_split = version.Split('.');

                                    ulong first_number = ulong.Parse(first_split[0]) * 100000000 + ulong.Parse(first_split[1]) * 10000 + ulong.Parse(first_split[2]);
                                    ulong version_number = ulong.Parse(version_split[0]) * 100000000 + ulong.Parse(version_split[1]) * 10000 + ulong.Parse(version_split[2]);

                                    return first_number <= version_number;
                                }

                                ModuleSignature local_signature = JsonSerializer.Deserialize<ModuleSignature>(File.ReadAllText(d.GetFiles()
                                    .First((f) => f.Name == d.Name + ".modulesign").FullName));

                                Ed25519 ed25519 = SignatureAlgorithm.Ed25519;
                                Sha512 sha512 = HashAlgorithm.Sha512;

                                byte[] module_blob = File.ReadAllBytes(d.FullName + "/" + d.Name + ".dll");

                                byte[] finale_blob = sha512.Hash(sha512.Hash(module_blob).Concat(Encoding.UTF8.GetBytes(local_signature.Version)).ToArray());

                                PublicKey public_key = PublicKey.Import(ed25519, hex_converter(info.PublicKey), KeyBlobFormat.RawPublicKey);

                                bool SignatureIsValid = ed25519.Verify(public_key, finale_blob, hex_converter(local_signature.Signature));

                                if(SignatureIsValid)
                                {
                                    bool version_checked = false;
                                    if(info.MinVersion == null || info.MinVersion == String.Empty)
                                    {
                                        if(info.MaxVersion == null || info.MaxVersion == String.Empty)
                                        {
                                            version_checked = true;
                                        }
                                        else if (is_in_interval(info.MaxVersion, local_signature.Signature))
                                        {
                                            version_checked = true;
                                        }
                                        else
                                        {
                                            version_checked = false;
                                        }
                                    }
                                    else
                                    {
                                        if(is_in_interval(local_signature.Version, info.MaxVersion))
                                        {
                                            if(info.MaxVersion == null || info.MaxVersion == String.Empty)
                                            {
                                                version_checked = true;
                                            }
                                            else if(is_in_interval(info.MaxVersion, local_signature.Version))
                                            {
                                                version_checked = true;
                                            }
                                            else
                                            {
                                                version_checked = false;
                                            }
                                        }
                                        else
                                        {
                                            version_checked = false;
                                        }
                                    }

                                    if(version_checked)
                                    {
                                        ModuleAssemblyLoadContext module_context = new ModuleAssemblyLoadContext(d.Name);

                                        Assembly module_asm = module_context.LoadFromAssemblyPath(d.FullName + "/" + d.Name + ".dll");

                                        Type[] module_classes = module_asm.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t)).ToArray();

                                        List<IModule> local_modules = new List<IModule>();

                                        foreach(Type t in module_classes)
                                        {
                                            local_modules.Add((IModule)Activator.CreateInstance(t));
                                        }

                                        module_holders.Add(new ModuleHolder(module_context, local_modules));
                                    }
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            PrintToConsole(lua, "Error while processing .NET module clientside: " + e.GetType().ToString() + " " + e.Message);
                        }
                    }

                    PrintToConsole(lua, "Loading .NET modules...");

                    foreach(ModuleHolder m in module_holders)
                    {
                        foreach(IModule mm in m.modules)
                        {
                            PrintToConsole(lua, "Loading module " + mm.ModuleName + " " + mm.ModuleVersion);
                            mm.Load(this.lua, isServerSide, LuaInterop.ExtructLua, m.context);
                        }
                    }

                    PrintToConsole(lua, "All managed modules were loaded clientside");

                    lua.Pop(lua.Top());

                    this.AreModulesWereLoaded = true;

                    return 0;
                };

                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "net");
                lua.GetField(-1, "Receive");
                lua.PushString("gmodnet_request_clientside_modules_response");
                lua.PushCFunction(this.ClientContinueLoad);
                lua.Call(2, 0);

                lua.Pop(lua.Top());
            }

            lua.Pop(lua.Top());

            LoadAll(lua);
        }

        private GlobalContext()
        {
            throw new NotImplementedException();
        }

        ~GlobalContext()
        {
            foreach(ModuleHolder m in module_holders)
            {
                m.context.Unload();
            }
        }

        internal void OnNativeUnload()
        {
            List<WeakReference> weak_refs = new List<WeakReference>();

            foreach(ModuleHolder mh in module_holders)
            {
                mh.context.Unload();
                weak_refs.Add(new WeakReference(mh.context));
            }

            module_holders.Clear();
            modules_for_client.Clear();

            while(weak_refs.Any(r => r.IsAlive))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        static void PrintToConsole(ILua lua, string msg)
        {
            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, "print");
            lua.PushString(msg);
            lua.Call(1, 0);
            lua.Pop(1);
        }

        private void LoadAll(ILua lua)
        {
            if(isServerSide)
            {
                LoadAllServerSide(lua);
            }
            else
            {
                LoadAllClientSide(lua);
            }
        }

        private void UnloadAll(ILua lua)
        {
            List<WeakReference> weak_references = new List<WeakReference>();

            if(!this.AreModulesWereLoaded)
            {
                PrintToConsole(lua, "Unable to unload modules: modules are already unloaded");
                return;
            }

            PrintToConsole(lua, "Unloading .NET modules...");

            foreach(ModuleHolder mh in this.module_holders)
            {
                foreach(IModule m in mh.modules)
                {
                    PrintToConsole(lua, "Unloading module " + m.ModuleName);
                    m.Unload();
                }

                mh.context.Unload();

                mh.modules.Clear();

                weak_references.Add(new WeakReference(mh.context));
            }

            this.modules_for_client.Clear();
            this.module_holders.Clear();

            this.modules_for_client = new List<ManagedModuleInfoForClient>();
            this.module_holders = new List<ModuleHolder>();

            while(weak_references.Any(r => r.IsAlive))
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            PrintToConsole(lua, "All managed modules were unloaded");

            this.AreModulesWereLoaded = false;
        }

        private void LoadAllServerSide(ILua lua)
        {
            if(this.AreModulesWereLoaded)
            {
                PrintToConsole(lua, "Unable to load modules: modules are already loaded.");
                return;
            }

            PrintToConsole(lua, "Loading .NET modules...");

            DirectoryInfo modules_directory_info;
            try
            {
                modules_directory_info = new DirectoryInfo("garrysmod/lua/bin/Modules");
            }
            catch(Exception e)
            {
                lua.Pop(lua.Top());

                PrintToConsole(lua, "Unable to access .NET modules directory: " + e.GetType().ToString() + " " + e.Message);

                return;
            }

            var proper_modules_directories = modules_directory_info.GetDirectories();

            foreach(DirectoryInfo d in proper_modules_directories)
            {
                string type_file_content;
                if(d.GetFiles().Any((f) => f.Name == "TYPE"))
                {
                    type_file_content = File.ReadAllText(d.GetFiles().First((f) => f.Name == "TYPE").FullName);
                }
                else
                {
                    type_file_content = null;
                }

                int type_mode = type_file_content switch
                {
                    "server" => 0,
                    "client" => 1,
                    "shared" => 2,
                    _ => -1
                };

                string maxversion_file = null;
                if(d.GetFiles().Any((f) => f.Name == "MAXVERSION"))
                {
                    string tmp = File.ReadAllText(d.GetFiles().First((f) => f.Name == "MAXVERSION").FullName);
                    Regex version_matcher = new Regex(@"\b\d+\.\d+\.\d+$\b", RegexOptions.ECMAScript | RegexOptions.Multiline | RegexOptions.Compiled);
                    if(version_matcher.IsMatch(tmp))
                    {
                        maxversion_file = tmp;
                    }
                }

                string minversion_file = null;
                if(d.GetFiles().Any((f) => f.Name == "MINVERSION"))
                {
                    string tmp = File.ReadAllText(d.GetFiles().First((f) => f.Name == "MINVERSION").FullName);
                    Regex version_matcher = new Regex(@"\b\d+\.\d+\.\d+$\b", RegexOptions.ECMAScript | RegexOptions.Multiline | RegexOptions.Compiled);
                    if(version_matcher.IsMatch(tmp))
                    {
                        minversion_file = tmp;
                    }
                }

                KeyPair local_key_pair = new KeyPair{ PublicKey = String.Empty };
                if(d.GetFiles().Any((f) => f.Name == d.Name + ".modulekey"))
                {
                    try
                    {
                        local_key_pair = JsonSerializer.Deserialize<KeyPair>(File.ReadAllBytes(d.GetFiles().First((f) => f.Name == d.Name + ".modulekey").FullName));
                    }
                    catch
                    {
                        local_key_pair = new KeyPair{ PublicKey = String.Empty };
                    }
                }

                if(type_mode == 1 || type_mode == 2 || type_mode == -1)
                {
                    modules_for_client.Add(new ManagedModuleInfoForClient
                    {
                        MinVersion = minversion_file,
                        MaxVersion = maxversion_file,
                        Name = d.Name,
                        PublicKey = local_key_pair.PublicKey
                    });
                }

                if(type_mode == 0 || type_mode == 2 || type_mode == -1)
                {
                    if(d.GetFiles().Any((f) => f.Name == d.Name + ".dll"))
                    {
                        ModuleAssemblyLoadContext local_context = new ModuleAssemblyLoadContext(d.Name);

                        Assembly module_asm = local_context.LoadFromAssemblyPath(d.GetFiles().First((f) => f.Name == d.Name + ".dll").FullName);

                        List<IModule> local_modules = new List<IModule>();

                        Type[] module_classes = module_asm.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t)).ToArray();

                        foreach(Type t in module_classes)
                        {
                            local_modules.Add((IModule)Activator.CreateInstance(t));
                        }

                        ModuleHolder mh = new ModuleHolder(local_context, local_modules);

                        module_holders.Add(mh);
                    }
                }
            }

            foreach(ModuleHolder mh in module_holders)
            {
                foreach(IModule module in mh.modules)
                {
                    PrintToConsole(lua, "Loading module " + module.ModuleName + " " + module.ModuleVersion);

                    module.Load(this.lua, isServerSide, LuaInterop.ExtructLua, mh.context);
                }
            }

            PrintToConsole(lua, "All managed modules were loaded serverside.");

            this.AreModulesWereLoaded = true;
        }

        private void LoadAllClientSide(ILua lua)
        {
            if(this.AreModulesWereLoaded)
            {
                PrintToConsole(lua, "Unable to load .NET modules: modules are already loaded");

                return;
            }

            PrintToConsole(lua, "Requesting a list of clientside modules from server...");

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, "net");
            lua.GetField(-1, "Start");
            lua.PushString("gmodnet_request_clientside_modules");
            lua.Call(1, 1);
            lua.Pop(1);

            lua.GetField(-1, "WriteBool");
            lua.PushBool(true);
            lua.Call(1, 0);

            lua.GetField(-1, "SendToServer");
            lua.Call(0, 0);
            lua.Pop(lua.Top());
        }
    }

    class BaseVerificationHandler
    {
        List<Tuple<Task<bool>, int>> list_of_validation_tasks;

        CFuncManagedDelegate onServerTickDelegate;

        public CFuncManagedDelegate OnServerTickDelegate
        {
            get
            {
                return onServerTickDelegate;
            }
        }
        
        public BaseVerificationHandler()
        {
            list_of_validation_tasks = new List<Tuple<Task<bool>, int>>();

            onServerTickDelegate = this.OnServerTick;
        }

        public int NetMessageListener(IntPtr lua_state)
        { 
            ILua lua = LuaInterop.ExtructLua(lua_state);

            int message_length = (int)lua.GetNumber(1);

            if(message_length == 0)
            {
                return 0;
            }

            int player_reference = lua.ReferenceCreate();

            lua.Pop(lua.Top());

            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);

            lua.GetField(-1, "net");

            lua.GetField(-1, "ReadString");

            lua.Call(0, 1);

            string message = lua.GetString(-1);

            lua.Pop(lua.Top());

            if(message == String.Empty)
            {
                return 0;
            }

            list_of_validation_tasks.Add(new Tuple<Task<bool>, int>(Task<Task<bool>>.Factory.StartNew(() => ValidateBase(message)).Unwrap(), 
                player_reference));

            return 0;
        }

        public int OnServerTick(IntPtr lua_state)
        {
            ILua lua = LuaInterop.ExtructLua(lua_state);

            for(int i = list_of_validation_tasks.Count - 1; i >= 0; i--)
            {
                if(list_of_validation_tasks[i].Item1.IsCompleted)
                {
                    if(list_of_validation_tasks[i].Item1.IsFaulted)
                    {
                        lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                        lua.GetField(-1, "net");
                        lua.GetField(-1, "Start");
                        lua.PushString("gmodnet_verify_response");
                        lua.Call(1, 1);
                        lua.Pop(1);

                        lua.GetField(-1, "WriteBool");
                        lua.PushBool(false);
                        lua.Call(1, 0);

                        lua.GetField(-1, "WriteString");
                        lua.PushString("Serverside exception while verification: " + list_of_validation_tasks[i].Item1.Exception.Message);
                        lua.Call(1, 0);

                        lua.GetField(-1, "Send");
                        lua.ReferencePush(list_of_validation_tasks[i].Item2);
                        lua.Call(1, 0);

                        lua.Pop(lua.Top());
                    }
                    else
                    {
                        bool success = list_of_validation_tasks[i].Item1.Result;
                        string res_msg;
                        if(success)
                        {
                            res_msg = "Validation was successful";
                        }
                        else
                        {
                            res_msg = "Validation was denied";
                        }

                        lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                        lua.GetField(-1, "net");
                        lua.GetField(-1, "Start");
                        lua.PushString("gmodnet_verify_response");
                        lua.Call(1, 1);
                        lua.Pop(1);

                        lua.GetField(-1, "WriteBool");
                        lua.PushBool(success);
                        lua.Call(1, 0);

                        lua.GetField(-1, "WriteString");
                        lua.PushString(res_msg);
                        lua.Call(1, 0);

                        lua.GetField(-1, "Send");
                        lua.ReferencePush(list_of_validation_tasks[i].Item2);
                        lua.Call(1, 0);

                        lua.Pop(lua.Top());
                    }
                    lua.ReferenceFree(list_of_validation_tasks[i].Item2);
                    
                    list_of_validation_tasks.RemoveAt(i);
                }
            }

            return 0;
        }

        struct MessageStruct
        {
            public string NativeHash {get; set; }
            public string ManagedHash {get; set; }
            public ModuleSignature NativeSignature {get; set; }
            public ModuleSignature ManagedSignature {get; set; }
        }
        async static Task<bool> ValidateBase(string message)
        {
            byte[] native_key_file_blob = await File.ReadAllBytesAsync("garrysmod/lua/bin/gmcl_dotnet.modulekey");
            if (native_key_file_blob.Length == 0)
            { 
                throw new FileLoadException("gmcl_dotnet.modulekey is empty. Contact server administrator.");
            }

            byte[] managed_key_file_blob = await File.ReadAllBytesAsync("garrysmod/lua/bin/GmodNET/GmodNET.modulekey");
            if (managed_key_file_blob.Length == 0)
            { 
                throw new FileLoadException("GmodNET.modulekey is empty. Contact server administrator.");
            }

            KeyPair native_key_pair = JsonSerializer.Deserialize<KeyPair>(native_key_file_blob);
            if(native_key_pair.PublicKey == String.Empty || native_key_pair.PublicKey == null || native_key_pair.PublicKey.Length != 64)
            {
                throw new Exception("Server's native public key is invalid. Contact server administrator.");
            }

            KeyPair managed_key_pair = JsonSerializer.Deserialize<KeyPair>(managed_key_file_blob);
            if (managed_key_pair.PublicKey == String.Empty || managed_key_pair.PublicKey == null || managed_key_pair.PublicKey.Length != 64)
            { 
                throw new Exception("Server's managed public key is invalid. Contact server administrator.");
            }

            MessageStruct parsed_message = JsonSerializer.Deserialize<MessageStruct>(message);

            Sha512 sha512 = HashAlgorithm.Sha512;
            Ed25519 ed25519 = SignatureAlgorithm.Ed25519;
            PublicKey native_public_key = PublicKey.Import(ed25519, await Task<byte[]>.Factory.StartNew(() => HexToString(native_key_pair.PublicKey)), 
                KeyBlobFormat.RawPublicKey);
            PublicKey managed_public_key = PublicKey.Import(ed25519, await Task<byte[]>.Factory.StartNew(() => HexToString(managed_key_pair.PublicKey)), 
                KeyBlobFormat.RawPublicKey);

            byte[] native_final_hash = (await Task<byte[]>.Factory.StartNew(() => HexToString(parsed_message.NativeHash)))
                .Concat(Encoding.UTF8.GetBytes(parsed_message.NativeSignature.Version)).ToArray();
            native_final_hash = await Task<byte[]>.Factory.StartNew(() => sha512.Hash(native_final_hash));

            byte[] managed_final_hash = (await Task<byte[]>.Factory.StartNew(() => HexToString(parsed_message.ManagedHash)))
                .Concat(Encoding.UTF8.GetBytes(parsed_message.ManagedSignature.Version)).ToArray();
            managed_final_hash = await Task<byte[]>.Factory.StartNew(() => sha512.Hash(managed_final_hash));

            bool is_managed_valid = await Task<bool>.Factory.StartNew(() => ed25519.Verify(managed_public_key, managed_final_hash, 
                HexToString(parsed_message.ManagedSignature.Signature)));
            bool is_native_valid = await Task<bool>.Factory.StartNew(() => ed25519.Verify(native_public_key, native_final_hash, 
                HexToString(parsed_message.NativeSignature.Signature)));

            return is_managed_valid && is_native_valid;
        }

        static byte[] HexToString(in string hex)
        {
            int str_length = hex.Length;
            if(str_length % 2 != 0)
            {
                throw new ArgumentException("hex string is of odd length.");
            }

            byte[] res = new byte[str_length / 2];

            for(int i = 0; i < str_length; i += 2)
            {
                res[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return res;
        }
    }

    internal class ModuleHolder
    {
        internal ModuleAssemblyLoadContext context;
        internal List<IModule> modules;

        internal ModuleHolder(ModuleAssemblyLoadContext context, List<IModule> modules)
        {
            this.context = context;
            this.modules = modules;
        }
    }
}
