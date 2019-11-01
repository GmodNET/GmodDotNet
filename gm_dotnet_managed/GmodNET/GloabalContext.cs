using System;
using System.Collections.Generic;
using System.Text;
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
    internal class GlobalContext
    {
        ILua lua;

        bool isServerSide;

        List<ModuleHolder> module_holders;

        CFuncManagedDelegate LuaLoadBridge;
        CFuncManagedDelegate LuaUnloadBridge;

        BaseVerificationHandler base_verification_handler;

        bool AreModulesWereLoaded;

        internal GlobalContext(ILua lua)
        { 
            this.lua = lua;

            AreModulesWereLoaded = false;

            LuaLoadBridge = (lua_state) =>
            {
                LoadAll();
                return 0;
            };
            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushCFunction(LuaLoadBridge);
            lua.SetField(-2, "gmod_net_load_all_function");
            lua.Pop(1);

            LuaUnloadBridge = (lua_state) =>
            {
                UnloadAll();
                return 0;
            };
            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.PushCFunction(LuaUnloadBridge);
            lua.SetField(-2, "gmod_net_unload_all_function");
            lua.Pop(1);

            lua.GetField(-10002, "SERVER");
            isServerSide = lua.GetBool(-1);

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
                lua.PushCFunction(base_verification_handler.OnServerTick);
                lua.Call(3, 0);
                lua.Pop(lua.Top());
            }

            LoadAll();
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

        void PrintToConsole(string msg)
        {
            lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
            lua.GetField(-1, "print");
            lua.PushString(msg);
            lua.Call(1, 0);
            lua.Pop(1);
        }

        void LoadAll()
        {
            if(AreModulesWereLoaded)
            {
                PrintToConsole("Unable to load .NET modules: modules are already loaded!");
                return;
            }
            PrintToConsole("Loading managed .NET modules...");

            module_holders = new List<ModuleHolder>();

            string[] module_directories = Directory.GetDirectories("garrysmod/lua/bin/Modules");

            List<string> module_names = new List<string>();

            foreach(string s in module_directories)
            {
                module_names.Add(s.Split(new char[] { '\\', '/' }).Last().Split('.')[0]);
            }

            foreach(string s in module_names)
            {
                var con = new ModuleAssemblyLoadContext(s);

                byte[] ass_im = File.ReadAllBytes("garrysmod/lua/bin/Modules/"+s+"/"+s+".dll");
                Assembly mod_ass = con.LoadFromStream(new MemoryStream(ass_im));

                Type[] module_types = mod_ass.GetTypes().Where(t => typeof(IModule).IsAssignableFrom(t)).ToArray();

                List<IModule> list_of_modules = new List<IModule>();

                foreach (Type t in module_types)
                { 
                    list_of_modules.Add((IModule)Activator.CreateInstance(t));
                }

                module_holders.Add(new ModuleHolder(con, list_of_modules));
            }

            foreach(ModuleHolder m in module_holders)
            {
                foreach(IModule mm in m.modules)
                {
                    PrintToConsole("Loading module " + mm.ModuleName + ". Version " + mm.ModuleVersion +".");

                    mm.Load(lua, isServerSide, LuaInterop.ExtructLua);
                }
            }

            PrintToConsole("All managed modules were loaded.");

            AreModulesWereLoaded = true;
        }

        void UnloadAll()
        {
            if (!AreModulesWereLoaded)
            {
                PrintToConsole("Unable to unload .NET modules: modules were already unloaded!");
                return;
            }

            PrintToConsole("Unloading managed modules...");

            foreach(ModuleHolder m in module_holders)
            {
                foreach(IModule mm in m.modules)
                {
                    mm.Unload();
                }

                m.context.Unload();
            }

            module_holders = new List<ModuleHolder>();

            PrintToConsole("All managed modules were unloaded.");

            AreModulesWereLoaded = false;
        }
    }

    class BaseVerificationHandler
    {
        List<Tuple<Task<bool>, int>> list_of_validation_tasks;
        
        public BaseVerificationHandler()
        {
            list_of_validation_tasks = new List<Tuple<Task<bool>, int>>();
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

        struct KeyPair
        {
            public string PublicKey {get; set; }
            public string PrivateKey {get; set; }
        }
        struct ModuleSignature
        {
            public string Version {get; set; }
            public string Signature {get; set; }
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
}
