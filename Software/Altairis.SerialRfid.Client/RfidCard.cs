using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.SerialRfid.Client {
    /// <summary>
    /// Class representing the card being read.
    /// </summary>
    public class RfidCard {

        /// <summary>
        /// Gets or sets the card unique identifier (UID).
        /// </summary>
        /// <value>
        /// The card UID value.
        /// </value>
        public string Uid { get; set; }

        /// <summary>
        /// Gets or sets the card type.
        /// </summary>
        /// <value>
        /// The string description of card type.
        /// </value>
        public string Type { get; set; }

    }
}
