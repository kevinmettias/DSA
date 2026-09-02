using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.SegmentTree;
using DSAExperimentation.LeetCode.MinimumStabilityFactorOfArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumStabilityFactorOfArraySolution's, the same
// methods MinimumStabilityFactorOfArrayTests proves correct. The composed arm is
// handed a prebuilt SegmentTree<int,GcdOperation>, so the O(n log n) tree build is
// charged to [GlobalSetup] rather than the binary search being measured.
[MemoryDiagnoser]
public class MinimumStabilityFactorOfArrayBenchmarks
{
    private const int Seed = 3605;
    private const int MaxModifications = 20;

    [Params(200, 2_000)]
    public int Length;

    private int[] _nums = null!;
    private SegmentTree<int, GcdOperation> _gcdTree = null!;

    [GlobalSetup]
    public void Setup()
    {
        _nums = StabilityFactorWorkloads.Build(Length, seed: Seed);
        _gcdTree = new SegmentTree<int, GcdOperation>(_nums);
    }

    [Benchmark(Baseline = true)]
    public int BruteForceGcdScan() =>
        MinimumStabilityFactorOfArraySolution.MinStabilityByBruteForceGcdScan(_nums, MaxModifications);

    [Benchmark]
    public int SegmentTreeGcd() =>
        MinimumStabilityFactorOfArraySolution.MinStabilityBySegmentTreeGcd(_gcdTree, MaxModifications);
}
