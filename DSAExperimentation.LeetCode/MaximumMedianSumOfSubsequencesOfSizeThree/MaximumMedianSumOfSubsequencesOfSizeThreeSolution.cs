using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumMedianSumOfSubsequencesOfSizeThree;

// LeetCode 3627. Maximum Median Sum of Subsequences of Size 3: repeatedly pick
// any three remaining elements, take their median, remove them, until the
// array is empty; maximize the sum of medians collected. Both strategies
// answer the same question with the same signature (TwoSumSolution
// precedent).
internal static class MaximumMedianSumOfSubsequencesOfSizeThreeSolution
{
    // Textbook baseline: try every way to partition the array into groups of
    // three, recursing on what is left after each choice and taking the best
    // total - correct, and the arm the sorted-greedy strategy below has to
    // beat, but its search tree blows up combinatorially past a dozen or so
    // elements, unusable at the real problem's n up to 5*10^5.
    public static long MaximumMedianSumByBruteForce(int[] nums)
    {
        var used = new bool[nums.Length];

        return BestMedianSum(nums, used, nums.Length);
    }

    // Sorted ascending, the largest third of the array holds every eventual
    // median: pairing each of those with its two largest still-unclaimed
    // neighbors below it (one to its immediate left, the smallest of the
    // bottom two-thirds still spared for a later median) wastes nothing on
    // values too small to ever win a median contest. This repo's own
    // MergeSort.Sort<Element, TSequence> over an ArrayIndexedSequence (the
    // MatrixCellsInDistanceOrderBenchmarks precedent) does the ordering; the
    // greedy sum itself is then one linear pass, every other element from the
    // n/3 mark onward.
    public static long MaximumMedianSumBySortedGreedy(int[] nums)
    {
        var sorted = (int[])nums.Clone();

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        var sum = 0L;

        for (var i = sorted.Length / 3; i < sorted.Length; i += 2)
        {
            sum += sorted[i];
        }

        return sum;
    }

    private static long BestMedianSum(int[] nums, bool[] used, int remainingCount)
    {
        if (remainingCount == 0)
        {
            return 0;
        }

        var best = long.MinValue;

        for (var a = 0; a < nums.Length; a++)
        {
            if (used[a])
            {
                continue;
            }

            best = BestMedianSumWithFirst((nums, used), a, remainingCount, best);
        }

        return best;
    }

    // The `b` level of the enumeration, for one already-chosen first element: every unused
    // second element after `first` is tried, and the best total seen is carried in and out.
    private static long BestMedianSumWithFirst(
        (int[] Nums, bool[] Used) board, int first, int remainingCount, long best)
    {
        var (nums, used) = board;

        for (var b = first + 1; b < nums.Length; b++)
        {
            if (used[b])
            {
                continue;
            }

            for (var c = b + 1; c < nums.Length; c++)
            {
                if (used[c])
                {
                    continue;
                }

                best = BestMedianSumIncluding(board, (first, b, c), remainingCount, best);
            }
        }

        return best;
    }

    // The `c` level: one completed triple is claimed, scored by its median plus whatever the
    // rest of the array still yields, and released again, so the next triple sees the array
    // exactly as this one found it. `best` is the best total seen so far.
    private static long BestMedianSumIncluding(
        (int[] Nums, bool[] Used) board, (int First, int Second, int Third) triple, int remainingCount, long best)
    {
        var (nums, used) = board;
        var (a, b, c) = triple;
        used[a] = used[b] = used[c] = true;
        var candidate = MedianOfThree(nums[a], nums[b], nums[c]) + BestMedianSum(nums, used, remainingCount - 3);
        used[a] = used[b] = used[c] = false;

        return Math.Max(best, candidate);
    }

    // The middle of three values, computed without sorting: the sum less the largest and
    // the smallest leaves exactly the one in between.
    private static int MedianOfThree(int firstValue, int secondValue, int thirdValue)
    {
        var total = firstValue + secondValue + thirdValue;
        var high = Math.Max(secondValue, thirdValue);
        var largest = Math.Max(firstValue, high);
        var low = Math.Min(secondValue, thirdValue);
        var smallest = Math.Min(firstValue, low);

        return total - largest - smallest;
    }
}
