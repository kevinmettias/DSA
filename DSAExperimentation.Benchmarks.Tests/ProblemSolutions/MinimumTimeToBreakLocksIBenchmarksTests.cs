using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for MinimumTimeToBreakLocksIBenchmarks (ARCHITECTURE 17.9): its two arms are
// competing strategies for the same question - the shortest total time to break every lock with the
// recharge in play - so a harness whose arms disagree is timing two different problems. The permuting
// arm enumerates every order and the bitmask arm caches over subsets, so agreement is what proves the
// memo's subset state carries everything the explicit order carried. Both arms read the one strength
// array [GlobalSetup] built from a fixed seed, so the same LockCount must rebuild the same strengths.
public sealed partial class MinimumTimeToBreakLocksIBenchmarksTests
{
    private const int SmallestLockCount = 4;

    [Fact]
    public void Setup_SameLockCount_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BitmaskMemo(), BuildHarness().BitmaskMemo());

    [Fact]
    public void BitmaskMemo_SameStrengthRun_AgreesWithPermutationBruteForce()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.PermutationBruteForce(), harness.BitmaskMemo());
    }

    [Fact]
    public void PermutationBruteForce_SameStrengthRun_AgreesWithBitmaskMemo()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BitmaskMemo(), harness.PermutationBruteForce());
    }

    private static MinimumTimeToBreakLocksIBenchmarks BuildHarness()
    {
        var harness = new MinimumTimeToBreakLocksIBenchmarks { LockCount = SmallestLockCount };
        harness.Setup();

        return harness;
    }
}
