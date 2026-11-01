using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for DistinctSubsequencesBenchmarks (ARCHITECTURE 17.9): an expected count known
// from the workload's shape. Setup's strings come from DistinctSubsequencesWorkloads, whose source is
// its target with three letters doubled and whose target never repeats a letter back to back - a
// shape its own tests pin. Each occurrence of the target then takes one letter from each doubled
// pair and the only letter of every other run, so the decisive count is 2 * 2 * 2 = 8 at any length.
public sealed partial class DistinctSubsequencesBenchmarksTests
{
    private const int SmallestSourceLength = 100;
    private const int ExpectedDistinctSubsequences = 8;

    [Fact]
    public void IterativeTable_ThreeDoubledLetters_CountsEightSubsequences() =>
        Assert.Equal(ExpectedDistinctSubsequences, BuildHarness().IterativeTable());

    private static DistinctSubsequencesBenchmarks BuildHarness()
    {
        var harness = new DistinctSubsequencesBenchmarks { SourceLength = SmallestSourceLength };
        harness.Setup();

        return harness;
    }
}
