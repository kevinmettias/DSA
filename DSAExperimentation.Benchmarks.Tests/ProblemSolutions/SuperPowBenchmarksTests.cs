using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SuperPowBenchmarks (ARCHITECTURE 17.9): both arms are SuperPowSolution's - the
// per-unit multiplication loop against Horner's rule with modular squaring - so a harness whose arms
// disagree is timing two different questions. Both answer with a bare int, and the exponent is the
// digits [GlobalSetup] derives from the smaller of the two Exponent values, so the remainder is
// decisive rather than merely agreed: 7^10000 mod 1337 = 574, where 1337 is the modulus the solution
// reduces under. Each arm is pinned to that value next to the agreement.
public sealed partial class SuperPowBenchmarksTests
{
    private const int SmallestExponent = 10_000;
    private const int ExpectedRemainderForTenThousand = 574;

    [Fact]
    public void Setup_SameExponent_RebuildsTheSameDigits() =>
        Assert.Equal(
            BuildHarness().RepeatedModularMultiplication(),
            BuildHarness().RepeatedModularMultiplication());

    [Fact]
    public void RepeatedModularMultiplication_ExponentTenThousand_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.HornerSquaring(), harness.RepeatedModularMultiplication());
        Assert.Equal(ExpectedRemainderForTenThousand, harness.RepeatedModularMultiplication());
    }

    [Fact]
    public void HornerSquaring_ExponentTenThousand_AgreesWithTheOtherArm()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedModularMultiplication(), harness.HornerSquaring());
        Assert.Equal(ExpectedRemainderForTenThousand, harness.HornerSquaring());
    }

    private static SuperPowBenchmarks BuildHarness()
    {
        var harness = new SuperPowBenchmarks { Exponent = SmallestExponent };
        harness.Setup();

        return harness;
    }
}
