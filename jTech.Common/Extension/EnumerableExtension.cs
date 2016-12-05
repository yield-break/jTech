using System;
using System.Collections.Generic;
using System.Linq;

namespace jTech.Common.Extension
{
    public static class EnumerableExtension
    {
        public static IEnumerable<T> TakeExactly<T>(this IEnumerable<T> source, int amount)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (amount < 0)
            {
                throw new ArgumentOutOfRangeException("amount");
            }

            int taken = 0;
            foreach (var item in source.Take(amount))
            {
                taken++;
                yield return item;
            }

            for (int i = 0; i < amount - taken; i++)
            {
                yield return default(T);
            }
        }

    }
}
