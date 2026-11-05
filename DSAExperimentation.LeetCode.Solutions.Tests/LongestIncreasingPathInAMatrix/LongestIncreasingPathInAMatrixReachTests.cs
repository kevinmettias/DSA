using DSAExperimentation.LeetCode.Solutions.LongestIncreasingPathInAMatrix;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LongestIncreasingPathInAMatrix;

public sealed partial class LongestIncreasingPathInAMatrixReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(LongestIncreasingPathInAMatrixReach.IsWithin(new int[5, 10]));

    [Fact]
    public void IsWithin_CellCountPastTheReach_IsFalse() => Assert.False(LongestIncreasingPathInAMatrixReach.IsWithin(new int[3, 17]));
}
