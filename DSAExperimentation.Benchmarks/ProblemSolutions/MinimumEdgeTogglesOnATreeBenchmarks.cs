using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumEdgeTogglesOnATree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumEdgeTogglesOnATreeSolution's, the same
// methods MinimumEdgeTogglesOnATreeTests proves correct. Each node i > 0 attaches
// to a uniformly random earlier node - the same randomized-parent shape
// ShortestPathInAWeightedTreeBenchmarks builds inline. start is random and target
// starts as a copy of it, then every edge independently gets toggled into target
// with 50% probability - applying a real sequence of toggles rather than drawing
// target bit-by-bit guarantees the workload is always solvable, so neither arm
// ever gets to shortcut on the immediate [-1] case. The fold arm is handed a
// pre-built ToggleTree via its hoisted overload, so BFS-to-parent-array
// construction is charged to [GlobalSetup] rather than to the fold being
// measured; the brute-force arm's own adjacency list stays inside the measured
// call, matching its own textbook character (§17.5) exactly as
// ShortestPathInAWeightedTreeBenchmarks' brute-force arm does.
[MemoryDiagnoser]
public class MinimumEdgeTogglesOnATreeBenchmarks
{
    private const int RandomSeed = 3812; // LeetCode problem number

    [Params(200, 2_000)]
    public int NodeCount;

    private int _n;
    private int[][] _edges = null!;
    private string _start = null!;
    private string _target = null!;
    private ToggleTree _tree = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _n = NodeCount;
        _edges = new int[_n - 1][];

        for (var i = 1; i < _n; i++)
        {
            var parent = random.Next(0, i);
            _edges[i - 1] = [parent, i];
        }

        var start = new char[_n];

        for (var i = 0; i < _n; i++)
        {
            start[i] = random.Next(2) == 0 ? '0' : '1';
        }

        var target = (char[])start.Clone();

        foreach (var edge in _edges)
        {
            if (random.Next(2) != 0)
            {
                continue;
            }

            target[edge[0]] = Flip(target[edge[0]]);
            target[edge[1]] = Flip(target[edge[1]]);
        }

        _start = new string(start);
        _target = new string(target);
        _tree = ToggleTree.Build(_n, _edges);
    }

    private static char Flip(char bit) => bit == '0' ? '1' : '0';

    [Benchmark(Baseline = true)]
    public int[] BruteForceDfs() =>
        MinimumEdgeTogglesOnATreeSolution.MinTogglesByBruteForceDfs(_n, _edges, _start, _target);

    [Benchmark]
    public int[] TreeFold() =>
        MinimumEdgeTogglesOnATreeSolution.MinTogglesByTreeFold(_tree, _start, _target);
}
