using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PlusOneBenchmarks (ARCHITECTURE 17.9): the baseline's answer against a
// length derived from the workload's shape, which the generic check cannot supply - it holds the
// digit stack to the baseline. Setup hands both arms the all-nines operand the class comment names,
// so the carry cascades through every position and the number gains a leading digit.
public sealed partial class PlusOneBenchmarksTests
{
    private const int SmallestLength = 10;

    // The all-nines operand grows the number by exactly one digit.
    private const int GrownLength = SmallestLength + 1;

    [Fact]
    public void ArrayWalk_AllNines_GrowsTheNumberByOneDigit() =>
        Assert.Equal(GrownLength, BuildHarness().ArrayWalk().Length);

    private static PlusOneBenchmarks BuildHarness()
    {
        var harness = new PlusOneBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
