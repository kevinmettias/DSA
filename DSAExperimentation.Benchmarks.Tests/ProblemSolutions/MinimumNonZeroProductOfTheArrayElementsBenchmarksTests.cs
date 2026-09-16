using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumNonZeroProductOfTheArrayElementsBenchmarks (ARCHITECTURE 17.9):
// both arms are MinimumNonZeroProductOfTheArrayElementsSolution's, the same methods
// MinimumNonZeroProductOfTheArrayElementsTests proves correct, and both return the same modular
// product for LeetCode's own p. Repeated multiplication and exponentiation by squaring are
// competing strategies for that one number, so arms that disagree are timing two different
// problems.
//
// There is no workload to rebuild here: the input is the [Params] value itself, so the harness
// is a bare initializer and the arms are compared directly.
public sealed partial class MinimumNonZeroProductOfTheArrayElementsBenchmarksTests
{
    // The smallest declared [Params] value: it already puts the exponentiation strategy against
    // a real repeated-multiplication run, without the larger case's multiply count.
    private const int SmallestPower = 15;

    [Fact]
    public void RepeatedModularMultiplication_AgreesWithModPowBySquaring()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.ModPowBySquaring(), harness.RepeatedModularMultiplication());
    }

    [Fact]
    public void ModPowBySquaring_AgreesWithRepeatedModularMultiplication()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.RepeatedModularMultiplication(), harness.ModPowBySquaring());
    }

    private static MinimumNonZeroProductOfTheArrayElementsBenchmarks BuildHarness() =>
        new() { Power = SmallestPower };
}
