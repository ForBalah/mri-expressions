#region Directives

using System;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using NppPluginNET.Core.Collections;
using NppPluginNET.Core.Rules.Enums;
using System.Collections.Generic;

#endregion

namespace NppPluginNET.Core.Rules
{
    public class FormatRules
    {
        #region Fields

        private LineRuleCollection _LineRules;
        private OperatorCollection _Operators;
        private RuleCollection _Rules;
        public static Regex ControlRegex = new Regex(@"`&[^;]*;");

        /// <summary>
        /// control to indent contents 1 tab to the left
        /// </summary>
        public const string LEFTINDENT = "`&lind;";
        /// <summary>
        /// control to indent contents 1 tab to the right
        /// </summary>
        public const string RIGHTINDENT = "`&rind;";
        /// <summary>
        /// control to 'shift-tab' on current line to the current indent. only has an effect on lines with trailing spaces
        /// </summary>
        public const string INLINE = "`&inln;";

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the line rules.
        /// </summary>
        /// <value>The line rules.</value>
        public LineRuleCollection LineRules
        {
            get
            {
                if (_LineRules == null)
                {
                    _LineRules = new LineRuleCollection();
                }
                return _LineRules;
            }
        }

        /// <summary>
        /// Gets the operators.
        /// </summary>
        public OperatorCollection Operators
        {
            get
            {
                if (_Operators == null)
                {
                    _Operators = new OperatorCollection();
                }
                return _Operators;
            }
        }

        /// <summary>
        /// Gets the rules.
        /// </summary>
        public RuleCollection Rules
        {
            get
            {
                if (_Rules == null)
                {
                    _Rules = new RuleCollection();
                }
                return _Rules;
            }
        }

        #endregion

        #region Constructors and Destructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="FormatRules"/> class.
        /// </summary>
        public FormatRules()
        {

        } 

        #endregion

        #region Methods

        /// <summary>
        /// Determines whether the specified op contains operator.
        /// </summary>
        /// <param name="op">The op.</param>
        /// <returns>
        /// 	<c>true</c> if the specified op contains operator; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsOperator(string opString)
        {
            if (string.IsNullOrEmpty(opString))
            {
                return false;
            }

            return Operators[opString.Trim()] != null;
        }

        /// <summary>
        /// Determines whether the specified keyword contains rule.
        /// </summary>
        /// <param name="keyword">The keyword.</param>
        /// <returns>
        ///   <c>true</c> if the specified keyword contains rule; otherwise, <c>false</c>.
        /// </returns>
        public bool ContainsRule(string keyword)
        {
            return Rules[keyword] != null;
        }

        /// <summary>
        /// Loads the rules.
        /// </summary>
        /// <param name="ruleFileName">Name of the rule file.</param>
        /// <returns></returns>
        public void LoadRules(string ruleFileName)
        {
            XDocument xDocument = XDocument.Load(ruleFileName);
            foreach (XElement element in xDocument.Root.Descendants("rule"))
            {
                Rule rule = new Rule();
                rule.Keyword = element.Attribute("keyword") != null ?
                    element.Attribute("keyword").Value.Trim() : null;
                rule.IsDefault = element.Attribute("default") != null ?
                    bool.Parse(element.Attribute("default").Value) : false;
                rule.Display = (DisplayType)Enum.Parse(typeof(DisplayType), element.Attribute("display").Value, true);
                rule.Type = (RuleType)Enum.Parse(typeof(RuleType), element.Attribute("type").Value, true);
                rule.PreserveSpaces = element.Attribute("preservespaces") != null ?
                    bool.Parse(element.Attribute("preservespaces").Value) :
                    false;
                rule.AlignOperators = element.Attribute("alignoperators") != null ?
                    bool.Parse(element.Attribute("alignoperators").Value) :
                    false;
                foreach (XElement child in element.Elements())
                {
                    if (child.Name == "separator")
                    {
                        rule.Separators.Add(new RuleSeparator()
                        {
                            Value = child.Value.Trim(),
                            MinOccurs = int.Parse(child.Attribute("minoccurs").Value),
                            MaxOccurs = int.Parse(child.Attribute("maxoccurs").Value),
                            SeparatorIgnore = child.Attribute("separatorignore") != null ? 
                                (char?)child.Attribute("separatorignore").Value[0] :
                                null
                        });
                    }
                    else if (child.Name == "predicate")
                    {
                        rule.Predicates.Add(new RulePredicate()
                        {
                            Position = int.Parse(child.Attribute("position").Value),
                            PreserveSpaces = child.Attribute("preservespaces") != null ?
                                bool.Parse(child.Attribute("preservespaces").Value) :
                                rule.PreserveSpaces,
                            AlignOperators = child.Attribute("alignoperators") != null ?
                                bool.Parse(child.Attribute("alignoperators").Value) :
                                false
                        });
                    }
                    else if (child.Name == "adapter")
                    {
                        rule.Adapters.Add(new RuleAdapter()
                        {
                            Formatter = Type.GetType(child.Attribute("formatter").Value)
                        });
                    }
                }

                Rules.Add(rule);
            }

            //operators
            if (xDocument.Root.Element("operators") != null)
            {
                foreach (XElement element in xDocument.Root.Element("operators").Descendants("operator"))
                {
                    if (!string.IsNullOrEmpty(element.Value))
                    {
                        Operators.Add(new RuleOperator()
                        {
                            Operator = element.Value.Trim(),
                            SpaceAfter = element.Attribute("spaceafter") != null ?
                                bool.Parse(element.Attribute("spaceafter").Value) :
                                false,
                            SpaceBefore = element.Attribute("spacebefore") != null ?
                                bool.Parse(element.Attribute("spacebefore").Value) :
                                false,
                            NewLine = element.Attribute("newline") != null ?
                                (NewLineType)Enum.Parse(typeof(NewLineType), element.Attribute("newline").Value, true) :
                                NewLineType.None
                        });
                    }
                }
            }

            //line rules
            if (xDocument.Root.Element("linerules") != null)
            {
                foreach (XElement element in xDocument.Root.Element("linerules").Descendants("linerule"))
                {
                    LineRules.Add(new LineRule()
                    {
                        Name = element.Attribute("name").Value.Trim(),
                        Pattern = element.Attribute("pattern").Value,
                        Type = (LineRuleType)Enum.Parse(typeof(LineRuleType), element.Attribute("type").Value, true)
                    });
                }
            }
        }

        /// <summary>
        /// Matcheses the line rule.
        /// </summary>
        /// <param name="line">The line.</param>
        /// <returns>returns the matches for each line rule</returns>
        public IDictionary<LineRuleType, MatchCollection> MatchLineRule(string line)
        {
            IDictionary<LineRuleType, MatchCollection> matchedLineRules = new Dictionary<LineRuleType, MatchCollection>();
            foreach (LineRule lineRule in LineRules)
            {
                MatchCollection matches = Regex.Matches(line, lineRule.Pattern);
                if (matches != null && matches.Count > 0)
                {
                    matchedLineRules.Add(lineRule.Type, matches);
                }
            }

            return matchedLineRules;
        }

        #endregion
    }
}
