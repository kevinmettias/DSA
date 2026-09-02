using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ValidAnagram;

// LeetCode 242. Valid Anagram: decide whether t is a rearrangement of s.
//
// The two strategies differ in how they check every character of s is matched by
// one in t: an O(n) frequency-count pass through this repo's own
// HashMap<char,int>, or the O(n^2) textbook approach of scanning t for an
// unmatched occurrence of each character of s.
internal static class ValidAnagramSolution
{
    // The textbook answer: for every character of s, linearly scan t for an
    // unmatched occurrence. Deliberately written without this repo's primitives -
    // it is the arm the frequency-count strategy has to justify itself against.
    public static bool IsAnagramByBruteForce(string s, string t)
    {
        if (s.Length != t.Length)
        {
            return false;
        }

        var matched = new bool[t.Length];

        foreach (var c in s)
        {
            var found = false;

            for (var j = 0; j < t.Length; j++)
            {
                if (!matched[j] && t[j] == c)
                {
                    matched[j] = true;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return false;
            }
        }

        return true;
    }

    // One O(n) pass: increment per character of s, decrement per character of t,
    // and reject as soon as a character of t has nothing left to consume.
    public static bool IsAnagramByHashMapFrequencyCount(string s, string t)
    {
        if (s.Length != t.Length)
        {
            return false;
        }

        var counts = BuildCharacterCounts(s);

        return ConsumesAllCounts(t, counts);
    }

    private static HashMap<char, int> BuildCharacterCounts(string s)
    {
        var counts = new HashMap<char, int>();

        foreach (var c in s)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        return counts;
    }

    private static bool ConsumesAllCounts(string t, HashMap<char, int> counts)
    {
        foreach (var c in t)
        {
            if (!counts.TryGetValue(c, out var count) || count == 0)
            {
                return false;
            }

            counts.Set(c, count - 1);
        }

        return true;
    }
}
