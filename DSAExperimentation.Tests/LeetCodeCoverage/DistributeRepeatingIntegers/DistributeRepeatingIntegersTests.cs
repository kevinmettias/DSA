using DSAExperimentation.LeetCode.DistributeRepeatingIntegers;

namespace DSAExperimentation.Tests.LeetCodeCoverage.DistributeRepeatingIntegers;

// Harness only. Both strategies are DistributeRepeatingIntegersSolution's - this
// file just pins them to LeetCode's published examples, plus the cases that
// separate "enough copies in total" from "enough copies of one value", which is
// the whole point of the problem.
public sealed partial class DistributeRepeatingIntegersTests
{
    public static TheoryData<DistributionCase> Examples =>
        new()
        {
            { new DistributionCase([1, 2, 3, 4], [2], CanDistribute: false) },
            { new DistributionCase([1, 2, 3, 3], [2], CanDistribute: true) },
            { new DistributionCase([1, 1, 2, 2], [2, 2], CanDistribute: true) },
            { new DistributionCase([1, 1, 2, 3], [2, 2], CanDistribute: false) },
            { new DistributionCase([1, 1, 1, 1, 1], [2, 3], CanDistribute: true) },
            { new DistributionCase([1, 1, 2, 2], [3, 1], CanDistribute: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanDistributeByNaiveBacktracking_LeetCodeExamples_ReturnsWhetherEveryOrderFitsOneValuesStock(
        DistributionCase example)
    {
        var actual = DistributeRepeatingIntegersSolution.CanDistributeByNaiveBacktracking(
            example.Nums, example.Quantity);

        Assert.Equal(example.CanDistribute, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanDistributeByGenericBacktrack_LeetCodeExamples_ReturnsWhetherEveryOrderFitsOneValuesStock(
        DistributionCase example)
    {
        var actual = DistributeRepeatingIntegersSolution.CanDistributeByGenericBacktrack(
            example.Nums, example.Quantity);

        Assert.Equal(example.CanDistribute, actual);
    }

    // One LeetCode example: the values in stock, the per-order quantities asked of them,
    // and whether every order can be filled from its own value's stock. The expected
    // value is named at every construction site, so a row reads as the case it is rather
    // than as a bare `true` whose meaning is its position. Nested because it is only ever
    // used inside this test class - it is this harness's own vocabulary, not a type
    // another file would import.
    public readonly record struct DistributionCase(int[] Nums, int[] Quantity, bool CanDistribute);
}
