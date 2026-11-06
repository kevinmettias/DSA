using DSAExperimentation.Domain.Games.Turns;

namespace DSAExperimentation.Tests.Domain.Games.Turns;

// The count is the fact; RoundRobinTests and SimultaneousTests read it through the turn orders.
public sealed partial class TwoPlayersTests
{
    [Fact]
    public void Players_IsTwo() => Assert.Equal(2, TwoPlayers.Players);
}
