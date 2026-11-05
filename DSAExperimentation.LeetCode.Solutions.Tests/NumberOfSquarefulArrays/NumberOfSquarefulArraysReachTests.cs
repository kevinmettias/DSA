using DSAExperimentation.LeetCode.Solutions.NumberOfSquarefulArrays;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfSquarefulArrays;

public sealed partial class NumberOfSquarefulArraysReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NumberOfSquarefulArraysReach.IsWithin(new int[7]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(NumberOfSquarefulArraysReach.IsWithin(new int[8]));
}
