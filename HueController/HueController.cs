using Q42.HueApi;
using Q42.HueApi.ColorConverters;
using Q42.HueApi.ColorConverters.Original;
using Q42.HueApi.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HueController
{
    internal class HueController
    {
        internal static TimeSpan TimeOut { get; set; } = TimeSpan.FromSeconds(5);

        private ILocalHueClient client;

        internal async Task GetClientAsync()
        {
            int index = 0;

            IBridgeLocator locator = new HttpBridgeLocator();
            var bridges = (await locator.LocateBridgesAsync(TimeOut)).ToArray();
            if (bridges.Length < 1)
            {
                Console.WriteLine("No bridge detected");
                Environment.Exit(3);
            }
            else if (bridges.Length > 1)
            {
                Console.WriteLine("More than 2 bridges detected");
                for (int i = 0; i < bridges.Length; i++)
                {
                    Console.WriteLine($"{i}: {bridges[i].IpAddress} ({bridges[i].BridgeId})");
                }
                string codestr = Console.ReadLine();
                if (!int.TryParse(codestr, out var code))
                {
                    Console.WriteLine("Not a number. Aborted.");
                    Environment.Exit(4);
                    return;
                }
                else if (code < 0 || code > bridges.Length)
                { 
                    Console.WriteLine("Not in list. Aborted.");
                    Environment.Exit(4);
                    return;
                }

                index = code;
            }

            client = new LocalHueClient(bridges[index].IpAddress);
        }

        internal async Task<string> RegistAsync()
        {
            Console.WriteLine("===== Register Hue Bridge =====");
            Console.WriteLine("1. Press Hue Bridge's Button.");
            Console.WriteLine("2. Wait 1 second.");
            Console.WriteLine("3. Press Return (Enter).");
            Console.ReadLine();
            try
            {
                return await client.RegisterAsync("HueController", "HueController");
            }
            catch
            {
                Console.WriteLine("Failed. Bridge is not register mode.");
                Environment.Exit(5);
                return null;
            }
        }

        internal void Connect()
        {
            client.Initialize(Program.Config.ApplicationKey);
        }

        internal async Task GetLightsListAsync()
        {
            var lights = await client.GetLightsAsync();

            foreach (var l in lights)
            {
                Console.WriteLine($"\"{l.Id}\": {l.Name}");
            }
        }

        private async Task<bool> CheckLightAsync(string id)
        {
            var l = await client.GetLightAsync(id);
            if (l == null)
            {
                Console.WriteLine("No Light Detected");
                Environment.Exit(6);
                return false;
            }
            return true;
        }

        private async Task ParseAndControlLight(string lightid, ControlItem citem)
        {
            var cmd = new LightCommand();

            if (citem.Powered.HasValue)
                cmd.On = citem.Powered.Value;

            if (citem.Color.HasValue)
                cmd.TurnOn().SetColor(citem.Color.Value);

            if (citem.Alert.HasValue)
                cmd.Alert = citem.Alert.Value;

            if (citem.Effect.HasValue)
                cmd.Effect = citem.Effect.Value;

            await client.SendCommandAsync(cmd, new List<string> { lightid });
        }

        internal async Task ControlLight(string id, ControlItem items)
        {
            if (!await CheckLightAsync(id))
                return;

            await ParseAndControlLight(id, items);
        }
    }
}
