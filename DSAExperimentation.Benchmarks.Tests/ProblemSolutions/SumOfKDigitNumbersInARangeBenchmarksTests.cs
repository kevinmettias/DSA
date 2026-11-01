using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfKDigitNumbersInARangeBenchmarks (ARCHITECTURE 17.9): the class has no
// [GlobalSetup] - its digit range and DigitCount are the whole workload. Both arms answer with a
// bare long, and the total is decisive rather than merely agreed: the range is fixed at 0..9, so for
// DigitCount = 4 the enumeration's answer is the sum of every four-digit string 0000..9999, which is
// 45 * 10^3 per position over 4 positions, and that sum is well below the modulus the closed form
// reduces under. The baseline is pinned to it; the generic arm check holds the closed form to the
// baseline's answer.
public sealed partial class SumOfKDigitNumbersInARangeBenchmarksTests
{
    private const int SmallestDigitCount = 4;
    private const long ExpectedSumOfEveryFourDigitString = 49_995_000;

    [Fact]
    public void BruteForceEnumeration_DigitsZeroThroughNine_SumsEveryFourDigitString() =>
        Assert.Equal(
            ExpectedSumOfEveryFourDigitString,
            new SumOfKDigitNumbersInARangeBenchmarks { DigitCount = SmallestDigitCount }.BruteForceEnumeration());
}
