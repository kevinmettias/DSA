using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for LongestCommonPrefixBenchmarks (ARCHITECTURE 17.9): Setup gives every string
// the same PrefixLength run of 'x' and then a distinct single-character suffix, so the answer is
// exactly that run - an expected value derived from the workload's construction rather than from
// either arm, which the generic agreement check cannot supply. The baseline returns the shared prefix
// itself, a string compared directly.
public sealed partial class LongestCommonPrefixBenchmarksTests
{
    private const int SmallestPrefixLength = 64;

    private const char SharedPrefixCharacter = 'x';

    [Fact]
    public void LinearScan_SmallestPrefixLength_ReturnsTheWholeSharedRun() =>
        Assert.Equal(ExpectedSharedPrefix(), BuildHarness().LinearScan());

    // Setup's strings share their leading 'x' run and diverge only at the suffix, so the longest
    // common prefix is that whole run.
    private static string ExpectedSharedPrefix() => new(SharedPrefixCharacter, SmallestPrefixLength);

    private static LongestCommonPrefixBenchmarks BuildHarness()
    {
        var harness = new LongestCommonPrefixBenchmarks { PrefixLength = SmallestPrefixLength };
        harness.Setup();

        return harness;
    }
}
