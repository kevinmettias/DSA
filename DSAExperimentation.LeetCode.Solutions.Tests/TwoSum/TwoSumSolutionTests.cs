using DSAExperimentation.LeetCode.Solutions.TwoSum;

namespace DSAExperimentation.LeetCode.Solutions.Tests.TwoSum;

// Harness only: the algorithms live in TwoSumSolution, and CatalogCaseTests runs both strategies over
// every case the LeetCode catalog holds for LC 1 (ARCHITECTURE 17.13). What stays here is the one input
// the catalog cannot pose: a target no pair reaches, which LC 1's promise rules out, so LeetCode never
// asks it. Each strategy answers that there is no pair.
public sealed partial class TwoSumSolutionTests
{
    private const int UnreachableTarget = 100;

    private static readonly int[] Nums = [1, 2, 3];

    [Fact]
    public void TryFindIndicesByBruteForce_TargetNoPairReaches_FindsNone() =>
        Assert.False(TwoSumSolution.TryFindIndicesByBruteForce(Nums, UnreachableTarget, out _, out _));

    [Fact]
    public void TryFindIndicesByHashMap_TargetNoPairReaches_FindsNone() =>
        Assert.False(TwoSumSolution.TryFindIndicesByHashMap(Nums, UnreachableTarget, out _, out _));
}
