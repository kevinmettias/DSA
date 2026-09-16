using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PowerOfTwoBenchmarks (ARCHITECTURE 17.9): its two arms are competing strategies
// for one question - is the operand exactly 2^k? - so a harness whose arms disagree is timing two
// different problems. The class carries no [Params] and no [GlobalSetup]: its single operand is a
// fixed large prime, chosen so neither arm gets an early exit.
//
// That operand is not a power of two, so both arms answer no and the agreement witnessed here is
// weak by construction: it catches an arm that ever says yes, but an arm stuck on no would pass it.
// The operand is a private constant of the benchmark, so there is no second input this test could
// pin the verdict with; that is a fixture decision, not a gap in this assertion.
public sealed partial class PowerOfTwoBenchmarksTests
{
    [Fact]
    public void IsPowerOfTwoByRepeatedDivision_AgreesWithBitTrick()
    {
        var harness = new PowerOfTwoBenchmarks();

        Assert.Equal(harness.IsPowerOfTwoByBitTrick(), harness.IsPowerOfTwoByRepeatedDivision());
    }

    [Fact]
    public void IsPowerOfTwoByBitTrick_AgreesWithRepeatedDivision()
    {
        var harness = new PowerOfTwoBenchmarks();

        Assert.Equal(harness.IsPowerOfTwoByRepeatedDivision(), harness.IsPowerOfTwoByBitTrick());
    }
}
