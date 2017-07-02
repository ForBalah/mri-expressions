#region Directives

using NppPluginNET.Core.Rules.Enums; 

#endregion

namespace NppPluginNET.Core.Rules
{
    /// <summary>
    /// Defines the rules that should be applied to each line of the output
    /// </summary>
    public class LineRule
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        public LineRuleType Type { get; set; }

        /// <summary>
        /// Gets or sets the pattern.
        /// </summary>
        /// <value>The pattern.</value>
        public string Pattern { get; set; } 

        #endregion
    }
}
