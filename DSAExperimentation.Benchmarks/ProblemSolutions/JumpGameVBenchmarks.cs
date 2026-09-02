using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game V (LC 1340): the textbook per-start memoized DFS (Dictionary<int,int>
// memo, one recursive call tree rooted at each of the n starting indices) vs.
// building the exact same "strictly decreasing, within d" reachability once as an
// implicit graph and answering it with this repo's own TopologicalSort.TrySort
// (Kahn's algorithm) plus a single linear longest-path relaxation pass over that
// order - the same composition JumpGameVTests uses. Values are a random shuffle
// (no ties) so every index has a genuinely different rank, keeping the DAG's
// longest path - and therefore the memoized DFS's recursion depth - realistic
// instead of degenerate.
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
        _arr = Enumerable.Range(0, Length).ToArray();

        for (var i = _arr.Length - 1; i > 0; i--)
        {
            var swapIndex = random.Next(i + 1);
            (_arr[i], _arr[swapIndex]) = (_arr[swapIndex], _arr[i]);
        }
    }

    [Benchmark(Baseline = true)]
    public int MemoizedDfsPerStart()
    {
        var memo = new Dictionary<int, int>();
        var best = 0;

        for (var start = 0; start < _arr.Length; start++)
        {
            var longestFromStart = LongestPathFrom(start, memo);
            best = Math.Max(best, longestFromStart);
        }

        return best;
    }

    private int LongestPathFrom(int i, Dictionary<int, int> memo)
    {
        if (memo.TryGetValue(i, out var cached))
        {
            return cached;
        }

        var best = 1;

        for (var j = i + 1; j <= Math.Min(_arr.Length - 1, i + MaxJumpDistance) && _arr[j] < _arr[i]; j++)
        {
            best = Math.Max(best, 1 + LongestPathFrom(j, memo));
        }

        for (var j = i - 1; j >= Math.Max(0, i - MaxJumpDistance) && _arr[j] < _arr[i]; j--)
        {
            best = Math.Max(best, 1 + LongestPathFrom(j, memo));
        }

        memo[i] = best;
        return best;
    }

    [Benchmark]
    public int TopologicalSortLongestPath()
    {
        var nodes = BuildJumpNodes();
        ConnectReachableEdges(nodes);

        TopologicalSort.TrySort<
            JumpNode, JumpTopology, ListChildren<JumpNode>,
            NaturalChildOrder<JumpNode, ListChildren<JumpNode>>, ListChildren<JumpNode>>(
            nodes, out var ordering);

        var longestPath = ComputeLongestPaths(ordering);
        return longestPath.Values.Max();
    }

    private JumpNode[] BuildJumpNodes()
    {
        var nodes = new JumpNode[_arr.Length];
        for (var i = 0; i < _arr.Length; i++)
        {
            nodes[i] = new JumpNode(i);
        }

        return nodes;
    }

    private void ConnectReachableEdges(JumpNode[] nodes)
    {
        for (var i = 0; i < _arr.Length; i++)
        {
            for (var j = i + 1; j <= Math.Min(_arr.Length - 1, i + MaxJumpDistance) && _arr[j] < _arr[i]; j++)
            {
                nodes[i].ReachableIndices.Add(nodes[j]);
            }

            for (var j = i - 1; j >= Math.Max(0, i - MaxJumpDistance) && _arr[j] < _arr[i]; j--)
            {
                nodes[i].ReachableIndices.Add(nodes[j]);
            }
        }
    }

    private static Dictionary<JumpNode, int> ComputeLongestPaths(IEnumerable<JumpNode> ordering)
    {
        var longestPath = new Dictionary<JumpNode, int>();
        foreach (var node in ordering)
        {
            longestPath.TryAdd(node, 1);

            foreach (var next in node.ReachableIndices)
            {
                var candidate = longestPath[node] + 1;
                if (candidate > longestPath.GetValueOrDefault(next, 1))
                {
                    longestPath[next] = candidate;
                }
            }
        }

        return longestPath;
    }

    // See JumpGameVTests.Fixtures for the full explanation - repeated here rather
    // than shared because TwoSumBenchmarks/MedianOfTwoSortedArraysBenchmarks
    // establish this project keeps its own copy of the solution rather than
    // depending on the Tests project.
    private sealed class JumpNode(int index)
    {
        public int Index { get; } = index;

        public List<JumpNode> ReachableIndices { get; } = [];
    }

    private readonly struct JumpTopology : IGraphTopology<JumpNode, ListChildren<JumpNode>>
    {
        public static ListChildren<JumpNode> GetChildren(JumpNode node) => new(node.ReachableIndices);
    }
}
