using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfMovesToKillAllPawns;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfMovesToKillAllPawns;

public sealed partial class MaximumNumberOfMovesToKillAllPawnsReachTests
{
    [Fact]
    public void IsWithin_PawnsAtTheReach_IsTrue() => Assert.True(MaximumNumberOfMovesToKillAllPawnsReach.IsWithin(0, 0, [[1, 2], [2, 4]]));

    [Fact]
    public void IsWithin_PawnsPastTheReach_IsFalse() =>
        Assert.False(MaximumNumberOfMovesToKillAllPawnsReach.IsWithin(0, 2, [[1, 1], [2, 2], [3, 3]]));
}
