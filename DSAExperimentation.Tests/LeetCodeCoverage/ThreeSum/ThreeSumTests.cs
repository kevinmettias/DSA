using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThreeSum;

// LeetCode 15. 3Sum: sort with this repo's MergeSort over ArrayIndexedSequence,
// then use the usual linear two-pointer sweep over each fixed first element.
public sealed partial class ThreeSumTests
{
    [Fact]
    public void FindTriplets_ClassicExample_ReturnsUniqueZeroSumTriplets()
    {
        int[] nums = [-1, 0, 1, 2, -1, -4];

        var triplets = FindTriplets(nums);

        Assert.Equal(2, triplets.Count);
        Assert.Contains((-1, -1, 2), triplets);
        Assert.Contains((-1, 0, 1), triplets);
    }

    [Fact]
    public void FindTriplets_AllZeroes_ReturnsSingleTriplet()
    {
        int[] nums = [0, 0, 0, 0];

        var triplets = FindTriplets(nums);

        Assert.Equal([new ValueTuple<int, int, int>(0, 0, 0)], triplets);
    }

    private static List<(int First, int Second, int Third)> FindTriplets(int[] nums)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var results = new List<(int First, int Second, int Third)>();

        for (var i = 0; i < sorted.Length - 2; i++)
        {
            if (i > 0 && sorted[i] == sorted[i - 1])
            {
                continue;
            }

            var left = i + 1;
            var right = sorted.Length - 1;

            while (left < right)
            {
                var sum = sorted[i] + sorted[left] + sorted[right];
                if (sum == 0)
                {
                    results.Add((sorted[i], sorted[left], sorted[right]));
                    left++;
                    right--;

                    while (left < right && sorted[left] == sorted[left - 1])
                    {
                        left++;
                    }

                    while (left < right && sorted[right] == sorted[right + 1])
                    {
                        right--;
                    }
                }
                else if (sum < 0)
                {
                    left++;
                }
                else
                {
                    right--;
                }
            }
        }

        return results;
    }
}
