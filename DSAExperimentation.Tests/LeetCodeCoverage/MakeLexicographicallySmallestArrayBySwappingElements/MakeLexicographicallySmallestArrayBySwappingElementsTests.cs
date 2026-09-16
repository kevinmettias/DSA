using DSAExperimentation.LeetCode.MakeLexicographicallySmallestArrayBySwappingElements;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MakeLexicographicallySmallestArrayBySwappingElements;

// Harness only: both strategies live in
// MakeLexicographicallySmallestArrayBySwappingElementsSolution and are asserted
// against the same examples, including one where no pair is within `limit` of
// another and the array is already its own answer.
public sealed class MakeLexicographicallySmallestArrayBySwappingElementsTests
{
    public static TheoryData<int[], int, int[]> Examples =>
        new()
        {
            { [1, 5, 3, 9, 8], 2, [1, 3, 5, 8, 9] },
            { [1, 7, 6, 18, 2, 1], 3, [1, 6, 7, 18, 1, 2] },
            { [1, 7, 28, 19, 10], 3, [1, 7, 28, 19, 10] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LexicographicallySmallestArrayByContiguousGroups_LeetCodeExamples_ReturnsSmallestArray(
        int[] nums, int limit, int[] expected)
    {
        var actual = MakeLexicographicallySmallestArrayBySwappingElementsSolution
            .LexicographicallySmallestArrayByContiguousGroups(nums, limit);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void LexicographicallySmallestArrayByDisjointSet_LeetCodeExamples_ReturnsSmallestArray(
        int[] nums, int limit, int[] expected)
    {
        var actual = MakeLexicographicallySmallestArrayBySwappingElementsSolution
            .LexicographicallySmallestArrayByDisjointSet(nums, limit);

        Assert.Equal(expected, actual);
    }
}
