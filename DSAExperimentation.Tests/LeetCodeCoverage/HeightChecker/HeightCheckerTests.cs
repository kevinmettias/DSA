using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.HeightChecker;

// LeetCode 1051. Height Checker: sort a copy with this repo's MergeSort over
// ArrayIndexedSequence (same composition ArrayPartitionTests uses), then count
// the positions where that "expected" non-decreasing order differs from the
// original.
public sealed partial class HeightCheckerTests
{
    [Theory]
    [InlineData(new[] { 1, 1, 4, 2, 1, 3 }, 3)]
    [InlineData(new[] { 5, 1, 2, 3, 4 }, 5)]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, 0)]
    public void HeightChecker_LeetCodeExamples_ReturnsMismatchCount(int[] heights, int expected)
        => Assert.Equal(expected, HeightChecker(heights));

    private static int HeightChecker(int[] heights)
    {
        var expected = heights.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(expected));

        var mismatches = 0;
        for (var i = 0; i < heights.Length; i++)
        {
            if (heights[i] != expected[i])
            {
                mismatches++;
            }
        }

        return mismatches;
    }
}
