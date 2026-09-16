using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.PeaksInArrayII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PeaksInArrayIISolution's, the same methods
// PeaksInArrayIITests proves correct. Queries alternate type-1 full-array
// range counts with type-2 point updates, so the O((r-l)^3) rescan the
// brute-force arm pays per range query has real (cubic) work to do at every
// step, against the segment-tree arm's O(log n) query and O(log n) update.
// Length is kept well below LC 4017's own 1e5 ceiling because the brute-force
// arm exists only as a correctness baseline, not a contest-scale contender -
// at Length in the low hundreds it is already the dominant cost by a wide
// margin.
[MemoryDiagnoser]
public class PeaksInArrayIIBenchmarks
{
    // LC problem number, used as the deterministic seed for value generation.
    private const int RandomSeed = 4017;

    private int[] _nums = [];
    private int[][] _queries = [];

    [Params(30, 150)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => (_nums, _queries) = PeakQueryWorkloads.Build(Length, RandomSeed);

    [Benchmark(Baseline = true)]
    public List<long> BruteForce() => PeaksInArrayIISolution.CountPeakSubarraysByBruteForce(_nums, _queries);

    [Benchmark]
    public List<long> SegmentTree() => PeaksInArrayIISolution.CountPeakSubarraysBySegmentTree(_nums, _queries);
}
