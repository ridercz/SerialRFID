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

                    while (true) {
                        Console.Write("Waiting for card...");
                        var card = reader.WaitForCard();
                        Console.WriteLine("OK");
                        Console.WriteLine($" UID:  {card.Uid}");
                        Console.WriteLine($" Type: {card.Type}");
                    }
                }

            }
            catch (RfidException ex) {
                Console.WriteLine("Unexpected response!");
                Console.WriteLine(ex.Message);
                if (!string.IsNullOrEmpty(ex.DataReceived)) {
                    Console.WriteLine("Received data:");
                    Console.WriteLine(ex.DataReceived);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Failed!");
                Console.WriteLine(ex.Message);
            }

        }
    }
}
