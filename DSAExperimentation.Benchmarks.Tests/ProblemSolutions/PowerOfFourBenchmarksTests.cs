using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PowerOfFourBenchmarks (ARCHITECTURE 17.9): arm agreement on the true power of
// four, which the generic check never reaches - it runs at the smallest Value, the integer one below
// it, where the verdict is a no. Value is the only [Params] axis and it sweeps the two decisive
// operands the class comment names, so these tests are what hold the arms to the same yes.
public sealed partial class PowerOfFourBenchmarksTests
{
    private const int PowerOfFourValue = 1_073_741_824;

    [Fact]
    public void IsPowerOfFourByDivisionLoop_TruePowerOfFour_AgreesWithBinarySearch()
    {
        var powerOfFour = Harness(PowerOfFourValue);

        Assert.Equal(powerOfFour.IsPowerOfFourByBinarySearch(), powerOfFour.IsPowerOfFourByDivisionLoop());
    }

    private static PowerOfFourBenchmarks Harness(int value) => new() { Value = value };
}
