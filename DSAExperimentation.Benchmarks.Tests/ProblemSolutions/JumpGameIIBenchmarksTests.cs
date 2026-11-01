using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for JumpGameIIBenchmarks (ARCHITECTURE 17.9): BenchmarkArmsTests already holds
// the arms to agreeing on the smallest workload, so this asserts only a hop count derived
// independently of either arm. Setup's jump array is fully documented (seeded Random(1), every
// non-final entry drawn from [1, MaxJumpDistanceExclusive), the final entry left at its zeroed
// default), so the expected hop count is derived here from the plain greedy scan over a rebuilt
// array rather than restated from either arm.
public sealed partial class JumpGameIIBenchmarksTests
{
    private const int SmallestLength = 200;
    private const int RandomSeed = 1;
    private const int MinJumpDistance = 1;
    private const int MaxJumpDistance = 10;

    [Fact]
    public void GreedyTwoPointer_SeededJumps_MatchesTheIndependentlyCountedMinimum() =>
        Assert.Equal(IndependentMinJumpCount(RebuildJumps()), BuildHarness().GreedyTwoPointer());

    private static JumpGameIIBenchmarks BuildHarness()
    {
        var harness = new JumpGameIIBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }

    // The benchmark's own generator, rebuilt from its documented shape: seeded Random(1) over the
    // same open jump range, with the last index left zero because a jump from it is never taken.
    private static int[] RebuildJumps()
    {
        var random = new Random(RandomSeed);
        var jumps = new int[SmallestLength];

        for (var i = 0; i < SmallestLength - 1; i++)
        {
            jumps[i] = random.Next(MinJumpDistance, MaxJumpDistance + 1);
        }

        return jumps;
    }

    // The textbook greedy scan, written independently of either arm: extend the running farthest
    // reach, and start a new hop each time the scan arrives at the end of the current hop's window.
    private static int IndependentMinJumpCount(int[] jumps)
    {
        var hops = 0;
        var windowEnd = 0;
        var farthest = 0;

        for (var i = 0; i < jumps.Length - 1; i++)
        {
            farthest = Math.Max(farthest, i + jumps[i]);

            if (i == windowEnd)
            {
                hops++;
                windowEnd = farthest;
            }
        }

        return hops;
    }
}
