using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MaximumNestingDepthOfTheParenthesesBenchmarks (ARCHITECTURE 17.9): both arms
// are MaximumNestingDepthOfTheParenthesesSolution's competing strategies for one question - a plain
// running-depth counter against a Stack<char> of unmatched openers - so a harness whose arms
// disagree is timing two different problems. Both answer with the maximum depth reached, compared
// directly.
public sealed partial class MaximumNestingDepthOfTheParenthesesBenchmarksTests
{
    private const int SmallestPairCount = 1_000;

    [Fact]
    public void Setup_SamePairCount_RebuildsTheSameBalancedExpression()
    {
        var first = BuildHarness();
        var second = BuildHarness();

        // The expression is private, so the rebuild is pinned through the depth it reaches: the
        // same PairCount must draw the same seeded balanced expression and score it identically.
        Assert.Equal(first.RunningDepthCounter(), second.RunningDepthCounter());
        Assert.Equal(first.StackOfOpeners(), second.StackOfOpeners());
    }

    [Fact]
    public void RunningDepthCounter_SeededBalancedExpression_AgreesWithStackOfOpeners()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.StackOfOpeners(), harness.RunningDepthCounter());
    }

    [Fact]
    public void StackOfOpeners_SeededBalancedExpression_AgreesWithRunningDepthCounter()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RunningDepthCounter(), harness.StackOfOpeners());
    }

    private static MaximumNestingDepthOfTheParenthesesBenchmarks BuildHarness()
    {
        var harness = new MaximumNestingDepthOfTheParenthesesBenchmarks { PairCount = SmallestPairCount };
        harness.Setup();

        return harness;
    }
}
