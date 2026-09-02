using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.FirstUniqueCharacterInAString;

// LeetCode 387. First Unique Character in a String: the index of the first
// character that occurs exactly once, or -1 if every character repeats.
//
// The composed strategy is a HashMap<char,int> frequency count pass followed by a
// second pass returning the first index whose count is 1 - the same
// character-bookkeeping shape LongestSubstringWithoutRepeatingCharacters already
// uses this repo's own HashMap<TKey,TValue> for.
internal static class FirstUniqueCharacterInAStringSolution
{
    // The textbook answer: for every index, rescan the whole string counting
    // occurrences, with no early break on the first duplicate found - the O(n^2)
    // arm the HashMap pass below has to justify itself against. Deliberately
    // written without this repo's primitives.
    public static int FirstUniqCharByBruteForce(string s)
    {
        for (var i = 0; i < s.Length; i++)
        {
            var occurrences = 0;

            for (var j = 0; j < s.Length; j++)
            {
                if (s[j] == s[i])
                {
                    occurrences++;
                }
            }

            if (occurrences == 1)
            {
                return i;
            }
        }

        return LeetCodeAnswer.None;
    }

    public static int FirstUniqCharByHashMapTwoPass(string s)
    {
        var counts = new HashMap<char, int>();

        foreach (var c in s)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        for (var i = 0; i < s.Length; i++)
        {
            counts.TryGetValue(s[i], out var count);

            if (count == 1)
            {
                return i;
            }
        }

        return LeetCodeAnswer.None;
    }
}
