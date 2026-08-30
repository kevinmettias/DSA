using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestRepeatingCharacterReplacement;

// LeetCode 424. Longest Repeating Character Replacement: a single sliding-window
// pass tracking each character's count within the window in this repo's own
// HashMap<char,int> - the same primitive/precedent LongestSubstringWithoutRepeating
// CharactersTests and MinimumWindowSubstringTests already use for their own sliding
// windows, just tracking "count of the window's most frequent character" instead of
// "last seen"/"still needed". mostFrequentCount is a running historical max, never
// decreased when the window shrinks - the window's LENGTH can only ever grow or
// stay the same afterward, which is all Longest needs to stay correct.
public sealed partial class LongestRepeatingCharacterReplacementTests
{
    [Theory]
    [InlineData("ABAB", 2, 4)]
    [InlineData("AABABBA", 1, 4)]
    [InlineData("AAAA", 2, 4)]
    [InlineData("", 2, 0)]
    public void CharacterReplacement_SlidingWindowHashMap_ReturnsLongestAchievableRun(
        string s, int k, int expected)
        => Assert.Equal(expected, CharacterReplacement(s, k));

    private static int CharacterReplacement(string s, int k)
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
