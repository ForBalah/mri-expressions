namespace NppPluginNET.Core.Rules.Enums
{
    public enum NewLineType
    {
        /// <summary>
        /// no new line
        /// </summary>
        None = 0,
        /// <summary>
        /// make a new line and indent right
        /// </summary>
        RightIndent = 1,
        /// <summary>
        /// make a new line and indent left
        /// </summary>
        LeftIndent = 2,
        /// <summary>
        /// make a new line but don't indent
        /// </summary>
        Inline = 3
    }
}
