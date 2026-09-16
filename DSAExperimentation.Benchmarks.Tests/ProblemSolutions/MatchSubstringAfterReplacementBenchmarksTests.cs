using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MatchSubstringAfterReplacementBenchmarks (ARCHITECTURE 17.9): its two arms
// are competing strategies for the same question - scanning the raw mappings list per comparison
// against an O(1) two-step lookup through a mapping of character sets - so a harness whose arms
// disagree is timing two different problems. Both arms return a bare bool, and the fixture pins
// which bool: sub is all 'b' while the mappings' old characters are 'c' through 'v', so no start
// position has an allowed replacement for sub's first character and the verdict is false for both.
// Agreement here is therefore weak by construction - it witnesses that neither strategy ever
// reports a match, which a strategy that always said false would also satisfy - and the fixture's
// "unreachable target" shape is deliberate (it forces the full per-comparison scan), so the honest
// assertion is the two verdicts against the fixture's own value rather than only against each
// other. Setup draws nothing from a stream, so the same SLength rebuilds the same workload.
public sealed partial class MatchSubstringAfterReplacementBenchmarksTests
{
    private const int SmallestSourceLength = 500;

    // 'b' is not among the mapping keys ('c' + i % 20 for i in [0, 200)), so every start position
    // fails on its first character and both strategies must answer false.
    private const bool ExpectedMatchVerdict = false;

    [Fact]
    public void Setup_SameSourceLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(BuildHarness().IsMatchAtByLinearScan(), BuildHarness().IsMatchAtByLinearScan());
        Assert.Equal(BuildHarness().IsMatchAtByHashMapLookup(), BuildHarness().IsMatchAtByHashMapLookup());
    }

    [Fact]
    public void IsMatchAtByLinearScan_UnmappedSubstringCharacters_AgreesWithHashMapLookup()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchVerdict, harness.IsMatchAtByLinearScan());
        Assert.Equal(harness.IsMatchAtByHashMapLookup(), harness.IsMatchAtByLinearScan());
    }

    [Fact]
    public void IsMatchAtByHashMapLookup_UnmappedSubstringCharacters_AgreesWithLinearScan()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedMatchVerdict, harness.IsMatchAtByHashMapLookup());
        Assert.Equal(harness.IsMatchAtByLinearScan(), harness.IsMatchAtByHashMapLookup());
    }

    private static MatchSubstringAfterReplacementBenchmarks BuildHarness()
    {
        var harness = new MatchSubstringAfterReplacementBenchmarks { SLength = SmallestSourceLength };
        harness.Setup();

        return harness;
    }
}
