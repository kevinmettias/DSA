using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MaximizeAlternatingSumUsingSwaps;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeAlternatingSumUsingSwapsSolution's, the same
// methods MaximizeAlternatingSumUsingSwapsSolutionTests proves correct. Setup wires up
// roughly ElementCount/2 random swap pairs over ElementCount indices
// (MaximizeAlternatingSumUsingSwapsWorkloads), so the workload has a handful of
// nontrivial connected components rather than ElementCount singletons.
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
        _nums = SeededDraws.Values(ElementCount, 1, ValueUpperBound, random);
        _swaps = MaximizeAlternatingSumUsingSwapsWorkloads.BuildSwaps(ElementCount, random);
    }

    [Benchmark(Baseline = true)]
    public long ComponentBfs() =>
        MaximizeAlternatingSumUsingSwapsSolution.MaximumAlternatingSumByComponentBfs(_nums, _swaps);

    [Benchmark]
    public long DisjointSet() =>
        MaximizeAlternatingSumUsingSwapsSolution.MaximumAlternatingSumByDisjointSet(_nums, _swaps);
}
