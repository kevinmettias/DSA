using DSAExperimentation.LeetCode.ValidArrangementOfPairs;

namespace DSAExperimentation.Tests.LeetCodeCoverage.ValidArrangementOfPairs;

// Harness only. Both Hierholzer strategies are ValidArrangementOfPairsSolution's;
// this file pins them to LeetCode's published examples.
//
// The answer is not unique - LC 2097 accepts any arrangement that chains - so an
// example states the pairs and the node the walk must start from, and the shared
// check confirms the returned arrangement chains end-to-start and is a permutation
// of the input pairs. The start node IS pinned: it is the one part of the answer the
// problem determines, being the unique node with outdegree - indegree == 1, or
// pairs[0]'s source when the pairs form an Eulerian circuit instead.
public sealed class ValidArrangementOfPairsTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LC example 1: 11 is the unique imbalanced source.
            { [[5, 1], [4, 5], [11, 9], [9, 4]], 11 },
            // LC example 2: an Eulerian circuit, so the start falls back to pairs[0][0].
            { [[1, 3], [3, 2], [2, 1]], 1 },
            // LC example 3: 1 has two outgoing pairs and one incoming.
            { [[1, 2], [1, 3], [2, 1]], 1 },
            // Single pair - the walk is the pair itself.
            { [[7, 8]], 7 },
            // A plain two-pair chain with distinct endpoints.
            { [[1, 2], [2, 3]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValidArrangementByDictionaryWithList_LeetCodeExamples_ChainsEveryPairFromTheImbalancedSource(
        int[][] pairs, int expectedStart) =>
        AssertIsValidArrangement(
            pairs, expectedStart, ValidArrangementOfPairsSolution.ValidArrangementByDictionaryWithList(pairs));

    [Theory]
    [MemberData(nameof(Examples))]
    public void ValidArrangementByRepoHashMapWithStack_LeetCodeExamples_ChainsEveryPairFromTheImbalancedSource(
        int[][] pairs, int expectedStart) =>
        AssertIsValidArrangement(
            pairs, expectedStart, ValidArrangementOfPairsSolution.ValidArrangementByRepoHashMapWithStack(pairs));

    private static void AssertIsValidArrangement(int[][] pairs, int expectedStart, int[][] arrangement)
    {
        Assert.Equal(pairs.Length, arrangement.Length);
        Assert.Equal(expectedStart, arrangement[0][0]);

        for (var i = 1; i < arrangement.Length; i++)
        {
            Assert.Equal(arrangement[i - 1][1], arrangement[i][0]);
        }

        var expected = pairs.Select(pair => (pair[0], pair[1])).OrderBy(pair => pair.Item1).ThenBy(pair => pair.Item2);
        var actual = arrangement.Select(pair => (pair[0], pair[1])).OrderBy(pair => pair.Item1).ThenBy(pair => pair.Item2);
        Assert.Equal(expected, actual);
    }
}
