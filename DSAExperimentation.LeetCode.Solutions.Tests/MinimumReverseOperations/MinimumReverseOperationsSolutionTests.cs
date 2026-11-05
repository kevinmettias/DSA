using DSAExperimentation.LeetCode.Solutions.MinimumReverseOperations;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumReverseOperations;

// Harness only. Both strategies - the O(n)-per-pop brute-force scan and the
// Reduce.Graph walk over ReversalTopology - are MinimumReverseOperationsSolution's;
// this file just pins them to window sizes on a board with nothing banned: K = 1,
// where no reversal moves anything, K = n, where the only window there is splits the
// array into mirror pairs, and K = 2, where each reversal is an adjacent swap, so the
// count is the distance from the start. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MinimumReverseOperationsSolutionTests
{
    public static TheoryData<ReverseCase> Cases =>
        new()
        {
            { new ReverseCase(N: 3, P: 1, Banned: [], K: 1, Expected: [-1, 0, -1]) },
            { new ReverseCase(N: 4, P: 0, Banned: [], K: 4, Expected: [0, -1, -1, 1]) },
            { new ReverseCase(N: 5, P: 2, Banned: [], K: 2, Expected: [2, 1, 0, 1, 2]) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinOperationsByBruteForceScan_Cases_ReturnsShortestOperationCounts(ReverseCase testCase)
    {
        var actual = MinimumReverseOperationsSolution.MinOperationsByBruteForceScan(
            testCase.N, testCase.P, testCase.Banned, testCase.K);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinOperationsByReduceGraph_Cases_ReturnsShortestOperationCounts(ReverseCase testCase)
    {
        var actual = MinimumReverseOperationsSolution.MinOperationsByReduceGraph(
            testCase.N, testCase.P, testCase.Banned, testCase.K);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the board length, the start index, the banned indices, the
    // reversal window, and the shortest operation count to each index. The first four
    // are all `int`, so the fields name each one rather than leaving a row where the
    // length, the start and the window size are a transposition apart.
    public readonly record struct ReverseCase(
        int N,
        int P,
        int[] Banned,
        int K,
        int[] Expected);
}
