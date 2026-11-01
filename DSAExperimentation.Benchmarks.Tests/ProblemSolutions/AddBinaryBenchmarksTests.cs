using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for AddBinaryBenchmarks (ARCHITECTURE 17.9): the generic check holds the arms to each other,
// and this pins the baseline's answer to a value derived without any arm. Setup hands both arms all-ones
// operands, which is the decisive case the class comment names: every digit position carries, so the result
// always grows by one bit. Two Length-bit all-ones numbers sum to a number whose top bit is set and whose bottom
// bit is clear, so the answer is Length ones followed by a zero - asserted literally.
public sealed partial class AddBinaryBenchmarksTests
{
    private const int SmallestLength = 200;

    [Fact]
    public void CharArrayReverse_AllOnesOperands_ReturnsLengthOnesThenAZero() =>
        Assert.Equal(ExpectedCarriedSum, BuildHarness().CharArrayReverse());

    // Both arms are handed the same all-ones pair, so the sum is fixed by arithmetic alone: n ones is 2^n - 1,
    // twice that is 2^(n+1) - 2, whose binary form is n ones with a zero in the last place.
    private static string ExpectedCarriedSum => new string('1', SmallestLength) + "0";

    private static AddBinaryBenchmarks BuildHarness()
    {
        var harness = new AddBinaryBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
