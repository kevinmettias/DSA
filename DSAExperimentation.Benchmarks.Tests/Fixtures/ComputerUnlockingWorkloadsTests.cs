using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for ComputerUnlockingWorkloads (ARCHITECTURE 17.7). The reading depends on
// every generated instance being solvable: the first complexity is the minimum possible value
// and every other one is drawn strictly above it, which keeps the backtracking arm on its full
// uncollapsed search tree instead of pruning out early.
public sealed partial class ComputerUnlockingWorkloadsTests
{
    private const int ComputerCount = 8;
    private const int Seed = 3577; // LC problem number
    private const int MinComplexity = 1;
    private const int MaxComplexity = 999;
    private const int FixedLeadingComplexityCount = 1;

    [Fact]
    public void BuildSolvable_ComputerCount_ReturnsOneComplexityPerComputer() =>
        Assert.Equal(ComputerCount, ComputerUnlockingWorkloads.BuildSolvable(ComputerCount, Seed).Length);

    [Fact]
    public void BuildSolvable_FirstComplexity_IsTheMinimum()
    {
        var complexity = ComputerUnlockingWorkloads.BuildSolvable(ComputerCount, Seed);

        Assert.Equal(MinComplexity, complexity[0]);
    }

    [Fact]
    public void BuildSolvable_EveryOtherComplexity_StaysStrictlyAboveTheFirst()
    {
        var complexity = ComputerUnlockingWorkloads.BuildSolvable(ComputerCount, Seed);

        Assert.All(complexity.Skip(FixedLeadingComplexityCount), value => Assert.True(value > complexity[0]));
    }

    [Fact]
    public void BuildSolvable_EveryComplexity_StaysWithinTheDocumentedBound()
    {
        var complexity = ComputerUnlockingWorkloads.BuildSolvable(ComputerCount, Seed);

        Assert.All(complexity, value => Assert.InRange(value, MinComplexity, MaxComplexity));
    }

    [Fact]
    public void BuildSolvable_SameSeed_ReturnsTheSameWorkload() =>
        Assert.Equal(
            ComputerUnlockingWorkloads.BuildSolvable(ComputerCount, Seed),
            ComputerUnlockingWorkloads.BuildSolvable(ComputerCount, Seed));
}
