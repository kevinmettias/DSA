using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.CheckIfThereIsAValidPathInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfThereIsAValidPathInAGridSolution's, the same
// methods CheckIfThereIsAValidPathInAGridTests proves correct - a hand-rolled
// recursive DFS over a bool[,] visited array vs. this repo's own
// DepthFirstSearch.Traverse over a bespoke street-opening successor function. Both
// walk the identical street-compatibility rule, so the comparison isolates the
// traversal machinery (explicit stack + HashSet vs. recursion + array) rather than
// the per-cell logic. Grid generation is charged to [GlobalSetup].
[MemoryDiagnoser]
public class CheckIfThereIsAValidPathInAGridBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1391;

    private const int StreetTypeUpperBound = 7;

    [Params(10, 30)]
    public int Size;

    private int[][] _grid = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => Enumerable.Range(0, Size).Select(_ => random.Next(1, StreetTypeUpperBound)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool RecursiveDfs() =>
        CheckIfThereIsAValidPathInAGridSolution.HasValidPathByRecursiveDfs(_grid);

    [Benchmark]
    public bool DepthFirstSearchTraverse() =>
        CheckIfThereIsAValidPathInAGridSolution.HasValidPathByDepthFirstTraverse(_grid);
}
