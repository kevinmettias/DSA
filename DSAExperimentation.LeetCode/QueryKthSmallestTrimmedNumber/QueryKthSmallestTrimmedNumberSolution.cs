using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.QueryKthSmallestTrimmedNumber;

// LeetCode 2343. Query Kth Smallest Trimmed Number: for each query [k, trim], trim every
// number to its last `trim` digits and report which ORIGINAL index lands at position k in
// trimmed order. All numbers are the same length, so comparing equal-length digit suffixes
// lexicographically is the same order as comparing them numerically, and ties resolve toward
// the smaller original index - which is what LeetCode's own definition of "smallest trimmed
// number" requires.
//
// Both strategies answer every query independently and share one allocation-free trimmed
// comparison; they differ only in how they locate position k.
internal static class QueryKthSmallestTrimmedNumberSolution
{
    // Layout of a single LeetCode query: [k, trim].
    private const int QueryK = 0;
    private const int QueryTrim = 1;

    // k is 1-based in the problem statement; the sorted array is not.
    private const int FirstRank = 1;

    // The textbook approach before reaching for a sort: k rounds of "scan every remaining
    // number for the smallest trimmed suffix and take it", O(n*k) per query. Pure BCL - a
    // bool[] of what has already been taken - since this is the baseline the sorted strategy
    // is measured against. Scanning ascending and replacing only on a strictly smaller
    // suffix keeps the tie rule: the smaller original index wins.
    public static int[] AnswerQueriesBySelectionScan(string[] nums, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            var query = queries[q];
            answers[q] = SelectionScan(nums, query[QueryK], query[QueryTrim]);
        }

        return answers;
    }

    private static int SelectionScan(string[] nums, int k, int trim)
    {
        var taken = new bool[nums.Length];
        var answer = -1;

        for (var round = 0; round < k; round++)
        {
            answer = TakeSmallestRemaining(nums, taken, trim);
        }

        return answer;
    }

    // Marks and returns the untaken index whose trimmed suffix is smallest,
    // scanning ascending and replacing only on a strictly smaller suffix so the
    // smaller original index keeps the tie.
    private static int TakeSmallestRemaining(string[] nums, bool[] taken, int trim)
    {
        var bestIndex = -1;

        for (var i = 0; i < nums.Length; i++)
        {
            if (taken[i])
            {
                continue;
            }

            if (bestIndex == -1 || CompareTrimmed(nums[i], nums[bestIndex], trim) < 0)
            {
                bestIndex = i;
            }
        }

        taken[bestIndex] = true;

        return bestIndex;
    }

    // Sort an index array once per query with this repo's own MergeSort over
    // ArrayIndexedSequence<int>, ordered by each index's trimmed suffix, then read off
    // position k directly - O(n log n) per query. MergeSort's merge step is documented
    // stable (its `<= 0` tie rule), so equal trimmed values keep their original relative
    // order and the tie rule needs no extra bookkeeping.
    public static int[] AnswerQueriesByMergeSort(string[] nums, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var q = 0; q < queries.Length; q++)
        {
            var query = queries[q];
            answers[q] = MergeSortedIndex(nums, query[QueryK], query[QueryTrim]);
        }

        return answers;
    }

    private static int MergeSortedIndex(string[] nums, int k, int trim)
    {
        var indices = Enumerable.Range(0, nums.Length).ToArray();
        var comparer = Comparer<int>.Create((a, b) => CompareTrimmed(nums[a], nums[b], trim));

        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(indices), comparer);

        return indices[k - FirstRank];
    }

    // Compares the last `trim` characters of two equal-length digit strings without
    // allocating the suffixes.
    private static int CompareTrimmed(string first, string second, int trim)
    {
        var startFirst = first.Length - trim;
        var startSecond = second.Length - trim;

        for (var i = 0; i < trim; i++)
        {
            var comparison = first[startFirst + i].CompareTo(second[startSecond + i]);
            if (comparison != 0)
            {
                return comparison;
            }
        }

        return 0;
    }
}
