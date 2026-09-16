using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RepeatedStringMatchBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - the BCL's own ordinal substring search against this repo's
// KMP-based PrefixFunctionSearch, each asked over the same candidate repeat counts - so a harness
// whose arms disagree is timing two different problems. Setup builds both inputs from Length alone
// and from a fixed filler, so the same Length must rebuild the same pair; otherwise two published
// numbers were never comparable in the first place.
//
// The class comment fixes what both arms must answer: the repeated unit is a run of 'a' ending in
// 'b' and the target is a run of 'a' ending in 'c', so no repetition of the unit ever contains the
// target, both arms exhaust their candidate lengths, and the answer is the "no repeat count" this
// problem reserves for a target that never occurs.
public sealed partial class RepeatedStringMatchBenchmarksTests
{
    private const int SmallestLength = 200;

    // LeetCode 686 has no repeat count for a target that never occurs.
    private const int ExpectedNoRepeatCount = -1;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().StringContains(),
            BuildHarness().StringContains());

    [Fact]
    public void StringContains_TargetNeverOccurs_AgreesWithPrefixFunctionSearchContains()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedNoRepeatCount, harness.StringContains());
        Assert.Equal(harness.PrefixFunctionSearchContains(), harness.StringContains());
    }

    [Fact]
    public void PrefixFunctionSearchContains_TargetNeverOccurs_AgreesWithStringContains()
    {
        var harness = BuildHarness();

        Assert.Equal(ExpectedNoRepeatCount, harness.PrefixFunctionSearchContains());
        Assert.Equal(harness.StringContains(), harness.PrefixFunctionSearchContains());
    }

    private static RepeatedStringMatchBenchmarks BuildHarness()
    {
        var harness = new RepeatedStringMatchBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
