using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for NumberOfWaysToRearrangeSticksWithKSticksVisibleBenchmarks (ARCHITECTURE 17.9):
// the factorial enumeration of every arrangement and the memoized unsigned-Stirling recurrence are
// competing strategies for one count, so a harness whose arms disagree is timing two different
// problems. The class has no [GlobalSetup], so a harness is a bare initializer plus the tuned
// StickCount; the smallest tuned value is used because it is the one the factorial arm can still
// afford. VisibleCount is a fixed private constant, so nothing else needs setting.
public sealed partial class NumberOfWaysToRearrangeSticksWithKSticksVisibleBenchmarksTests
{
    private const int SmallestStickCount = 8;

    [Fact]
    public void BruteForcePermutations_SmallestStickCount_AgreesWithMemoizedStirlingRecurrence()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.MemoizedStirlingRecurrence(), harness.BruteForcePermutations());
    }

    [Fact]
    public void MemoizedStirlingRecurrence_SmallestStickCount_AgreesWithBruteForcePermutations()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForcePermutations(), harness.MemoizedStirlingRecurrence());
    }

    private static NumberOfWaysToRearrangeSticksWithKSticksVisibleBenchmarks BuildHarness() =>
        new() { StickCount = SmallestStickCount };
}
