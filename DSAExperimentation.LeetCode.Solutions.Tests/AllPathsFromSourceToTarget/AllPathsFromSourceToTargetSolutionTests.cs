using DSAExperimentation.LeetCode.Solutions.AllPathsFromSourceToTarget;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AllPathsFromSourceToTarget;

// Harness only. Both strategies are AllPathsFromSourceToTargetSolution's - the
// hand-written recursive walk and the Backtrack composition - pinned here to
// the single-node graph, where the source is
// already the target and the only path is [0].
//
// Path order is not part of LeetCode's answer, so each case is compared as an
// unordered set of sequences rather than position by position.
public sealed partial class AllPathsFromSourceToTargetSolutionTests
{
    public static TheoryData<int[][], int[][]> Cases =>
        new()
        {
            { [[]], [[0]] },
            { [[1, 2, 3], [2, 3], [3], []], [[0, 3], [0, 2, 3], [0, 1, 3], [0, 1, 2, 3]] },
            { [[], []], [] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void AllPathsByRecursiveWalk_Cases_ReturnsEverySourceToTargetPath(
        int[][] graph, int[][] expected) =>
        AssertSamePaths(expected, AllPathsFromSourceToTargetSolution.AllPathsByRecursiveWalk(graph));

    [Theory]
    [MemberData(nameof(Cases))]
    public void AllPathsByBacktracking_Cases_ReturnsEverySourceToTargetPath(
        int[][] graph, int[][] expected) =>
        AssertSamePaths(expected, AllPathsFromSourceToTargetSolution.AllPathsByBacktracking(graph));

    private static void AssertSamePaths(int[][] expected, List<List<int>> actual)
    {
        Assert.Equal(expected.Length, actual.Count);

        var expectedPaths = expected.Select(Describe).OrderBy(path => path, StringComparer.Ordinal);
        var actualPaths = actual.Select(Describe).OrderBy(path => path, StringComparer.Ordinal);

        Assert.Equal(expectedPaths, actualPaths);
    }

    private static string Describe(IEnumerable<int> path) => string.Join(",", path);
}
