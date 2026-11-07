using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Algorithms.GameSearch.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames;
using DSAExperimentation.Domain.Games.PositionalGames.InARow;
using DSAExperimentation.Domain.Mathematics.GameTheory;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;
using DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames.Fixtures;
using DSAExperimentation.Tests.Domain.Games.PositionalGames.InARow.Fixtures;
using ConnectFourGame = DSAExperimentation.Domain.Games.PositionalGames.BitboardGame<
    DSAExperimentation.Domain.Games.PositionalGames.InARow.Gravity<
        DSAExperimentation.Domain.Games.PositionalGames.InARow.ConnectFourBoard>>;

namespace DSAExperimentation.Tests.Algorithms.GameSearch.PositionalGames;

// The order is checked as written, and by what it is for: on Connect Four positions from the middle
// of a game, alpha-beta trying the centre first searches well under half the positions it searches
// trying the edges first, and finds the same value.
public sealed partial class CentreFirstColumnsTests
{
    private const int Depth = 6;

    private static readonly int[][] MidGames =
    [
        [3, 3, 2, 4, 4, 2, 3, 5],
        [3, 2, 3, 3, 4, 4, 2, 5, 1, 0],
        [2, 3, 4, 3, 3, 2, 4, 4, 5, 1, 6, 0],
    ];

    private static ConnectFourGame ConnectFour =>
        new(MnkGame<ConnectFourBoard>.Board, MnkGame<ConnectFourBoard>.Lines);

    [Fact]
    public void Order_EmptyBoard_RunsFromTheCentreOut() =>
        Assert.Equal([3, 2, 4, 1, 5, 0, 6], Ordered(ConnectFour, default));

    // The centre column full: the open columns 0, 1, 2, 4, 5 and 6 are moves 0 to 5.
    [Fact]
    public void Order_AFullColumn_NamesTheRestByTheGamesNumbers()
    {
        var game = ConnectFour;

        Assert.Equal([2, 3, 1, 4, 0, 5], Ordered(game, game.Replay([3, 3, 3, 3, 3, 3])));
    }

    [Fact]
    public void Order_FourColumns_TakesTheLeftOfTheTwoCentresFirst()
    {
        var game = new BitboardGame<Gravity<FourByFourBoard>>(
            MnkGame<FourByFourBoard>.Board, MnkGame<FourByFourBoard>.Lines);

        Assert.Equal([1, 2, 0, 3], Ordered(game, default));
    }

    [Fact]
    public void Order_ConnectFourMidGames_SearchesUnderHalfWhatEdgesFirstDoes()
    {
        var (centreFirst, centreValues) =
            Searched<CentreFirstColumns<ConnectFourBoard>>(new(ConnectFour));
        var (edgesFirst, edgeValues) = Searched<EdgesFirstColumns>(new(ConnectFour));
        var isUnderHalf = centreFirst + centreFirst < edgesFirst;

        Assert.Equal((true, true), (isUnderHalf, centreValues.SequenceEqual(edgeValues)));
    }

    private static OpenLinesEvaluation Evaluation() => new(MnkGame<ConnectFourBoard>.Lines);

    private static int[] Ordered<TShape>(
        BitboardGame<Gravity<TShape>> game, BitboardPosition position)
        where TShape : struct, IMnkShape
    {
        var moves = new int[game.MoveCount(position)];
        new CentreFirstColumns<TShape>(game).Order(position, moves);

        return moves;
    }

    // The positions alpha-beta searches over every mid-game position at the depth, and the values.
    private static (long Positions, int[] Values) Searched<TOrdering>(TOrdering ordering)
        where TOrdering : struct, IMoveOrdering<BitboardPosition>
    {
        var moves = new long[1];
        var counted = new CountingGame<ConnectFourGame, BitboardPosition, int>(
            ConnectFour, moves);
        int[] values = [.. MidGames.Select(opening => AlphaBeta.Value<
            CountingGame<ConnectFourGame, BitboardPosition, int>,
            OpenLinesEvaluation, TOrdering, BitboardPosition, int>(
            counted, Evaluation(), ordering, ConnectFour.Replay(opening), Depth))];

        return (moves[0], values);
    }
}
