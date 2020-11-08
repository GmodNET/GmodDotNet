using GmodNET.SourceSDK;
using System;
using System.IO;
using System.Text;

namespace GmodNET.Helpers
{
    public class GameConsoleWriter : TextWriter
    {
        private static GameConsoleWriter instance;
        private int references;
        public override Encoding Encoding => throw new NotImplementedException();
        public override string NewLine { get => "\n"; } // by default it's \r\n, what causes problems
        public override void Write(string value)
        {
            Tier0.Msg(value);
        }
        public static void Load()
        {
            if (instance == null)
            {
                instance = new GameConsoleWriter { references = 1 };
                Console.SetOut(instance);
            }
            else
            {
                instance.references++;
            }
        }
        public static void Unload()
        {
            instance.references--;
            if (instance.references == 0)
            {
                Console.SetOut(TextWriter.Null);
                instance = null;
            }
        }
    }
}
