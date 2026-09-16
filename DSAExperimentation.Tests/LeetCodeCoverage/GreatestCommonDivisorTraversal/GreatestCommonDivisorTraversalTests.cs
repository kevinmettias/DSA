using DSAExperimentation.LeetCode.GreatestCommonDivisorTraversal;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GreatestCommonDivisorTraversal;

// Harness only: both strategies live in GreatestCommonDivisorTraversalSolution and are
// pinned to LeetCode's published examples, plus the degenerate cases the two arms have to
// agree on - a lone value (vacuously traversable), a lone 1, a 1 sitting alongside values
// that do connect to each other, and a chain that is only connected transitively
// (30-77-22-35 has no edge between 30 and 77 at all, only the path through 22 and 35).
public sealed class GreatestCommonDivisorTraversalTests
{
    public static TheoryData<TraversalExample> Examples =>
        new()
        {
            { new TraversalExample(Nums: [2, 3, 6], Expected: true) },
            { new TraversalExample(Nums: [3, 9, 5], Expected: false) },
            { new TraversalExample(Nums: [4, 3, 12, 8], Expected: true) },
            { new TraversalExample(Nums: [7], Expected: true) },
            { new TraversalExample(Nums: [1], Expected: true) },
            { new TraversalExample(Nums: [1, 2, 4], Expected: false) },
            { new TraversalExample(Nums: [30, 77, 22, 35], Expected: true) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanTraverseAllPairsByPairwiseGcd_LeetCodeExamples_ReportsWhetherEveryIndexIsReachable(
        TraversalExample example)
    {
        var actual = GreatestCommonDivisorTraversalSolution.CanTraverseAllPairsByPairwiseGcd(example.Nums);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CanTraverseAllPairsByPrimeFactorUnion_LeetCodeExamples_ReportsWhetherEveryIndexIsReachable(
        TraversalExample example)
    {
        var actual = GreatestCommonDivisorTraversalSolution.CanTraverseAllPairsByPrimeFactorUnion(example.Nums);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the values to decide about, and whether every index is
    // reachable. The `bool` is the expected answer rather than a mode, so the row names
    // it instead of leaving a bare `true` in a position the reader has to decode.
    public readonly record struct TraversalExample(int[] Nums, bool Expected);
}
