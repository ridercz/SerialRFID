using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.SerialRfid.Client {
    public abstract class ReaderMessage {

        internal ReaderMessage(string line) {
            if (line == null) throw new ArgumentNullException(nameof(line));
            if (string.IsNullOrWhiteSpace(line)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(line));

            line = line.Trim();
            var separatorPos = line.IndexOf(':');
            if (separatorPos < 1 || separatorPos == line.Length - 1) throw new ArgumentException("Unexpected line format - ':' is missing or at wrong place", nameof(line));

            this.Type = line.Substring(0, separatorPos).ToUpper();
            var data = line.Substring(separatorPos + 1);
            this.Fields = data.Split(new char[] { ' ' }, this.FieldCount);
        }

        public string Type { get; }

        public string[] Fields { get; }

        protected abstract int FieldCount { get; }

    }
}
