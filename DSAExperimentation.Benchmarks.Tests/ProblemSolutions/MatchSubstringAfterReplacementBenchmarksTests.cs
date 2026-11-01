using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MatchSubstringAfterReplacementBenchmarks (ARCHITECTURE 17.9): the baseline
// returns a bare bool, and the fixture pins which bool: sub is all 'b' while the mappings' old
// characters are 'c' through 'v', so no start position has an allowed replacement for sub's first
// character and the verdict is false - an expected value the generic agreement check, which only
// compares arms with each other, cannot supply. It is weak by construction - it witnesses that the
// baseline never reports a match, which a strategy that always said false would also satisfy - and
// the fixture's "unreachable target" shape is deliberate (it forces the full per-comparison scan).
public sealed partial class MatchSubstringAfterReplacementBenchmarksTests
{
    private const int SmallestSourceLength = 500;

    // 'b' is not among the mapping keys ('c' + i % 20 for i in [0, 200)), so every start position
    // fails on its first character and both strategies must answer false.
    private const bool ExpectedMatchVerdict = false;

    [Fact]
    public void IsMatchAtByLinearScan_UnmappedSubstringCharacters_FindsNoMatch() =>
        Assert.Equal(ExpectedMatchVerdict, BuildHarness().IsMatchAtByLinearScan());

    private static MatchSubstringAfterReplacementBenchmarks BuildHarness()
    {
        var harness = new MatchSubstringAfterReplacementBenchmarks { SLength = SmallestSourceLength };
        harness.Setup();

        return harness;
    }
}
