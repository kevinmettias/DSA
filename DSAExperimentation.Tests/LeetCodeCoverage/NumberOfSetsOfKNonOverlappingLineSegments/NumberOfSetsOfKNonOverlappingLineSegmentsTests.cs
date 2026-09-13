using DSAExperimentation.LeetCode.NumberOfSetsOfKNonOverlappingLineSegments;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfSetsOfKNonOverlappingLineSegments;

// Harness only. Both strategies are
// NumberOfSetsOfKNonOverlappingLineSegmentsSolution's - this file just pins them to
// LeetCode's published examples plus the two boundary shapes the k > n guard
// exists for: k segments seated in exactly k+1 points by touching, and k segments
// that cannot be seated at all.
public sealed class NumberOfSetsOfKNonOverlappingLineSegmentsTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            { 4, 2, 5 },
            { 5, 3, 7 },
            { 3, 1, 3 },
            { 2, 1, 1 },
            { 5, 2, 15 },
            { 6, 2, 35 },
            { 6, 3, 28 },
            { 1, 0, 1 },
            { 3, 2, 1 },
            { 2, 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfSetsByTabulation_LeetCodeExamples_ReturnsNonOverlappingSegmentSetCount(
        int n, int k, int expected) =>
        Assert.Equal(expected, NumberOfSetsOfKNonOverlappingLineSegmentsSolution.NumberOfSetsByTabulation(n, k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfSetsByMemoizedPascal_LeetCodeExamples_ReturnsNonOverlappingSegmentSetCount(
        int n, int k, int expected) =>
        Assert.Equal(expected, NumberOfSetsOfKNonOverlappingLineSegmentsSolution.NumberOfSetsByMemoizedPascal(n, k));
}
