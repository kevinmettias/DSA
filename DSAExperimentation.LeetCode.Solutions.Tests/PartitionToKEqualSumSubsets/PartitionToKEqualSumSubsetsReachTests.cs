using DSAExperimentation.LeetCode.Solutions.PartitionToKEqualSumSubsets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.PartitionToKEqualSumSubsets;

public sealed partial class PartitionToKEqualSumSubsetsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(PartitionToKEqualSumSubsetsReach.IsWithin(new int[15], 4));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(PartitionToKEqualSumSubsetsReach.IsWithin(new int[16], 4));

    [Fact]
    public void IsWithin_BucketCountPastTheReach_IsFalse() => Assert.False(PartitionToKEqualSumSubsetsReach.IsWithin(new int[15], 5));
}
