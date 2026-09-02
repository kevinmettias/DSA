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
        var bounds = new GoodStringBounds(n, s1, s2, evil, failure);

        var result = Memoizer.Memoize<(int Position, int Matched, bool TightLow, bool TightHigh), long>(
            (0, 0, true, true), (state, count) => CountFrom(bounds, state, count));

        return (int)result;
    }

    private static long CountFrom(
        GoodStringBounds bounds,
        (int Position, int Matched, bool TightLow, bool TightHigh) state,
        Func<(int Position, int Matched, bool TightLow, bool TightHigh), long> count)
    {
        if (state.Matched == bounds.Evil.Length)
        {
            return 0;
        }

        if (state.Position == bounds.N)
        {
            return 1;
        }

        var low = state.TightLow ? bounds.S1[state.Position] : 'a';
        var high = state.TightHigh ? bounds.S2[state.Position] : 'z';
        var walk = new GoodStringWalk(bounds.Evil, bounds.Failure, state, low, high, count);

        return SumTransitions(walk, low, high);
    }

    private static long SumTransitions(GoodStringWalk walk, char low, char high)
    {
        var total = 0L;

        for (var c = low; c <= high; c++)
        {
            var contribution = ComputeTransitionContribution(walk, c);

            if (contribution is not null)
            {
                total = (total + contribution.Value) % Modulus;
            }
        }

        return total;
    }

    private readonly record struct GoodStringBounds(int N, string S1, string S2, string Evil, int[] Failure);

    private static long? ComputeTransitionContribution(GoodStringWalk walk, char c)
    {
        var matched = AdvanceAutomaton(walk.Evil, walk.Failure, walk.State.Matched, c);
        if (matched == walk.Evil.Length)
        {
            return null;
        }

        var next = (walk.State.Position + 1, matched, walk.State.TightLow && c == walk.Low, walk.State.TightHigh && c == walk.High);
        return walk.Count(next);
    }

    private readonly record struct GoodStringWalk(
        string Evil,
        int[] Failure,
        (int Position, int Matched, bool TightLow, bool TightHigh) State,
        char Low,
        char High,
        Func<(int Position, int Matched, bool TightLow, bool TightHigh), long> Count);

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
