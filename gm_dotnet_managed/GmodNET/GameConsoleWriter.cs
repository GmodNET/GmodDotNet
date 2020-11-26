using System;
using System.IO;
using System.Linq;
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
    public override void Write(char[] buffer, int index, int count)
    {
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (buffer.Length - index < count)
        {
            throw new ArgumentException();
        }
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < count; i++) stringBuilder.Append(buffer[index + i]);
        Write(stringBuilder.ToString());
    }
    // TODO: it is work?
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
    public override void WriteLine(string value)
    {
        Write(new string(value) + NewLine);
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
        if (buffer == null)
        {
            throw new ArgumentNullException(nameof(buffer));
        }
        if (index < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(index));
        }
        if (count < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(count));
        }
        if (buffer.Length - index < count)
        {
            throw new ArgumentException();
        }
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < count; i++) stringBuilder.Append(buffer[index + i]);
        stringBuilder.Append(NewLine);
        Write(stringBuilder.ToString());
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
}
