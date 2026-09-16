using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CreateMaximumNumberBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for the same question - a naive subsequence scan against the monotonic-stack merge - so a
// harness whose arms disagree is timing two different problems. Setup draws both digit arrays from one
// fixed seed and asks for as many digits as each array holds, and the maximum number of that many
// digits is the whole answer, so the reading's documented shape is exactly Length digits; the same
// Length must rebuild the same two arrays and with them the same maximum.
public sealed partial class CreateMaximumNumberBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload()
    {
        Assert.Equal(SmallestLength, BuildHarness().NaiveSubsequenceScan().Length);
        Assert.Equal(
            AnswerText.Of(BuildHarness().NaiveSubsequenceScan()),
            AnswerText.Of(BuildHarness().NaiveSubsequenceScan()));
    }

    [Fact]
    public void NaiveSubsequenceScan_TwoTwentyDigitArrays_AgreesWithMonotonicStackSubsequence()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.MonotonicStackSubsequence()),
            AnswerText.Of(harness.NaiveSubsequenceScan()));
    }

    [Fact]
    public void MonotonicStackSubsequence_TwoTwentyDigitArrays_AgreesWithNaiveSubsequenceScan()
    {
        var harness = BuildHarness();

        Assert.Equal(
            AnswerText.Of(harness.NaiveSubsequenceScan()),
            AnswerText.Of(harness.MonotonicStackSubsequence()));
    }

    private static CreateMaximumNumberBenchmarks BuildHarness()
    {
        var harness = new CreateMaximumNumberBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
