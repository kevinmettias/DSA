using DSAExperimentation.LeetCode.Solutions.DistributeCandiesAmongChildrenII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.DistributeCandiesAmongChildrenII;

public sealed partial class DistributeCandiesAmongChildrenIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(DistributeCandiesAmongChildrenIIReach.IsWithin(1, 12_345));

    [Fact]
    public void IsWithin_LimitPastTheReach_IsFalse() => Assert.False(DistributeCandiesAmongChildrenIIReach.IsWithin(1, 12_346));
}
