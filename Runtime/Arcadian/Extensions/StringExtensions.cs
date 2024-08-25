namespace Arcadian.Extensions
{
    public static class StringExtensions
    {
        public static string ToPlural(this string word)
        {
            if (string.IsNullOrWhiteSpace(word))
                return word;

            // Rules for regular nouns
            if (word.EndsWith("s") || word.EndsWith("ss") || word.EndsWith("sh") || word.EndsWith("ch") || word.EndsWith("x") || word.EndsWith("o"))
                return word + "es";
            
            if (word.EndsWith("f"))
                return word[..^1] + "ves";
            
            if (word.EndsWith("fe"))
                return word[..^2] + "ves";
            
            if (word.EndsWith("y") && !IsVowel(word[^2]))
                return word[..^1] + "ies";
            
            return word + "s";
        }

        private static bool IsVowel(char c)
        {
            return "aeiou".IndexOf(char.ToLower(c)) != -1;
        }
    }
}