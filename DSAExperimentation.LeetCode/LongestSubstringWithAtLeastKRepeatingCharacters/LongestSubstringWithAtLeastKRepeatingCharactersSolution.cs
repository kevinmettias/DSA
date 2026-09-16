using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.LongestSubstringWithAtLeastKRepeatingCharacters;

// LeetCode 395. Longest Substring with At Least K Repeating Characters: the
// length of the longest substring of `text` in which every distinct character
// occurs at least `minimumRepeats` times.
//
// LongestByBruteForce is the textbook O(n^2) scan over every substring,
// tracking with a plain BCL Dictionary how many distinct characters in the
// growing window are still below minimumRepeats. LongestByDivideAndConquer is
// the expected-case-linear alternative: any character whose total count across
// the current range falls below minimumRepeats can never appear in any valid
// answer inside that range, so it safely splits the range at every occurrence of
// that character and recurses on both halves; a range with no such
// character is itself a valid answer. It counts with this repo's own
// HashMap<char,int> - the same frequency-count primitive
// MinimumWindowSubstringSolution uses for its own sliding window.
internal static class LongestSubstringWithAtLeastKRepeatingCharactersSolution
{
    public static int LongestByBruteForce(string text, int minimumRepeats)
    {
        var best = 0;

        for (var start = 0; start < text.Length; start++)
        {
            var counts = new Dictionary<char, int>();
            var belowK = 0;

            for (var end = start; end < text.Length; end++)
            {
                var ch = text[end];
                belowK = AdvanceRunCount(ch, belowK, counts, minimumRepeats);

                if (belowK == 0)
                {
                    best = Math.Max(best, end - start + 1);
                }
            }
        }

        return best;
    }

    private static int AdvanceRunCount(
        char ch, int belowK, Dictionary<char, int> counts, int minimumRepeats)
    {
        var newCount = counts.GetValueOrDefault(ch) + 1;
        counts[ch] = newCount;

        if (newCount == 1)
        {
            belowK++;
        }
        else if (newCount == minimumRepeats)
        {
            belowK--;
        }

        return belowK;
    }

    public static int LongestByDivideAndConquer(string text, int minimumRepeats) =>
        LongestInRange(text, 0, text.Length, minimumRepeats);

    private static int LongestInRange(string text, int start, int end, int minimumRepeats)
    {
        if (end - start < minimumRepeats)
        {
            return 0;
        }

        var counts = BuildFrequencyCounts(text, start, end);
        var splitIndex = FindSplitIndex(text, (Start: start, End: end), minimumRepeats, counts);

        if (splitIndex is not { } index)
        {
            return end - start;
        }

        return BestOfHalves(text, index, (Start: start, End: end), minimumRepeats);
    }

    private static HashMap<char, int> BuildFrequencyCounts(string text, int start, int end)
    {
        var counts = new HashMap<char, int>();

        for (var i = start; i < end; i++)
        {
            counts.TryGetValue(text[i], out var count);
            counts.Set(text[i], count + 1);
        }

        return counts;
    }

    // The half-open range being split is one range: neither endpoint is ever passed
    // without the other, and the counts were built for that same range.
    private static int? FindSplitIndex(
        string text, (int Start, int End) range, int minimumRepeats, HashMap<char, int> counts)
    {
        for (var i = range.Start; i < range.End; i++)
        {
            counts.TryGetValue(text[i], out var count);

            if (count < minimumRepeats)
            {
                return i;
            }
        }

        return null;
    }

    // The two halves either side of the splitting character: it can never appear in
    // a valid answer inside the range, so the answer is the larger of the two.
    private static int BestOfHalves(
        string text, int splitIndex, (int Start, int End) range, int minimumRepeats)
    {
        var left = LongestInRange(text, range.Start, splitIndex, minimumRepeats);
        var right = LongestInRange(text, splitIndex + 1, range.End, minimumRepeats);
        return Math.Max(left, right);
    }
}
