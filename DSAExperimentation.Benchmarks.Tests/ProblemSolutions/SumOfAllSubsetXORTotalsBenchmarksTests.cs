using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for SumOfAllSubsetXORTotalsBenchmarks (ARCHITECTURE 17.9): both arms are
// SumOfAllSubsetXORTotalsSolution's, so a harness whose arms disagree is timing two different
// questions. Both answer with a bare int and the workload is a seeded draw, so beside the agreement
// each arm is pinned to an oracle derived from that workload rather than read back out of an arm:
// a bit position that any drawn value sets is set in the XOR of exactly half of the 2^Length subsets,
// so the total over all subsets is the bitwise OR of the drawn values scaled by 2^(Length-1).
public sealed partial class SumOfAllSubsetXORTotalsBenchmarksTests
{
    // Mirrors SumOfAllSubsetXORTotalsBenchmarks' own private RandomSeed and MaxValueBitWidth.
    private const int RandomSeed = 1863;
    private const int MaxValueBitWidth = 20;
    private const int SmallestLength = 10;

    [Fact]
    public void Setup_SameLength_RebuildsTheSameWorkload() =>
        Assert.Equal(BuildHarness().BruteForceBitmask(), BuildHarness().BruteForceBitmask());

    [Fact]
    public void BruteForceBitmask_SeededValues_AgreesWithTheOtherArmAndTheOrScalingRule()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.Backtracking(), harness.BruteForceBitmask());
        Assert.Equal(ExpectedXorTotal(), harness.BruteForceBitmask());
    }

    [Fact]
    public void Backtracking_SeededValues_AgreesWithTheOtherArmAndTheOrScalingRule()
    {
        var harness = BuildHarness();

        Assert.Equal(harness.BruteForceBitmask(), harness.Backtracking());
        Assert.Equal(ExpectedXorTotal(), harness.Backtracking());
    }

    // Mirrors [GlobalSetup]'s own draw of Length values from [1, 2^MaxValueBitWidth).
    private static int[] WorkloadValues()
    {
        var random = new Random(RandomSeed);

        return Enumerable.Range(0, SmallestLength)
            .Select(_ => random.Next(1, 1 << MaxValueBitWidth))
            .ToArray();
    }

    // Each set position contributes its place value once per subset whose XOR sets it, and exactly
    // half of the subsets set it, so the whole total is the OR of the values shifted by Length - 1 -
    // derived from the drawn values, not from either arm.
    private static int ExpectedXorTotal() =>
        WorkloadValues().Aggregate(0, (total, value) => total | value) << (SmallestLength - 1);

    private static SumOfAllSubsetXORTotalsBenchmarks BuildHarness()
    {
        var harness = new SumOfAllSubsetXORTotalsBenchmarks { Length = SmallestLength };
        harness.Setup();

        return harness;
    }
}
