using DSAExperimentation.LeetCode.MinimumReverseOperations;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumReverseOperations;

// Harness only. Both strategies - the O(n)-per-pop brute-force scan and the
// Reduce.Graph walk over ReversalTopology - are MinimumReverseOperationsSolution's;
// this file just pins them to LeetCode's published examples, plus the two degenerate
// window sizes the examples never reach: K = 1, where no reversal moves anything, and
// K = n, where the only window there is splits the array into mirror pairs.
public sealed partial class MinimumReverseOperationsTests
{
    public static TheoryData<ReverseExample> Examples =>
        new()
        {
            { new ReverseExample(N: 4, P: 0, Banned: [1, 2], K: 4, Expected: [0, -1, -1, 1]) },
            { new ReverseExample(N: 5, P: 0, Banned: [2, 4], K: 3, Expected: [0, -1, -1, -1, -1]) },
            { new ReverseExample(N: 4, P: 2, Banned: [0, 1, 3], K: 1, Expected: [-1, -1, 0, -1]) },
            { new ReverseExample(N: 3, P: 1, Banned: [], K: 1, Expected: [-1, 0, -1]) },
            { new ReverseExample(N: 4, P: 0, Banned: [], K: 4, Expected: [0, -1, -1, 1]) },
            { new ReverseExample(N: 5, P: 2, Banned: [], K: 2, Expected: [2, 1, 0, 1, 2]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByBruteForceScan_LeetCodeExamples_ReturnsShortestOperationCounts(ReverseExample example)
    {
        var actual = MinimumReverseOperationsSolution.MinOperationsByBruteForceScan(
            example.N, example.P, example.Banned, example.K);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinOperationsByReduceGraph_LeetCodeExamples_ReturnsShortestOperationCounts(ReverseExample example)
    {
        var actual = MinimumReverseOperationsSolution.MinOperationsByReduceGraph(
            example.N, example.P, example.Banned, example.K);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the board length, the start index, the banned indices, the
    // reversal window, and the shortest operation count to each index. The first four
    // are all `int`, so the fields name each one rather than leaving a row where the
    // length, the start and the window size are a transposition apart.
    public readonly record struct ReverseExample(
        int N,
        int P,
        int[] Banned,
        int K,
        int[] Expected);
}
