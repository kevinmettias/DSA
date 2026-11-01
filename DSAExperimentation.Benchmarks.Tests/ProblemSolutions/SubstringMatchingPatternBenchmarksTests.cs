using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SubstringMatchingPatternBenchmarks (ARCHITECTURE 17.9): both arms answer
// with a bare bool, and the workload [GlobalSetup] builds makes that bool decisive rather than
// merely agreed: subject is a run of 'a's while both halves of pattern are a run of 'a's followed
// by a 'b', a character subject never contains, so no window of subject can start with the prefix
// and the honest verdict is false - which the baseline is pinned to, since two arms that were
// wrong in the same way would still pass the generic arm check's agreement.
public sealed partial class SubstringMatchingPatternBenchmarksTests
{
    private const int SmallestLength = 5;

    [Fact]
    public void HasMatchByBruteForce_HalvesEndingInAnAbsentCharacter_FindsNoMatch() =>
        Assert.False(BuildHarness().HasMatchByBruteForce());

    private static SubstringMatchingPatternBenchmarks BuildHarness()
    {
        var harness = new SubstringMatchingPatternBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
