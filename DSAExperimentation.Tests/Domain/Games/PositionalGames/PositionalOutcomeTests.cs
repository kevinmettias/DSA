using DSAExperimentation.Domain.Games.PositionalGames;

namespace DSAExperimentation.Tests.Domain.Games.PositionalGames;

public sealed partial class PositionalOutcomeTests
{
    private const int Seat = 2;

    [Fact]
    public void Pending_IsNotOverAndHasNoWinner() =>
        Assert.Equal((false, PositionalOutcome.NoWinner), Read(PositionalOutcome.Pending));

    [Fact]
    public void Drawn_IsOverWithNoWinner() =>
        Assert.Equal((true, PositionalOutcome.NoWinner), Read(PositionalOutcome.Drawn));

    [Fact]
    public void WonBy_IsOverWithThatSeatTheWinner() =>
        Assert.Equal((true, Seat), Read(PositionalOutcome.WonBy(Seat)));

    private static (bool IsOver, int Winner) Read(PositionalOutcome outcome) =>
        (outcome.IsOver, outcome.Winner);
}
