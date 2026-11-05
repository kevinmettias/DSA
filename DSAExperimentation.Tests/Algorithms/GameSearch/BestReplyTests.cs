using DSAExperimentation.Algorithms.GameSearch;
using DSAExperimentation.Tests.Algorithms.GameSearch.Fixtures;

namespace DSAExperimentation.Tests.Algorithms.GameSearch;

// Companion of BestReply, the recurrence Negamax hands Memoizer: a finished position is worth what
// its rules say, and any other the best gain less the reply. It is private to Negamax, so it is
// driven through Negamax.Value.
public sealed partial class BestReplyTests
{
    private const int MostPerTurn = 3;

    [Fact]
    public void Replay_FinishedPosition_IsWorthWhatItsRulesSay() =>
        Assert.Equal(-1, Negamax.Value<TakeAwayGame, int, int>(new TakeAwayGame(MostPerTurn), 0));

    // One stone: taking it leaves the opponent an empty pile, worth -1 to them, so +1 to the mover.
    [Fact]
    public void Replay_OneMoveToAFinishedPosition_IsTheNegatedReply() =>
        Assert.Equal(1, Negamax.Value<TakeAwayGame, int, int>(new TakeAwayGame(MostPerTurn), 1));
}
