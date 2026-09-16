using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PowerOfFourBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - is the operand exactly 4^k? - so a harness whose arms disagree is
// timing two different problems. Value is the only [Params] axis and it sweeps the two decisive
// operands the class comment names, a true power of four and the integer one below it, so the arms
// must agree on a yes and on a no rather than on one verdict twice.
public sealed partial class PowerOfFourBenchmarksTests
{
    private const int PowerOfFourValue = 1_073_741_824;

    private const int NotPowerOfFourValue = 1_073_741_823;

    [Fact]
    public void IsPowerOfFourByDivisionLoop_AgreesWithBinarySearch()
    {
        var powerOfFour = Harness(PowerOfFourValue);
        var notPowerOfFour = Harness(NotPowerOfFourValue);

        Assert.Equal(powerOfFour.IsPowerOfFourByBinarySearch(), powerOfFour.IsPowerOfFourByDivisionLoop());
        Assert.Equal(
            notPowerOfFour.IsPowerOfFourByBinarySearch(),
            notPowerOfFour.IsPowerOfFourByDivisionLoop());
    }

    [Fact]
    public void IsPowerOfFourByBinarySearch_AgreesWithDivisionLoop()
    {
        var powerOfFour = Harness(PowerOfFourValue);
        var notPowerOfFour = Harness(NotPowerOfFourValue);

        Assert.Equal(powerOfFour.IsPowerOfFourByDivisionLoop(), powerOfFour.IsPowerOfFourByBinarySearch());
        Assert.Equal(
            notPowerOfFour.IsPowerOfFourByDivisionLoop(),
            notPowerOfFour.IsPowerOfFourByBinarySearch());
    }

    private static PowerOfFourBenchmarks Harness(int value) => new() { Value = value };
}
