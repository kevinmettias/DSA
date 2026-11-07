using DSAExperimentation.Algorithms.ExactCover;
using DSAExperimentation.Domain.Games.Chess;
using DSAExperimentation.Domain.Mathematics.Combinatorics.ExactCovers;

namespace DSAExperimentation.Tests.Domain.Games.Chess;

// The reduction's numbers, then the reduction proved by counting: Algorithm X's covers of the
// n-queens matrix are the puzzle's solutions, whose numbers for n = 1 to 8 are known (OEIS
// A000170). The diagonals are secondary, so a cover may leave one empty, and each of the 92
// eight-queens covers keeps the exact cover rule.
public sealed partial class NQueensCoverTests
{
    private const int EightQueens = 8;

    public static TheoryData<int, long> SolutionCounts => new()
    {
        { 1, 1 }, { 2, 0 }, { 3, 0 }, { 4, 2 }, { 5, 10 }, { 6, 4 }, { 7, 40 }, { 8, 92 },
    };

    [Fact]
    public void BoardSize_Constructed_IsTheSide() => Assert.Equal(5, new NQueensCover(5).BoardSize);

    [Fact]
    public void Constructor_NegativeSide_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new NQueensCover(-1));

    [Fact]
    public void PrimaryColumnCount_EightQueens_IsTheRowsAndColumns() =>
        Assert.Equal(16, new NQueensCover(EightQueens).PrimaryColumnCount);

    [Fact]
    public void SecondaryColumnCount_EightQueens_IsTheDiagonalsBothWays() =>
        Assert.Equal(30, new NQueensCover(EightQueens).SecondaryColumnCount);

    [Fact]
    public void RowCount_EightQueens_IsTheSquares() =>
        Assert.Equal(64, new NQueensCover(EightQueens).RowCount);

    [Fact]
    public void ColumnCountOf_AnySquare_IsFour() =>
        Assert.Equal(4, new NQueensCover(EightQueens).ColumnCountOf(0));

    // Row 1, column 3 of a 4x4 board: row 1, column 4 + 3, diagonal 8 + (1 - 3 + 3), and
    // anti-diagonal 8 + 7 + (1 + 3).
    [Fact]
    public void WriteColumns_Square_MeetsItsRowColumnAndBothDiagonals()
    {
        var columns = new int[4];

        new NQueensCover(4).WriteColumns(7, columns);

        Assert.Equal([1, 7, 9, 19], columns);
    }

    [Fact]
    public void SquareOf_SquareNumber_IsItsRowAndColumn() =>
        Assert.Equal((1, 3), new NQueensCover(4).SquareOf(7));

    [Theory]
    [MemberData(nameof(SolutionCounts))]
    public void CountCovers_Board_IsTheNumberOfSolutions(int boardSize, long expected) =>
        Assert.Equal(expected, AlgorithmX.CountCovers(new NQueensCover(boardSize), []));

    [Fact]
    public void TrySearch_EightQueens_EveryCoverKeepsTheRule()
    {
        var queens = new NQueensCover(EightQueens);
        var visitor = new RuleCheck(queens, []);

        AlgorithmX.TrySearch(queens, [], ref visitor);

        Assert.Equal(92, visitor.Verdicts.Count);
        Assert.All(visitor.Verdicts, Assert.True);
    }

    // Checks each cover against the exact cover rule.
    private readonly record struct RuleCheck(NQueensCover Queens, List<bool> Verdicts)
        : ICoverVisitor
    {
        public bool ShouldStopAfter(ReadOnlySpan<int> cover)
        {
            Verdicts.Add(ExactCover<NQueensCover>.IsCover(Queens, cover));

            return false;
        }
    }
}
