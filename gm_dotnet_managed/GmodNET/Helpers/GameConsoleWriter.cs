using GmodNET.SourceSDK;
using System;
using System.IO;
using System.Text;

namespace GmodNET.Helpers
{
    public class GameConsoleWriter : TextWriter
    {
        private static GameConsoleWriter instance;
        public override Encoding Encoding => throw new NotImplementedException();
        public override string NewLine { get => "\n"; } // by default it's `\r\n`, what causes problems
        public override void Write(string value)
        {
            Tier0.Msg(value);
        }
        public static void Load()
        {
            instance = new GameConsoleWriter();
            Console.SetOut(instance);
        }
        public static void Unload()
        {
            Console.SetOut(TextWriter.Null);
            instance = null;
        }
    }
}
