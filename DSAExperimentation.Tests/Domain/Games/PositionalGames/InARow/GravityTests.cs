using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow;

// On Connect Four's board, rows numbered from the top: a piece dropped into an empty column lands
// on the bottom row, the next on top of it, and a column of six is full. The rule read on a bit
// mask must agree with the rule read on an array of owners, so both are asked about every column
// of boards drawn at random: pieces, gaps and holes anywhere, floating or not.
public sealed partial class GravityTests
{
    private const int Column = 3;
    private const int RandomBoards = 200;
    private const int Seed = 4064;

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

    [Fact]
    public void Choices_EmptyBoard_IsEveryColumnsTopCell() =>
        Assert.Equal(0b111_1111UL, Gravity<ConnectFourBoard>.Choices(ulong.MaxValue));

    [Fact]
    public void CellClaimed_AnEmptyColumn_LandsOnTheLowestCell() =>
        Assert.Equal(
            Cell(BottomRow), Gravity<ConnectFourBoard>.CellClaimed(ulong.MaxValue, Column));

    [Fact]
    public void ChoicesAndCellClaimed_RandomBoards_AgreeWithTryPlace() =>
        Assert.All(RandomOwners(), owners => Assert.All(
            Enumerable.Range(0, ConnectFourBoard.Columns),
            column => Assert.Equal(PlacedByOwners(owners, column), PlacedByMask(owners, column))));

    private static int Cell(int row) => MnkGame<ConnectFourBoard>.CellAt(row, Column);

    // Boards whose every cell is open, either player's, or a hole, drawn independently.
    private static IEnumerable<int[]> RandomOwners()
    {
        var random = new Random(Seed);
        int[] kinds = [BoardMask.Unclaimed, BoardMask.Unclaimed, 0, 1, BoardMask.Hole];

        for (var board = 0; board < RandomBoards; board++)
        {
            yield return [.. Enumerable.Range(0, MnkGame<ConnectFourBoard>.CellCount)
                .Select(_ => kinds[random.Next(kinds.Length)])];
        }
    }

    // The cell a column's piece lands in, or none when the column is no choice.
    private static int? PlacedByOwners(int[] owners, int column)
    {
        if (!Gravity<ConnectFourBoard>.TryPlace(owners, column, out var cell))
        {
            return null;
        }

        return cell;
    }

    private static int? PlacedByMask(int[] owners, int column)
    {
        var open = OpenMask(owners);
        var isOpen = ((Gravity<ConnectFourBoard>.Choices(open) >> column) & 1) != 0;

        if (!isOpen)
        {
            return null;
        }

        return Gravity<ConnectFourBoard>.CellClaimed(open, column);
    }

    private static ulong OpenMask(int[] owners) =>
        BitboardPosition.MaskOf([.. Enumerable.Range(0, owners.Length)
            .Where(cell => owners[cell] == BoardMask.Unclaimed)]);

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
