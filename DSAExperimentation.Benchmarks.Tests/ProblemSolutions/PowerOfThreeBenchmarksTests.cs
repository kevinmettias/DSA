using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PowerOfThreeBenchmarks (ARCHITECTURE 17.9): its two arms are competing
// strategies for one question - is the operand exactly 3^k? - so a harness whose arms disagree is
// timing two different problems. Value is the only [Params] axis and it sweeps the two decisive
// operands the class comment names, a true power of three and the integer one below it, so the arms
// must agree on a yes and on a no rather than on one verdict twice. Setup is what wraps the
// precomputed powers-of-three table the binary-search arm searches, so the same Value must rebuild
// that same table.
public sealed partial class PowerOfThreeBenchmarksTests
{
    private const int PowerOfThreeValue = 1_162_261_467;

    private const int NotPowerOfThreeValue = 1_162_261_466;

    [Fact]
    public void Setup_SameValue_RebuildsTheSamePowersTable() =>
        Assert.Equal(
            Harness(PowerOfThreeValue).IsPowerOfThreeByBinarySearch(),
            Harness(PowerOfThreeValue).IsPowerOfThreeByBinarySearch());

    [Fact]
    public void IsPowerOfThreeByDivisionLoop_AgreesWithBinarySearch()
    {
        var powerOfThree = Harness(PowerOfThreeValue);
        var notPowerOfThree = Harness(NotPowerOfThreeValue);

        Assert.Equal(powerOfThree.IsPowerOfThreeByBinarySearch(), powerOfThree.IsPowerOfThreeByDivisionLoop());
        Assert.Equal(
            notPowerOfThree.IsPowerOfThreeByBinarySearch(),
            notPowerOfThree.IsPowerOfThreeByDivisionLoop());
    }

    [Fact]
    public void IsPowerOfThreeByBinarySearch_AgreesWithDivisionLoop()
    {
        var powerOfThree = Harness(PowerOfThreeValue);
        var notPowerOfThree = Harness(NotPowerOfThreeValue);

        Assert.Equal(powerOfThree.IsPowerOfThreeByDivisionLoop(), powerOfThree.IsPowerOfThreeByBinarySearch());
        Assert.Equal(
            notPowerOfThree.IsPowerOfThreeByDivisionLoop(),
            notPowerOfThree.IsPowerOfThreeByBinarySearch());
    }

    private static PowerOfThreeBenchmarks Harness(int value)
    {
        var harness = new PowerOfThreeBenchmarks { Value = value };
        harness.Setup();

        return harness;
    }
}
