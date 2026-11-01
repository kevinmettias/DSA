using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfComputerUnlockingPermutationsBenchmarks (ARCHITECTURE 17.9):
// BenchmarkArmsTests already holds the arms to agreeing on a rebuilt workload, so this asserts only
// an expected count derived from the workload's construction rather than from either arm.
//
// The complexity array is private, and the workload's defining property - it is always solvable,
// complexity[0] being the strict minimum - is exactly what the arms read, so the setup is asserted
// through that: a solvable instance of ComputerCount computers is unlocked in (ComputerCount - 1)!
// orders, and an unsolvable one in none.
public sealed partial class CountTheNumberOfComputerUnlockingPermutationsBenchmarksTests
{
    private const int SmallestComputerCount = 7;

    private const long ExpectedUnlockOrders = 720; // (SmallestComputerCount - 1)!

    [Fact]
    public void FactorialFormula_MinimumComplexityFirst_CountsEveryOrderOfTheOthers() =>
        Assert.Equal(ExpectedUnlockOrders, BuildHarness().FactorialFormula(SmallestComputerCount));

    private static CountTheNumberOfComputerUnlockingPermutationsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfComputerUnlockingPermutationsBenchmarks();
        harness.Setup();

        return harness;
    }
}
