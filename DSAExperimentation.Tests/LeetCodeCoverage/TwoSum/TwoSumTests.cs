using DSAExperimentation.LeetCode.TwoSum;

namespace DSAExperimentation.Tests.LeetCodeCoverage.TwoSum;

// Harness only: the algorithms live in TwoSumSolution. One test method per
// strategy over one shared set of LeetCode's own examples, so a failure names the
// strategy that broke.
public sealed class TwoSumTests
{
    public static TheoryData<TwoSumExample> Examples =>
        new()
        {
            { new TwoSumExample(Nums: [2, 7, 11, 15], Target: 9, ExpectedFound: true, ExpectedFirst: 0, ExpectedSecond: 1) },
            { new TwoSumExample(Nums: [3, 2, 4], Target: 6, ExpectedFound: true, ExpectedFirst: 1, ExpectedSecond: 2) },
            { new TwoSumExample(Nums: [3, 3], Target: 6, ExpectedFound: true, ExpectedFirst: 0, ExpectedSecond: 1) },
            { new TwoSumExample(Nums: [1, 2, 3], Target: 100, ExpectedFound: false, ExpectedFirst: 0, ExpectedSecond: 0) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindIndicesByBruteForce_LeetCodeExamples_ReturnsMatchingPairIndices(TwoSumExample example)
    {
        var found = TwoSumSolution.TryFindIndicesByBruteForce(
            example.Nums, example.Target, out var first, out var second);

        AssertResult((found, first, second), example);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void TryFindIndicesByHashMap_LeetCodeExamples_ReturnsMatchingPairIndices(TwoSumExample example)
    {
        var found = TwoSumSolution.TryFindIndicesByHashMap(
            example.Nums, example.Target, out var first, out var second);

        AssertResult((found, first, second), example);
    }

    private static void AssertResult((bool Found, int First, int Second) actual, TwoSumExample example)
    {
        Assert.Equal(example.ExpectedFound, actual.Found);
        Assert.Equal(example.ExpectedFirst, actual.First);
        Assert.Equal(example.ExpectedSecond, actual.Second);
    }

    // One LeetCode example: the array and target the strategy is given, and what it must
    // answer - whether a pair exists, and the two indices when it does. The five travel
    // together into every assertion, so each is named rather than left as a position in a
    // row of five literals.
    public readonly record struct TwoSumExample(
        int[] Nums,
        int Target,
        bool ExpectedFound,
        int ExpectedFirst,
        int ExpectedSecond);
}
