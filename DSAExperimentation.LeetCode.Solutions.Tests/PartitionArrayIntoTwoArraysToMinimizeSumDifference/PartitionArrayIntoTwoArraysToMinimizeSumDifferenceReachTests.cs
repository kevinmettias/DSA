using DSAExperimentation.LeetCode.Solutions.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PartitionArrayIntoTwoArraysToMinimizeSumDifference;

public sealed partial class PartitionArrayIntoTwoArraysToMinimizeSumDifferenceReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(PartitionArrayIntoTwoArraysToMinimizeSumDifferenceReach.IsWithin(new int[18]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(PartitionArrayIntoTwoArraysToMinimizeSumDifferenceReach.IsWithin(new int[19]));
}
