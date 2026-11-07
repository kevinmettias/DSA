namespace DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;

// An exact cover problem is a set of constraints and a family of choices, each choice meeting some
// of the constraints: pick choices so that every constraint is met exactly once. Written as a 0/1
// matrix, the choices are its rows and the constraints its columns, and a cover is a set of rows
// with exactly one 1 in every column. Knuth's generalization adds secondary columns, met at most
// once rather than exactly once, which is what the n-queens puzzle needs of its diagonals: a
// diagonal may hold no queen. Sudoku, the n-queens puzzle and polyomino tilings are each this
// problem; which rows and columns they make is each one's own fact, and the procedures that find a
// cover are Algorithms/ExactCover's. Primary columns are numbered first, from 0, and secondary
// columns after them. Every row meets at least one primary column, as Knuth assumes: a row meeting
// only secondary columns could join any cover leaving them free, so a search has no reason to
// choose it. The problem is an instance struct, so it can carry a puzzle's input, and a
// solver generic over it compiles each problem's rows into its own code. A row lists its columns
// by writing them into a span, so reading one allocates nothing.
/// <summary>An exact cover problem: rows meeting columns, to pick so each is met once.</summary>
public interface IExactCoverProblem
{
    /// <summary>Gets how many columns a cover meets exactly once, numbered from 0.</summary>
    int PrimaryColumnCount { get; }

    /// <summary>Gets how many columns a cover meets at most once, numbered next.</summary>
    int SecondaryColumnCount { get; }

    /// <summary>Gets how many rows there are to choose from.</summary>
    int RowCount { get; }

    /// <summary>Counts the columns a row meets.</summary>
    /// <param name="row">The row, from 0 below <see cref="RowCount"/>.</param>
    /// <returns>How many columns the row meets.</returns>
    int ColumnCountOf(int row);

    /// <summary>Writes the columns a row meets, each once, in any order.</summary>
    /// <param name="row">The row, from 0 below <see cref="RowCount"/>.</param>
    /// <param name="columns">A span of exactly <see cref="ColumnCountOf"/> entries.</param>
    void WriteColumns(int row, Span<int> columns);
}
