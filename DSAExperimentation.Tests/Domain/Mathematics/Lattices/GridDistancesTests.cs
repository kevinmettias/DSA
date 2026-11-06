using System.Collections.Immutable;
using DSAExperimentation.Domain.Mathematics.Lattices;

namespace DSAExperimentation.Tests.Domain.Mathematics.Lattices;

// Each closed form is held to the thing it claims to count: a breadth-first search over the table's
// steps on a bounded board, from every cell to every cell. A board's edges cannot lengthen an
// orthogonal or octilinear path, so those agree everywhere, even on a board one cell wide. They do
// lengthen an L-shaped path near a corner, and on boards of 5 by 5 cells and up the only pairs
// that differ are a corner and the cell diagonally inside it, 4 jumps apart rather than 2. From the
// middle of a board large enough that no edge is in the way, every cell agrees.
public sealed partial class GridDistancesTests
{
    // The search's mark for a cell it has not reached.
    private const int Unreached = -1;

    // The L-shaped distance from a corner to the cell diagonally inside it: on a board, unbounded.
    private const int CornerToDiagonalOnABoard = 4;
    private const int CornerToDiagonalUnbounded = 2;

    // A board whose middle is farther than any displacement checked from every edge.
    private const int LargeBoardSide = 41;

    public static TheoryData<int, int> Boards => new()
    {
        { 1, 1 },
        { 1, 6 },
        { 4, 7 },
        { 6, 6 },
    };

    public static TheoryData<int, int> BoardsOfAtLeastFiveByFive => new()
    {
        { 5, 5 },
        { 5, 8 },
        { 8, 8 },
    };

    public static TheoryData<int, int, int> LShapedExamples => new()
    {
        { 0, 0, 0 },
        { 1, 2, 1 },
        { 1, 1, 2 },
        { 1, 0, 3 },
        { 2, 2, 4 },
        { 3, 0, 3 },
        { 3, 3, 2 },
        { 4, 4, 4 },
        { -7, 2, 5 },
        { 0, -10, 6 },
    };

    [Theory]
    [MemberData(nameof(Boards))]
    public void Orthogonal_EveryPairOfCellsOnABoard_IsTheBreadthFirstCount(int rows, int cols) =>
        Assert.Empty(
            Disagreements(rows, cols, GridDirections.Orthogonal, GridDistances.Orthogonal));

    [Theory]
    [MemberData(nameof(Boards))]
    public void Octilinear_EveryPairOfCellsOnABoard_IsTheBreadthFirstCount(int rows, int cols) =>
        Assert.Empty(
            Disagreements(rows, cols, GridDirections.Octilinear, GridDistances.Octilinear));

    [Theory]
    [MemberData(nameof(LShapedExamples))]
    public void LShaped_Displacement_IsTheFewestJumps(int deltaRow, int deltaCol, int expected) =>
        Assert.Equal(expected, GridDistances.LShaped(new GridStep(deltaRow, deltaCol)));

    [Fact]
    public void LShaped_FromTheMiddleOfALargeBoard_IsTheBreadthFirstJumpCountToEveryCell()
    {
        var middle = new Cell(LargeBoardSide / 2, LargeBoardSide / 2);
        var jumps = BreadthFirstSteps(
            LargeBoardSide, LargeBoardSide, middle, GridDirections.LShaped);

        Assert.All(CellsOf(LargeBoardSide, LargeBoardSide), cell => Assert.Equal(
            GridDistances.LShaped(Displacement(middle, cell)),
            jumps[cell.Row, cell.Col]));
    }

    [Theory]
    [MemberData(nameof(BoardsOfAtLeastFiveByFive))]
    public void LShaped_EveryPairOfCellsOnABoard_DiffersOnlyFromACornerToItsDiagonal(
        int rows, int cols) =>
        Assert.Equal(
            CornerDiagonalPairs(rows, cols).Order(),
            Disagreements(rows, cols, GridDirections.LShaped, GridDistances.LShaped).Order());

