using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;

namespace DSAExperimentation.Tests.Domain.Mathematics.Combinatorics.ExactCovers.Fixtures;

// An exact cover problem written out as its rows, each the list of columns it meets, with the
// primary columns first and the rest secondary: the form a paper or a test states a small matrix
// in.
internal readonly struct MatrixProblem(int primaryColumns, int secondaryColumns, int[][] rows)
    : IExactCoverProblem
{
    public int PrimaryColumnCount => primaryColumns;

    public int SecondaryColumnCount => secondaryColumns;

    public int RowCount => rows.Length;

    public int ColumnCountOf(int row) => rows[row].Length;

    public void WriteColumns(int row, Span<int> columns) => rows[row].CopyTo(columns);
}
