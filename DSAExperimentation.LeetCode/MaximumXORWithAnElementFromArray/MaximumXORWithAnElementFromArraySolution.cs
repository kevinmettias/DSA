using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.MaximumXORWithAnElementFromArray;

// LeetCode 1707. Maximum XOR With an Element From Array: each query (xi, mi) asks
// for the largest xi XOR num over every num <= mi, or -1 when no element of nums
// is small enough.
//
// The two strategies differ only in how the "num <= mi" restriction is honoured.
// The per-query scan re-reads all of nums for every query; the offline sweep
// exploits the fact that the restriction is monotone - answering the queries in
// increasing order of mi means the eligible set only ever grows, so one
// insert-only BitTrie can be filled incrementally and never has to forget an
// element. That is why this composes with this repo's BitTrie as it stands:
// BitTrie has no delete.
internal static class MaximumXORWithAnElementFromArraySolution
{
    // The textbook answer: for every query, walk all of nums and keep the best
    // XOR among the eligible ones. Deliberately written without this repo's
    // primitives - it is the O(n*q) arm the offline sweep below has to justify
    // itself against.
    public static int[] MaximizeXorByLinearScanPerQuery(int[] nums, int[][] queries)
    {
        var answers = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            answers[i] = BestXorUnderLimit(nums, queries[i][0], queries[i][1]);
        }

        return answers;
    }

    private static int BestXorUnderLimit(int[] nums, int xi, int mi)
    {
        var best = LeetCodeAnswer.None;

        foreach (var num in nums)
        {
            if (num <= mi)
            {
                best = Math.Max(best, xi ^ num);
            }
        }

        return best;
    }

    // This repo's own offline sweep: MergeSort (Algorithms/Sorting/MergeSort.cs)
    // orders nums ascending and orders the query indices by their own limit mi,
    // then one pass over the queries in increasing-limit order inserts every
    // newly eligible num into this repo's own BitTrie
    // (DataStructures/Graph/Engines/Dags/Trees/BitTrie.cs - the same MaxXor-family
    // primitive MaximumXOROfTwoNumbersInAnArraySolution composes) before answering
    // that query with TryMaxXor in O(32). O((n + q) log(n + q)) overall.
    public static int[] MaximizeXorByOfflineBitTrieSweep(int[] nums, int[][] queries)
    {
        var sortedNums = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedNums));

        var queryOrder = Enumerable.Range(0, queries.Length).ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(
            new ArrayIndexedSequence<int>(queryOrder),
            Comparer<int>.Create((a, b) => queries[a][1].CompareTo(queries[b][1])));

        return AnswerInLimitOrder(new Sweep(sortedNums, new BitTrie()), queries, queryOrder);
    }

    private static int[] AnswerInLimitOrder(Sweep sweep, int[][] queries, int[] queryOrder)
    {
        var answers = new int[queries.Length];
        var numIndex = 0;

        foreach (var queryIndex in queryOrder)
        {
            answers[queryIndex] = ResolveQuery(sweep, queries[queryIndex], ref numIndex);
        }

        return answers;
    }

    // Inserts every not-yet-inserted num <= this query's limit (mi) into the trie,
    // advancing the shared cursor, then answers this query with TryMaxXor. A query
    // whose limit is below every num leaves the trie empty and reports -1.
    private static int ResolveQuery(Sweep sweep, int[] query, ref int numIndex)
    {
        var (sortedNums, trie) = sweep;
        var xi = query[0];
        var mi = query[1];

        while (numIndex < sortedNums.Length && sortedNums[numIndex] <= mi)
        {
            trie.Insert(sortedNums[numIndex]);
            numIndex++;
        }

        return trie.TryMaxXor(xi, out var candidate) ? candidate : LeetCodeAnswer.None;
    }

    // The sweep's two halves - the ascending values and the trie holding the ones
    // already eligible - travel together, so the cursor stays the only thing the
    // per-query step has to thread through by reference.
    private readonly record struct Sweep(int[] SortedNums, BitTrie Trie);
}
