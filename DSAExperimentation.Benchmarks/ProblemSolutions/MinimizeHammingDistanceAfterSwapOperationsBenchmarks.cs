using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimizeHammingDistanceAfterSwapOperations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimizeHammingDistanceAfterSwapOperationsSolution's,
// the same methods MinimizeHammingDistanceAfterSwapOperationsTests proves correct.
// Adjacency-list BFS to find each index's swap-component (the same baseline
// SmallestStringWithSwapsBenchmarks uses) vs. this repo's own DisjointSet(n) -
// O(1) Union per allowed swap and O(a(n)) Find per index instead of a queue-driven
// walk. Both finish the per-component shortfall the same way, with a frequency
// count over the component's own values.
//
// LeetCode's own input shape - two arrays and a jagged swap-pair array - is already
// what both strategies take, so [GlobalSetup] only decides how large the workload is
// and hands the finished input straight over; there is no construction left for a
// hoisted overload to lift out of the measured methods.
[MemoryDiagnoser]
public class MinimizeHammingDistanceAfterSwapOperationsBenchmarks
{
    private const int RandomSeed = 1722; // LC problem number
    private const int MaxValueExclusive = 50;

    private int[] _source = [];

    private int[] _target = [];
    private int[][] _allowedSwaps = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _source = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
        _target = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();

        _allowedSwaps = new int[Length][];
        for (var i = 0; i < Length; i++)
        {
            _allowedSwaps[i] = [random.Next(Length), random.Next(Length)];
        }
    }

    [Benchmark(Baseline = true)]
    public int AdjacencyListBfs() =>
        MinimizeHammingDistanceAfterSwapOperationsSolution.MinimumHammingDistanceByAdjacencyListBfs(
            _source, _target, _allowedSwaps);

    [Benchmark]
    public int DisjointSetUnionFind() =>
        MinimizeHammingDistanceAfterSwapOperationsSolution.MinimumHammingDistanceByDisjointSet(
            _source, _target, _allowedSwaps);
}
