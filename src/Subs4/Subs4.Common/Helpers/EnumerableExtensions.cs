using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Subs4.Common.Helpers
{
    public static class EnumerableExtensions
    {
        [Pure]
        public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T value)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return AppendIterator(source, value);
        }

        private static IEnumerable<T> AppendIterator<T>(IEnumerable<T> source, T value)
        {
            foreach (var item in source)
                yield return item;

            yield return value;
        }
    }
}
