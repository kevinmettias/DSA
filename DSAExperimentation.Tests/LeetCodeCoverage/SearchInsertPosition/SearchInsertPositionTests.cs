using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SearchInsertPosition;

public sealed partial class SearchInsertPositionTests
{
    [Theory]
    [InlineData(new[] { 1, 3, 5, 6 }, 5, 2)]
    [InlineData(new[] { 1, 3, 5, 6 }, 2, 1)]
    [InlineData(new[] { 1, 3, 5, 6 }, 7, 4)]
    public void SearchInsert_LeetCodeExamples_ReturnsLowerBoundIndex(int[] nums, int target, int expected)
        => Assert.Equal(expected, BinarySearch.LowerBound(new ArraySequence<int>(nums), target));
}
