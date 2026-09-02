using DSAExperimentation.LeetCode.FindMaximumAreaOfATriangle;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindMaximumAreaOfATriangle;

// Harness only: both strategies are FindMaximumAreaOfATriangleSolution's - this
// file just pins them to LeetCode's published examples (OpenTheLockTests
// precedent).
public sealed class FindMaximumAreaOfATriangleTests
{
    public static TheoryData<int[][], long> Examples =>
        new()
        {
            { [[1, 1], [1, 2], [3, 2], [3, 3]], 2 },
            { [[1, 1], [2, 2], [3, 3]], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAreaByBruteForce_LeetCodeExamples_ReturnsTwiceMaxArea(int[][] coords, long expected) =>
        Assert.Equal(expected, FindMaximumAreaOfATriangleSolution.MaxAreaByBruteForce(coords));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxAreaBySpreadHashMap_LeetCodeExamples_ReturnsTwiceMaxArea(int[][] coords, long expected) =>
        Assert.Equal(expected, FindMaximumAreaOfATriangleSolution.MaxAreaBySpreadHashMap(coords));
}
