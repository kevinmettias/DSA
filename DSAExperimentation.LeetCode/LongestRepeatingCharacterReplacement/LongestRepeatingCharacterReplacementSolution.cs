using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.LongestRepeatingCharacterReplacement;

// LeetCode 424. Longest Repeating Character Replacement: the length of the
// longest substring achievable by replacing at most k characters so the whole
// window shares one letter.
//
// Both strategies are the same sliding window over "count of the window's most
// frequent character" - mostFrequentCount is a running historical max, never
// decreased when the window shrinks, which is all Longest needs to stay correct
// since the window's length can then only ever grow or stay the same. They
// differ only in how per-character counts are tracked.
internal static class LongestRepeatingCharacterReplacementSolution
{
    private const int AlphabetSize = 26;

    // The textbook answer: re-scan from every start index with a plain int[26]
    // count table (LC's alphabet is uppercase A-Z), extending right until more
    // than k characters would need replacing. Deliberately written without this
    // repo's primitives - it is the arm the sliding window below has to justify
    // itself against.
    public static int LongestRunByBruteForce(string s, int k)
    {
        var longest = 0;

        for (var start = 0; start < s.Length; start++)
        {
            var counts = new int[AlphabetSize];
            var mostFrequentCount = 0;

            for (var end = start; end < s.Length; end++)
            {
                var index = s[end] - 'A';
                counts[index]++;
                mostFrequentCount = Math.Max(mostFrequentCount, counts[index]);

                if (end - start + 1 - mostFrequentCount > k)
                {
                    break;
                }

                longest = Math.Max(longest, end - start + 1);
            }
        }

        return longest;
    }

    // A single O(n) pass: the window's left edge only ever advances forward,
    // tracking per-character counts in this repo's own HashMap<char,int> instead
    // of restarting the count from scratch at every start index.
    public static int LongestRunBySlidingWindowHashMap(string s, int k)
    {
        var counts = new HashMap<char, int>();
        var windowStart = 0;
        var mostFrequentCount = 0;
        var longest = 0;

        for (var windowEnd = 0; windowEnd < s.Length; windowEnd++)
        {
            var incoming = s[windowEnd];
            counts.TryGetValue(incoming, out var incomingCount);
            counts.Set(incoming, incomingCount + 1);
            mostFrequentCount = Math.Max(mostFrequentCount, incomingCount + 1);

            if (windowEnd - windowStart + 1 - mostFrequentCount > k)
            {
                var outgoing = s[windowStart];
                counts.TryGetValue(outgoing, out var outgoingCount);
                counts.Set(outgoing, outgoingCount - 1);
                windowStart++;
            }

            longest = Math.Max(longest, windowEnd - windowStart + 1);
        }

        return longest;
    }
}
