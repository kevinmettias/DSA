using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Games.TicTacToe;
using DSAExperimentation.Domain.Mathematics.GameTheory;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// Alpha-beta finds the value plain negamax finds. Searched to the end of the game, with an
// evaluation that throws if asked, it agrees with Negamax's exact value on the take-away game, the
// end-pick scoring game (whose gains shift the window), tic-tac-toe and the 4,3,3 game; to a
// limited depth it agrees with Negamax.ValueToDepth at every depth; and the order its moves are
// tried in changes the positions it searches, never the value. Connect Four's own play is checked
// in ConnectFourPlayTests.
public sealed partial class AlphaBetaTests
{
    private const int MostPerTurn = 3;
    private const int LargestPile = 20;
    private const int FiveStones = 5;
    private const int Draw = 0;
    private const int Win = 1;
    private const long EmptyBoard = 0;

    public static TheoryData<int[]> EndPickRows => new()
    {
        { [1, 5, 2] },
        { [1, 5, 233, 7] },
        { [7] },
        { [3, 9, 1, 2, 8, 4, 6, 5] },
    };

    [Fact]
    public void Value_TakeAwayToTheEnd_IsNegamaxsValue() =>
        Assert.All(
            Enumerable.Range(0, LargestPile + 1),
            pile => Assert.Equal(
                Negamax.Value<TakeAwayGame, int, int>(TakeAway(), pile),
                TakeAwayValue<UnreachedEvaluation<int>>(pile, pile)));

    [Theory]
    [MemberData(nameof(EndPickRows))]
    public void Value_EndPickToTheEnd_IsNegamaxsValue(int[] row) =>
        Assert.Equal(
            ExactEndPick(row),
            EndPickToTheEnd<MoveNumberOrder<(int, int)>>(row));

    [Fact]
    public void Value_TicTacToeToTheEnd_IsADraw() =>
        Assert.Equal(Draw, LineGameValue(new LineGame(TicTacToeRules.Game.WinningSets)));

    [Fact]
    public void Value_FourByThreeThreeInARowToTheEnd_IsAFirstPlayerWin() =>
        Assert.Equal(Win, LineGameValue(new LineGame(MnkGame<FourByThreeBoard>.Lines)));

    // The scoring game's gains shift every window, and an estimate of nothing stops each line of
    // play short: at every depth the two searches must still agree.
    [Theory]
    [MemberData(nameof(EndPickRows))]
    public void Value_EndPickToEachDepth_IsPlainNegamaxsValue(int[] row) =>
        Assert.All(
            Enumerable.Range(0, row.Length + 1),
            depth => Assert.Equal(
                Negamax.ValueToDepth<EndPickGame, ZeroEvaluation<(int, int)>, (int, int), int>(
                    new EndPickGame(row), default, Span(row), depth),
                EndPickToDepth(row, depth)));

    [Theory]
    [MemberData(nameof(EndPickRows))]
    public void Value_MovesTriedLastFirst_IsTheSameValue(int[] row) =>
        Assert.Equal(
            ExactEndPick(row),
            EndPickToTheEnd<ReversedMoveOrder<(int, int)>>(row));

    [Fact]
    public void Value_DepthZero_IsTheEstimate() =>
        Assert.Equal(0, TakeAwayValue<ZeroEvaluation<int>>(LargestPile, 0));

    // Five stones: taking one leaves four, a multiple of four, so it wins; taking two or three
    // leaves a pile the opponent wins from.
    [Fact]
    public void BestMove_TakeAway_TakesWhatLeavesAMultipleOfFour() =>
        Assert.Equal((0, Win), TakeAwayBestMove<UnreachedEvaluation<int>>(FiveStones, FiveStones));

    [Fact]
    public void BestMove_DepthBelowOne_IsRefused() =>
        Assert.Throws<ArgumentOutOfRangeException>(
            () => TakeAwayBestMove<ZeroEvaluation<int>>(LargestPile, 0));

    [Fact]
    public void BestMove_GameOver_IsRefused() =>
        Assert.Throws<ArgumentException>(() => TakeAwayBestMove<ZeroEvaluation<int>>(0, 1));

    // The early passes stop short of the end and take estimates, but the last reaches the end.
    [Theory]
    [MemberData(nameof(EndPickRows))]
    public void BestMoveByDeepening_EndPick_HasNegamaxsValue(int[] row) =>
        Assert.Equal(
            ExactEndPick(row),
            AlphaBeta.BestMoveByDeepening<
                EndPickGame, ZeroEvaluation<(int, int)>, MoveNumberOrder<(int, int)>,
                (int, int), int>(new EndPickGame(row), default, default, Span(row), row.Length)
                .Value);

    [Fact]
    public void BestMoveByDeepening_GameOver_IsRefused() =>
        Assert.Throws<ArgumentException>(
            () => AlphaBeta.BestMoveByDeepening<
                TakeAwayGame, ZeroEvaluation<int>, MoveNumberOrder<int>, int, int>(
                TakeAway(), default, default, 0, 1));

    private static TakeAwayGame TakeAway() => new(MostPerTurn);

    private static (int Left, int Right) Span(int[] row) => (0, row.Length - 1);

    private static int ExactEndPick(int[] row) =>
        Negamax.Value<EndPickGame, (int, int), int>(new EndPickGame(row), Span(row));

    private static int TakeAwayValue<TEvaluation>(int pile, int depth)
        where TEvaluation : struct, IStaticEvaluation<int, int>
        => AlphaBeta.Value<TakeAwayGame, TEvaluation, MoveNumberOrder<int>, int, int>(
            TakeAway(), default, default, pile, depth);

    private static (int Move, int Value) TakeAwayBestMove<TEvaluation>(int pile, int depth)
        where TEvaluation : struct, IStaticEvaluation<int, int>
        => AlphaBeta.BestMove<TakeAwayGame, TEvaluation, MoveNumberOrder<int>, int, int>(
            TakeAway(), default, default, pile, depth);

    // Searched as deep as the row is long, so every line of play ends before the depth does.
    private static int EndPickToTheEnd<TOrdering>(int[] row)
        where TOrdering : struct, IMoveOrdering<(int, int)>
        => AlphaBeta.Value<
            EndPickGame, UnreachedEvaluation<(int, int)>, TOrdering, (int, int), int>(
            new EndPickGame(row), default, default, Span(row), row.Length);

    private static int EndPickToDepth(int[] row, int depth) =>
        AlphaBeta.Value<
            EndPickGame, ZeroEvaluation<(int, int)>, MoveNumberOrder<(int, int)>,
            (int, int), int>(new EndPickGame(row), default, default, Span(row), depth);

    private static int LineGameValue(LineGame game) =>
        AlphaBeta.Value<LineGame, UnreachedEvaluation<long>, MoveNumberOrder<long>, long, int>(
            game, default, default, EmptyBoard, game.MoveCount(EmptyBoard));
}
