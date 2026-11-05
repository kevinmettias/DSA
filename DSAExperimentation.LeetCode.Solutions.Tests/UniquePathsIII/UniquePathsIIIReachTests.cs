using DSAExperimentation.LeetCode.Solutions.UniquePathsIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.UniquePathsIII;

public sealed partial class UniquePathsIIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(UniquePathsIIIReach.IsWithin([new int[28]]));

    [Fact]
    public void IsWithin_GridElementsPastTheReach_IsFalse() => Assert.False(UniquePathsIIIReach.IsWithin([new int[29]]));
}
