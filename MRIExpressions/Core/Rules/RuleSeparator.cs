using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NppPluginNET.Core.Rules
{
    [Serializable]
    public class RuleSeparator
    {
        #region Properties

        /// <summary>
        /// Gets or sets the min occurs.
        /// </summary>
        /// <value>
        /// The min occurs.
        /// </value>
        public int MinOccurs { get; set; }

        /// <summary>
        /// Gets or sets the max occurs.
        /// </summary>
        /// <value>
        /// The max occurs.
        /// </value>
        public int MaxOccurs { get; set; }

        /// <summary>
        /// Gets or sets the separator ignore.
        /// </summary>
        /// <value>
        /// The separator ignore.
        /// </value>
        public char? SeparatorIgnore { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }

        #endregion
    }
}
