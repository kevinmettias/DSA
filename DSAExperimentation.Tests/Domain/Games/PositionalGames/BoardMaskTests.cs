using DSAExperimentation.Domain.Games.PositionalGames;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

// A 3 by 3 board with its centre and a corner cut out, the corner named twice.
public sealed partial class BoardMaskTests
{
    private const int Cells = 9;
    private const int Centre = 4;
    private const int Corner = 8;
    private const int Open = BoardMask.Unclaimed;
    private const int Hole = BoardMask.Hole;

    public static TheoryData<int> CellsOnTheBoard => new() { 0, 7 };

    public static TheoryData<int> CellsOffTheBoard => new() { Centre, Corner, -1, Cells };

    [Fact]
    public void CellCount_CountsTheHolesToo() => Assert.Equal(Cells, Holed().CellCount);

    [Fact]
    public void PlayableCount_IsTheCellsLessTheHoles() =>
        Assert.Equal(Cells - 2, Holed().PlayableCount);

    [Fact]
    public void Full_HasNoHole() =>
        Assert.Equal(
            [Cells, Cells],
            [BoardMask.Full(Cells).CellCount, BoardMask.Full(Cells).PlayableCount]);

    [Theory]
    [MemberData(nameof(CellsOnTheBoard))]
    public void HasCell_ANumberedCellThatIsNoHole_IsTrue(int cell) =>
        Assert.True(Holed().HasCell(cell));

    [Theory]
    [MemberData(nameof(CellsOffTheBoard))]
    public void HasCell_AHoleOrACellNotNumbered_IsFalse(int cell) =>
        Assert.False(Holed().HasCell(cell));

    [Fact]
    public void EmptyPosition_MarksTheHolesAndLeavesEveryOtherCellOpen() =>
        Assert.Equal(
            [Open, Open, Open, Open, Hole, Open, Open, Open, Hole],
            Holed().EmptyPosition());

    [Fact]
    public void Constructor_AHoleOffTheBoard_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new BoardMask(Cells, Cells));

    private static BoardMask Holed() => new(Cells, Centre, Corner, Corner);
}
