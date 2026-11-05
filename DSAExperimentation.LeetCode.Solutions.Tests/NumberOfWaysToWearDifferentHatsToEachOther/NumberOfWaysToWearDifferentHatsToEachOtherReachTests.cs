using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToWearDifferentHatsToEachOther;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToWearDifferentHatsToEachOther;

public sealed partial class NumberOfWaysToWearDifferentHatsToEachOtherReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NumberOfWaysToWearDifferentHatsToEachOtherReach.IsWithin([new int[50]]));

    [Fact]
    public void IsWithin_HatsLengthPastTheReach_IsFalse() => Assert.False(NumberOfWaysToWearDifferentHatsToEachOtherReach.IsWithin(Enumerable.Repeat(Array.Empty<int>(), 9).ToArray()));

    [Fact]
    public void IsWithin_HatsElementsPastTheReach_IsFalse() => Assert.False(NumberOfWaysToWearDifferentHatsToEachOtherReach.IsWithin([new int[51]]));
}
