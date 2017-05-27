using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Altairis.SerialRfid.Client {
    /// <summary>
    /// Class representing the RFID reader.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class RfidReader :IDisposable {
        private const int PORT_SPEED = 115200;
        private const string DEBUG_STRING = "VMDPV";
        private SerialPort _port;

        /// <summary>
        /// Initializes a new instance of the <see cref="RfidReader"/> class.
        /// </summary>
        /// <param name="portName">Name of the serial port, ie. <c>COM1</c>.</param>
        /// <exception cref="System.ArgumentNullException">portName</exception>
        /// <exception cref="System.ArgumentException">Value cannot be empty or whitespace only string.</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">portName - Port not available.</exception>
        public RfidReader(string portName) {
            if (portName == null) throw new ArgumentNullException(nameof(portName));
            if (string.IsNullOrWhiteSpace(portName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(portName));

            var availablePorts = SerialPort.GetPortNames();
            if (!availablePorts.Contains(portName, StringComparer.OrdinalIgnoreCase)) throw new ArgumentOutOfRangeException(nameof(portName), "Port not available. Available ports: " + string.Join(", ", availablePorts));

            this._port = new SerialPort(portName, PORT_SPEED, Parity.None, 8, StopBits.One);
            this._port.DtrEnable = true;
            this._port.NewLine = "\r\n";
        }

        /// <summary>
        /// Gets the name of the serial port.
        /// </summary>
        /// <value>
        /// The name of the serial port.
        /// </value>
        public string PortName {
            get {
                return this._port.PortName;
            }
        }

        /// <summary>
        /// Gets the reader firmware version.
        /// </summary>
        /// <value>
        /// The reader firmware version.
        /// </value>
        public string Version { get; private set; }

        /// <summary>
        /// Gets the reader firmware name.
        /// </summary>
        /// <value>
        /// The reader firmware name.
        /// </value>
        public string Signature { get; private set; }

        /// <summary>
        /// Opens the communication with reader and waits for initial message (version and signature).
        /// </summary>
        public void Open() {
            if (!this._port.IsOpen) this._port.Open();

            // Wait for version message
            while (true) {
                var versionLine = this._port.ReadLine();
                var versionData = versionLine.Split(new char[] { ' ' }, 3);
                if (versionData.Length != 3 || !RfidReaderVerb.Version.Equals(versionData[0], StringComparison.OrdinalIgnoreCase)) continue;

                // Read version and signature
                this.Version = versionData[1];
                this.Signature = versionData[2];
                break;
            }
        }

        /// <summary>
        /// Waits for card being read and returns card information.
        /// </summary>
        /// <param name="throwOnError">if set to <c>true</c> (default) will throw <see cref="RfidException"/> on error. Otherwise it will silently eat the exception and wait for next successful read.</param>
        /// <returns>Returns instance of <see cref="RfidCard"/> containing information read from card.</returns>
        /// <exception cref="RfidException"></exception>
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

        /// <summary>
        /// Closes communication with reader.
        /// </summary>
        public void Close() {
            if (this._port.IsOpen) this._port.Close();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                this._port?.Dispose();
                this._port = null;
            }
        }

    }
}
