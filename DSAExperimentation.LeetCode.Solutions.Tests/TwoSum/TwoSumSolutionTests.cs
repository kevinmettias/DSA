using DSAExperimentation.LeetCode.Catalog;
using DSAExperimentation.LeetCode.Solutions.Conventions;
using DSAExperimentation.LeetCode.Solutions.TwoSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TwoSum;

// Harness only: the algorithms live in TwoSumSolution. One test method per strategy over every case
// the LeetCode catalog holds for LC 1 - its published examples, then LeetCodeDataset's - plus the
// one case LeetCode never poses, a target no pair reaches, so a failure names the strategy that
// broke. LeetCode accepts the pair in either order, so the pair is compared as a set.
//
// LC 1 promises that exactly one pair reaches the target, and LeetCodeDataset's generated inputs do
// not always keep that promise: 30 of them have several pairs, where each strategy may name a
// different and equally valid one. So a case is kept only when TwoSumPromise holds for it.
public sealed partial class TwoSumSolutionTests
{
    private const string TitleSlug = "two-sum";

    public static TheoryData<TwoSumExample> Cases => [.. CatalogCases(), NoPairExample];

    private static TwoSumExample NoPairExample => new(Nums: [1, 2, 3], Target: 100, ExpectedPair: []);

    [Theory]
    [MemberData(nameof(Cases))]
    public void TryFindIndicesByBruteForce_LeetCodeCases_ReturnsMatchingPairIndices(TwoSumExample example)
    {
        var found = TwoSumSolution.TryFindIndicesByBruteForce(
            example.Nums, example.Target, out var first, out var second);

        AssertResult(PairOf(found, first, second), example);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void TryFindIndicesByHashMap_LeetCodeCases_ReturnsMatchingPairIndices(TwoSumExample example)
    {
        var found = TwoSumSolution.TryFindIndicesByHashMap(
            example.Nums, example.Target, out var first, out var second);

        AssertResult(PairOf(found, first, second), example);
    }

    private static IEnumerable<TwoSumExample> CatalogCases()
        => LeetCodeCatalog.Cases(TitleSlug)
            .Where(testCase => testCase.HasExpectedOutput)
            .Select(ExampleOf)
            .Where(example => TwoSumPromise.IsKeptBy(example.Nums, example.Target));

    private static TwoSumExample ExampleOf(LeetCodeCase testCase)
        => new(testCase.Input<int[]>(0), testCase.Input<int>(1), testCase.Expected<int[]>());

    // A strategy that finds no pair answers with none, which is what LeetCode's answer would be
    // for the one target its promise rules out.
    private static int[] PairOf(bool found, int first, int second)
    {
        if (!found)
        {
            return [];
        }

        return [first, second];
    }

    private static void AssertResult(int[] actual, TwoSumExample example)
        => Assert.True(
            LeetCodeAnswers.IsSetEqual(actual, example.ExpectedPair),
            $"nums [{string.Join(',', example.Nums)}], target {example.Target}: expected [{string.Join(',', example.ExpectedPair)}], got [{string.Join(',', actual)}]");

    // One case: the array and target the strategy is given, and the pair of indices it must answer
    // with, empty when no pair reaches the target.
    public readonly record struct TwoSumExample(int[] Nums, int Target, int[] ExpectedPair);
}
