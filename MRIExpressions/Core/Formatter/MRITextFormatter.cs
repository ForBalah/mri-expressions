#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NppPluginNET.Core.Rules;
using NppPluginNET.Core.Collections;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;
using NppPluginNET.Core.Constants;
using NppPluginNET.Core.Rules.Enums;

#endregion

namespace NppPluginNET.Core.Formatter
{
    /// <summary>
    /// Provides methods for formatting a string buffer
    /// </summary>
    public class MRITextFormatter : FormatterBase
    {
        #region Fields

        private FormatRules _Rules;
        private Stack<Rule> _ruleStack;
        private Stack<int> _ruleStartPosStack;
        private int _TabWidth = 4;
        private bool _AutoCaseFunctions = true;
        private uint _currentDirective = 0;

        //directives
        /// <summary>
        /// trim tokens with just spaces until a non space token is found
        /// </summary>
        private const uint TF_TRIMNEXTSPACE = 0x1;
        /// <summary>
        /// count the number of indents till now-space is reached on a line
        /// </summary>
        private const uint TF_COUNTINDENTS = 0x2;
        /// <summary>
        /// Track the number of columns between tokens being printed out
        /// </summary>
        private const uint TF_TRACKCOLUMNS = 0x4;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether [auto case functions].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [auto case functions]; otherwise, <c>false</c>.
        /// </value>
        public bool AutoCaseFunctions
        {
            get { return _AutoCaseFunctions; }
            set { _AutoCaseFunctions = value; }
        }

        /// <summary>
        /// Gets or sets the rules.
        /// </summary>
        /// <value>
        /// The rules.
        /// </value>
        public FormatRules Rules
        {
            get
            {
                return _Rules;
            }
            set
            {
                _Rules = value;
                if (_Rules != null)
                {
                    _IsLoaded = _Rules.Rules.IsSynchronized;
                }
                else
                {
                    _IsLoaded = false;
                }
            }
        }

