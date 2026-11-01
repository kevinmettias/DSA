using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SuperPowBenchmarks (ARCHITECTURE 17.9): both arms answer with a bare int, and
// the exponent is the digits [GlobalSetup] derives from the smaller of the two Exponent values, so the
// remainder is decisive rather than merely agreed: 7^10000 mod 1337 = 574, where 1337 is the modulus
// the solution reduces under. The baseline is pinned to that value; the generic arm check holds
// Horner's arm to the baseline's answer.
public sealed partial class SuperPowBenchmarksTests
{
    private const int SmallestExponent = 10_000;
    private const int ExpectedRemainderForTenThousand = 574;

    [Fact]
    public void RepeatedModularMultiplication_ExponentTenThousand_ReturnsTheKnownRemainder() =>
        Assert.Equal(ExpectedRemainderForTenThousand, BuildHarness().RepeatedModularMultiplication());

    private static SuperPowBenchmarks BuildHarness()
    {
        var harness = new SuperPowBenchmarks { Exponent = SmallestExponent };
        harness.Setup();

        return harness;
    }
}
