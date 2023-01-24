using GmodNET.API;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace GmodNET
{
    /// <summary>
    /// Redirects Console's output to game's console
    /// </summary>
    internal class GameConsoleWriter : TextWriter
    {
        public override string NewLine => "\n";
        public override Encoding Encoding => Encoding.UTF8;

        public GameConsoleWriter(ILua lua)
        {
            if (OperatingSystem.IsWindows())
            {
                Msg = Msg_Tier0;
            }
            else if (OperatingSystem.IsLinux())
            {
                lua.PushSpecial(SPECIAL_TABLES.SPECIAL_GLOB);
                lua.GetField(-1, "game");
                lua.GetField(-1, "IsDedicated");
                lua.MCall(0, 1);
                bool is_dedicated = lua.GetBool(-1);
                lua.Pop(3);

                if (is_dedicated)
                {
                    Msg = Msg_Tier0;
                }
                else
                {
                    Msg = Msg_Tier0_Client;
                }
            }
            else if (OperatingSystem.IsMacOS())
            {
                unsafe
                {
                    IntPtr lib_handle = NativeLibrary.Load(Directory.GetCurrentDirectory() + "/GarrysMod_Signed.app/Contents/MacOS/libtier0.dylib");
                    delegate* unmanaged[Cdecl]<byte*, void> msg_func = (delegate* unmanaged[Cdecl]<byte*, void>)NativeLibrary.GetExport(lib_handle, "Msg");

                    Msg = (message) =>
                    {
                        byte* __message_gen_native = default;
                        //
                        // Setup
                        //
                        bool message__allocated = false;
                        try
                        {
                            //
                            // Marshal
                            //
                            if (message != null)
                            {
                                int message__bytelen = (message.Length + 1) * 3 + 1;
                                if (message__bytelen > 260)
                                {
                                    __message_gen_native = (byte*)System.Runtime.InteropServices.Marshal.StringToCoTaskMemUTF8(message);
                                    message__allocated = true;
                                }
                                else
                                {
                                    byte* path__stackptr = stackalloc byte[message__bytelen];
                                    {
                                        message__bytelen = System.Text.Encoding.UTF8.GetBytes(message, new System.Span<byte>(path__stackptr, message__bytelen));
                                        path__stackptr[message__bytelen] = 0;
                                    }

                                    __message_gen_native = (byte*)path__stackptr;
                                }
                            }

                            //
                            // Invoke
                            //
                            msg_func(__message_gen_native);
                        }
                        finally
                        {
                            //
                            // Cleanup
                            //
                            if (message__allocated)
                            {
                                System.Runtime.InteropServices.Marshal.FreeCoTaskMem((System.IntPtr)__message_gen_native);
                            }
                        }
                    };
                }
            }
            else Msg = (string msg) => throw new PlatformNotSupportedException();
        }

#pragma warning disable CA2101 // workaround for https://github.com/dotnet/roslyn-analyzers/issues/5009

        [DllImport("tier0", EntryPoint = "Msg", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Msg_Tier0([MarshalAs(UnmanagedType.LPUTF8Str)] string msg);

        [DllImport("tier0_client", EntryPoint = "Msg", CallingConvention = CallingConvention.Cdecl)]
        private static extern void Msg_Tier0_Client([MarshalAs(UnmanagedType.LPUTF8Str)] string msg);

#pragma warning restore CA2101

        private delegate void MsgFunc(string str);

        private static MsgFunc? Msg;

        public override void Write(string? value)
        {
            if (!String.IsNullOrEmpty(value) && Msg is not null)
            {
                Msg(value);
            }
        }
        public override void Write(char value)
        {
            Write(value.ToString());
        }
        public override void Write(char[]? value)
        {
            Write(new string(value));
        }
        public override void Write(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count));
        }
        public override void Write(ReadOnlySpan<char> buffer)
        {
            Write(new string(buffer));
        }
        public override void Write(StringBuilder? value)
        {
            if (value != null)
            {
                Write(value.ToString());
            }
        }
        // \n begins here
        public override void WriteLine(string? value)
        {
            if (!String.IsNullOrEmpty(value))
            {
                Write(value + NewLine);
            }
            else
            {
                WriteLine();
            }
        }
        public override void WriteLine(char value)
        {
            Write(value.ToString() + NewLine);
        }
        public override void WriteLine(char[]? buffer)
        {
            Write(new string(buffer) + NewLine);
        }
        public override void WriteLine(char[] buffer, int index, int count)
        {
            Write(new string(buffer, index, count) + NewLine);
        }
        public override void WriteLine(ReadOnlySpan<char> buffer)
        {
            Write(new string(buffer) + NewLine);
        }
        public override void WriteLine(StringBuilder? value)
        {
            if (value != null)
            {
                Write(value.ToString() + NewLine);
            }
            else
            {
                WriteLine();
            }
        }
        public override void WriteLine(bool value)
        {
            Write((value ? "True" : "False") + NewLine);
        }
        public override void WriteLine(int value)
        {
            Write(value.ToString() + NewLine);
        }
        public override void WriteLine(uint value)
        {
            Write(value.ToString() + NewLine);
        }
        public override void WriteLine(long value)
        {
            Write(value.ToString() + NewLine);
        }
        public override void WriteLine(ulong value)
        {
            Write(value.ToString() + NewLine);
        }
        public override void WriteLine(float value)
        {
            Write(value.ToString() + NewLine);
        }
        public override void WriteLine(double value)
        {
            Write(value.ToString() + NewLine);
        }
        public override void WriteLine(decimal value)
        {
            Write(value.ToString() + NewLine);
        }
        public override void WriteLine(object? value)
        {
            if (value == null)
            {
                WriteLine();
            }
            else
            {
                // Call WriteLine(value.ToString), not Write(Object), WriteLine().
                // This makes calls to WriteLine(Object) atomic.
                if (value is IFormattable f)
                {
                    WriteLine(f.ToString(null, FormatProvider));
                }
                else
                {
                    WriteLine(value.ToString());
                }
            }
        }
    }
}
