using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Games.TicTacToe;
using DSAExperimentation.Domain.Games.Turns;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

// Cells on tic-tac-toe's board are numbered 0 1 2 / 3 4 5 / 6 7 8. The games below are played under
// each turn order and both placement rules, on a board with a hole and with winning sets of their
// own.
public sealed partial class PositionalGameTests
{
    private const int First = TwoPlayersAlternating.First;
    private const int Second = TwoPlayersAlternating.Second;
    private const int Cells = TicTacToeBoard.Size * TicTacToeBoard.Size;
    private const int Centre = 4;
    private const int Corner = 0;
    private const int Edge = 1;

    public static TheoryData<int, int> HolesToTheLinesLeft => new()
    {
        { Centre, 4 },
        { Corner, 5 },
        { Edge, 6 },
    };

    public static TheoryData<int> CellsNotOpen => new() { Centre, Corner, Cells };

    public static TheoryData<int[], PositionalOutcome> TicTacToeGames => new()
    {
        { [0, 3, 1, 4, 2], PositionalOutcome.WonBy(First) },
        { [0, 3, 1, 4, 8, 5], PositionalOutcome.WonBy(Second) },
        { [0, 1, 2, 3, 5, 7, 4, 8, 6], PositionalOutcome.WonBy(First) },
        { [0, 4, 6, 3, 5, 7, 1, 2, 8], PositionalOutcome.Drawn },
        { [0, 4], PositionalOutcome.Pending },
        { [], PositionalOutcome.Pending },
    };

    public static TheoryData<int[]> RefusedTicTacToeGames => new()
    {
        new[] { 0, 0 },
        new[] { 0, 3, 1, 4, 2, 5 },
        new[] { Cells },
        new[] { -1 },
    };

    public static TheoryData<int[], PositionalOutcome> SimultaneousGames => new()
    {
        { [0, 3, 1, 4, 2, 5], PositionalOutcome.Drawn },
        { [0, 3, 1, 4, 2, 8], PositionalOutcome.WonBy(First) },
        { [0, 3, 1, 4, 2], PositionalOutcome.Pending },
    };

    [Theory]
    [MemberData(nameof(HolesToTheLinesLeft))]
    public void WinningSets_AHole_RemovesEveryLineThroughIt(int hole, int expected) =>
        Assert.Equal(expected, Holed(hole).WinningSets.Count);

    [Fact]
    public void Board_IsTheBoardGiven() =>
        Assert.Equal(Cells - 1, Holed(Centre).Board.PlayableCount);

    [Fact]
    public void IsClaimable_AnOpenCell_IsTrue() =>
        Assert.True(Holed(Centre).IsClaimable(Position(Centre), Corner));

    [Theory]
    [MemberData(nameof(CellsNotOpen))]
    public void IsClaimable_AHoleATakenCellOrACellOffTheBoard_IsFalse(int cell) =>
        Assert.False(Holed(Centre).IsClaimable(Position(Centre, (Corner, First)), cell));

    [Fact]
    public void HasCompletedASet_TheLastCellOfALineTheSeatHolds_IsTrue() =>
        Assert.True(TicTacToe().HasCompletedASet(TopRow(First, First, First), 2, First));

    [Fact]
    public void HasCompletedASet_ALineShared_IsFalse() =>
        Assert.False(TicTacToe().HasCompletedASet(TopRow(First, Second, First), 2, First));

    [Fact]
    public void IsWonBy_ALineAnywhereTheSeatHolds_IsTrue() =>
        Assert.True(TicTacToe().IsWonBy(AntiDiagonalHeldByTheFirstSeat(), First));

    [Fact]
    public void IsWonBy_ALineTheOtherSeatHolds_IsFalse() =>
        Assert.False(TicTacToe().IsWonBy(AntiDiagonalHeldByTheFirstSeat(), Second));

    [Theory]
    [MemberData(nameof(TicTacToeGames))]
    public void Replay_TicTacToe_WinsDrawsAndPends(int[] moves, PositionalOutcome expected) =>
        Assert.Equal(expected, TicTacToe().Replay<FreePlacement>(moves));

