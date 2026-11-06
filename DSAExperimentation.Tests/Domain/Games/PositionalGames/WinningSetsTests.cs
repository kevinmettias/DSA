using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Games.TicTacToe;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

// The incidence is checked against the sets it is built from, cell by cell, and against
// tic-tac-toe's known counts: the centre lies on four lines, a corner on three and an edge on two.
public sealed partial class WinningSetsTests
{
    private const int Cells = TicTacToeBoard.Size * TicTacToeBoard.Size;
    private const int Centre = 4;
    private const int Corner = 0;
    private const int Edge = 1;

    public static TheoryData<int[][]> BrokenSets => new()
    {
        new int[][] { [] },
        new int[][] { [0, 1, 0] },
        new int[][] { [0, Cells] },
        new int[][] { [-1, 0] },
    };

    public static TheoryData<int, int> CellsToTheirLineCounts => new()
    {
        { Centre, 4 },
        { Corner, 3 },
        { Edge, 2 },
    };

    [Fact]
    public void CellCount_IsTheBoards() => Assert.Equal(Cells, TicTacToeLines().CellCount);

    [Fact]
    public void Count_IsTheSetsGiven() => Assert.Equal(2, CornersAndCentre().Count);

    [Fact]
    public void CellsOf_IsEachSetAsGiven() =>
        Assert.Equal([[0, 2, 6, 8], [Centre]], Kept(CornersAndCentre()));

    [Theory]
    [MemberData(nameof(CellsToTheirLineCounts))]
    public void SetsThrough_TicTacToe_CountsTheLinesThroughACell(int cell, int expected) =>
        Assert.Equal(expected, TicTacToeLines().SetsThrough(cell).Length);

    [Fact]
    public void SetsThrough_EveryCell_IsEverySetHoldingItInOrder() =>
        Assert.All(
            Enumerable.Range(0, MnkGame<ConnectFourBoard>.CellCount),
            cell => Assert.Equal(SetsHolding(cell), ConnectFourLines().SetsThrough(cell)));

    [Theory]
    [MemberData(nameof(BrokenSets))]
    public void Constructor_AnEmptySetARepeatOrACellOffTheBoard_Throws(int[][] sets) =>
        Assert.Throws<ArgumentException>(() => new WinningSets(Cells, sets));

    [Fact]
    public void Avoiding_AHole_RemovesEverySetThroughIt() =>
        Assert.Equal(
            [[0, 1, 2], [6, 7, 8], [0, 3, 6], [2, 5, 8]],
            Kept(TicTacToeLines().Avoiding(new BoardMask(Cells, Centre))));

    [Fact]
    public void Avoiding_NoHole_KeepsTheSameSets() =>
        Assert.Same(TicTacToeLines(), TicTacToeLines().Avoiding(MnkGame<TicTacToeBoard>.Board));

    [Fact]
    public void Avoiding_ABoardOfAnotherSize_Throws() =>
        Assert.Throws<ArgumentException>(
            () => TicTacToeLines().Avoiding(BoardMask.Full(Cells + 1)));

    private static WinningSets TicTacToeLines() => MnkGame<TicTacToeBoard>.Lines;

    // A winning condition of its own: the four corners, or the centre alone.
    private static WinningSets CornersAndCentre() => new(Cells, [[0, 2, 6, 8], [Centre]]);

    private static WinningSets ConnectFourLines() => MnkGame<ConnectFourBoard>.Lines;

    private static int[] SetsHolding(int cell)
    {
        var lines = ConnectFourLines();

        return
            [.. Enumerable.Range(0, lines.Count).Where(set => lines.CellsOf(set).Contains(cell))];
    }

    private static int[][] Kept(WinningSets sets) =>
        [.. Enumerable.Range(0, sets.Count).Select(set => sets.CellsOf(set).ToArray())];
}
