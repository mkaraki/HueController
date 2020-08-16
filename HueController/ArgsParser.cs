using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HueController
{
    class ArgsParser
    {
        internal static ControlItem ParseAsync(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Not valid args");
                Console.WriteLine("use \"--help\" option to check usage.");
                Environment.Exit(7);
                return null;
            }

            var cmd = new ControlItem();

            switch (args[0])
            {
                case "on":
                    cmd.Powered = true;
                    break;

                case "off":
                    cmd.Powered = false;
                    break;

                case "color":
                    ColorParser(ref cmd, args.Skip(1).ToArray());
                    break;
            }

            return cmd;
        }

        private static void ColorParser(ref ControlItem c, string[] args)
        { 
            if (args.Length < 2)
            {
                Console.WriteLine("Not valid args");
                Console.WriteLine("use \"--help\" option to check usage.");
                Environment.Exit(7);
            }

            switch (args[0])
            {
                case "hex":
                    c.CSharpColor = HexParser(args[1]);
                    break;
            }
        }

        private static Color HexParser(string hex)
        {
            try
            {
                var c = ColorTranslator.FromHtml(hex);
                if (c == null) throw new Exception();
                return c;
            }
            catch
            { 
                Console.WriteLine("Invalid Color");
                Console.WriteLine("use \"--help\" option to check usage.");
                Environment.Exit(7);
            }
            return Color.Empty;
        }
    }
}