    [Theory]
    [MemberData(nameof(RefusedTicTacToeGames))]
    public void Replay_ATakenCellAMoveAfterTheEndOrACellOffTheBoard_Throws(int[] moves) =>
        Assert.Throws<ArgumentException>(() => TicTacToe().Replay<FreePlacement>(moves));

    [Fact]
    public void Replay_AHole_CannotBeClaimed() =>
        Assert.Throws<ArgumentException>(() => Holed(Centre).Replay<FreePlacement>([Centre]));

    [Fact]
    public void Replay_ABoardWithAHole_IsFullAndDrawnWhenItsCellsAre() =>
        Assert.Equal(
            PositionalOutcome.Drawn,
            Holed(Centre).Replay<FreePlacement>([1, 0, 3, 2, 5, 6, 7, 8]));

    [Fact]
    public void Replay_ThreePlayersInTurn_TheFirstToALineWins() =>
        Assert.Equal(
            PositionalOutcome.WonBy(First),
            Game<RoundRobin<ThreePlayers>>().Replay<FreePlacement>([0, 1, 2, 3, 4, 5, 6]));

    [Theory]
    [MemberData(nameof(SimultaneousGames))]
    public void Replay_Simultaneous_ATurnIsJudgedWhole(int[] moves, PositionalOutcome expected) =>
        Assert.Equal(expected, Game<Simultaneous<TwoPlayers>>().Replay<FreePlacement>(moves));

    [Fact]
    public void Replay_CustomWinningSets_TheFourCornersWin() =>
        Assert.Equal(
            PositionalOutcome.WonBy(First),
            FourCorners().Replay<FreePlacement>([0, 1, 2, 3, 6, 4, 8]));

    [Fact]
    public void Replay_ConnectFourUnderGravity_FourDroppedInAColumnWin() =>
        Assert.Equal(
            PositionalOutcome.WonBy(First),
            ConnectFour().Replay<Gravity<ConnectFourBoard>>([0, 1, 0, 1, 0, 1, 0]));

    [Fact]
    public void Replay_ConnectFourUnderGravity_ASeventhPieceInAColumn_Throws() =>
        Assert.Throws<ArgumentException>(
            () => ConnectFour().Replay<Gravity<ConnectFourBoard>>([3, 3, 3, 3, 3, 3, 3]));

    private static PositionalGame<TwoPlayersAlternating> TicTacToe() =>
        Game<TwoPlayersAlternating>();

    private static PositionalGame<TTurns> Game<TTurns>()
        where TTurns : struct, ITurnOrder
        => new(MnkGame<TicTacToeBoard>.Board, MnkGame<TicTacToeBoard>.Lines);

    private static PositionalGame<TwoPlayersAlternating> Holed(int hole) =>
        new(new BoardMask(Cells, hole), MnkGame<TicTacToeBoard>.Lines);

    // A winning condition of its own: the four corners.
    private static PositionalGame<TwoPlayersAlternating> FourCorners() =>
        new(MnkGame<TicTacToeBoard>.Board, new WinningSets(Cells, [[0, 2, 6, 8]]));

    private static PositionalGame<TwoPlayersAlternating> ConnectFour() =>
        new(MnkGame<ConnectFourBoard>.Board, MnkGame<ConnectFourBoard>.Lines);

    private static int[] AntiDiagonalHeldByTheFirstSeat() =>
        Position(null, (2, First), (4, First), (6, First));

    // The top row's three cells held by the given seats.
    private static int[] TopRow(int left, int middle, int right) =>
        Position(null, (0, left), (1, middle), (2, right));

    // Tic-tac-toe's empty position, with a hole if one is named, then the given cells claimed.
    private static int[] Position(int? hole, params (int Cell, int Seat)[] claims)
    {
        var board = hole is int cell ? new BoardMask(Cells, cell) : MnkGame<TicTacToeBoard>.Board;
        var owners = board.EmptyPosition();

        foreach (var (claimed, seat) in claims)
        {
            owners[claimed] = seat;
        }

        return owners;
    }
}
