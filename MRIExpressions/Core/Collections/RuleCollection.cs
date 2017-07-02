#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using NppPluginNET.Core.Rules; 

#endregion

namespace NppPluginNET.Core.Collections
{
    public class RuleCollection : ICollection<Rule>
    {
        #region Fields

        private IList<Rule> _innerCollection;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="NppPluginNET.Core.Rules.Rule"/> at the specified index.
        /// </summary>
        public Rule this[int index]
        {
            get { return _innerCollection[index]; }
            set { _innerCollection[index] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="NppPluginNET.Core.Rules.Rule"/> with the specified keyword.
        /// </summary>
        public Rule this[string keyword]
        {
            get
            {
                Rule ruleToReturn = _innerCollection.FirstOrDefault(rule => rule.Keyword == keyword);
                if (ruleToReturn == null)
                {
                    return _innerCollection.FirstOrDefault(rule => rule.IsDefault);
                }
                return ruleToReturn;
            }
            set
            {
                Rule rule = _innerCollection.First(r => r.Keyword == keyword);
                _innerCollection[_innerCollection.IndexOf(rule)] = value;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="NppPluginNET.Core.Rules.Rule"/> contains a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="NppPluginNET.Core.Rules.Rule"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see cref="NppPluginNET.Core.Rules.Rule"/>; otherwise, false.
        /// </returns>
        public bool Contains(Rule item)
        {
            return _innerCollection.Contains(item);
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="NppPluginNET.Core.Rules.Rule"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="NppPluginNET.Core.Rules.Rule"/>.
        ///   </returns>
        public int Count
        {
            get { return _innerCollection.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="NppPluginNET.Core.Rules.Rule"/> is read-only.
        /// </summary>
        /// <returns>true if the <see cref="NppPluginNET.Core.Rules.Rule"/> is read-only; otherwise, false.
        ///   </returns>
        public bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is synchronized.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is synchronized; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynchronized
        {
            get
            {
                return _innerCollection.Count > 0;
            }
        }

        /// <summary>
        /// Gets the sync root.
        /// </summary>
        public object SyncRoot
        {
            get { return null; }
        }

        #endregion

        #region Constructors and destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCollection"/> class.
        /// </summary>
        public RuleCollection()
        {
            this._innerCollection = new List<Rule>();
        }

        #endregion

        #region Interface Implementations

        /// <summary>
        /// Adds an item to the <see cref="NppPluginNET.Core.Rules.Rule"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="NppPluginNET.Core.Rules.Rule"/>.</param>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="NppPluginNET.Core.Rules.Rule"/> is read-only.
        ///   </exception>
        public void Add(Rule item)
        {
            if (!_innerCollection.Contains(item))
            {
                _innerCollection.Add(item);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        /// <summary>
        /// Removes all items from the <see cref="NppPluginNET.Core.Rules.Rule"/>.
        /// </summary>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="NppPluginNET.Core.Rules.Rule"/> is read-only.
        ///   </exception>
        public void Clear()
        {
            _innerCollection.Clear();
        }

        /// <summary>
        /// Copies to.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(Rule[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="NppPluginNET.Core.Rules.Rule"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="NppPluginNET.Core.Rules.Rule"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see cref="NppPluginNET.Core.Rules.Rule"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="NppPluginNET.Core.Rules.Rule"/>.
        /// </returns>
        /// <exception cref="T:System.NotSupportedException">
        /// The <see cref="NppPluginNET.Core.Rules.Rule"/> is read-only.
        ///   </exception>
        public bool Remove(Rule item)
        {
            return _innerCollection.Remove(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<Rule> IEnumerable<Rule>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
