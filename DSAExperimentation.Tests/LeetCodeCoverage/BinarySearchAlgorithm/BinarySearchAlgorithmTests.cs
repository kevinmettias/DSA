using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.BinarySearchAlgorithm;

// LeetCode 704. Binary Search: a direct call into this repo's own BinarySearch.Find
// over an ArraySequence<int> - Find already returns null when target is absent,
// matching LC704's "-1 when missing" contract via a single null-coalescing
// translation at the call site. (Folder/namespace named "...BinarySearchAlgorithm"
// rather than the bare title, because a sibling namespace literally named
// "BinarySearch" would shadow the production DSAExperimentation.Algorithms.Searching.
// BinarySearch type for every other LeetCodeCoverage test file that imports it
// unqualified.)
public sealed partial class BinarySearchAlgorithmTests
{
    [Theory]
    [InlineData(new[] { -1, 0, 3, 5, 9, 12 }, 9, 4)]
    [InlineData(new[] { -1, 0, 3, 5, 9, 12 }, 2, -1)]
    public void Search_LeetCodeExamples_ReturnsIndexOrNegativeOne(int[] nums, int target, int expected)
    {
        var actual = Search(nums, target);
        Assert.Equal(expected, actual);
    }

    private static int Search(int[] nums, int target)
        => BinarySearch.Find(new ArraySequence<int>(nums), target) ?? -1;
}
