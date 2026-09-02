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
    {
        var actual = CharacterReplacement(s, k);
        Assert.Equal(expected, actual);
    }

    private readonly record struct ReplacementContext(string S, int K, HashMap<char, int> Counts);

    private readonly record struct WindowState(int Start, int MostFrequentCount, int Longest);

    private static int CharacterReplacement(string s, int k)
    {
        var context = new ReplacementContext(s, k, new HashMap<char, int>());
        var state = new WindowState(0, 0, 0);

        for (var windowEnd = 0; windowEnd < s.Length; windowEnd++)
        {
            state = AdvanceWindow(context, windowEnd, state);
        }

        return state.Longest;
    }

    private static WindowState AdvanceWindow(ReplacementContext context, int windowEnd, WindowState state)
    {
        var incoming = context.S[windowEnd];
        context.Counts.TryGetValue(incoming, out var incomingCount);
        context.Counts.Set(incoming, incomingCount + 1);
        var mostFrequentCount = Math.Max(state.MostFrequentCount, incomingCount + 1);

        var windowStart = state.Start;
        if (windowEnd - windowStart + 1 - mostFrequentCount > context.K)
        {
            var outgoing = context.S[windowStart];
            context.Counts.TryGetValue(outgoing, out var outgoingCount);
            context.Counts.Set(outgoing, outgoingCount - 1);
            windowStart++;
        }

        var longest = Math.Max(state.Longest, windowEnd - windowStart + 1);

        return new WindowState(windowStart, mostFrequentCount, longest);
    }
}
