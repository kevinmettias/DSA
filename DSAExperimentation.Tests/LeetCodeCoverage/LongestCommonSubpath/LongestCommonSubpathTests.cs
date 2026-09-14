using DSAExperimentation.LeetCode.LongestCommonSubpath;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestCommonSubpath;

// Harness only. Both strategies are LongestCommonSubpathSolution's - the naive
// window-key intersection that used to live untested as the benchmark baseline, and
// the RollingHash-screened one - pinned here to LeetCode's published examples plus
// the three edge shapes the original test carried: no overlap at all, a single
// shared city, and a run that has to survive an intersection across three paths.
public sealed class LongestCommonSubpathTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LeetCode example 1: [2,3] is the longest run all three paths share.
            { [[0, 1, 2, 3, 4], [2, 3, 4], [4, 0, 1, 2, 3]], 2 },

            // LeetCode example 2: three one-city paths with no city in common.
            { [[0], [1], [2]], 0 },

            // LeetCode example 3: one path is the other reversed, so no run longer
            // than a single city survives.
            { [[0, 1, 2, 3, 4], [4, 3, 2, 1, 0]], 1 },

            // One path only - the whole path is trivially shared.
            { [[0, 1, 2, 3]], 4 },

            // Identical paths - again the whole path.
            { [[1, 2, 3], [1, 2, 3]], 3 },

            // The shared run sits at a different offset in each path, and a longer
            // run exists in two of the three but not the third.
            { [[9, 1, 2, 3, 8], [1, 2, 3, 7], [6, 1, 2, 5]], 2 },

            // A repeated city forces the window keys to distinguish positions
            // rather than membership.
            { [[1, 1, 1, 2], [1, 1, 2, 1]], 3 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestCommonSubpathByNaiveWindowKeys_LeetCodeExamples_ReturnsSharedRunLength(
        int[][] paths, int expected) =>
        Assert.Equal(expected, LongestCommonSubpathSolution.LongestCommonSubpathByNaiveWindowKeys(paths));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LongestCommonSubpathByRollingHash_LeetCodeExamples_ReturnsSharedRunLength(
        int[][] paths, int expected) =>
        Assert.Equal(expected, LongestCommonSubpathSolution.LongestCommonSubpathByRollingHash(paths));
}
