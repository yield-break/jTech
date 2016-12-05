using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jTech.Common.Caching
{
    public interface IExpiringCachedItem<T> where T : class
    {
        DateTime Expiry { get; set; }
        bool IsExpired { get; }
        T Item { get; set; }
    }
    public class ExpiringCachedItem<T> : IExpiringCachedItem<T> where T : class
    {
        private readonly Func<T, DateTime, bool> _isExpired;
        private T _item;

        public ExpiringCachedItem(T item, Func<T, DateTime, bool> isExpired)
        {
            _item = item;
            _isExpired = isExpired;
        }

        public DateTime Expiry { get; set; }
        public bool IsExpired { get { return _isExpired(_item, Expiry); } }
        public T Item
        {
            get
            {
                if (_item != null && IsExpired) // null check first to avoid unnecessary IsExpireed function checks. 
                {
                    _item = null;
                }
                return _item;
            }
            set
            {
                if (!IsExpired)
                {
                    _item = value;
                }
            }
        }

    }
}
