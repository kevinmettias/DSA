using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for CountTheNumberOfComputerUnlockingPermutationsBenchmarks (ARCHITECTURE 17.9):
// its two arms are competing strategies for the same question - a DFS over every legal unlock order
// against the closed (n-1)! form - so a harness whose arms disagree is timing two different problems,
// not two ways of answering one. Setup draws the complexities from one fixed seed, so the same
// ComputerCount must rebuild the same workload; otherwise two published numbers were never
// comparable in the first place.
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
    public void Setup_SameComputerCount_RebuildsTheSameSolvableWorkload()
    {
        Assert.Equal(ExpectedUnlockOrders, BuildHarness().FactorialFormula());
        Assert.Equal(BuildHarness().FactorialFormula(), BuildHarness().FactorialFormula());
    }

    [Fact]
    public void Backtracking_MinimumComplexityFirst_AgreesWithFactorialFormula()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.FactorialFormula(), harness.Backtracking());
    }

    [Fact]
    public void FactorialFormula_MinimumComplexityFirst_AgreesWithBacktracking()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Backtracking(), harness.FactorialFormula());
    }

    private static CountTheNumberOfComputerUnlockingPermutationsBenchmarks BuildHarness()
    {
        var harness = new CountTheNumberOfComputerUnlockingPermutationsBenchmarks { ComputerCount = SmallestComputerCount };
        harness.Setup();

        return harness;
    }
}
