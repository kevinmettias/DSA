using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidParenthesesBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question, so a harness whose arms disagree is timing two different
// problems. Setup builds the workload from a seed, so the same pair count must rebuild the same
// bracket string - and since the generator claims to emit a properly nested string, that claim is
// checked here rather than assumed.
public sealed partial class ValidParenthesesBenchmarksTests
{
    private const int SmallestPairCount = 64;

    [Fact]
    public void Setup_GeneratesAProperlyNestedWorkload() =>
        Assert.True(BuildHarness().BracketStack());

    [Fact]
    public void RepeatedPairRemoval_AgreesWithBracketStack()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BracketStack(), harness.RepeatedPairRemoval());
    }

    private static ValidParenthesesBenchmarks BuildHarness()
    {
        var harness = new ValidParenthesesBenchmarks { PairCount = SmallestPairCount };
        harness.Setup();

        return harness;
    }
}
