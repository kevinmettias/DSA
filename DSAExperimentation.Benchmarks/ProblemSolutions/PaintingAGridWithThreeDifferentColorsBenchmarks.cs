using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Painting a Grid With Three Different Colors (LC 1931): the textbook approach -
// enumerate every 3^(rows*columns) full-grid coloring and count the valid ones -
// vs. this repo's own Backtrack.Search (enumerate each column's <=3*2^(rows-1)
// valid patterns once) composed with Memoizer.Memoize (column-by-column DP keyed
// on (column, previous column's pattern)). Rows/Columns are kept small enough for
// the brute force to stay tractable; the DP side alone scales to LeetCode's real
// n<=1000 columns with no change.
[MemoryDiagnoser]
public class PaintingAGridWithThreeDifferentColorsBenchmarks
{
    private const int Rows = 3;
    private const int Mod = 1_000_000_007;
    private const int NumberOfColors = 3;

    [Params(3, 4)]
    public int Columns;

    [Benchmark(Baseline = true)]
    public long BruteForceFullGrid() => CountFromCell(new int[Rows * Columns], 0);

    private long CountFromCell(int[] colors, int index)
    {
        if (index == colors.Length)
        {
            return 1;
        }

        var total = 0L;

        for (var color = 0; color < NumberOfColors; color++)
        {
            total += CountFromCellWithColor(colors, index, color);
        }

        return total;
    }

    private long CountFromCellWithColor(int[] colors, int index, int color)
    {
        var row = index / Columns;
        var col = index % Columns;

        if (row > 0 && colors[index - Columns] == color)
        {
            return 0;
        }

        if (col > 0 && colors[index - 1] == color)
        {
            return 0;
        }

        colors[index] = color;
        return CountFromCell(colors, index + 1);
    }

    [Benchmark]
    public long ColumnPatternDynamicProgramming()
    {
        var patterns = GenerateColumnPatterns(Rows);

        return Memoizer.Memoize<(int Column, int PreviousPattern), long>(
            (0, -1),
            (state, recurse) => Recurrence(state, recurse, patterns));
    }

    private long Recurrence(
        (int Column, int PreviousPattern) state,
        Func<(int Column, int PreviousPattern), long> recurse,
        List<int[]> patterns)
    {
        if (state.Column == Columns)
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

    private static List<int[]> GenerateColumnPatterns(int rows)
    {
        var patterns = new List<int[]>();
        var current = new List<int>();

        Backtrack.Search<List<int>, int>(
            current,
            state => state.Count == rows,
            state => Candidates(state, rows),
            (state, color) => state.Add(color),
            (state, _) => state.RemoveAt(state.Count - 1),
            state => patterns.Add(state.ToArray()));

        return patterns;
    }

    private static IEnumerable<int> Candidates(List<int> state, int rows)
    {
        if (state.Count == rows)
        {
            yield break;
        }

        for (var color = 0; color < NumberOfColors; color++)
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
