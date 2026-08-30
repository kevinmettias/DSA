using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumXORWithAnElementFromArray;

// LeetCode 1707. Maximum XOR With an Element From Array: offline processing -
// sort nums with this repo's own MergeSort (Algorithms/Sorting/MergeSort.cs)
// and sort query indices by their own limit mi, then sweep both in
// increasing-limit order, inserting every num <= mi into this repo's own
// BitTrie (DataStructures/Graph/Engines/Dags/Trees/BitTrie.cs - the same
// MaxXor-family primitive MaximumXOROfTwoNumbersInAnArrayTests already
// composes) before answering that query with TryMaxXor. A query answers -1
// when no num <= mi has been inserted into the trie yet.
public sealed partial class MaximumXORWithAnElementFromArrayTests
{
    [Fact]
    public void MaximizeXor_ClassicExample_MatchesExpectedResults()
    {
        int[] nums = [0, 1, 2, 3, 4];
        int[][] queries = [[3, 1], [1, 3], [5, 6]];

        Assert.Equal([3, 3, 7], MaximizeXor(nums, queries));
    }

    [Fact]
    public void MaximizeXor_SomeQueriesHaveNoEligibleElement_ReturnsNegativeOneForThose()
    {
        int[] nums = [5, 2, 4, 6, 6, 3];
        int[][] queries = [[12, 4], [8, 1], [6, 3]];

        Assert.Equal([15, -1, 5], MaximizeXor(nums, queries));
    }

    private static int[] MaximizeXor(int[] nums, int[][] queries)
    {
        var sortedNums = nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sortedNums));

        var queryOrder = Enumerable.Range(0, queries.Length).ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(
            new ArrayIndexedSequence<int>(queryOrder),
            Comparer<int>.Create((a, b) => queries[a][1].CompareTo(queries[b][1])));

        var answers = new int[queries.Length];
        var trie = new BitTrie();
        var numIndex = 0;

        foreach (var queryIndex in queryOrder)
        {
            var xi = queries[queryIndex][0];
            var mi = queries[queryIndex][1];

            while (numIndex < sortedNums.Length && sortedNums[numIndex] <= mi)
            {
                trie.Insert(sortedNums[numIndex]);
                numIndex++;
            }

            answers[queryIndex] = trie.TryMaxXor(xi, out var candidate) ? candidate : -1;
        }

        return answers;
    }
}
