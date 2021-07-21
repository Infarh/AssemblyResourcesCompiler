using System.Collections.Generic;
using System.Linq;

namespace AssemblyResourcesCompiler.Extensions
{
    internal static class EnumExt
    {
        public static IEnumerable<T> Append<T>(this IEnumerable<T> items, IEnumerable<T> other) => items is null ? other : items.Concat(other);

        public static IEnumerable<T> Append<T>(this IEnumerable<T> items, T other)
        {
            if(items is { })
                foreach (var item in items)
                    yield return item;
            yield return other;
        }
    }
}
