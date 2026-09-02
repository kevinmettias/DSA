using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PeaksInArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PeaksInArraySolution's, the same methods
// PeaksInArrayTests proves correct. Queries alternate type-1 range counts over
// windows spanning most of the array with type-2 point updates, so the O(n)
// rescan the brute-force arm pays per range query has real work to do at every
// step, against the Fenwick-tree arm's O(log n) query and O(log n) update.
[MemoryDiagnoser]
public class PeaksInArrayBenchmarks
{
    // LC problem number, used as the deterministic seed for value generation.
    private const int RandomSeed = 3187;

    private int[] _nums = null!;
    private int[][] _queries = null!;

    [Params(200, 2_000)]
    public int Length;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, Length)).ToArray();
        _queries = new int[Length][];

        for (var i = 0; i < Length; i++)
        {
            _queries[i] = i % 2 == 0
                ? [1, 0, Length - 1]
                : [2, random.Next(Length), random.Next(1, Length)];
        }
    }

    [Benchmark(Baseline = true)]
    public List<int> BruteForce() => PeaksInArraySolution.CountPeaksByBruteForce(_nums, _queries);

    [Benchmark]
    public List<int> FenwickTree() => PeaksInArraySolution.CountPeaksByFenwickTree(_nums, _queries);
}
