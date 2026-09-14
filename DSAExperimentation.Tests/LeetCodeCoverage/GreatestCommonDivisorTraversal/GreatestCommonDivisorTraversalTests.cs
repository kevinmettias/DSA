using DSAExperimentation.LeetCode.GreatestCommonDivisorTraversal;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GreatestCommonDivisorTraversal;

// Harness only: both strategies live in GreatestCommonDivisorTraversalSolution and are
// pinned to LeetCode's published examples, plus the degenerate cases the two arms have to
// agree on - a lone value (vacuously traversable), a lone 1, a 1 sitting alongside values
// that do connect to each other, and a chain that is only connected transitively
// (30-77-22-35 has no edge between 30 and 77 at all, only the path through 22 and 35).
public sealed class GreatestCommonDivisorTraversalTests
{
    public static TheoryData<int[], bool> Examples =>
        new()
        {
            { [2, 3, 6], true },
            { [3, 9, 5], false },
            { [4, 3, 12, 8], true },
            { [7], true },
            { [1], true },
            { [1, 2, 4], false },
            { [30, 77, 22, 35], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanTraverseAllPairsByPairwiseGcd_LeetCodeExamples_ReportsWhetherEveryIndexIsReachable(
        int[] nums, bool expected) =>
        Assert.Equal(expected, GreatestCommonDivisorTraversalSolution.CanTraverseAllPairsByPairwiseGcd(nums));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanTraverseAllPairsByPrimeFactorUnion_LeetCodeExamples_ReportsWhetherEveryIndexIsReachable(
        int[] nums, bool expected) =>
        Assert.Equal(expected, GreatestCommonDivisorTraversalSolution.CanTraverseAllPairsByPrimeFactorUnion(nums));
}
