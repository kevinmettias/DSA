using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;
using DSAExperimentation.Domain.Modular;

namespace DSAExperimentation.LeetCode.ThreeSumWithMultiplicity;

// LeetCode 923. 3Sum With Multiplicity: how many INDEX triplets i < j < k satisfy
// arr[i] + arr[j] + arr[k] == target, reported modulo 1e9+7.
//
// This is 3Sum (LC 15) with the opposite de-duplication rule: LC 15 collects each
// distinct value triplet once, LC 923 counts every index triplet, so repeated
// values multiply rather than collapse. Both strategies below answer that count;
// they differ only in whether they visit every triplet or count each equal-valued
// span's combinations in closed form.
internal static class ThreeSumWithMultiplicitySolution
{
    // Need at least 2 more elements after the anchor for a triple.
    private const int RemainingPairSize = 2;

    // Binomial "n choose 2" = n * (n - 1) / 2.
    private const int ChooseTwoDivisor = 2;

    // The textbook baseline: the cubic triple loop, counting every matching index
    // triplet one at a time. Deliberately written without this repo's primitives -
    // it is the arm the composed solution below has to justify itself against.
    public static int CountTripletsByBruteForce(int[] arr, int target)
    {
        long count = 0;

        for (var i = 0; i < arr.Length - RemainingPairSize; i++)
        {
            for (var j = i + 1; j < arr.Length - 1; j++)
            {
                count += CountThirdIndexMatches(arr, i, j, target);
            }
        }

        return (int)(count % ModularArithmetic.Modulo);
    }

    private static long CountThirdIndexMatches(int[] arr, int firstIndex, int secondIndex, int target)
    {
        long count = 0;

        for (var k = secondIndex + 1; k < arr.Length; k++)
        {
            if (arr[firstIndex] + arr[secondIndex] + arr[k] == target)
            {
                count++;
            }
        }

        return count;
    }

    // Sort with this repo's own MergeSort over ArrayIndexedSequence (the same
    // primitive ThreeSumSolution uses for LC 15), then run the sorted two-pointer
    // sweep per anchor. When the two pointers land on equal values, every pair
    // inside that equal-valued span forms a valid triplet with the anchor, so the
    // span's C(span, 2) combinations are added directly instead of being visited
    // one pair at a time - that closed form is what keeps multiplicity from
    // degenerating back to the cubic walk.
    public static int CountTripletsByMergeSortTwoPointers(int[] arr, int target)
    {
        // The input is sorted in place, so the caller's array is left untouched -
        // which also keeps repeated benchmark invocations measuring the same work.
        var sorted = arr.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        long count = 0;

        for (var i = 0; i < sorted.Length - RemainingPairSize; i++)
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
                count = (count + ((long)span * (span - 1) / ChooseTwoDivisor)) % ModularArithmetic.Modulo;
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

        count = (count + ((long)leftCount * rightCount)) % ModularArithmetic.Modulo;
        left++;
        right--;
    }
}
