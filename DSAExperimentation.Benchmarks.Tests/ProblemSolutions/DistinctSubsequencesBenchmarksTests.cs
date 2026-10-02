using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistinctSubsequencesBenchmarks (ARCHITECTURE 17.9). Its two arms are
// competing strategies for the same question - this repo's Memoizer in front of LC 115's
// suffix-pair recurrence and the bottom-up rolling row - so a harness whose arms disagree is
// counting two different things: both must report the same count. The class pins its own operands
// as the constants "rabbbit" and "rabbit", which is LC 115's published example, and the example's
// published answer for that pair is 3: the three ways to drop one of the three interior b's. That
// is the decisive value both arms are asserted against.
public sealed partial class DistinctSubsequencesBenchmarksTests
{
    private const int ExpectedDistinctSubsequencesOfRabbitInRabbbit = 3;

    [Fact]
    public void MemoizedRecursion_LeetCodeExamplePair_CountsTheThreePublishedSubsequences() =>
        Assert.Equal(ExpectedDistinctSubsequencesOfRabbitInRabbbit, BuildHarness().MemoizedRecursion());

    [Fact]
    public void IterativeTable_LeetCodeExamplePair_CountsTheThreePublishedSubsequences() =>
        Assert.Equal(ExpectedDistinctSubsequencesOfRabbitInRabbbit, BuildHarness().IterativeTable());

    [Fact]
    public void IterativeTable_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.IterativeTable());
    }

    private static DistinctSubsequencesBenchmarks BuildHarness() => new();
}
