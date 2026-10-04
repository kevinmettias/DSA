using DSAExperimentation.LeetCode.NumberOfSetsOfKNonOverlappingLineSegments;

namespace DSAExperimentation.LeetCode.Tests.NumberOfSetsOfKNonOverlappingLineSegments;

// Harness only. Both strategies are
// NumberOfSetsOfKNonOverlappingLineSegmentsSolution's - this file just pins them to
// LeetCode's published examples plus the two boundary shapes the
// segmentCount > pointCount guard exists for: segmentCount segments seated in
// exactly segmentCount+1 points by touching, and segmentCount segments that cannot
// be seated at all.
public sealed partial class NumberOfSetsOfKNonOverlappingLineSegmentsSolutionTests
{
    public static TheoryData<int, int, int> Examples =>
        new()
        {
            // LeetCode examples 1-3.
            { 4, 2, 5 },
            { 3, 1, 3 },
            { 30, 7, 796_297_179 },

            // k segments over n points number C(n + k - 1, 2k): k - 1 extra points stand
            // in for the endpoints touching segments may share, and a set is then any 2k
            // distinct endpoints, paired off left to right. So C(7, 6) = 7, C(2, 2) = 1,
            // C(6, 4) = 15, C(7, 4) = 35, C(8, 6) = 28, C(0, 0) = 1 for no segments over
            // one point, C(4, 4) = 1 for two touching segments over three points, and
            // C(3, 4) = 0 for two segments that two points cannot seat.
            { 5, 3, 7 },
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
        int pointCount, int segmentCount, int expected)
    {
        var actual = NumberOfSetsOfKNonOverlappingLineSegmentsSolution.NumberOfSetsByTabulation(pointCount, segmentCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void NumberOfSetsByMemoizedPascal_LeetCodeExamples_ReturnsNonOverlappingSegmentSetCount(
        int pointCount, int segmentCount, int expected)
    {
        var actual = NumberOfSetsOfKNonOverlappingLineSegmentsSolution.NumberOfSetsByMemoizedPascal(pointCount, segmentCount);

        Assert.Equal(expected, actual);
    }
}