    // A metric: zero only for no displacement, the same either way, and never more than two
    // displacements that add up to it.
    [Fact]
    public void LShaped_DisplacementsWithinFour_IsAMetric()
    {
        ImmutableArray<GridStep> displacements =
            [GridStep.AdditiveIdentity, .. GridDirections.WithinChebyshev(4)];

        Assert.All(displacements, first =>
        {
            Assert.Equal(first == GridStep.AdditiveIdentity, GridDistances.LShaped(first) == 0);
            Assert.Equal(GridDistances.LShaped(first), GridDistances.LShaped(-first));
            Assert.All(displacements, second => Assert.InRange(
                GridDistances.LShaped(first + second),
                0,
                GridDistances.LShaped(first) + GridDistances.LShaped(second)));
        });
    }

    // Every pair of cells whose search count is not the closed form's, with both counts.
    private static IEnumerable<(Cell From, Cell To, int Searched, int ClosedForm)> Disagreements(
        int rows, int cols, ImmutableArray<GridStep> steps, Func<GridStep, int> closedForm) =>
        CellsOf(rows, cols).SelectMany(from =>
        {
            var searched = BreadthFirstSteps(rows, cols, from, steps);

            return CellsOf(rows, cols)
                .Select(to => (
                    From: from,
                    To: to,
                    Searched: searched[to.Row, to.Col],
                    ClosedForm: closedForm(Displacement(from, to))))
                .Where(pair => pair.Searched != pair.ClosedForm);
        });

    // The fewest steps from one cell to every cell of a board, or Unreached.
    private static int[,] BreadthFirstSteps(
        int rows, int cols, Cell from, ImmutableArray<GridStep> steps)
    {
        var counts = UnreachedBoard(rows, cols);
        var frontier = new Queue<Cell>([from]);
        counts[from.Row, from.Col] = 0;

        while (frontier.TryDequeue(out var cell))
        {
            ReachNeighbours(cell, steps, counts, frontier);
        }

        return counts;
    }

    // A board with every cell marked Unreached.
    private static int[,] UnreachedBoard(int rows, int cols)
    {
        var counts = new int[rows, cols];

        foreach (var cell in CellsOf(rows, cols))
        {
            counts[cell.Row, cell.Col] = Unreached;
        }

        return counts;
    }

    private static IEnumerable<Cell> CellsOf(int rows, int cols) =>
        Enumerable.Range(0, rows)
            .SelectMany(row => Enumerable.Range(0, cols).Select(col => new Cell(row, col)));

    // Counts and queues each cell one step from this one that is on the board and not yet reached.
    private static void ReachNeighbours(
        Cell cell, ImmutableArray<GridStep> steps, int[,] counts, Queue<Cell> frontier)
    {
        foreach (var step in steps)
        {
            var next = new Cell(cell.Row + step.DeltaRow, cell.Col + step.DeltaCol);
            var isNew = IsOnBoard(next, counts) && counts[next.Row, next.Col] == Unreached;

            if (isNew)
            {
                counts[next.Row, next.Col] = counts[cell.Row, cell.Col] + 1;
                frontier.Enqueue(next);
            }
        }
    }

    private static bool IsOnBoard(Cell cell, int[,] counts) =>
        (uint)cell.Row < (uint)counts.GetLength(0) && (uint)cell.Col < (uint)counts.GetLength(1);

    private static GridStep Displacement(Cell from, Cell to) =>
        new(to.Row - from.Row, to.Col - from.Col);

    // Each corner and the cell diagonally inside it, both ways round, with their two counts.
    private static IEnumerable<(Cell From, Cell To, int Searched, int ClosedForm)>
        CornerDiagonalPairs(int rows, int cols)
    {
        var lastRow = rows - 1;
        var lastCol = cols - 1;
        (Cell Corner, Cell Inside)[] corners =
        [
            (new(0, 0), new(1, 1)),
            (new(0, lastCol), new(1, lastCol - 1)),
            (new(lastRow, 0), new(lastRow - 1, 1)),
            (new(lastRow, lastCol), new(lastRow - 1, lastCol - 1)),
        ];

        return corners.SelectMany(pair => new[]
        {
            (pair.Corner, pair.Inside, CornerToDiagonalOnABoard, CornerToDiagonalUnbounded),
            (pair.Inside, pair.Corner, CornerToDiagonalOnABoard, CornerToDiagonalUnbounded),
        });
    }

    // A cell of a board, by its row and column.
    private readonly record struct Cell(int Row, int Col) : IComparable<Cell>
    {
        public int CompareTo(Cell other) => (Row, Col).CompareTo((other.Row, other.Col));
    }
}
