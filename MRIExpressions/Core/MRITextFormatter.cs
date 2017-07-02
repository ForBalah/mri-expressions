#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#endregion

namespace NppPluginNET.Core
{
    /// <summary>
    /// Provides methods for formatting a string buffer
    /// </summary>
    public class MRITextFormatter : IDisposable
    {
        #region Fields

        private StringBuilder _FormattedBuffer;
        private int _TabSpaces = 4;

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
        /// Gets or sets the rules.
        /// </summary>
        /// <value>The rules.</value>
        public FormatRules Rules { get; set; }

        /// <summary>
        /// Gets or sets the tab spaces.
        /// </summary>
        /// <value>The tab spaces.</value>
        public int TabSpaces
        {
            get { return _TabSpaces; }
            set { _TabSpaces = value; }
        }

        /// <summary>
        /// Gets or sets the text buffer.
        /// </summary>
        /// <value>The text buffer.</value>
        public StringBuilder TextBuffer
        {
            get; set;
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MRITextFormatter"/> class.
        /// </summary>
        public MRITextFormatter() : this(new StringBuilder())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MRITextFormatter"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public MRITextFormatter(StringBuilder buffer)
        {
            TextBuffer = buffer;
        }

        #endregion

        #region Interface Implementations

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _FormattedBuffer.Remove(0, _FormattedBuffer.Length);
        }

        #endregion
    }
}
