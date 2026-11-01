using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumWindowSubstringBenchmarks (ARCHITECTURE 17.9): the baseline's answer
// against the one the workload's shape fixes, which the generic check cannot supply - it holds the
// sliding window to the baseline. The fixture's target is three characters that deliberately do not
// occur in the text, so neither strategy can satisfy "missing == 0" and the answer is the no-window
// answer whatever text was drawn.
public sealed partial class MinimumWindowSubstringBenchmarksTests
{
    private const int SmallestLength = 200;

    // LeetCode 76's answer when no window covers every required character.
    private const string NoWindowAnswer = "";

    [Fact]
    public void BruteForce_TargetAbsentFromText_ReportsNoWindow() =>
        Assert.Equal(NoWindowAnswer, BuildHarness().BruteForce());

    private static MinimumWindowSubstringBenchmarks BuildHarness()
    {
        var harness = new MinimumWindowSubstringBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
