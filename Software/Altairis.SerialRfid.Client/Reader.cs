using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;

namespace Altairis.SerialRfid.Client {
    public class Reader {
        private const int PORT_SPEED = 115200;
        private string _signature;
        private string _version;
        private string _key;
        private SerialPort _port;

        public Reader(string portName) {
            if (portName == null) throw new ArgumentNullException(nameof(portName));
            if (string.IsNullOrWhiteSpace(portName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(portName));

            var availablePorts = SerialPort.GetPortNames();
            if (!availablePorts.Contains(portName, StringComparer.OrdinalIgnoreCase)) throw new ArgumentOutOfRangeException(nameof(portName), "Port not available. Available ports: " + string.Join(", ", availablePorts));

            this._port = new SerialPort(portName, PORT_SPEED);
        }

        public string PortName {
            get {
                return this._port.PortName;
            }
        }

        public string Signature {
            get {
                if (string.IsNullOrEmpty(this._signature)) throw new InvalidOperationException();
                return this._signature;
            }
        }

        public string Version {
            get {
                if (string.IsNullOrEmpty(this._version)) throw new InvalidOperationException();
                return this._version;
            }
        }

        public string Key {
            get {
                if (string.IsNullOrEmpty(this._key)) throw new InvalidOperationException();
                return this._signature;
            }
        }

        public void Open() {
            if (!this._port.IsOpen) this._port.Open();
        }

        public void Close() {
            if (this._port.IsOpen) this._port.Close();
        }

    }
}
