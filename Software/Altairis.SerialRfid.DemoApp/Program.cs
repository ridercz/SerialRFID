using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altairis.SerialRfid.Client;

namespace Altairis.SerialRfid.DemoApp {
    class Program {
        private const string SERIAL_PORT_NAME = "COM3";

        static void Main(string[] args) {
            try {
                Console.Write($"Connecting to reader on {SERIAL_PORT_NAME}...");
                using (var reader = new RfidReader(SERIAL_PORT_NAME)) {
                    reader.Open();
                    Console.WriteLine("OK");
                    Console.WriteLine($"  {reader.Signature} version {reader.Version}");
                    Console.WriteLine($"  Key: {reader.Key}");

                    while (true) {
                        Console.Write("Waiting for card...");
                        var card = reader.WaitForCard();
                        Console.WriteLine("OK");
                        Console.WriteLine($" UID:  {card.Uid}");
                        Console.WriteLine($" Type: {card.Type}");
                    }
                }

            }
            catch (Exception ex) {
                Console.WriteLine("Failed!");
                Console.WriteLine(ex.Message);
            }


        }

    }
}
