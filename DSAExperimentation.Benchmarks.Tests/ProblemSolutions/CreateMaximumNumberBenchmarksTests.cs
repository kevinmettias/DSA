using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CreateMaximumNumberBenchmarks (ARCHITECTURE 17.9): the generic arm check
// compares the arms only with each other, so what stays here is the workload's documented shape. Setup
// asks for as many digits as each digit array holds, and the maximum number of that many digits is the
// whole answer, so the reading's documented shape is exactly Length digits.
public sealed partial class CreateMaximumNumberBenchmarksTests
{
    private const int SmallestLength = 20;

    [Fact]
    public void NaiveSubsequenceScan_TwoTwentyDigitArrays_AnswersLengthDigits() =>
        Assert.Equal(SmallestLength, BuildHarness().NaiveSubsequenceScan().Length);

    private static CreateMaximumNumberBenchmarks BuildHarness()
    {
        var harness = new CreateMaximumNumberBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
