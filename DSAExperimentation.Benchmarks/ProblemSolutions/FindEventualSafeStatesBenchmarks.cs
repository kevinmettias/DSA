using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.FindEventualSafeStates;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindEventualSafeStatesSolution's, the same methods
// FindEventualSafeStatesTests proves correct - the textbook per-node three-color DFS
// against modeling the same question as Kahn's algorithm over the reversed graph.
// The Kahn arm is handed the prepared List<SafeStateNode> its hoisted overload
// takes, so edge reversal is charged to [GlobalSetup] rather than to the peel being
// measured. Nodes split into a forward DAG half (always safe, funneling into a
// terminal node) and a ring-cycle half (never safe), so both strategies do real work
// discovering both outcomes instead of one trivial all-safe or all-unsafe graph.
[MemoryDiagnoser]
public class FindEventualSafeStatesBenchmarks
{
    private const int MaxForwardFanOut = 3;

    private int[][] _graph = [];

    private List<SafeStateNode> _reversed = new();
    [Params(50, 1_000)]
    public int NodeCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var half = NodeCount / AlgorithmConstants.HalvingFactor;
        _graph = new int[NodeCount][];

        for (var i = 0; i < half; i++)
        {
            var fanOut = Math.Min(MaxForwardFanOut, half - 1 - i);
            _graph[i] = Enumerable.Range(i + 1, fanOut).ToArray();
        }

        for (var i = half; i < NodeCount; i++)
        {
            var hasForwardNeighbour = i + 1 < NodeCount;
            var next = hasForwardNeighbour ? ForwardNeighbour(i) : half;
            _graph[i] = [next];
        }

        _reversed = FindEventualSafeStatesSolution.BuildReversedGraph(_graph);
    }

    // The ring half's forward step: the node one index on, which the wrap arm
    // replaces at the ring's last node.
    private static int ForwardNeighbour(int nodeIndex) => nodeIndex + 1;

    [Benchmark(Baseline = true)]
    public int[] DfsThreeColoring() =>
        FindEventualSafeStatesSolution.EventualSafeNodesByDfsThreeColoring(_graph);

    [Benchmark]
    public int[] ReversedKahnsTopologicalSort() =>
        FindEventualSafeStatesSolution.EventualSafeNodesByReversedKahnsTopologicalSort(_reversed);
}
