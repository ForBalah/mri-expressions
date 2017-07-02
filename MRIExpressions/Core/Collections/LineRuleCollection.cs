#region Directives

using System;
using System.Collections.Generic;
using System.Linq;
using NppPluginNET.Core.Rules; 

#endregion

namespace NppPluginNET.Core.Collections
{
    public class LineRuleCollection : ICollection<LineRule>
    {
        #region Fields

        private IList<LineRule> _innerCollection;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the <see cref="NppPluginNET.Core.Rules.Rule"/> at the specified index.
        /// </summary>
        public LineRule this[int index]
        {
            get { return _innerCollection[index]; }
            set { _innerCollection[index] = value; }
        }

        /// <summary>
        /// Gets or sets the <see cref="NppPluginNET.Core.Rules.Rule"/> with the specified keyword.
        /// </summary>
        public LineRule this[string rulename]
        {
            get
            {
                return _innerCollection.FirstOrDefault(rule => rule.Name == rulename);
            }
            set
            {
                LineRule rule = _innerCollection.First(r => r.Name == rulename);
                _innerCollection[_innerCollection.IndexOf(rule)] = value;
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <returns>
        /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        ///   </returns>
        public int Count
        {
            get { return _innerCollection.Count; }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        /// <returns>true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
        ///   </returns>
        public bool IsReadOnly
        {
            get { return true; }
        }

        #endregion

        #region Constructors and destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RuleCollection"/> class.
        /// </summary>
        public LineRuleCollection()
        {
            this._innerCollection = new List<LineRule>();
        }

        #endregion

        #region ICollection<LineRule> Members

        /// <summary>
        /// Adds the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        public void Add(LineRule item)
        {
            _innerCollection.Add(item);
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            _innerCollection.Clear();
        }

        /// <summary>
        /// Determines whether [contains] [the specified item].
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>
        /// 	<c>true</c> if [contains] [the specified item]; otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(LineRule item)
        {
            return _innerCollection.Contains(item);
        }

        public void CopyTo(LineRule[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the specified item.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns></returns>
        public bool Remove(LineRule item)
        {
            return _innerCollection.Remove(item);
        }

        #endregion

        #region IEnumerable<LineRule> Members

        /// <summary>
        /// Gets the enumerator.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<LineRule> GetEnumerator()
        {
            return _innerCollection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
