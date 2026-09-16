using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for RemoveOutermostParenthesesBenchmarks (ARCHITECTURE 17.9): both arms are
// RemoveOutermostParenthesesSolution's, competing strategies for the same question - a running depth
// counter against this repo's own Stack<char> of unmatched openers - so a harness whose arms disagree
// strips two different strings. Setup sizes and seeds a balanced expression of PairCount pairs from
// one fixture, so the same PairCount must rebuild the same expression.
public sealed partial class RemoveOutermostParenthesesBenchmarksTests
{
    private const int SmallestPairCount = 1_000;

    [Fact]
    public void Setup_SamePairCount_RebuildsTheSameWorkload() =>
        Assert.Equal(
            BuildHarness().RunningDepthCounter(),
            BuildHarness().RunningDepthCounter());

    [Fact]
    public void RunningDepthCounter_AgreesWithStackOfOpeners()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackOfOpeners(), harness.RunningDepthCounter());
    }

    [Fact]
    public void StackOfOpeners_AgreesWithRunningDepthCounter()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RunningDepthCounter(), harness.StackOfOpeners());
    }

    private static RemoveOutermostParenthesesBenchmarks BuildHarness()
    {
        var harness = new RemoveOutermostParenthesesBenchmarks { PairCount = SmallestPairCount };
        harness.Setup();

        return harness;
    }
}
