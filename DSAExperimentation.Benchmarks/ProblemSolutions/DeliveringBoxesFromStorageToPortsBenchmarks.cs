using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.DeliveringBoxesFromStorageToPorts;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are DeliveringBoxesFromStorageToPortsSolution's, the
// same methods DeliveringBoxesFromStorageToPortsSolutionTests proves correct - rescanning
// the valid window at every position (O(n * maxBoxes)) against this repo's
// MonotonicDeque carrying the window's minimum (O(n), each index pushed and popped at
// most once). The seeded boxes and their prefix scans are built into a
// BoxDeliverySchedule in [GlobalSetup], so only the dp sweep is measured.
public class DeliveringBoxesFromStorageToPortsBenchmarks
{
    private const int MaxBoxes = 50;
    private const int MaxWeight = 150;
    private const int RandomSeed = 1687; // LC problem number
    private const int SyntheticValueExclusiveUpperBound = 6;

    private BoxDeliverySchedule _schedule = null!;

    [Params(2_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var ports = SeededDraws.Values(Length, 1, SyntheticValueExclusiveUpperBound, random);
        var weights = SeededDraws.Values(Length, 1, SyntheticValueExclusiveUpperBound, random);

        _schedule = BoxDeliverySchedule.Build(
            [.. Enumerable.Range(0, Length).Select(i => new[] { ports[i], weights[i] })]);
    }

    [Benchmark(Baseline = true)]
    public int RescanWindowEachPosition() =>
        DeliveringBoxesFromStorageToPortsSolution.MinTripsByWindowRescan(_schedule, MaxBoxes, MaxWeight);

    [Benchmark]
    public int MonotonicDequeDp() =>
        DeliveringBoxesFromStorageToPortsSolution.MinTripsByMonotonicDeque(_schedule, MaxBoxes, MaxWeight);
}
