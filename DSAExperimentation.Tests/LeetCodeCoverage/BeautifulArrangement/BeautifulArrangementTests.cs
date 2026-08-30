using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BeautifulArrangement;

// LeetCode 526. Beautiful Arrangement: count permutations of [1..n] where every
// 1-indexed position i holds a value v with v % i == 0 or i % v == 0 - this repo's
// own Backtrack.Search engine (the same Permutations/PermutationSequence
// composition), with the divisibility rule folded into Candidates so an illegal
// value is never placed in the first place, rather than generating every full
// permutation and rejecting it afterward.
public sealed partial class BeautifulArrangementTests
{
    [Fact]
    public void CountArrangements_TwoNumbers_ReturnsTwo() => Assert.Equal(2, CountArrangements(2));

    [Fact]
    public void CountArrangements_SingleNumber_ReturnsOne() => Assert.Equal(1, CountArrangements(1));

    [Fact]
    public void CountArrangements_ThreeNumbers_ReturnsThree() => Assert.Equal(3, CountArrangements(3));

    [Fact]
    public void CountArrangements_FourNumbers_ReturnsKnownCount() => Assert.Equal(8, CountArrangements(4));

    private static int CountArrangements(int n)
    {
        var count = 0;
        var state = new State(n);

        Backtrack.Search<State, int>(
            state,
            s => s.Values.Count == n,
            s =>
            {
                if (s.Values.Count == n)
                {
                    return [];
                }

                var position = s.Values.Count + 1;
                return Enumerable.Range(1, n).Where(v => !s.Used[v - 1] && (v % position == 0 || position % v == 0));
            },
            (s, v) => { s.Used[v - 1] = true; s.Values.Add(v); },
            (s, v) => { s.Used[v - 1] = false; s.Values.RemoveAt(s.Values.Count - 1); },
            _ => count++);

        return count;
    }

    private sealed class State(int length) { public bool[] Used { get; } = new bool[length]; public List<int> Values { get; } = []; }
}
