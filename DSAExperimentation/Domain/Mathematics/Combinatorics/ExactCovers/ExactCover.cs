namespace DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;

// What every solver of an exact cover problem must agree on: a set of rows is a cover when it
// meets each primary column exactly once and each secondary column at most once. A row listed
// twice meets its columns twice, so it is no cover. This is the definition, which a search such as
// Algorithm X finds rows to satisfy; it is checked by counting, once per row's columns.
/// <summary>The exact cover rule: primary columns met once, secondary at most once.</summary>
/// <typeparam name="TProblem">The problem, such as a Sudoku grid's.</typeparam>
public static class ExactCover<TProblem>
    where TProblem : struct, IExactCoverProblem
{
    /// <summary>Whether a set of rows is an exact cover of a problem.</summary>
    /// <param name="problem">The problem.</param>
    /// <param name="rows">The rows chosen, each from 0 below the problem's row count.</param>
    /// <returns>
    /// <see langword="true"/> when the rows meet every primary column once and no secondary column
    /// twice.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">A row is not one of the problem's.</exception>
    public static bool IsCover(TProblem problem, ReadOnlySpan<int> rows)
    {
        var meetings = MeetingsPerColumn(problem, rows);

        for (var column = 0; column < meetings.Length; column++)
        {
            var fewest = column < problem.PrimaryColumnCount ? 1 : 0;

            if (meetings[column] > 1 || meetings[column] < fewest)
            {
                return false;
            }
        }

        return true;
    }

    // How many of the rows meet each column.
    private static int[] MeetingsPerColumn(TProblem problem, ReadOnlySpan<int> rows)
    {
        var meetings = new int[problem.PrimaryColumnCount + problem.SecondaryColumnCount];

        foreach (var row in rows)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(row, nameof(rows));
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(
                row, problem.RowCount, nameof(rows));
            var columns = new int[problem.ColumnCountOf(row)];
            problem.WriteColumns(row, columns);

            foreach (var column in columns)
            {
                meetings[column]++;
            }
        }

        return meetings;
    }
}
