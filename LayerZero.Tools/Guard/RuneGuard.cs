using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LayerZero.Tools.Guard
{
    /// <summary>
    /// General/core validations
    /// </summary>
    public static class RuneGuard
    {
        public static T NotNull<T>(this T? obj,
                                   [CallerArgumentExpression("source")] string sourceExpression = "")
        {
            if (obj is null)
                throw new ArgumentNullException(sourceExpression);

            return obj!;
        }

        public static T[] NotEmpty<T>(this T[] array,
                                   [CallerArgumentExpression("source")] string sourceExpression = "")
        {
            if (array.Length == 0)
                throw new ArgumentException($"{sourceExpression} cannot be empty.");

            return array;
        }

        public static IEnumerable<T> HasIndex<T>(this IEnumerable<T> source, int index,
                                   [CallerArgumentExpression("index")] string indexExpression = "")
        {
            var count = source.Count();
            if (index < 0 || index >= count)
                throw new IndexOutOfRangeException($"{indexExpression} refers to an index out of bounds (Count: {count}).");

            return source;
        }
    }
}
