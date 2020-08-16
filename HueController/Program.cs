using Mono.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;
using YamlDotNet.Serialization;

namespace HueController
{
    class Program
    {
        internal static string ApplicationDirectory = AppDomain.CurrentDomain.BaseDirectory;

        internal static HueConfigFile Config;

        static async Task Main(string[] o_args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            string[] args;
            bool regist = false;
            bool help = false;
            int timeout = 5;

            OptionSet o = new OptionSet() {
                { "timeout=", "Set time out", (int v) => timeout = v},
                { "regist", "Regist Hue Bridge", v =>  regist = v != null},
                { "h|help", "Get help", v =>  help = v != null},
            };

            try
            {
                args = o.Parse(o_args).ToArray();
            }
            catch (OptionException ex)
            {
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
                return;
            }

            if (help)
            {
                WriteHelp();
                return;
            }

            string appConfigPath = Path.Combine(ApplicationDirectory, "hueconfig.yaml");
            if (!regist && !File.Exists(appConfigPath))
            {
                Console.WriteLine("Not configurated");
                Console.WriteLine("use \"--regist\" option to regist hue.");
                Environment.Exit(2);
                return;
            }

            HueController.TimeOut = TimeSpan.FromSeconds(timeout);

            try
            {
                var h = new HueController();
                await h.GetClientAsync();
                if (regist)
                {
                    string key = await h.RegistAsync();
                    
                    Config = new HueConfigFile();
                    Config.ApplicationKey = key;
                    await SaveConfigAsync(appConfigPath);
                    
                    Console.WriteLine("Registed");
                    Environment.Exit(0);
                }
                else
                { 
                    await LoadConfigAsync(appConfigPath);
                    h.Connect();
                }

                if (args.Length < 1)
                {
                    Console.WriteLine("No args");
                    Console.WriteLine("use \"--help\" option to check usage.");
                    Environment.Exit(7);
                }

                if (args[0] == "list")
                {
                    await h.GetLightsListAsync();
                    return;
                }

                string lid = args[0];

                var cmd = ArgsParser.ParseAsync(args.Skip(1).ToArray());

                if (cmd != null)
                    await h.ControlLight(lid, cmd);
    
            }
            catch(Exception ex)
            {
                Console.WriteLine("Unknown Error");
                Console.WriteLine(ex.Message);
                Environment.Exit(1);
            }
        }

        static async Task LoadConfigAsync(string file)
        {
            using (var sr = new StreamReader(file, Encoding.UTF8))
            using (var strreader = new StringReader(await sr.ReadToEndAsync()))
            {
                var d = new Deserializer();
                Config = d.Deserialize<HueConfigFile>(strreader);
            }
        }

        static async Task SaveConfigAsync(string file)
        {
            var s = new Serializer();
            string yaml = s.Serialize(Config);

            await File.WriteAllTextAsync(file, yaml, Encoding.UTF8);
        }

        static void WriteHelp()
        {
            Console.WriteLine(@"Hue Controller Help

Options:
--regist
    Regist Hue Bridge. (Registed Hue Bridge will save in 'hueconfig.yaml')

--help
    This help.

--timeout
    Set timeout seconds. (default 5)


Usage:
<exe> list
    List all light in bridge.

<exe> <light id> on|off
    Turn on or off light

<exe> <light id> color <format> <Colors>
    Trun On and Set Color


Color Formats:
hex (#0077FF):
    color hex #0077FF
");
        }
    }
}
