using DSAExperimentation.LeetCode.Solutions.GreatestCommonDivisorTraversal;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GreatestCommonDivisorTraversal;

// Harness only: both strategies live in GreatestCommonDivisorTraversalSolution and are
// pinned to the degenerate cases the two arms have to
// agree on - a lone value (vacuously traversable), a lone 1, a 1 sitting alongside values
// that do connect to each other, and a chain that is only connected transitively
// (30-77-22-35 has no edge between 30 and 77 at all, only the path through 22 and 35).
public sealed partial class GreatestCommonDivisorTraversalSolutionTests
{
    public static TheoryData<TraversalCase> Cases =>
        new()
        {
            { new TraversalCase(Nums: [7], Expected: true) },
            { new TraversalCase(Nums: [1, 2, 4], Expected: false) },
            { new TraversalCase(Nums: [30, 77, 22, 35], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanTraverseAllPairsByPairwiseGcd_Cases_ReportsWhetherEveryIndexIsReachable(
        TraversalCase testCase)
    {
        var actual = GreatestCommonDivisorTraversalSolution.CanTraverseAllPairsByPairwiseGcd(testCase.Nums);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CanTraverseAllPairsByPrimeFactorUnion_Cases_ReportsWhetherEveryIndexIsReachable(
        TraversalCase testCase)
    {
        var actual = GreatestCommonDivisorTraversalSolution.CanTraverseAllPairsByPrimeFactorUnion(testCase.Nums);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the values to decide about, and whether every index is
    // reachable. The `bool` is the expected answer rather than a mode, so the row names
    // it instead of leaving a bare `true` in a position the reader has to decode.
    public readonly record struct TraversalCase(int[] Nums, bool Expected);
}
