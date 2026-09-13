using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.JumpGameV;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are JumpGameVSolution's, the same methods JumpGameVTests
// proves correct. [GlobalSetup] builds nothing but LeetCode's own int[] input, so
// there is no hoisted overload to take - building the reachability DAG is part of
// what the TopologicalSortLongestPath arm has to pay for, which is exactly the
// trade being measured against the memoized DFS's zero-setup recursion. Values are
// a random shuffle (no ties) so every index has a genuinely different rank,
// keeping the DAG's longest path - and therefore the DFS's recursion depth -
// realistic instead of degenerate.
[MemoryDiagnoser]
public class JumpGameVBenchmarks
{
    private const int MaxJumpDistance = 5;

    // LeetCode problem number, reused as the RNG seed for reproducible benchmark input.
    private const int RandomSeed = 1340;

    [Params(200, 2_000)]
    public int Length;

    private int[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _arr = [.. Enumerable.Range(0, Length)];

        for (var i = _arr.Length - 1; i > 0; i--)
        {
            var swapIndex = random.Next(i + 1);
            (_arr[i], _arr[swapIndex]) = (_arr[swapIndex], _arr[i]);
        }
    }

    [Benchmark(Baseline = true)]
    public int MemoizedDfsPerStart() =>
        JumpGameVSolution.MaxIndicesVisitedByMemoizedDfs(_arr, MaxJumpDistance);

    [Benchmark]
    public int TopologicalSortLongestPath() =>
        JumpGameVSolution.MaxIndicesVisitedByTopologicalSort(_arr, MaxJumpDistance);
}
