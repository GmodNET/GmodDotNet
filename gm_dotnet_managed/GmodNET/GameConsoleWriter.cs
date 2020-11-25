using System.IO;
using System.Runtime.InteropServices;
using System.Text;

/// <summary>
/// Redirects Console's output to game's console
/// </summary>
public class GameConsoleWriter : TextWriter
{
    public override string NewLine => "\n";
    public override Encoding Encoding => Encoding.UTF8;

#pragma warning disable CA2101
    [DllImport("tier0", CallingConvention = CallingConvention.Cdecl)]
    private static extern void Msg([MarshalAs(UnmanagedType.LPUTF8Str)] string msg);

    public override void Write(string value)
    {
        Msg(value);
    }
    public override void Write(char value)
    {
        Write(value.ToString());
    }
    public override void Write(char[] value)
    {
        Write(new string(value));
    }
}
