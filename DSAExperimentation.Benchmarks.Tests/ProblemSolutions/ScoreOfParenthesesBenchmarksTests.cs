using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for ScoreOfParenthesesBenchmarks (ARCHITECTURE 17.9): both arms are competing
// strategies for the same question - the score of one balanced-parenthesis string - so a harness
// whose arms disagree is timing two different problems. Setup generates that string from one fixed
// seed, so the same PairCount must rebuild the same expression and with it the same score; neither
// arm mutates the string, so one harness instance is safe to read twice in either order.
public sealed partial class ScoreOfParenthesesBenchmarksTests
{
    private const int SmallestPairCount = 100;

    [Fact]
    public void Setup_SamePairCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().NestedDepthScan(), BuildHarness().NestedDepthScan());

    [Fact]
    public void NestedDepthScan_BalancedExpression_AgreesWithMonotonicStackFold()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MonotonicStackFold(), harness.NestedDepthScan());
    }

    [Fact]
    public void MonotonicStackFold_BalancedExpression_AgreesWithNestedDepthScan()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.NestedDepthScan(), harness.MonotonicStackFold());
    }

    private static ScoreOfParenthesesBenchmarks BuildHarness()
    {
        var harness = new ScoreOfParenthesesBenchmarks { PairCount = SmallestPairCount };
        harness.Setup();

        return harness;
    }
}
