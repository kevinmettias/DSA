using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Algorithms.StringMatching;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindAllGoodStrings;

// LeetCode 1397. Find All Good Strings: KMP's own prefix/failure function
// (PrefixFunctionSearch.ComputeFailureFunction over evil) turns "how much of evil
// does the string built so far already match" into a small automaton state - the
// classic KMP fallback walk, rebuilt here from that public failure array since
// PrefixFunctionSearch keeps the walk itself private. Wiring that automaton
// through Memoizer's memoized recursion over (position, automaton state,
// isTightToS1, isTightToS2) is then a standard digit DP: it counts every string in
// [s1, s2] whose automaton state never reaches evil.Length, without ever
// materializing a candidate string.
public sealed partial class FindAllGoodStringsTests
{
    private const int Modulus = 1_000_000_007;

    [Fact]
    public void CountGoodStrings_ClassicExample_ReturnsCountExcludingEvilSubstring()
    {
        var count = CountGoodStrings(n: 2, s1: "aa", s2: "da", evil: "b");

        Assert.Equal(51, count);
    }

    [Fact]
    public void CountGoodStrings_EveryCandidateStartsWithEvil_ReturnsZero()
    {
        var count = CountGoodStrings(n: 8, s1: "leetcode", s2: "leetgoes", evil: "leet");

        Assert.Equal(0, count);
    }

    [Fact]
    public void CountGoodStrings_NarrowRange_ExcludesOnlyTheEvilCandidate()
    {
        var count = CountGoodStrings(n: 2, s1: "gx", s2: "gz", evil: "x");

        Assert.Equal(2, count);
    }

    private static int CountGoodStrings(int n, string s1, string s2, string evil)
    {
        var failure = PrefixFunctionSearch.ComputeFailureFunction(evil);

        long CountFrom(
            (int Position, int Matched, bool TightLow, bool TightHigh) state,
            Func<(int Position, int Matched, bool TightLow, bool TightHigh), long> count)
        {
            if (state.Matched == evil.Length)
            {
                return 0;
            }

            if (state.Position == n)
            {
                return 1;
            }

            var low = state.TightLow ? s1[state.Position] : 'a';
            var high = state.TightHigh ? s2[state.Position] : 'z';
            var total = 0L;

            for (var c = low; c <= high; c++)
            {
                var matched = AdvanceAutomaton(evil, failure, state.Matched, c);
                if (matched == evil.Length)
                {
                    continue;
                }

                var next = (state.Position + 1, matched, state.TightLow && c == low, state.TightHigh && c == high);
                total = (total + count(next)) % Modulus;
            }

            return total;
        }

        var result = Memoizer.Memoize<(int Position, int Matched, bool TightLow, bool TightHigh), long>(
            (0, 0, true, true), CountFrom);

        return (int)result;
    }

    // The KMP fallback walk PrefixFunctionSearch.ComputeFailureFunction's own
    // Advance performs internally, rebuilt here from its public failure array -
    // the one piece of automaton-transition logic this problem needs that
    // ComputeFailureFunction doesn't expose as a standalone step.
    private static int AdvanceAutomaton(string evil, int[] failure, int matched, char next)
    {
        while (matched > 0 && evil[matched] != next)
        {
            matched = failure[matched - 1];
        }

        return evil[matched] == next ? matched + 1 : matched;
    }
}
