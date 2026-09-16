using DSAExperimentation.LeetCode.MaximumGoodSubtreeScore;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumGoodSubtreeScore;

// Harness only. Both strategies are MaximumGoodSubtreeScoreSolution's - this file
// just pins them to LeetCode's published examples, including the case where a
// node's own value (22) has a digit repeated within itself and so can never appear
// in any good subset, not even alone.
public sealed class MaximumGoodSubtreeScoreTests
{
    public static TheoryData<int[], int[], int> Examples =>
        new()
        {
            { [2, 3], [-1, 0], 8 },
            { [1, 5, 2], [-1, 0, 0], 15 },
            { [34, 1, 2], [-1, 0, 1], 42 },
            { [3, 22, 5], [-1, 0, 1], 18 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void GoodSubtreeScoreSumByBruteForce_LeetCodeExamples_ReturnsSumOfMaxScores(
        int[] vals, int[] par, int expected)
    {
        var actual = MaximumGoodSubtreeScoreSolution.GoodSubtreeScoreSumByBruteForce(vals, par);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void GoodSubtreeScoreSumByBitmaskTreeFold_LeetCodeExamples_ReturnsSumOfMaxScores(
        int[] vals, int[] par, int expected)
    {
        var actual = MaximumGoodSubtreeScoreSolution.GoodSubtreeScoreSumByBitmaskTreeFold(vals, par);

        Assert.Equal(expected, actual);
    }
}
