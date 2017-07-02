using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NppPluginNET.Core.Rules
{
    [Serializable]
    public class RulePredicate
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether operators should get formatted and aligned.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [format ops]; otherwise, <c>false</c>.
        /// </value>
        public bool AlignOperators { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [preserve spaces].
        /// </summary>
        /// <value><c>true</c> if [preserve spaces]; otherwise, <c>false</c>.</value>
        public bool PreserveSpaces { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        /// <value>The position.</value>
        public int Position { get; set; }

        #endregion
    }
}
