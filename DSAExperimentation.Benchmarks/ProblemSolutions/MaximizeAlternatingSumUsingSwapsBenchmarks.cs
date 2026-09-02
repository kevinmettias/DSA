using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximizeAlternatingSumUsingSwaps;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeAlternatingSumUsingSwapsSolution's, the same
// methods MaximizeAlternatingSumUsingSwapsTests proves correct. Setup wires up
// roughly N/2 random swap pairs over N indices, so the workload has a handful of
// nontrivial connected components rather than N singletons.
[MemoryDiagnoser]
public class MaximizeAlternatingSumUsingSwapsBenchmarks
{
    private const int RandomSeed = 3695; // LC problem number
    private const int ValueUpperBound = 1_000_000_000; // exclusive upper bound; LC 3695 allows values up to 1e9

    [Params(1_000, 10_000)]
    public int N;

    private int[] _nums = null!;
    private int[][] _swaps = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, N).Select(_ => random.Next(1, ValueUpperBound)).ToArray();
        _swaps = Enumerable.Range(0, N / 2)
            .Select(_ => new[] { random.Next(N), random.Next(N) })
            .Where(swap => swap[0] != swap[1])
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public long ComponentBfs() =>
        MaximizeAlternatingSumUsingSwapsSolution.MaximumAlternatingSumByComponentBfs(_nums, _swaps);

    [Benchmark]
    public long DisjointSet() =>
        MaximizeAlternatingSumUsingSwapsSolution.MaximumAlternatingSumByDisjointSet(_nums, _swaps);
}
