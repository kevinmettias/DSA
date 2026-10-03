using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.CheckIfThereIsAValidPathInAGrid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CheckIfThereIsAValidPathInAGridSolution's, the same
// methods CheckIfThereIsAValidPathInAGridSolutionTests proves correct - a hand-rolled
// recursive DFS over a bool[,] visited array vs. this repo's own
// DepthFirstSearch.Traverse over a bespoke street-opening successor function. Both
// walk the identical street-compatibility rule, so the comparison isolates the
// traversal machinery (explicit stack + HashSet vs. recursion + array) rather than
// the per-cell logic. Grid generation is charged to [GlobalSetup].
public class CheckIfThereIsAValidPathInAGridBenchmarks
{
    // LC problem number, reused as the deterministic benchmark seed.
    private const int RandomSeed = 1391;

    private const int StreetTypeUpperBound = 7;

    private int[][] _grid = [];

    [Params(10, 30)]
    public int Size { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _grid = Enumerable.Range(0, Size)
            .Select(_ => SeededDraws.Values(Size, 1, StreetTypeUpperBound, random))
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool HasValidPathByRecursiveDfs() =>
        CheckIfThereIsAValidPathInAGridSolution.HasValidPathByRecursiveDfs(_grid);

    [Benchmark]
    public bool HasValidPathByDepthFirstTraverse() =>
        CheckIfThereIsAValidPathInAGridSolution.HasValidPathByDepthFirstTraverse(_grid);
}
