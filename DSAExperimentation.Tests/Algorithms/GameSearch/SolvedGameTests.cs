using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Domain.Mathematics.GameTheory;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// What a solved game reports, read from games solved by RetrogradeAnalysis whose lengths of best
// play are known: in the take-away game taking one to three, every two moves take four stones
// however the loser plays, so a multiple of four lasts half its stones and any other pile one move
// more than the multiple of four below it.
public sealed partial class SolvedGameTests
{
    private const int MostPerTurn = 3;
    private const int LargestPile = 40;
    private const int MovesPerRound = 2;

    // Vertex 7 can only hand the opponent a win or go round the cycle, so it draws.
    private const int LoopyDrawnVertex = 7;

    // Each decided vertex of TokenGraphs.Loopy with how long best play lasts from it, worked out by
    // hand: 5 loses in two, its only move handing the opponent 4, a win in one.
    public static TheoryData<int, int> LoopyMovesToEnd => new()
    {
        { 3, 0 },
        { 4, 1 },
        { 5, 2 },
        { 6, 3 },
        { 10, 2 },
        { 11, 1 },
    };

    // A path of one vertex, over before any move, under misere play: the player stuck wins.
    [Fact]
    public void OutcomeOf_PositionOverAtOnce_IsWhatTheRulesSay()
    {
        var game = TokenGraphs.Path(1, GameOutcome.Win);
        var solved = RetrogradeAnalysis.Solve<TokenGraphGame, int>(game);

        Assert.Equal(GameOutcome.Win, solved.OutcomeOf(0));
    }

    [Fact]
    public void MovesToEnd_TakeAway_IsAsLongAsTheLoserCanHoldOut()
    {
        var game = TokenGraphs.TakeAway(MostPerTurn, LargestPile);
        var solved = RetrogradeAnalysis.Solve<TokenGraphGame, int>(game);

        Assert.All(
            Enumerable.Range(0, LargestPile + 1),
            pile => Assert.Equal(ExpectedTakeAwayMovesToEnd(pile), solved.MovesToEnd(pile)));
    }

    [Theory]
    [MemberData(nameof(LoopyMovesToEnd))]
    public void MovesToEnd_LoopyGraph_IsTheLengthOfBestPlay(int vertex, int expected)
    {
        var solved = RetrogradeAnalysis.Solve<TokenGraphGame, int>(TokenGraphs.Loopy);

        Assert.Equal(expected, solved.MovesToEnd(vertex));
    }

    [Fact]
    public void MovesToEnd_DrawnPosition_IsZero()
    {
        var solved = RetrogradeAnalysis.Solve<TokenGraphGame, int>(TokenGraphs.Loopy);

        Assert.Equal(0, solved.MovesToEnd(LoopyDrawnVertex));
    }

    // The loser's piles are the multiples of four; the winner's first move takes the rest.
    private static int ExpectedTakeAwayMovesToEnd(int pile)
    {
        var round = MostPerTurn + 1;
        var roundMoves = pile / round * MovesPerRound;

        if (pile % round == 0)
        {
            return roundMoves;
        }

        return roundMoves + 1;
    }
}
