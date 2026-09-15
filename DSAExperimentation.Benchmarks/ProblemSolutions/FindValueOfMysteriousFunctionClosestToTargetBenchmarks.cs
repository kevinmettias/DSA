using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindValueOfMysteriousFunctionClosestToTarget;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindValueOfMysteriousFunctionClosestToTargetSolution's - the
// textbook O(n^2) all-subarrays-ANDed-in-place brute force against the O(n log(max(arr)))
// distinct-AND-values scan. Random 20-bit values keep the distinct-value sets at their full
// width, and the target sits mid-range where no single value can reach it, forcing a full scan.
[MemoryDiagnoser]
public class FindValueOfMysteriousFunctionClosestToTargetBenchmarks
{
    private const int Target = 1 << 15; // mid-range target unreachable by any single value, forces a full scan
    private const int RandomSeed = 1521; // LC problem number
    private const int ValueBitWidth = 20;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, 1 << ValueBitWidth)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllSubarrays() =>
        FindValueOfMysteriousFunctionClosestToTargetSolution.ClosestToTargetByBruteForce(_values, Target);

    [Benchmark]
    public int DistinctAndValuesHashMap() =>
        FindValueOfMysteriousFunctionClosestToTargetSolution.ClosestToTargetByDistinctAndValues(_values, Target);
}
