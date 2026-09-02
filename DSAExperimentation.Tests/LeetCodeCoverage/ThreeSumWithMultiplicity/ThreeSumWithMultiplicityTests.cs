using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ThreeSumWithMultiplicity;

// LeetCode 923. 3Sum With Multiplicity: sort with this repo's MergeSort over
// ArrayIndexedSequence (the same primitive ThreeSumTests already uses), then run
// the sorted two-pointer sweep counting INDEX triplets rather than collecting
// unique value triplets - when the two pointers land on equal values, every pair
// inside that equal-valued span forms a valid triplet with the fixed first
// element, so the span's C(span, 2) combinations are added directly instead of
// being visited one pair at a time.
public sealed partial class ThreeSumWithMultiplicityTests
{
    private const int Modulo = 1_000_000_007;

    [Fact]
    public void CountTriplets_ClassicExample_ReturnsTwentyMatchingTriplets()
    {
        int[] arr = [1, 1, 2, 2, 3, 3, 4, 4, 5, 5];

        var actual = CountTriplets(arr, target: 8);

        Assert.Equal(20, actual);
    }

    [Fact]
    public void CountTriplets_RepeatedValuesAcrossBothPointers_ReturnsTwelveMatchingTriplets()
    {
        int[] arr = [1, 1, 2, 2, 2, 2];

        var actual = CountTriplets(arr, target: 5);

        Assert.Equal(12, actual);
    }

    [Fact]
    public void CountTriplets_NoValidTriplet_ReturnsZero()
    {
        int[] arr = [0, 0, 0];

        var actual = CountTriplets(arr, target: 1);

        Assert.Equal(0, actual);
    }

    private static int CountTriplets(int[] arr, int target)
    {
        var sorted = arr.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        long count = 0;

        for (var i = 0; i < sorted.Length - 2; i++)
        {
            AccumulateTripletsForAnchor(sorted, i, target, ref count);
        }

        return (int)count;
    }

    private static void AccumulateTripletsForAnchor(int[] sorted, int anchorIndex, int target, ref long count)
    {
        var remaining = target - sorted[anchorIndex];
        var left = anchorIndex + 1;
        var right = sorted.Length - 1;

        while (left < right)
        {
            var pairSum = sorted[left] + sorted[right];

            if (pairSum < remaining)
            {
                left++;
            }
            else if (pairSum > remaining)
            {
                right--;
            }
            else if (sorted[left] != sorted[right])
            {
                AccumulateDistinctSpanPairs(sorted, ref left, ref right, ref count);
            }
            else
            {
                var span = right - left + 1;
                count = (count + ((long)span * (span - 1) / 2)) % Modulo;
                break;
            }
        }
    }

    private static void AccumulateDistinctSpanPairs(int[] sorted, ref int left, ref int right, ref long count)
    {
        var leftCount = 1;
        while (left + 1 < right && sorted[left + 1] == sorted[left])
        {
            leftCount++;
            left++;
        }

        var rightCount = 1;
        while (right - 1 > left && sorted[right - 1] == sorted[right])
        {
            rightCount++;
            right--;
        }

        count = (count + ((long)leftCount * rightCount)) % Modulo;
        left++;
        right--;
    }
}
