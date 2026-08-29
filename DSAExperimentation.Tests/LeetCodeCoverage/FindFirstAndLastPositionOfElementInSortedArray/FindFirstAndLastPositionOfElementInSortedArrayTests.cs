using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindFirstAndLastPositionOfElementInSortedArray;

public sealed partial class FindFirstAndLastPositionOfElementInSortedArrayTests
{
    [Theory]
    [InlineData(new[] { 5, 7, 7, 8, 8, 10 }, 8, new[] { 3, 4 })]
    [InlineData(new[] { 5, 7, 7, 8, 8, 10 }, 6, new[] { -1, -1 })]
    [InlineData(new int[] { }, 0, new[] { -1, -1 })]
    public void SearchRange_LeetCodeExamples_ReturnsClosedRange(int[] nums, int target, int[] expected)
        => Assert.Equal(expected, SearchRange(nums, target));

    private static int[] SearchRange(int[] nums, int target)
    {
        var sequence = new ArraySequence<int>(nums);
        var lower = BinarySearch.LowerBound(sequence, target);
        var upper = BinarySearch.UpperBound(sequence, target);
        return lower == upper ? [-1, -1] : [lower, upper - 1];
    }
}
