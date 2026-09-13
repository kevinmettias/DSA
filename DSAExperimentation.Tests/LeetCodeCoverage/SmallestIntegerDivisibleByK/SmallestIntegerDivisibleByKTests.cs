using DSAExperimentation.LeetCode.SmallestIntegerDivisibleByK;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SmallestIntegerDivisibleByK;

// Harness only. Both strategies are SmallestIntegerDivisibleByKSolution's - the
// modular walk that used to live only in the benchmark's baseline arm, and the
// Reduce.Graph BFS the test used to inline.
public sealed class SmallestIntegerDivisibleByKTests
{
    public static TheoryData<int, int> Examples =>
        new()
        {
            // LC examples 1-3.
            { 1, 1 },
            { 2, -1 },
            { 3, 3 },

            // Sharing a factor with 10 is what makes an answer impossible.
            { 4, -1 },
            { 5, -1 },
            { 10, -1 },
            { 20, -1 },

            // Coprime to 10: the answer is the multiplicative order of 10 mod k,
            // scaled by whatever factor of 9 k carries.
            { 7, 6 },
            { 9, 9 },
            { 11, 2 },
            { 13, 6 },
            { 17, 16 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestRepunitLengthByModularWalk_LeetCodeExamples_ReturnsShortestLengthOrNegativeOne(
        int k, int expected) =>
        Assert.Equal(expected, SmallestIntegerDivisibleByKSolution.SmallestRepunitLengthByModularWalk(k));

    [Theory]
    [MemberData(nameof(Examples))]
    public void SmallestRepunitLengthByReduceGraph_LeetCodeExamples_ReturnsShortestLengthOrNegativeOne(
        int k, int expected) =>
        Assert.Equal(expected, SmallestIntegerDivisibleByKSolution.SmallestRepunitLengthByReduceGraph(k));
}
