using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.SerialRfid.Client {
    [Serializable]
    public class RfidException : Exception {

        public string DataReceived { get; private set; }

        public RfidException() : base("Error communicating with reader") { }

        public RfidException(string message) : base(message) {
        }

        public RfidException(string message, string dataReceived) : base(message) {
            this.DataReceived = dataReceived;
        }

        public RfidException(string message, Exception innerException) : base(message, innerException) {
        }

        protected RfidException(SerializationInfo info, StreamingContext context) : base(info, context) {
            this.DataReceived = info.GetString(nameof(this.DataReceived));
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            if (info == null) throw new ArgumentNullException(nameof(info));
            info.AddValue(nameof(this.DataReceived), this.DataReceived);
            base.GetObjectData(info, context);
        }

    }
}
