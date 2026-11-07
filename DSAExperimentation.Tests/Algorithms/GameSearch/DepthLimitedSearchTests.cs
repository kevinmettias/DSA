using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;
using ConnectFourGame = DSAExperimentation.Domain.Games.PositionalGames.BitboardGame<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.Gravity<
        DSAExperimentation.Domain.Games.PositionalGames.InARow.ConnectFourBoard>>;
using NoTable = DSAExperimentation.Algorithms.GameSearch.NoTranspositions<
    DSAExperimentation.Domain.Games.PositionalGames.BitboardPosition, int>;
using Table = DSAExperimentation.Algorithms.GameSearch.TranspositionTable<
    DSAExperimentation.Domain.Games.PositionalGames.BitboardPosition, int>;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// What each axis of the search saves, counted in the positions it reaches from a Connect Four
// position in the middle of a game, while the value stays plain negamax's: the cutoff against none,
// a transposition table against none, and passes deepening one ply at a time with a table against
// one pass without.
public sealed partial class DepthLimitedSearchTests
{
    private const int ShallowDepth = 4;
    private const int Depth = 6;

    private static readonly int[] MidGame = [3, 3, 2, 4, 4, 2, 3, 5];

    private static ConnectFourGame ConnectFour =>
        new(MnkGame<ConnectFourBoard>.Board, MnkGame<ConnectFourBoard>.Lines);

    [Fact]
    public void ValueOf_WithTheCutoff_SearchesFewerPositionsThanWithout()
    {
        var withCutoff = Search<AlphaBetaCutoff<int>, NoTable>(default, ShallowDepth);
        var without = Search<NoCutoff<int>, NoTable>(default, ShallowDepth);
        var isFewer = withCutoff.Positions < without.Positions;

        Assert.Equal((true, without.Value), (isFewer, withCutoff.Value));
    }

    [Fact]
    public void ValueOf_WithATable_SearchesFewerPositionsThanWithout()
    {
        var withTable = Search<AlphaBetaCutoff<int>, Table>(new(), Depth);
        var without = Search<AlphaBetaCutoff<int>, NoTable>(default, Depth);
        var isFewer = withTable.Positions < without.Positions;

        Assert.Equal((true, without.Value), (isFewer, withTable.Value));
    }

    [Fact]
    public void BestMove_DeepeningWithATable_SearchesFewerPositionsThanOnePassWithout()
    {
        var (deepening, deepened) = Counted(game => AlphaBeta.BestMoveByDeepening<
            CountingGame<ConnectFourGame, BitboardPosition, int>,
            OpenLinesEvaluation, CentreFirstColumns<ConnectFourBoard>, BitboardPosition, int>(
            game, Evaluation(), new(ConnectFour), ConnectFour.Replay(MidGame), Depth).Value);
        var (onePass, value) = Counted(game => AlphaBeta.BestMove<
            CountingGame<ConnectFourGame, BitboardPosition, int>,
            OpenLinesEvaluation, CentreFirstColumns<ConnectFourBoard>, BitboardPosition, int>(
            game, Evaluation(), new(ConnectFour), ConnectFour.Replay(MidGame), Depth).Value);

        Assert.Equal((true, value), (deepening < onePass, deepened));
    }

    private static OpenLinesEvaluation Evaluation() => new(MnkGame<ConnectFourBoard>.Lines);

    private static (long Positions, int Value) Search<TCutoff, TTable>(TTable table, int depth)
        where TCutoff : struct, ICutoffRule<int>
        where TTable : struct, ITranspositionTable<BitboardPosition, int>
        => Counted(game => new DepthLimitedSearch<
            CountingGame<ConnectFourGame, BitboardPosition, int>,
            OpenLinesEvaluation, CentreFirstColumns<ConnectFourBoard>, TCutoff, TTable,
            BitboardPosition, int>(game, Evaluation(), new(ConnectFour), table)
            .ValueOf(ConnectFour.Replay(MidGame), depth));

    private static (long Positions, int Value) Counted(
        Func<CountingGame<ConnectFourGame, BitboardPosition, int>, int> search)
    {
        var moves = new long[1];
        var value = search(new(ConnectFour, moves));

        return (moves[0], value);
    }
}
