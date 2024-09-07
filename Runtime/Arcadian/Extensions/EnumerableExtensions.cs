using System;
using System.Collections.Generic;
using System.Linq;

namespace Arcadian.Extensions
{
    /// <summary>
    /// Contains extensions for Enumerables.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Iterates through an Enumerable.
        /// </summary>
        /// <param name="items">Enumerable</param>
        /// <param name="action">Function to run for each item</param>
        /// <typeparam name="T">Type of item to iterate across.</typeparam>
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static T Random<T>(this T[] choices)
        {
            return Random(choices, _ => 1);
        }

        public static T Random<T>(this T[] items, Func<T, float> weight)
        {
            var totalWeight = items.Sum(weight);
            var randomNumber = UnityEngine.Random.Range(0f, totalWeight);

            foreach (var item in items)
            {
                if (randomNumber < weight(item))
                {
                    return item;
                }

                randomNumber -= weight(item);
            }

            return items.Last();
        }
    }
}