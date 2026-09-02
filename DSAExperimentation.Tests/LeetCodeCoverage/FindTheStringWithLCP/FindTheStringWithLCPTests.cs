using DSAExperimentation.LeetCode.FindTheStringWithLCP;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindTheStringWithLCP;

// Harness only: the algorithms live in FindTheStringWithLCPSolution. One test method
// per strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke (TwoSumTests precedent).
public sealed class FindTheStringWithLCPTests
{
    public static TheoryData<int[][], string> Examples =>
        new()
        {
            {
                [[4, 0, 2, 0], [0, 3, 0, 1], [2, 0, 2, 0], [0, 1, 0, 1]],
                "abab"
            },
            {
                [[4, 3, 2, 1], [3, 3, 2, 1], [2, 2, 2, 1], [1, 1, 1, 1]],
                "aaaa"
            },
            {
                // lcp[3][3] must equal n - 3 = 1, but it's given as 3 - no real string
                // could ever have that LCP matrix.
                [[4, 3, 2, 1], [3, 3, 2, 1], [2, 2, 2, 1], [1, 1, 1, 3]],
                ""
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConstructByDirectSweep_LeetCodeExamples_ReturnsSmallestConsistentWord(int[][] lcp, string expected)
    {
        var actual = FindTheStringWithLCPSolution.ConstructByDirectSweep(lcp);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ConstructByDisjointSet_LeetCodeExamples_ReturnsSmallestConsistentWord(int[][] lcp, string expected)
    {
        var actual = FindTheStringWithLCPSolution.ConstructByDisjointSet(lcp);
        Assert.Equal(expected, actual);
    }
}