        /// <summary>
        /// Gets or sets the tab spaces.
        /// </summary>
        /// <value>The tab spaces.</value>
        public int TabWidth
        {
            get { return _TabWidth; }
            set { _TabWidth = value; }
        }

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MRITextFormatter"/> class.
        /// </summary>
        public MRITextFormatter()
            : this(new StringBuilder())
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="MRITextFormatter"/> class.
        /// </summary>
        /// <param name="buffer">The buffer.</param>
        public MRITextFormatter(StringBuilder buffer)
        {
            TextBuffer = buffer;
            _IsLoaded = false;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Aligns the contents of the function according to the indent level
        /// </summary>
        /// <param name="currentRule">The current rule.</param>
        private void AlignContents(Rule currentRule)
        {
            if (_ruleStartPosStack.Count > 0)
            {
                int startIndex = _ruleStartPosStack.Peek();
                int length = _FormattedBuffer.Length - startIndex;
                if (length < 1)
                {
                    return;
                }

                string contents = _FormattedBuffer.ToString(startIndex, length);
                //replace tabs with spaces for easy processing
                contents = contents.Replace("\t", string.Empty.PadLeft(TabWidth));
                MatchCollection spaces = Regex.Matches(contents, @"^\s*", RegexOptions.Multiline);

                int minSpaces = int.MaxValue;
                for (int i = 1; i < spaces.Count; i++)
                {
                    Match match = spaces[i];
                    if (match.Length >= (_currentIndentLevel * TabWidth) && match.Length < minSpaces)
                    {
                        minSpaces = match.Length;
                    }
                }

                //shift the contents left
                if (minSpaces < int.MaxValue)
                {
                    contents = Regex.Replace(contents,
                        @"^ {" + minSpaces + "}",
                        string.Empty.PadLeft(_currentIndentLevel * TabWidth, ' '),
                        RegexOptions.Multiline);
                }

                //finally update the buffer
                _FormattedBuffer.Remove(startIndex, length);
                _FormattedBuffer.Append(contents);
            }
        }

        /// <summary>
        /// Cleans the brace space.
        /// </summary>
        /// <param name="_FormattedBuffer">The _ formatted buffer.</param>
        private string CleanBracketSpaces(string text)
        {
            //clears the spaces around brackets
            //{ and }
            string cleanedString = Regex.Replace(text, @"[\s\r\n]*\{[\s\r\n]*", "{");
            cleanedString = Regex.Replace(cleanedString, @"[\s\r\n]*\}", "}");
            //[ and ]
            cleanedString = Regex.Replace(cleanedString, @"\[[\s\r\n]*", "[");
            cleanedString = Regex.Replace(cleanedString, @"[\s\r\n]*\]", "]");
            return cleanedString;
        }

        /// <summary>
        /// Clears the formatter directive.
        /// </summary>
        /// <param name="directive">The directive.</param>
        private void ClearDirective(uint directive)
        {
            if ((_currentDirective & directive) != 0)
            {
                _currentDirective ^= directive;
            }
        }

        /// <summary>
        /// Cleans the token array removing empty tokens and combining
        /// leftbraces into functions
        /// </summary>
        /// <param name="tokens">The tokens.</param>
        protected override IList<string> CleanTokenArray(string[] tokens)
        {
            IList<string> tokensList = new List<string>();
            for (int i = 0; i < tokens.Length; i++)
            {
                //only add non empty strings
                if (!string.IsNullOrEmpty(tokens[i] as string))
                {
                    if (tokens[i].Contains('{') && tokensList.Count > 0)
                    {
                        tokensList[tokensList.Count - 1] = tokensList.Last() + tokens[i];
                    }
                    else
                    {
                        tokensList.Add(tokens[i]);
                    }
                }
            }

            return tokensList;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public new void Dispose()
        {
            _ruleStack.Clear();
            _ruleStartPosStack.Clear();
        }

        /// <summary>
        /// Formats the text.
        /// </summary>
        public override void FormatText()
        {
            if (!IsLoaded)
            {
                throw new ArgumentException("Formatter cannot be executed. Make sure the textbuffer and rules have been set.");
            }

            InitializeEngine();

            //preprocess the input string
            string preformatted = CleanBracketSpaces(TextBuffer.ToString());
            string[] tokenArray = GetTokenArray(preformatted);
            IList<string> tokens = CleanTokenArray(tokenArray);

            int tokenCount = tokens.Count;
            for (int i = 0; i < tokenCount; i++)
            {
                string token = tokens[i];
                //first determine the current rule to apply
                Rule currentRule = null;
                //process open braces
                if (token.Contains('{'))
                {
                    try
                    {
                        currentRule = Rule.CopyRule(Rules.Rules[Regex.Replace(token, @"\W+", "")], true);
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(string.Format("{0}\r\n{1}", e.Message, e.StackTrace.Substring(0, 300)), e.GetType().Name);
                    }
                    _ruleStack.Push(currentRule);
                    //process function capitalization
                    if (AutoCaseFunctions)
                    {
                        token = token.ToUpper();
                    }
                }
                else if (token.Contains('}'))
                {
                    //process closed braces
                    if (_ruleStack.Count > 0)
                    {
                        currentRule = _ruleStack.Pop();
                    }
                }
                else
                {
                    currentRule = (_ruleStack.Count > 0) ? _ruleStack.Peek() : null;
                }

                ProcessRule(currentRule, token);
            }

            //do text finalizations
            PostProcessFormattedText();
        }

        /// <summary>
        /// Indents the token and performs any controls attached to it.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private string IndentToken(string token)
        {
            string controlRegex = string.Format(@"(\r\n|{0})", FormatRules.ControlRegex.ToString());
            string[] splitToken = Regex.Split(token, controlRegex);
            string result = string.Empty;

            foreach (string subToken in splitToken)
            {
                //do process
                if (subToken.Contains(FormatRules.LEFTINDENT))
                {
                    _currentIndentLevel--;
                }
                else if (subToken.Contains(FormatRules.RIGHTINDENT))
                {
                    _currentIndentLevel++;
                }
                else if (subToken.Contains(FormatRules.INLINE))
                {
                    //from whats in the buffer already, tab backwards or forwards
                    string buffer = _FormattedBuffer.ToString();
                    Match trailingSpaceMatch = Regex.Match(buffer, @"[ \t]+$|(?<=[^ \t][ \t]*\r\n)\s*$", RegexOptions.None);
                    Match lineMatch = Regex.Match(buffer, @"[^\r\n]+$", RegexOptions.None);
                    //work with spaces cos its easier
                    string lineString = lineMatch.Value.TrimEnd().Replace("\t", string.Empty.PadRight(TabWidth, ' '));

                    int padAmount = (_currentIndentLevel * TabWidth) - lineString.Length;
                    if (padAmount > 0)
                    {
                        result += string.Empty.PadRight(padAmount, ' ');
                    }

                    //remove spaces to prepare for token
                    _FormattedBuffer.Remove(trailingSpaceMatch.Index, trailingSpaceMatch.Length);
                }
                else if (subToken.Equals("\r\n"))
                {
                    result += "\r\n".PadRight((_currentIndentLevel * TabWidth) + 2, ' ');
                }
                else
                {
                    //can just add token
                    result += subToken;
                }
            }
            return result;
        }

        /// <summary>
        /// Initializes the format engine.
        /// </summary>
        private void InitializeEngine()
        {
            _currentDirective = 0;
            _FormattedBuffer = new StringBuilder();
            _ruleStack = new Stack<Rule>(); //each active rule is stored here
            _ruleStartPosStack = new Stack<int>(); //the position of the rule in the final string.
            RaiseDirective(TF_TRIMNEXTSPACE);
        }

        /// <summary>
        /// Preformats the operator in the token according to the rule
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private string PreFormatOperator(Rule currentRule, string token)
        {
            string op = RawToken(token);
            if (Rules.ContainsOperator(op))
            {
                RuleOperator ruleOp = Rules.Operators[op];
                //spaces
                if (ruleOp.SpaceBefore)
                {
                    op = string.Format(" {0}", op);
                }
                if (ruleOp.SpaceAfter)
                {
                    op = string.Format("{0} ", op);
                }
                //newlines
                if (ruleOp.NewLine == NewLineType.LeftIndent && currentRule.Display == DisplayType.Block)
                {
                    op = string.Format("{0}\r\n{1}", FormatRules.LEFTINDENT, op);
                }
                else if (ruleOp.NewLine == NewLineType.RightIndent && currentRule.Display == DisplayType.Block)
                {
                    op = string.Format("{0}{1}\r\n", op, FormatRules.RIGHTINDENT);
                }
                else if (ruleOp.NewLine == NewLineType.Inline && currentRule.Display == DisplayType.Block)
                {
                    op = string.Format("{0}\r\n", op);
                }
            }

            //add the formatted op back to the token
            string[] subTokens = Regex.Split(token, string.Format("({0})", FormatRules.ControlRegex.ToString()));
            StringBuilder tokenBuilder = new StringBuilder();
            for (int i = 0; i < subTokens.Length; i++)
            {
                if (subTokens[i] == RawToken(token))
                {
                    subTokens[i] = op;
                }
                tokenBuilder.Append(subTokens[i]);
            }

            return tokenBuilder.ToString();
        }

        /// <summary>
        /// Pre-processes the token according to the rules that should be applied beforehand.
        /// </summary>
        /// <param name="currentRule">The current rule.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private string PreProcessToken(Rule currentRule, string token)
        {
            //note predicates take precedence
            //spaces
            bool preserveSpaces = currentRule.CurrentPredicate != null ?
                currentRule.CurrentPredicate.PreserveSpaces :
                currentRule.PreserveSpaces;
            if (!preserveSpaces)
            {
                //spaces should be removed if they must not be there
                token = token.TrimStart();
            }

            //separator tokens
            if (currentRule.GetSeperator(RawToken(token)) != null)
            {
                switch (currentRule.Display)
                {
                    case DisplayType.Block:
                        //move new lines to the end of the subtoken for separators
                        //and align to current indent
                        token = string.Format("{0}{1}\r\n", FormatRules.INLINE, token);
                        break;
                    case DisplayType.Inline:
                        //for inlines, add spaces after the separator
                        token += " ";
                        break;
                }
            }

            //rule display=block processes
            if (token.Contains('{') && currentRule.Display == DisplayType.Block)
            {
                token = string.Format("{0}{1}\r\n", token, FormatRules.RIGHTINDENT);
            }

            //for close tokens process the contents of the function before closing it off
            if (token.Contains('}'))
            {
                //foreach (RuleAdapter adapter in currentRule.Adapters)
                //{
                //    RunAdapterOnContents(adapter);
                //}

                if (currentRule.Display == DisplayType.Block)
                {
                    //on function close align the contents of the function first
                    if (currentRule.PreserveSpaces)
                    {
                        AlignContents(currentRule);
                    }

                    //mark end of function's content
                    if (_ruleStartPosStack.Count > 0)
                    {
                        _ruleStartPosStack.Pop();
                    }

                    //append control to token
                    token = string.Format("{0}\r\n{1}", FormatRules.LEFTINDENT, token);
                }
            }

            //process operators
            bool alignoperators = currentRule.CurrentPredicate != null ?
                currentRule.CurrentPredicate.AlignOperators :
                currentRule.AlignOperators;

            if (alignoperators)
            {
                token = PreFormatOperator(currentRule, token);
            }

            return token;
        }

        /// <summary>
        /// Post-processes the final text before returning it
        /// </summary>
        private void PostProcessFormattedText()
        {
            string[] textLines = Regex.Split(_FormattedBuffer.ToString(), @"\r\n");
            _FormattedBuffer.Remove(0, _FormattedBuffer.Length); //the horror!
            for (int i = 0; i < textLines.Length; i++)
            {
                string textLine = textLines[i];
                //dont process the last line if it is empty.
                if (i != (textLines.Length - 1) || !string.IsNullOrEmpty(textLine))
                {
                    string spaceFormat = "{0} ";
                    IDictionary<LineRuleType, MatchCollection> matchedLineRules = Rules.MatchLineRule(textLine);
                    foreach (var lineMatch in matchedLineRules)
                    {
                        switch (lineMatch.Key)
                        {
                            case LineRuleType.SuppressSpace:
                                spaceFormat = spaceFormat.TrimEnd();
                                break;
                        }
                    }

                    _FormattedBuffer.AppendLine(string.Format(spaceFormat, textLine.TrimEnd()));
                }
            }
        }

        /// <summary>
        /// Postprocesses the token.
        /// </summary>
        /// <param name="currentRule">The current rule.</param>
        /// <param name="token">The token.</param>
        private void PostProcessToken(Rule currentRule, string token)
        {
            string rawToken = RawToken(token);
            //mark the start of the rule's content
            if (rawToken.Contains('{') && currentRule.Display == DisplayType.Block)
            {
                _ruleStartPosStack.Push(_FormattedBuffer.Length);
            }

            //final separator processing
            if (currentRule.GetSeperator(rawToken) != null)
            {
                //advance predicates
                currentRule.NextPredicate();
                //spaces after separator must be removed.
                RaiseDirective(TF_TRIMNEXTSPACE);
            }

            //separator ignore characters
            if (currentRule.ContainsIgnoreChar(rawToken))
            {
                if (currentRule.SeparatorIgnoreStack.Count > 0 && currentRule.SeparatorIgnoreStack.Peek() == rawToken[0])
                {
                    //close matching ignore characters
                    currentRule.SeparatorIgnoreStack.Pop();
                }
                else
                {
                    currentRule.SeparatorIgnoreStack.Push(rawToken[0]);
                }
            }
        }

        /// <summary>
        /// Processes the rule.
        /// </summary>
        /// <param name="currentRule">The current rule.</param>
        /// <param name="token">The token.</param>
        private void ProcessRule(Rule currentRule, string token)
        {
            //do directives first
            ProcessDirectives(ref token);

            if (currentRule == null)
            {
                //add spaces at the end of lines
                _FormattedBuffer.Append(Regex.Replace(token, @"\r\n", " \r\n"));
                //reset indent level
                _currentIndentLevel = 0;
                return;
            }

            //do rules before token is processed
            token = PreProcessToken(currentRule, token);

            //do process at token level
            ProcessToken(currentRule, token);

            //finalize processing for this token according to the rule
            PostProcessToken(currentRule, token);
        }

        /// <summary>
        /// Processes the formatter directive.
        /// </summary>
        /// <param name="token">The token to process in the directive.</param>
        private void ProcessDirectives(ref string token)
        {
            if ((_currentDirective & TF_TRIMNEXTSPACE) != 0)
            {
                //trim until a non empty token is found
                if (!string.IsNullOrEmpty(token.Trim()))
                {
                    ClearDirective(TF_TRIMNEXTSPACE);
                }
                else
                {
                    token = string.Empty;
                }
            }
        }

        /// <summary>
        /// Processes the token in rule.
        /// </summary>
        /// <param name="currentRule">The current rule.</param>
        /// <param name="token">The token.</param>
        private void ProcessToken(Rule currentRule, string token)
        {
            _FormattedBuffer.Append(IndentToken(token));
        }

        /// <summary>
        /// Raises the formatter directive for the next directive processing round
        /// </summary>
        /// <param name="directive">The directive.</param>
        private void RaiseDirective(uint directive)
        {
            //add the bit to the directive 'register'
            _currentDirective |= directive;
        }

        /// <summary>
        /// returns the raw token without any control characters and spaces
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        private string RawToken(string token)
        {
            return FormatRules.ControlRegex.Replace(token, string.Empty).Trim();
        }

        /// <summary>
        /// Runs the adapter on contents.
        /// </summary>
        /// <param name="adapter">The adapter.</param>
        private void RunAdapterOnContents(RuleAdapter adapter)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Sets the indent level.
        /// </summary>
        /// <param name="level">The level.</param>
        public void SetIndentLevel(int level)
        {
            if (level < 0)
            {
                _currentIndentLevel = 0;
                return;
            }

            _currentIndentLevel = level;
        }

        #endregion
    }
}
