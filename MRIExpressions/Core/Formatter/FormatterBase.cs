using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NppPluginNET.Core.Formatter
{
    public abstract class FormatterBase : IDisposable, IFormatter
    {
        #region Fields

        protected int _currentIndentLevel = 0;
        protected bool _IsLoaded;
        protected StringBuilder _FormattedBuffer;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the formatted buffer.
        /// </summary>
        /// <value>The formatted buffer.</value>
        public StringBuilder FormattedBuffer
        {
            get
            {
                if (_FormattedBuffer == null)
                {
                    _FormattedBuffer = new StringBuilder();
                }
                return _FormattedBuffer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the rules have been successfully loaded.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is loaded; otherwise, <c>false</c>.
        /// </value>
        public bool IsLoaded
        {
            get
            {
                return _IsLoaded && TextBuffer != null && TextBuffer.Length > 0;
            }
        }

        /// <summary>
        /// Gets or sets the text buffer.
        /// </summary>
        /// <value>The text buffer.</value>
        public StringBuilder TextBuffer
        {
            get;
            set;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Cleans the token array removing empty tokens and combining
        /// leftbraces into functions
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        protected virtual IList<string> CleanTokenArray(string[] tokens)
        {
            return tokens.ToList<string>();
        }

        /// <summary>
        /// Formats the text.
        /// </summary>
        public virtual void FormatText()
        {
            FormattedBuffer.Append(TextBuffer);
        }

        /// <summary>
        /// Gets the token array.
        /// </summary>
        /// <param name="preformatted">The preformatted.</param>
        /// <returns></returns>
        protected string[] GetTokenArray(string preformatted)
        {
            return Regex.Split(preformatted, @"(\w+|\s+|\W)", RegexOptions.CultureInvariant);
        }

        #endregion

        #region Interface Implementations
        
        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion 

        #endregion
    }
}
