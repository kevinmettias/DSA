using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.LongestSubstringWithAtLeastKRepeatingCharacters;

// LeetCode 395. Longest Substring with At Least K Repeating Characters: the
// length of the longest substring of `s` in which every distinct character
// occurs at least `k` times.
//
// LongestByBruteForce is the textbook O(n^2) scan over every substring,
// tracking with a plain BCL Dictionary how many distinct characters in the
// growing window are still below k. LongestByDivideAndConquer is the
// expected-case-linear alternative: any character whose total count across
// the current range falls below k can never appear in any valid answer
// inside that range, so it safely splits the range at every occurrence of
// that character and recurses on both halves; a range with no such
// character is itself a valid answer. It counts with this repo's own
// HashMap<char,int> - the same frequency-count primitive
// MinimumWindowSubstringSolution uses for its own sliding window.
internal static class LongestSubstringWithAtLeastKRepeatingCharactersSolution
{
    public static int LongestByBruteForce(string s, int k)
    {
        var best = 0;

        for (var start = 0; start < s.Length; start++)
        {
            var counts = new Dictionary<char, int>();
            var belowK = 0;

            for (var end = start; end < s.Length; end++)
            {
                var ch = s[end];
                belowK = AdvanceRunCount(ch, belowK, counts, k);

                if (belowK == 0)
                {
                    best = Math.Max(best, end - start + 1);
                }
            }
        }

        return best;
    }

    private static int AdvanceRunCount(char ch, int belowK, Dictionary<char, int> counts, int k)
    {
        var newCount = counts.GetValueOrDefault(ch) + 1;
        counts[ch] = newCount;

        if (newCount == 1)
        {
            belowK++;
        }
        else if (newCount == k)
        {
            belowK--;
        }

        return belowK;
    }

    public static int LongestByDivideAndConquer(string s, int k) =>
        LongestInRange(s, 0, s.Length, k);

    private static int LongestInRange(string s, int start, int end, int k)
    {
        if (end - start < k)
        {
            return 0;
        }

        var counts = BuildFrequencyCounts(s, start, end);
        var splitIndex = FindSplitIndex(s, start, end, k, counts);

        if (splitIndex is not { } index)
        {
            return end - start;
        }

        var left = LongestInRange(s, start, index, k);
        var right = LongestInRange(s, index + 1, end, k);
        return Math.Max(left, right);
    }

    private static HashMap<char, int> BuildFrequencyCounts(string s, int start, int end)
    {
        var counts = new HashMap<char, int>();

        for (var i = start; i < end; i++)
        {
            counts.TryGetValue(s[i], out var count);
            counts.Set(s[i], count + 1);
        }

        return counts;
    }

    private static int? FindSplitIndex(string s, int start, int end, int k, HashMap<char, int> counts)
    {
        for (var i = start; i < end; i++)
        {
            counts.TryGetValue(s[i], out var count);

            if (count < k)
            {
                return i;
            }
        }

        return null;
    }
}
