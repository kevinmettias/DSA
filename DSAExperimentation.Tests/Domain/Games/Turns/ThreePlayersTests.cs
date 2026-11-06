using DSAExperimentation.Domain.Games.Turns;

namespace DSAExperimentation.Tests.Domain.Games.Turns;

// The count is the fact; RoundRobinTests and SimultaneousTests read it through the turn orders.
public sealed partial class ThreePlayersTests
{
    [Fact]
    public void Players_IsThree() => Assert.Equal(3, ThreePlayers.Players);
}
