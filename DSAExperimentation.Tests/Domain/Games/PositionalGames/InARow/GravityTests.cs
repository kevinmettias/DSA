using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow;

// On Connect Four's board, rows numbered from the top: a piece dropped into an empty column lands
// on the bottom row, the next on top of it, and a column of six is full.
public sealed partial class GravityTests
{
    private const int Column = 3;

    public static TheoryData<int> ColumnsOffTheBoard => new() { -1, ConnectFourBoard.Columns };

    private static int BottomRow => ConnectFourBoard.Rows - 1;

    [Fact]
    public void TryPlace_AnEmptyColumn_LandsOnTheLowestCell() =>
        Assert.Equal(Cell(BottomRow), LandingCell(Empty()));

    [Fact]
    public void TryPlace_APieceBelow_LandsOnTopOfIt() =>
        Assert.Equal(Cell(BottomRow - 1), LandingCell(Filled(BottomRow)));

    [Fact]
    public void TryPlace_AHole_IsSolid_SoThePieceRestsOnIt() =>
        Assert.Equal(Cell(1), LandingCell(HoledAt(2)));

    [Fact]
    public void TryPlace_AFullColumn_IsRefused() =>
        Assert.False(Gravity<ConnectFourBoard>.TryPlace(FullColumn(), Column, out _));

    [Theory]
    [MemberData(nameof(ColumnsOffTheBoard))]
    public void TryPlace_AColumnOffTheBoard_IsRefused(int column) =>
        Assert.False(Gravity<ConnectFourBoard>.TryPlace(Empty(), column, out _));

    private static int Cell(int row) => MnkGame<ConnectFourBoard>.CellAt(row, Column);

    private static int[] Empty() => MnkGame<ConnectFourBoard>.Board.EmptyPosition();

    private static int[] HoledAt(int row) =>
        new BoardMask(MnkGame<ConnectFourBoard>.CellCount, Cell(row)).EmptyPosition();

    private static int[] FullColumn() => Filled([.. Enumerable.Range(0, ConnectFourBoard.Rows)]);

    // The empty position with the first seat's pieces in the given rows of the column.
    private static int[] Filled(params int[] rows)
    {
        var owners = Empty();

        foreach (var row in rows)
        {
            owners[Cell(row)] = 0;
        }

        return owners;
    }

    private static int LandingCell(int[] owners)
    {
        Assert.True(Gravity<ConnectFourBoard>.TryPlace(owners, Column, out var cell));

        return cell;
    }
}
