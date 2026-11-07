using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Games.TicTacToe;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;
using ConnectFourGame = DSAExperimentation.Domain.Games.PositionalGames.BitboardGame<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.Gravity<
        DSAExperimentation.Domain.Games.PositionalGames.InARow.ConnectFourBoard>>;
using FreeGame = DSAExperimentation.Domain.Games.PositionalGames.BitboardGame<
    DSAExperimentation.Domain.Games.PositionalGames.FreePlacement>;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

// Connect Four as the game contract plays it, rows numbered from the top: a choice is a column, a
// piece lands on the lowest open cell, a full column is no move, four in a row ends the game, and
// a loss is worth the claimed cells less WinValue. Free placement and holes are checked on boards
// of their own, and negamax over the masks gives tic-tac-toe and the 4,3,3 game their known values.
public sealed partial class BitboardGameTests
{
    private const int Columns = 7;
    private const int Column = 3;
    private const int ConnectFourLines = 69;
    private const int Draw = 0;
    private const int Centre = 4;
    private const int Corner = 8;

    private static ConnectFourGame ConnectFour =>
        new(MnkGame<ConnectFourBoard>.Board, MnkGame<ConnectFourBoard>.Lines);

    [Fact]
    public void MoveCount_EmptyBoard_IsEveryColumn() =>
        Assert.Equal(Columns, ConnectFour.MoveCount(default));

    [Fact]
    public void MoveCount_AFullColumn_IsLeftOut() =>
        Assert.Equal(Columns - 1, ConnectFour.MoveCount(FullFirstColumn()));

    [Fact]
    public void ChoiceOf_AFullColumn_NumbersTheOpenColumnsFromTheLeft() =>
        Assert.Equal(
            [1, 2, 3, 4, 5, 6],
            Enumerable.Range(0, Columns - 1)
                .Select(move => ConnectFour.ChoiceOf(FullFirstColumn(), move)));

    [Fact]
    public void ChoicesOf_EmptyBoard_IsTheTopRow() =>
        Assert.Equal((1UL << Columns) - 1, ConnectFour.ChoicesOf(default));

    [Fact]
    public void WinningSets_ConnectFour_AreItsSixtyNineLines() =>
        Assert.Equal(ConnectFourLines, ConnectFour.WinningSets.Count);

    [Fact]
    public void Replay_OnePiece_LandsOnTheBottomRow_AndTheOtherPlayerMoves() =>
        Assert.Equal(
            new BitboardPosition(0, CellBit(BottomRow(), Column), IsLost: false),
            ConnectFour.Replay([Column]));

    [Fact]
    public void Replay_AFullColumn_IsRefused() =>
        Assert.Throws<ArgumentException>(() => ConnectFour.Replay([0, 0, 0, 0, 0, 0, 0]));

    [Fact]
    public void Replay_AfterFourInARow_IsRefused() =>
        Assert.Throws<ArgumentException>(() => ConnectFour.Replay([.. ThreeStacked(), Column, 0]));

    // A hole in the bottom row of a column: a piece dropped there rests on the hole.
    [Fact]
    public void Replay_GravityOverAHole_RestsOnTheHole()
    {
        var hole = MnkGame<ConnectFourBoard>.CellAt(BottomRow(), Column);
        var board = new BoardMask(MnkGame<ConnectFourBoard>.CellCount, hole);
        var game = new ConnectFourGame(board, MnkGame<ConnectFourBoard>.Lines);

        Assert.Equal(CellBit(BottomRow() - 1, Column), game.Replay([Column]).Opponent);
    }

    [Fact]
    public void Next_DroppingOnAPiece_LandsOnTopOfIt()
    {
        var game = ConnectFour;
        var after = game.Next(game.Replay([Column]), Column);

        Assert.Equal(CellBit(BottomRow() - 1, Column), after.Opponent);
    }

    [Fact]
    public void Next_FourInARow_LeavesThePlayerToMoveLost()
    {
        var game = ConnectFour;
        var won = game.Next(game.Replay(ThreeStacked()), Column);

        Assert.Equal((true, 0), (won.IsLost, game.MoveCount(won)));
    }

