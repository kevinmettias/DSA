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
    private const int RandomSeed = 3812; private int _nodeCount;

    private int[][] _edges = [];
    private string _start = "";
    private string _target = "";
    private ToggleTree _tree = null!;
    // LeetCode problem number

    [Params(200, 2_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nodeCount = NodeCount;
        _edges = BuildRandomEdges(random, _nodeCount);

        var start = RandomBits(random, _nodeCount);
        var target = ToggledCopy(start, _edges, random);

        _start = new string(start);
        _target = new string(target);
        _tree = ToggleTree.Build(_nodeCount, _edges);
    }

    // One edge [parent, i] per node i > 0, each parent drawn uniformly from the
    // earlier nodes - a connected, cycle-free graph on nodeCount vertices.
    private static int[][] BuildRandomEdges(Random random, int nodeCount)
    {
        var edges = new int[nodeCount - 1][];

        for (var i = 1; i < nodeCount; i++)
        {
            var parent = random.Next(0, i);
            edges[i - 1] = [parent, i];
        }

        return edges;
    }

    // nodeCount independently drawn bits, half '0' and half '1' in expectation.
    private static char[] RandomBits(Random random, int nodeCount)
    {
        var bits = new char[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            var isZero = random.Next(2) == 0;
            bits[i] = isZero ? '0' : '1';
        }

        return bits;
    }

    // A copy of start with both endpoints of every coin-flipped edge toggled -
    // applying a real sequence of toggles rather than drawing the copy bit-by-bit,
    // so the workload is always solvable.
    private static char[] ToggledCopy(char[] start, int[][] edges, Random random)
    {
        var target = (char[])start.Clone();

        foreach (var edge in edges)
        {
            if (random.Next(2) != 0)
            {
                continue;
            }

            target[edge[0]] = Flip(target[edge[0]]);
            target[edge[1]] = Flip(target[edge[1]]);
        }

        return target;
    }

    private static char Flip(char bit) => bit == '0' ? '1' : '0';

    [Benchmark(Baseline = true)]
    public int[] BruteForceDfs() =>
        MinimumEdgeTogglesOnATreeSolution.MinTogglesByBruteForceDfs(_nodeCount, _edges, _start, _target);

    [Benchmark]
    public int[] TreeFold() =>
        MinimumEdgeTogglesOnATreeSolution.MinTogglesByTreeFold(_tree, _start, _target);
}
