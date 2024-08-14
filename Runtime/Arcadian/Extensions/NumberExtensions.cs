using System;
using System.Collections.Generic;

namespace Arcadian.Extensions
{
    public static class NumberExtensions
    {
        public static string ToRoman(this int num)
        {
            if (num is < 1 or > 3999)
            {
                throw new ArgumentOutOfRangeException(nameof(num), "Input should be between 1 and 3999.");
            }

            var romanDict = new Dictionary<int, string>
            {
                {1000, "M"}, {900, "CM"}, {500, "D"}, {400, "CD"},
                {100, "C"}, {90, "XC"}, {50, "L"}, {40, "XL"},
                {10, "X"}, {9, "IX"}, {5, "V"}, {4, "IV"}, {1, "I"}
            };

            var roman = "";
            foreach (var kvp in romanDict)
            {
                while (num >= kvp.Key)
                {
                    roman += kvp.Value;
                    num -= kvp.Key;
                }
            }

            return roman;
        }
    }
}