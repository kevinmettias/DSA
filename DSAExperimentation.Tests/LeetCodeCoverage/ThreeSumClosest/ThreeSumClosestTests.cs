using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThreeSumClosest;

// LeetCode 16. 3Sum Closest: this is the same sorted-search shape as 3Sum,
// with MergeSort providing the ordering step over an ArrayIndexedSequence.
public sealed partial class ThreeSumClosestTests
{
    [Theory]
    [InlineData(new[] { -1, 2, 1, -4 }, 1, 2)]
    [InlineData(new[] { 0, 0, 0 }, 1, 0)]
    [InlineData(new[] { 1, 1, 1, 0 }, -100, 2)]
    public void ClosestSum_LeetCodeExamples_ReturnsNearestTripletSum(int[] nums, int target, int expected)
        => Assert.Equal(expected, ClosestSum(nums, target));

    private static int ClosestSum(int[] nums, int target)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var best = sorted[0] + sorted[1] + sorted[2];

        for (var i = 0; i < sorted.Length - 2; i++)
        {
            var left = i + 1;
            var right = sorted.Length - 1;

            while (left < right)
            {
                var sum = sorted[i] + sorted[left] + sorted[right];
                if (Math.Abs(target - sum) < Math.Abs(target - best))
                {
                    best = sum;
                }

                if (sum < target)
                {
                    left++;
                }
                else if (sum > target)
                {
                    right--;
                }
                else
                {
                    return target;
                }
            }
        }

        return best;
    }
}
