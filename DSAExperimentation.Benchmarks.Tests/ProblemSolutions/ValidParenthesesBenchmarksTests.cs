using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ValidParenthesesBenchmarks (ARCHITECTURE 17.9): the generic check holds the
// arms to agreeing with each other, which a shared wrong verdict would pass. Setup's generator
// claims to emit a properly nested string, so that claim is checked here rather than assumed.
public sealed partial class ValidParenthesesBenchmarksTests
{
    private const int SmallestPairCount = 64;

    [Fact]
    public void Setup_GeneratesAProperlyNestedWorkload() =>
        Assert.True(BuildHarness().BracketStack());

    private static ValidParenthesesBenchmarks BuildHarness()
    {
        var harness = new ValidParenthesesBenchmarks { PairCount = SmallestPairCount };
        harness.Setup();

        return harness;
    }
}
