using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jTech.Common.Collection
{
    public class TimeSeries<TValue> : Dictionary<DateTime, TValue>
    {
        public TimeSeries() : base() { }
        public TimeSeries(IDictionary<DateTime, TValue> dictionary) : base(dictionary) { }

    }
}
