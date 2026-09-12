using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.ArrayPartition;

// LeetCode 561. Array Partition: split 2n numbers into n pairs and maximize the
// sum of each pair's minimum. Sorting first and summing every even-indexed
// element always achieves this - pairing each element with its immediate
// neighbor once sorted never wastes more than the unavoidable one element per
// pair.
internal static class ArrayPartitionSolution
{
    // Every pair contributes its smaller half; fixed by the problem itself.
    private const int PairSize = 2;

    // The textbook O(n^2) approach: repeatedly scan for the two smallest
    // remaining elements and take the smaller of each pair. Pure BCL - the
    // baseline the sorted approach is measured against.
    public static int MaxSumByRepeatedSmallestPairScan(int[] nums)
    {
        var used = new bool[nums.Length];
        var sum = 0;

        for (var pair = 0; pair < nums.Length / PairSize; pair++)
        {
            var (firstIndex, secondIndex) = FindTwoSmallestUnusedIndices(nums, used);
            used[firstIndex] = true;
            used[secondIndex] = true;
            sum += nums[firstIndex];
        }

        return sum;
    }

    private static (int FirstIndex, int SecondIndex) FindTwoSmallestUnusedIndices(int[] nums, bool[] used)
    {
        var firstIndex = -1;
        var secondIndex = -1;

        for (var i = 0; i < nums.Length; i++)
        {
            if (used[i])
            {
                continue;
            }

            if (firstIndex < 0 || nums[i] < nums[firstIndex])
            {
                secondIndex = firstIndex;
                firstIndex = i;
            }
            else if (secondIndex < 0 || nums[i] < nums[secondIndex])
            {
                secondIndex = i;
            }
        }

        return (firstIndex, secondIndex);
    }

    // Sort with this repo's MergeSort over ArrayIndexedSequence, then sum every
    // even-indexed element of the sorted array - the pair-minimum once sorted.
    public static int MaxSumByMergeSort(int[] nums)
    {
        var sorted = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var sum = 0;
        for (var i = 0; i < sorted.Length; i += PairSize)
        {
            sum += sorted[i];
        }

        return sum;
    }
}
