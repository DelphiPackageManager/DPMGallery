using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DPMGallery.Extensions
{
    public static class IEnumerableExtensions
    {
        public static bool AnySafe<T>(this IEnumerable<T> items)
        {
            if (items == null)
            {
                return false;
            }
            return items.Any();
        }

        public static bool AnySafe<T>(this IEnumerable<T> items, Func<T, bool> predicate)
        {
            if (items == null)
            {
                return false;
            }
            return items.Any(predicate);
        }

    }
}
