#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NppPluginNET.Core.Rules.Enums;

#endregion

namespace NppPluginNET.Core.Rules
{
    [Serializable]
    public class Rule : IComparer<Rule>
    {
        #region Fields

        private int _currentPredicateNumber = 0;
        private string _Keyword;
        private IList<RuleAdapter> _Adapters;
        private IList<RulePredicate> _Predicates;
        private IList<RuleSeparator> _Separators;
        private Stack<char> _SeparatorIgnoreStack;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the adapters.
        /// </summary>
        public IList<RuleAdapter> Adapters
        {
            get
            {
                if (_Adapters == null)
                {
                    _Adapters = new List<RuleAdapter>();
                } return _Adapters;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether operators should get formatted and aligned.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [format ops]; otherwise, <c>false</c>.
        /// </value>
        public bool AlignOperators { get; set; }

        /// <summary>
        /// Gets the current predicate.
        /// </summary>
        /// <value>The current predicate.</value>
        public RulePredicate CurrentPredicate
        {
            get
            {
                return Predicates.FirstOrDefault(pred => pred.Position == _currentPredicateNumber);
            }
        }

        /// <summary>
        /// Gets or sets the type of the rules align.
        /// </summary>
        /// <value>
        /// The type of the align.
        /// </value>
        public DisplayType Display { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the keyword.
        /// </summary>
        /// <value>
        /// The keyword.
        /// </value>
        public string Keyword
        {
            get
            {
                if (string.IsNullOrEmpty(_Keyword))
                {
                    _Keyword = "<default>";
                }
                return _Keyword;
            }
            set
            {
                _Keyword = value;
            }
        }

        /// <summary>
        /// Gets the separator.
        /// </summary>
        public IList<RulePredicate> Predicates
        {
            get
            {
                if (_Predicates == null)
                {
                    _Predicates = new List<RulePredicate>();
                } return _Predicates;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether spaces should be added around the function.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [add spaces]; otherwise, <c>false</c>.
        /// </value>
        public bool PreserveSpaces { get; set; }

        /// <summary>
        /// Gets or sets the separator ignore stack.
        /// </summary>
        /// <value>
        /// The separator ignore stack.
        /// </value>
        public Stack<char> SeparatorIgnoreStack
        {
            get
            {
                if (_SeparatorIgnoreStack == null)
                {
                    _SeparatorIgnoreStack = new Stack<char>();
                } return _SeparatorIgnoreStack;
            }
            set
            {
                _SeparatorIgnoreStack = value;
            }
        }

        /// <summary>
        /// Gets the separator regex.
        /// </summary>
        public string SeparatorRegex
        {
            get
            {
                //values must be enclosed in brackets to ensure that the delimiter is also
                //included in the split string array
                string regex = "(";
                foreach (RuleSeparator separator in Separators)
                {
                    if (Regex.IsMatch(separator.Value, @"\$|\||\^|\*|\(|\)|\+|\?\."))
                    {
                        regex += "\\" + separator.Value + "|";
                    }
                    else
                    {
                        regex += separator.Value + "|";
                    }
                }
                regex += ")";
                return regex.Replace("|)", ")");
            }
        }

        /// <summary>
        /// Gets the separator.
        /// </summary>
        public IList<RuleSeparator> Separators
        {
            get
            {
                if (_Separators == null)
                {
                    _Separators = new List<RuleSeparator>();
                } return _Separators;
            }
        }

        /// <summary>
        /// Gets or sets the rule.
        /// </summary>
        /// <value>
        /// The rule.
        /// </value>
        public RuleType Type { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// Value
        /// Condition
        /// Less than zero
        /// <paramref name="x"/> is less than <paramref name="y"/>.
        /// Zero
        /// <paramref name="x"/> equals <paramref name="y"/>.
        /// Greater than zero
        /// <paramref name="x"/> is greater than <paramref name="y"/>.
        /// </returns>
        public int Compare(Rule x, Rule y)
        {
            return (x.Keyword.CompareTo(y.Keyword));
        }

        /// <summary>
        /// Determines whether [contains ignore char] [the specified token].
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>
        ///   <c>true</c> if [contains ignore char] [the specified token]; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsIgnoreChar(string token)
        {
            return Separators.FirstOrDefault(separator => token.Contains(separator.SeparatorIgnore.ToString())) != null;
        }

        /// <summary>
        /// Gets the seperator.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public RuleSeparator GetSeperator(string token)
        {
            RuleSeparator separator = Separators.FirstOrDefault(seperator => seperator.Value == token);

            if (separator == null)
            {
                return null;
            }

            char? ignoreChar = this.SeparatorIgnoreStack.Count > 0 ?
                (char?)this.SeparatorIgnoreStack.Peek() :
                null;
            char? ruleIgnoreChar = separator.SeparatorIgnore;

            if (ignoreChar != ruleIgnoreChar)
            {
                //separator is not withinn ignore section
                return separator;
            }

            //ignore separator if is is within ignore block
            return null;
        }

        /// <summary>
        /// Advances the current predicate
        /// </summary>
        /// <returns></returns>
        public RulePredicate NextPredicate()
        {
            _currentPredicateNumber++;
            if (_currentPredicateNumber > Predicates.Count)
            {
                _currentPredicateNumber = Predicates.Count;
            }

            return CurrentPredicate;
        }
        
        #endregion

        #region Static Methods

        /// <summary>
        /// Copies the rule.
        /// </summary>
        /// <param name="ruleToCopy">The rule to copy.</param>
        /// <param name="reinitialize">if set to <c>true</c> reinitialize the rule's runtime variables.</param>
        /// <returns></returns>
        public static Rule CopyRule(Rule ruleToCopy, bool reinitialize)
        {
            Rule copiedRule = DeepClone(ruleToCopy);

            if (reinitialize)
            {
                copiedRule._currentPredicateNumber = 0;
                copiedRule.SeparatorIgnoreStack = null;
            }

            return copiedRule;
        }

        /// <summary>
        /// Provides a deep copy the rule.
        /// </summary>
        /// <param name="param1">The param1.</param>
        /// <returns></returns>
        private static Rule DeepClone(Rule obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (Rule)formatter.Deserialize(ms);
            }
        }

        #endregion
    }
}