    [Fact]
    public void Next_ThreeInARow_LeavesTheGameGoing() =>
        Assert.False(ConnectFour.Replay(ThreeStacked()).IsLost);

    [Fact]
    public void Gain_IsNothing() =>
        Assert.Equal(0, ConnectFour.Gain(default, Column));

    [Fact]
    public void ValueWhenOver_ALoss_IsTheClaimedCellsLessWinValue()
    {
        var game = ConnectFour;
        var won = game.Next(game.Replay(ThreeStacked()), Column);

        Assert.Equal(ThreeStacked().Length + 1 - ConnectFourGame.WinValue, game.ValueWhenOver(won));
    }

    [Fact]
    public void Constructor_MoreThanSixtyFourCells_IsRefused() =>
        Assert.Throws<ArgumentException>(
            () => new FreeGame(MnkGame<GomokuBoard>.Board, MnkGame<GomokuBoard>.Lines));

    // The same game under free placement: tic-tac-toe, with and without a hole in the centre.
    [Fact]
    public void WinningSets_AHole_DropsEverySetThroughIt() =>
        Assert.Equal(4, HoledTicTacToe().WinningSets.Count);

    [Fact]
    public void Replay_FreePlacement_ClaimsTheCellNamed() =>
        Assert.Equal(
            new BitboardPosition(0, 1UL << Corner, IsLost: false),
            TicTacToe().Replay([Corner]));

    [Fact]
    public void Replay_FreePlacementOnAHole_IsRefused() =>
        Assert.Throws<ArgumentException>(() => HoledTicTacToe().Replay([Centre]));

    // Two cells and one set of both, claimed one each: the board fills with no set held.
    [Fact]
    public void ValueWhenOver_AFullBoardWithNoSetHeld_IsADraw()
    {
        var game = new FreeGame(BoardMask.Full(2), new WinningSets(2, [[0, 1]]));
        var full = game.Replay([0, 1]);

        Assert.Equal((0, Draw), (game.MoveCount(full), game.ValueWhenOver(full)));
    }

    [Fact]
    public void Negamax_TicTacToe_IsADraw() =>
        Assert.Equal(Draw, Solved(TicTacToe()));

    [Fact]
    public void Negamax_FourByThreeThreeInARow_IsAFirstPlayerWin()
    {
        var game = new FreeGame(
            MnkGame<FourByThreeBoard>.Board, MnkGame<FourByThreeBoard>.Lines);

        Assert.True(Solved(game) > 0);
    }

    private static ulong CellBit(int row, int column) =>
        1UL << MnkGame<ConnectFourBoard>.CellAt(row, column);

    // Six pieces in the first column, the players alternating, so neither has four in it.
    private static BitboardPosition FullFirstColumn() => ConnectFour.Replay([0, 0, 0, 0, 0, 0]);

    private static int BottomRow() => ConnectFourBoard.Rows - 1;

    // The first player stacks three in the centre column while the second plays beside it.
    private static int[] ThreeStacked() => [Column, 4, Column, 5, Column, 4];

    private static FreeGame TicTacToe() =>
        new(TicTacToeRules.Game.Board, TicTacToeRules.Game.WinningSets);

    private static FreeGame HoledTicTacToe() =>
        new(new BoardMask(MnkGame<TicTacToeBoard>.CellCount, Centre),
            MnkGame<TicTacToeBoard>.Lines);

    private static int Solved(FreeGame game) =>
        Negamax.Value<FreeGame, BitboardPosition, int>(game, default);

    // The layer's contract, on Connect Four's 7 by 6 board with four in a row under free placement,
    // the game judged by the masks of its winning sets.
    public sealed partial class FreePlacementContractTests
        : BitboardGameContractTests<ConnectFourBoard, FreeGame, BitboardPosition>
    {
        protected override FreeGame GameOn(BoardMask board) =>
            new(board, MnkGame<ConnectFourBoard>.Lines);
    }
}
