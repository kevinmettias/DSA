using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestPerimeterTriangle;

// LeetCode 976. Largest Perimeter Triangle: this repo's own
// MergeSort.Sort<int,ArrayIndexedSequence<int>> (SortAnArray precedent) sorts the
// side lengths ascending, then a single backward scan checks each consecutive
// triple against the triangle inequality - the two largest remaining sides always
// beat any smaller pair for a fixed third side, so the first triple (from the top)
// that satisfies a[i-2] + a[i-1] > a[i] is provably the maximum-perimeter answer.
public sealed partial class LargestPerimeterTriangleTests
{
    [Fact]
    public void LargestPerimeter_ClassicExample_ReturnsSumOfValidTriple()
    {
        int[] nums = [2, 1, 2];

        var result = LargestPerimeter(nums);

        Assert.Equal(5, result);
    }

    [Fact]
    public void LargestPerimeter_NoValidTriangleExists_ReturnsZero()
    {
        int[] nums = [1, 2, 1, 10];

        var result = LargestPerimeter(nums);

        Assert.Equal(0, result);
    }

    [Fact]
    public void LargestPerimeter_PicksLargestValidTripleOverSmallerOnes()
    {
        int[] nums = [1, 2, 1, 10, 6, 5];

        var result = LargestPerimeter(nums);

        Assert.Equal(21, result);
    }

    private static int LargestPerimeter(int[] nums)
    {
        var sorted = (int[])nums.Clone();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        for (var i = sorted.Length - 1; i >= 2; i--)
        {
            if (sorted[i - 2] + sorted[i - 1] > sorted[i])
            {
                return sorted[i - 2] + sorted[i - 1] + sorted[i];
            }
        }

        return 0;
    }
}
