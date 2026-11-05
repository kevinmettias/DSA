using DSAExperimentation.LeetCode.Solutions.MaximumAndSumOfArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumAndSumOfArray;

public sealed partial class MaximumAndSumOfArrayReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MaximumAndSumOfArrayReach.IsWithin(new int[9], 7));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(MaximumAndSumOfArrayReach.IsWithin(new int[10], 7));

    [Fact]
    public void IsWithin_NumSlotsPastTheReach_IsFalse() => Assert.False(MaximumAndSumOfArrayReach.IsWithin(new int[9], 8));
}
