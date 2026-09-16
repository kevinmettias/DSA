using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfKDigitNumbersInARangeBenchmarks (ARCHITECTURE 17.9): both arms are
// SumOfKDigitNumbersInARangeSolution's - the c^k enumeration against the modular-repunit closed form -
// so a harness whose arms disagree is timing two different questions. The class has no [GlobalSetup]:
// its digit range and DigitCount are the whole workload, so each arm is called on the constants it
// names directly.
//
// Both answer with a bare long, and the total is decisive rather than merely agreed: the range is
// fixed at 0..9, so for DigitCount = 4 the enumeration's answer is the sum of every four-digit string
// 0000..9999, which is 45 * 10^3 per position over 4 positions, and that sum is well below the
// modulus the closed form reduces under.
public sealed partial class SumOfKDigitNumbersInARangeBenchmarksTests
{
    private const int SmallestDigitCount = 4;
    private const long ExpectedSumOfEveryFourDigitString = 49_995_000;

    [Fact]
    public void BruteForceEnumeration_DigitsZeroThroughNine_SumsEveryFourDigitString()
    {
        var harness = new SumOfKDigitNumbersInARangeBenchmarks { DigitCount = SmallestDigitCount };

        Assert.Equal(harness.ModularRepunit(), harness.BruteForceEnumeration());
        Assert.Equal(ExpectedSumOfEveryFourDigitString, harness.BruteForceEnumeration());
    }

    [Fact]
    public void ModularRepunit_DigitsZeroThroughNine_AgreesWithTheOtherArmOnTheSameTotal()
    {
        var harness = new SumOfKDigitNumbersInARangeBenchmarks { DigitCount = SmallestDigitCount };

        Assert.Equal(harness.BruteForceEnumeration(), harness.ModularRepunit());
        Assert.Equal(ExpectedSumOfEveryFourDigitString, harness.ModularRepunit());
    }
}
