using DSAExperimentation.LeetCode.MaxValueOfEquation;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaxValueOfEquation;

// Harness only. Both strategies are MaxValueOfEquationSolution's - LeetCode's
// published examples are stated once and replayed against each, so a failure names
// the strategy that broke rather than reporting a disagreement between an anonymous
// test helper and an anonymous benchmark arm. The all-negative case pins the
// behaviour when every candidate pair scores below zero.
public sealed class MaxValueOfEquationTests
{
    public static TheoryData<int[][], int, int> Examples =>
        new()
        {
            { [[1, 3], [2, 0], [5, 10], [6, -10]], 1, 4 },
            { [[0, 0], [3, 0], [9, 2]], 3, 3 },
            { [[1, 3], [2, 0]], 1, 4 },
            { [[0, 0], [1, 0], [2, 0]], 2, 2 },
            { [[1, -1], [2, -2], [3, -3]], 2, -2 },
            { [[1, 3], [2, 0], [5, 10], [6, -10]], 10, 17 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaxValueOfEquationByAllPairsScan_LeetCodeExamples_ReturnsMaximumInWindowPairValue(
        int[][] points, int k, int expected)
    {
        var actual = MaxValueOfEquationSolution.FindMaxValueOfEquationByAllPairsScan(points, k);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindMaxValueOfEquationByMonotonicDeque_LeetCodeExamples_ReturnsMaximumInWindowPairValue(
        int[][] points, int k, int expected)
    {
        var actual = MaxValueOfEquationSolution.FindMaxValueOfEquationByMonotonicDeque(points, k);
        Assert.Equal(expected, actual);
    }
}
