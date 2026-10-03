using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistinctSubsequencesBenchmarks (ARCHITECTURE 17.9). Its two arms are
// competing strategies for the same question - this repo's Memoizer in front of LC 115's
// suffix-pair recurrence and the bottom-up rolling row - so a harness whose arms disagree is
// counting two different things: both must report the same count. Setup's strings come from
// DistinctSubsequencesWorkloads, whose source is its target with three letters doubled and whose
// target never repeats a letter back to back - a shape its own tests pin. Each occurrence of the
// target then takes one letter from each doubled pair and the only letter of every other run, so
// the decisive count is 2 * 2 * 2 = 8 at any length.
public sealed partial class DistinctSubsequencesBenchmarksTests
{
    private const int SmallestSourceLength = 100;
    private const int ExpectedDistinctSubsequences = 8;

    [Fact]
    public void MemoizedRecursion_ThreeDoubledLetters_CountsEightSubsequences() =>
        Assert.Equal(ExpectedDistinctSubsequences, BuildHarness().MemoizedRecursion());

    [Fact]
    public void IterativeTable_ThreeDoubledLetters_CountsEightSubsequences() =>
        Assert.Equal(ExpectedDistinctSubsequences, BuildHarness().IterativeTable());

    [Fact]
    public void IterativeTable_AgreesWithMemoizedRecursion()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedRecursion(), harness.IterativeTable());
    }

    private static DistinctSubsequencesBenchmarks BuildHarness()
    {
        var harness = new DistinctSubsequencesBenchmarks { SourceLength = SmallestSourceLength };
        harness.Setup();

        return harness;
    }
}
