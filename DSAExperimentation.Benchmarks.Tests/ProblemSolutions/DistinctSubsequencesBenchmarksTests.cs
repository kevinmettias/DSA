using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistinctSubsequencesBenchmarks (ARCHITECTURE 17.9): it carries a single arm -
// this repo's Memoizer in front of LC 115's suffix-pair recurrence - so there is nothing to agree
// with. The class pins its own operands as the constants "rabbbit" and "rabbit", which is LC 115's
// published example, and the example's published answer for that pair is 3: the three ways to drop
// one of the three interior b's. That is the decisive value the lone arm is asserted against.
public sealed partial class DistinctSubsequencesBenchmarksTests
{
    private const int ExpectedDistinctSubsequencesOfRabbitInRabbbit = 3;

    [Fact]
    public void MemoizedRecursion_LeetCodeExamplePair_CountsTheThreePublishedSubsequences() =>
        Assert.Equal(ExpectedDistinctSubsequencesOfRabbitInRabbbit, BuildHarness().MemoizedRecursion());

    private static DistinctSubsequencesBenchmarks BuildHarness() => new();
}
