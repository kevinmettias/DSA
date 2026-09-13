using DSAExperimentation.LeetCode.AllPathsFromSourceToTarget;

namespace DSAExperimentation.Tests.LeetCodeCoverage.AllPathsFromSourceToTarget;

// Harness only. Both strategies are AllPathsFromSourceToTargetSolution's - the
// hand-written recursive walk and the Backtrack composition - pinned here to
// LeetCode's published examples plus the single-node graph, where the source is
// already the target and the only path is [0].
//
// Path order is not part of LeetCode's answer, so each example is compared as an
// unordered set of sequences rather than position by position.
public sealed class AllPathsFromSourceToTargetTests
{
    public static TheoryData<int[][], int[][]> Examples =>
        new()
        {
            { [[1, 2], [3], [3], []], [[0, 1, 3], [0, 2, 3]] },
            {
                [[4, 3, 1], [3, 2, 4], [3], [4], []],
                [[0, 4], [0, 3, 4], [0, 1, 3, 4], [0, 1, 2, 3, 4], [0, 1, 4]]
            },
            { [[]], [[0]] },
            { [[1], []], [[0, 1]] },
            { [[1, 2, 3], [2, 3], [3], []], [[0, 3], [0, 2, 3], [0, 1, 3], [0, 1, 2, 3]] },
            { [[], []], [] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AllPathsByRecursiveWalk_LeetCodeExamples_ReturnsEverySourceToTargetPath(
        int[][] graph, int[][] expected) =>
        AssertSamePaths(expected, AllPathsFromSourceToTargetSolution.AllPathsByRecursiveWalk(graph));

    [Theory]
    [MemberData(nameof(Examples))]
    public void AllPathsByBacktracking_LeetCodeExamples_ReturnsEverySourceToTargetPath(
        int[][] graph, int[][] expected) =>
        AssertSamePaths(expected, AllPathsFromSourceToTargetSolution.AllPathsByBacktracking(graph));

    private static void AssertSamePaths(int[][] expected, List<List<int>> actual)
    {
        Assert.Equal(expected.Length, actual.Count);
        Assert.Equal(
            expected.Select(Describe).OrderBy(path => path, StringComparer.Ordinal),
            actual.Select(Describe).OrderBy(path => path, StringComparer.Ordinal));
    }

    private static string Describe(IEnumerable<int> path) => string.Join(",", path);
}
