using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Tests.LeetCodeCoverage.PaintingAGridWithThreeDifferentColors;

// LeetCode 1931. Painting a Grid With Three Different Colors: m <= 5 keeps every
// column's own valid colorings (adjacent rows differ) to at most 3*2^(m-1) <= 48
// possibilities, enumerated once with this repo's own Backtrack.Search (choose one
// of 3 colors per row, skipping a choice equal to the row above - Candidates
// returning empty once a column is full is what stops the search there, the same
// "pruning lives in Candidates" contract Backtrack.cs's own doc comment states).
// The grid-wide count is then a column-by-column DP - state (column, previous
// column's pattern index), transition sums every next-column pattern with no row
// matching the previous column's same row - solved with this repo's own
// Memoizer.Memoize instead of a hand-rolled cache.
public sealed partial class PaintingAGridWithThreeDifferentColorsTests
{
    private const int Mod = 1_000_000_007;

    // Hand-counted, not LeetCode's own published examples: m=1 has no vertical
    // adjacency at all (n=1 -> 3 free colors, n=2 -> 3*2=6 since only the horizontal
    // neighbor constrains it); m=2,n=2 was counted by hand below to cross-check the
    // column-compatibility transition itself, not just the column-pattern count.
    // For pattern1=(a,b) with a!=b, a compatible pattern2=(c,d) needs c!=a, d!=b,
    // c!=d - working that out for every one of the 6 length-2 patterns gives exactly
    // 3 compatible successors each (color-symmetric), so 6*3=18 total colorings.
    [Theory]
    [InlineData(1, 1, 3)]
    [InlineData(2, 1, 6)]
    [InlineData(2, 2, 18)]
    public void ColorTheGrid_HandCountedSmallGrids_ReturnsExpectedTotal(int m, int n, int expected)
    {
        var actual = ColorTheGrid(m, n);
        Assert.Equal(expected, actual);
    }

    private static int ColorTheGrid(int m, int n)
    {
        var patterns = GenerateColumnPatterns(m);
        var total = Memoizer.Memoize<(int Column, int PreviousPattern), long>(
            (0, -1), (state, recurse) => ComputeColumnTotal(state, recurse, patterns, n));

        return (int)total;
    }

    private static long ComputeColumnTotal(
        (int Column, int PreviousPattern) state,
        Func<(int Column, int PreviousPattern), long> recurse,
        List<int[]> patterns,
        int n)
    {
        if (state.Column == n)
        {
            return 1L;
        }

        var total = 0L;

        for (var i = 0; i < patterns.Count; i++)
        {
            if (state.PreviousPattern == -1 || IsCompatible(patterns[state.PreviousPattern], patterns[i]))
            {
                total = (total + recurse((state.Column + 1, i))) % Mod;
            }
        }

        return total;
    }

    private static List<int[]> GenerateColumnPatterns(int m)
    {
        var patterns = new List<int[]>();
        var current = new List<int>();

        Backtrack.Search<List<int>, int>(
            current,
            state => state.Count == m,
            state => Candidates(state, m),
            (state, color) => state.Add(color),
            (state, _) => state.RemoveAt(state.Count - 1),
            state => patterns.Add(state.ToArray()));

        return patterns;
    }

    private static IEnumerable<int> Candidates(List<int> state, int m)
    {
        if (state.Count == m)
        {
            yield break;
        }

        for (var color = 0; color < 3; color++)
        {
            if (state.Count == 0 || state[^1] != color)
            {
                yield return color;
            }
        }
    }

    private static bool IsCompatible(int[] a, int[] b)
    {
        for (var i = 0; i < a.Length; i++)
        {
            if (a[i] == b[i])
            {
                return false;
            }
        }

        return true;
    }
}
