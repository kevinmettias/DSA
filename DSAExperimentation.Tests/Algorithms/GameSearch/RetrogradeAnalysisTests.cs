using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Domain.Mathematics.GameTheory;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// Retrograde analysis against games whose every position is known: a take-away game, which never
// returns to a position and so is decided everywhere; a token graph drawn by hand, whose cycles
// leave some positions drawn; and a path under each outcome the rules may give a stuck player.
public sealed partial class RetrogradeAnalysisTests
{
    private const int MostPerTurn = 3;
    private const int LargestPile = 40;
    private const int PathLength = 3;

    // Each vertex of TokenGraphs.Loopy with its outcome, worked out by hand.
    public static TheoryData<int, GameOutcome> LoopyOutcomes => new()
    {
        { 0, GameOutcome.Draw },
        { 1, GameOutcome.Draw },
        { 2, GameOutcome.Draw },
        { 3, GameOutcome.Loss },
        { 4, GameOutcome.Win },
        { 5, GameOutcome.Loss },
        { 6, GameOutcome.Win },
        { 7, GameOutcome.Draw },
        { 8, GameOutcome.Draw },
        { 9, GameOutcome.Draw },
        { 10, GameOutcome.Loss },
        { 11, GameOutcome.Win },
    };

    // A path's outcomes, first vertex first, when the player stuck at its end loses, wins or draws.
    public static TheoryData<GameOutcome, GameOutcome[]> PathOutcomes => new()
    {
        { GameOutcome.Loss, [GameOutcome.Loss, GameOutcome.Win, GameOutcome.Loss] },
        { GameOutcome.Win, [GameOutcome.Win, GameOutcome.Loss, GameOutcome.Win] },
        { GameOutcome.Draw, [GameOutcome.Draw, GameOutcome.Draw, GameOutcome.Draw] },
    };

    [Fact]
    public void Solve_TakeAway_LosesExactlyOnMultiplesOfFour()
    {
        var game = TokenGraphs.TakeAway(MostPerTurn, LargestPile);
        var solved = RetrogradeAnalysis.Solve<TokenGraphGame, int>(game);

        Assert.All(
            Enumerable.Range(0, LargestPile + 1),
            pile => Assert.Equal(ExpectedTakeAwayOutcome(pile), solved.OutcomeOf(pile)));
    }

    [Theory]
    [MemberData(nameof(LoopyOutcomes))]
    public void Solve_LoopyGraph_DrawsWhereNeitherPlayerCanForceAnEnd(
        int vertex, GameOutcome expected)
    {
        var solved = RetrogradeAnalysis.Solve<TokenGraphGame, int>(TokenGraphs.Loopy);

        Assert.Equal(expected, solved.OutcomeOf(vertex));
    }

    [Theory]
    [MemberData(nameof(PathOutcomes))]
    public void Solve_Path_AlternatesFromTheStuckPlayersOutcome(
        GameOutcome whenStuck, GameOutcome[] expected)
    {
        var game = TokenGraphs.Path(PathLength, whenStuck);
        var solved = RetrogradeAnalysis.Solve<TokenGraphGame, int>(game);

        Assert.Equal(expected, Enumerable.Range(0, PathLength).Select(solved.OutcomeOf));
    }

    // A pile that is a multiple of four loses, any other wins: a game that ends has no draw.
    private static GameOutcome ExpectedTakeAwayOutcome(int pile)
    {
        var losing = pile % (MostPerTurn + 1) == 0;

        return losing ? GameOutcome.Loss : GameOutcome.Win;
    }
}
