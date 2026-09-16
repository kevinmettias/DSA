using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximizeAlternatingSumUsingSwaps;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeAlternatingSumUsingSwapsSolution's, the same
// methods MaximizeAlternatingSumUsingSwapsTests proves correct. Setup wires up
// roughly ElementCount/2 random swap pairs over ElementCount indices, so the
// workload has a handful of nontrivial connected components rather than
// ElementCount singletons.
[MemoryDiagnoser]
public class MaximizeAlternatingSumUsingSwapsBenchmarks
{
    private const int RandomSeed = 3695; // LC problem number
    private const int ValueUpperBound = 1_000_000_000; private int[] _nums = [];

    private int[][] _swaps = [];
    // exclusive upper bound; LC 3695 allows values up to 1e9

    [Params(1_000, 10_000)]
    public int ElementCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, ElementCount)
            .Select(_ => random.Next(1, ValueUpperBound)).ToArray();
        _swaps = Enumerable.Range(0, ElementCount / 2)
            .Select(_ => new[] { random.Next(ElementCount), random.Next(ElementCount) })
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
