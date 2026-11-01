using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PowerOfThreeBenchmarks (ARCHITECTURE 17.9): the true power of three, which the
// generic check never reaches - it runs at the smallest Value, the integer one below it, where the
// verdict is a no. Value is the only [Params] axis and it sweeps the two decisive operands the class
// comment names, so these tests are what hold the arms to the same yes and a rebuilt harness to the
// same answer there. Setup is what wraps the precomputed powers-of-three table the binary-search arm
// searches.
public sealed partial class PowerOfThreeBenchmarksTests
{
    private const int PowerOfThreeValue = 1_162_261_467;

    [Fact]
    public void Setup_SameValue_RebuildsTheSamePowersTable() =>
        Assert.Equal(
            Harness(PowerOfThreeValue).IsPowerOfThreeByBinarySearch(),
            Harness(PowerOfThreeValue).IsPowerOfThreeByBinarySearch());

    [Fact]
    public void IsPowerOfThreeByDivisionLoop_TruePowerOfThree_AgreesWithBinarySearch()
    {
        var powerOfThree = Harness(PowerOfThreeValue);

        Assert.Equal(powerOfThree.IsPowerOfThreeByBinarySearch(), powerOfThree.IsPowerOfThreeByDivisionLoop());
    }

    private static PowerOfThreeBenchmarks Harness(int value)
    {
        var harness = new PowerOfThreeBenchmarks { Value = value };
        harness.Setup();

        return harness;
    }
}
