using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Altairis.SerialRfid.Client {
    public class RfidReader :IDisposable {
        private const int PORT_SPEED = 115200;
        private SerialPort _port;

        public RfidReader(string portName) {
            if (portName == null) throw new ArgumentNullException(nameof(portName));
            if (string.IsNullOrWhiteSpace(portName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(portName));

            var availablePorts = SerialPort.GetPortNames();
            if (!availablePorts.Contains(portName, StringComparer.OrdinalIgnoreCase)) throw new ArgumentOutOfRangeException(nameof(portName), "Port not available. Available ports: " + string.Join(", ", availablePorts));

            this._port = new SerialPort(portName, PORT_SPEED, Parity.None, 8, StopBits.One);
            this._port.DtrEnable = true;
            this._port.NewLine = "\r\n";
        }

        public string PortName {
            get {
                return this._port.PortName;
            }
        }

        public string Version { get; private set; }

        public string Signature { get; private set; }

        public string Key { get; private set; }

        public void Open() {
            if (!this._port.IsOpen) this._port.Open();

            // Read version
            var versionLine = this._port.ReadLine();
            var versionData = versionLine.Split(new char[] { ' ' }, 3);
            if (versionData.Length != 3 || !RfidReaderVerb.Version.Equals(versionData[0], StringComparison.OrdinalIgnoreCase)) throw new RfidException($"Expected '{RfidReaderVerb.Version}' verb.", versionLine);
            this.Version = versionData[1];
            this.Signature = versionData[2];

            // Read key
            var keyLine = this._port.ReadLine();
            var keyData = keyLine.Split(new char[] { ' ' }, 2);
            if (keyData.Length != 2 || !RfidReaderVerb.Key.Equals(keyData[0], StringComparison.OrdinalIgnoreCase)) throw new RfidException($"Expected '{RfidReaderVerb.Key}' verb.", keyLine);
            this.Key = keyData[1];
        }

        public RfidCard WaitForCard(bool throwOnError = true) {
            while (true) {
                try {
                    var cardLine = this._port.ReadLine();
                    var cardData = cardLine.Split(new char[] { ' ' }, 3);
                    if (cardData.Length != 3 || !RfidReaderVerb.Card.Equals(cardData[0], StringComparison.OrdinalIgnoreCase)) throw new RfidException($"Expected '{RfidReaderVerb.Card}' verb.", cardLine);

                    return new RfidCard {
                        Uid = cardData[1],
                        Type = cardData[2]
                    };
                }
                catch (RfidException) when (!throwOnError) { }
            }
        }

        public void Close() {
            if (this._port.IsOpen) this._port.Close();
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                this._port?.Dispose();
                this._port = null;
            }
        }

    }
}
