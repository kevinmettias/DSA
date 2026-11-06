using DSAExperimentation.Domain.Games.Turns;

namespace DSAExperimentation.Tests.Domain.Games.Turns;

// The count is the fact; RoundRobinTests and SimultaneousTests read it through the turn orders.
public sealed partial class FourPlayersTests
{
    [Fact]
    public void Players_IsFour() => Assert.Equal(4, FourPlayers.Players);
}
