using DSAExperimentation.LeetCode.RemoveBoxes;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RemoveBoxes;

// Harness only: the algorithms live in RemoveBoxesSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed partial class RemoveBoxesTests
{
    public static TheoryData<int[], int> Examples =>
        new()
        {
            { [1, 3, 2, 2, 2, 3, 4, 3, 1], 23 },
            { [1, 1, 1], 9 },
            { [1], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPointsByUnmemoizedRecursion_LeetCodeExamples_ReturnsMaximumPoints(int[] boxes, int expected) =>
        Assert.Equal(expected, RemoveBoxesSolution.MaxPointsByUnmemoizedRecursion(boxes));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxPointsByMemoizedRecursion_LeetCodeExamples_ReturnsMaximumPoints(int[] boxes, int expected) =>
        Assert.Equal(expected, RemoveBoxesSolution.MaxPointsByMemoizedRecursion(boxes));
}
