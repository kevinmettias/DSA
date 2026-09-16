using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.ValidAnagram;

// LeetCode 242. Valid Anagram: decide whether target is a rearrangement of source.
//
// The two strategies differ in how they check every character of source is matched by
// one in target: an O(n) frequency-count pass through this repo's own
// HashMap<char,int>, or the O(n^2) textbook approach of scanning target for an
// unmatched occurrence of each character of source.
internal static class ValidAnagramSolution
{
    // The textbook answer: for every character of source, linearly scan target for an
    // unmatched occurrence. Deliberately written without this repo's primitives -
    // it is the arm the frequency-count strategy has to justify itself against.
    public static bool IsAnagramByBruteForce(string source, string target)
    {
        if (source.Length != target.Length)
        {
            return false;
        }

        var matched = new bool[target.Length];

        foreach (var c in source)
        {
            if (!TryMatchCharacter(target, c, matched))
            {
                return false;
            }
        }

        return true;
    }

    // Claims the first not-yet-matched occurrence of `character` in target, if there is one.
    private static bool TryMatchCharacter(string target, char character, bool[] matched)
    {
        for (var j = 0; j < target.Length; j++)
        {
            if (!matched[j] && target[j] == character)
            {
                matched[j] = true;
                return true;
            }
        }

        return false;
    }

    // One O(n) pass: increment per character of source, decrement per character of target,
    // and reject as soon as a character of target has nothing left to consume.
    public static bool IsAnagramByHashMapFrequencyCount(string source, string target)
    {
        if (source.Length != target.Length)
        {
            return false;
        }

        var counts = BuildCharacterCounts(source);

        return TryConsumeAllCounts(target, counts);
    }

    private static HashMap<char, int> BuildCharacterCounts(string source)
    {
        var counts = new HashMap<char, int>();

        foreach (var c in source)
        {
            counts.TryGetValue(c, out var count);
            counts.Set(c, count + 1);
        }

        return counts;
    }

    // Attempts to consume one occurrence of every character of target from the counts,
    // reporting whether each one had an occurrence left to consume.
    private static bool TryConsumeAllCounts(string target, HashMap<char, int> counts)
    {
        foreach (var c in target)
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
