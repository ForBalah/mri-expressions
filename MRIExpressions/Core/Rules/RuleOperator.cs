using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NppPluginNET.Core.Rules.Enums;

namespace NppPluginNET.Core.Rules
{
    public class RuleOperator
    {
        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [new line].
        /// </summary>
        /// <value><c>true</c> if [new line]; otherwise, <c>false</c>.</value>
        public NewLineType NewLine { get; set; }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        public string Operator { get; set; }

        /// <summary>
        /// Gets or sets the space after.
        /// </summary>
        /// <value>The space after.</value>
        public bool SpaceAfter { get; set; }

        /// <summary>
        /// Gets or sets the space before.
        /// </summary>
        /// <value>The space before.</value>
        public bool SpaceBefore { get; set; }

        #endregion
    }
}
