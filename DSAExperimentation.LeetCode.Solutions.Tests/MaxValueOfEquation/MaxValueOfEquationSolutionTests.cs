using DSAExperimentation.LeetCode.Solutions.MaxValueOfEquation;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaxValueOfEquation;

// Harness only. Both strategies are MaxValueOfEquationSolution's - the cases are
// stated once and replayed against each, so a failure names the strategy that broke
// rather than reporting a disagreement between an anonymous test helper and an
// anonymous benchmark arm. They cover a single pair, points of equal y, and a window
// wide enough to admit every pair; the all-negative case pins the behaviour when
// every candidate pair scores below zero. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class MaxValueOfEquationSolutionTests
{
    public static TheoryData<int[][], int, int> Cases =>
        new()
        {
            { [[1, 3], [2, 0]], 1, 4 },
            { [[0, 0], [1, 0], [2, 0]], 2, 2 },
            { [[1, -1], [2, -2], [3, -3]], 2, -2 },
            { [[1, 3], [2, 0], [5, 10], [6, -10]], 10, 17 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMaxValueOfEquationByAllPairsScan_Cases_ReturnsMaximumInWindowPairValue(
        int[][] points, int maxDistance, int expected)
    {
        var actual = MaxValueOfEquationSolution.FindMaxValueOfEquationByAllPairsScan(points, maxDistance);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void FindMaxValueOfEquationByMonotonicDeque_Cases_ReturnsMaximumInWindowPairValue(
        int[][] points, int maxDistance, int expected)
    {
        var actual = MaxValueOfEquationSolution.FindMaxValueOfEquationByMonotonicDeque(points, maxDistance);
        Assert.Equal(expected, actual);
    }
}
