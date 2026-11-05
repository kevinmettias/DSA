using DSAExperimentation.LeetCode.Solutions.NumberOfSetsOfKNonOverlappingLineSegments;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfSetsOfKNonOverlappingLineSegments;

public sealed partial class NumberOfSetsOfKNonOverlappingLineSegmentsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NumberOfSetsOfKNonOverlappingLineSegmentsReach.IsWithin(700, 1));

    [Fact]
    public void IsWithin_PointCountPastTheReach_IsFalse() => Assert.False(NumberOfSetsOfKNonOverlappingLineSegmentsReach.IsWithin(701, 1));
}
