using DSAExperimentation.Algorithms.Backtracking;
using DSAExperimentation.Algorithms.DynamicProgramming;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.PaintingAGridWithThreeDifferentColors;

// LeetCode 1931. Painting a Grid With Three Different Colors: count the ways to
// color an m x n grid with three colors so that no two orthogonally adjacent cells
// share a color, modulo 1e9+7. LeetCode's `m` is the row count and `n` the column
// count; both are spelled out here because nothing about the problem makes one of
// them "m".
//
// Both strategies answer the same question - the count, reduced mod 1e9+7 - and
// differ only in what they enumerate: every cell of the whole grid, or every valid
// column pattern once and then the columns.
internal static class PaintingAGridWithThreeDifferentColorsSolution
{
    private const int NumberOfColors = 3;

    // Stands in for "no column has been placed yet", so the first column accepts
    // every pattern instead of having to match a predecessor.
    private const int NoPreviousPattern = -1;

    // The textbook answer: walk the grid cell by cell in row-major order, try all
    // three colors at each cell, and abandon a branch the moment it repeats the
    // color above or to the left. Deliberately plain BCL - an int[] board and
    // recursion - because this is the arm the composed solution below has to
    // justify itself against. Exponential in rows*columns; only the column DP
    // reaches LeetCode's real n <= 1000.
    public static int ColorTheGridByBruteForce(int rows, int columns)
    {
        var total = CountFromCell(new int[rows * columns], 0, columns);

        return (int)(total % ModularArithmetic.Modulo);
    }

    // This repo's own primitives, in two stages. rows <= 5 keeps a single column's
    // own valid colorings (adjacent rows differ) to at most 3*2^(rows-1) <= 48, so
    // Backtrack.Search enumerates them once - Candidates returning empty when the
    // column is full is what stops the search there, the "pruning lives in
    // Candidates" contract Backtrack.cs's own doc comment states. The grid-wide
    // count is then a column-by-column recurrence keyed on (column, previous
    // column's pattern), handed to Memoizer.Memoize instead of a hand-rolled cache.
    public static int ColorTheGridByColumnPatternDynamicProgramming(int rows, int columns)
    {
        var patterns = GenerateColumnPatterns(rows);

        var total = Memoizer.Memoize<(int Column, int PreviousPattern), long>(
            (0, NoPreviousPattern),
            new ColumnTotals(patterns, columns));

        return (int)total;
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
            state => patterns.Add([.. state]));

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

    // The recurrence itself, named: one column past the last is a finished grid, and
    // otherwise every pattern compatible with the column to its left contributes the
    // completions that follow it.
    private sealed class ColumnTotals(List<int[]> patterns, int columns)
        : IRecurrence<(int Column, int PreviousPattern), long>
    {
        public long Replay(
            (int Column, int PreviousPattern) state, IRecurrence<(int Column, int PreviousPattern), long> rest)
        {
            if (state.Column == columns)
            {
                return 1L;
            }

            var total = 0L;

            for (var i = 0; i < patterns.Count; i++)
            {
                if (state.PreviousPattern == NoPreviousPattern ||
                    IsCompatible(patterns[state.PreviousPattern], patterns[i]))
                {
                    total = (total + rest.Replay((state.Column + 1, i), rest)) % ModularArithmetic.Modulo;
                }
            }

            return total;
        }
    }

    // Two adjacent columns are compatible when no row holds the same color twice.
    private static bool IsCompatible(int[] left, int[] right)
    {
        for (var i = 0; i < left.Length; i++)
        {
            if (left[i] == right[i])
            {
                return false;
            }
        }

        return true;
    }

    private static long CountFromCell(int[] colors, int index, int columns)
    {
        if (index == colors.Length)
        {
            return 1L;
        }

        var total = 0L;

        for (var color = 0; color < NumberOfColors; color++)
        {
            total += CountFromCellWithColor(colors, index, columns, color);
        }

        return total;
    }

    private static long CountFromCellWithColor(int[] colors, int index, int columns, int color)
    {
        if (index >= columns && colors[index - columns] == color)
        {
            return 0L;
        }

        if (index % columns > 0 && colors[index - 1] == color)
        {
            return 0L;
        }

        colors[index] = color;

        return CountFromCell(colors, index + 1, columns);
    }
}
