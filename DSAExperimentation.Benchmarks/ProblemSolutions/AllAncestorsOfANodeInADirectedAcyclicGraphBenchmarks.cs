using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// All Ancestors of a Node in a Directed Acyclic Graph (LC 2192): NaivePerNodeForwardWalk
// re-derives every node's ancestor contribution from scratch with one fresh DFS PER
// START node, marking every descendant it reaches, O(n * (V+E)) total, since a
// deeply-nested node sits inside many other nodes' walks. TopologicalDpPass instead
// orders every node source-to-sink with this repo's own Kahn's-algorithm
// TopologicalSort.TrySort, then threads each node's own ancestor set forward across its
// outgoing edges in one linear pass, O(V+E) total
// (AllAncestorsOfANodeInADirectedAcyclicGraphTests' exact algorithm) - the canonical
// solution to this problem. Edges point from a lower id to a higher one (capped
// fan-out) so the relation is a guaranteed-acyclic DAG, the same generation shape
// CourseScheduleIIBenchmarks/LoudAndRichBenchmarks already use.
[MemoryDiagnoser]
public class AllAncestorsOfANodeInADirectedAcyclicGraphBenchmarks
{
    private const int MaxFanOut = 3;

    [Params(50, 1_000)]
    public int NodeCount;

    private List<AncestorNode> _nodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _nodes = Enumerable.Range(0, NodeCount).Select(id => new AncestorNode(id)).ToList();

        for (var i = 0; i < NodeCount; i++)
        {
            var fanOut = Math.Min(MaxFanOut, NodeCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                _nodes[i].Children.Add(_nodes[i + f]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaivePerNodeForwardWalk()
    {
        var ancestorCounts = new int[_nodes.Count];

        for (var start = 0; start < _nodes.Count; start++)
        {
            MarkDescendants(_nodes[start], ancestorCounts);
        }

        return ancestorCounts.Sum();
    }

    private static void MarkDescendants(AncestorNode start, int[] ancestorCounts)
    {
        var visited = new HashSet<AncestorNode>();
        var stack = new Stack<AncestorNode>();

        EnqueueUnvisitedChildren(start, visited, stack);
        DrainStack(stack, visited, ancestorCounts);
    }

    private static void EnqueueUnvisitedChildren(
        AncestorNode node, HashSet<AncestorNode> visited, Stack<AncestorNode> stack)
    {
        foreach (var child in node.Children)
        {
            if (visited.Add(child))
            {
                stack.Push(child);
            }
        }
    }

    private static void DrainStack(Stack<AncestorNode> stack, HashSet<AncestorNode> visited, int[] ancestorCounts)
    {
        while (stack.Count > 0)
        {
            var node = stack.Pop();
            ancestorCounts[node.Id]++;

            EnqueueUnvisitedChildren(node, visited, stack);
        }
    }

    [Benchmark]
    public int TopologicalDpPass()
    {
        TopologicalSort.TrySort<
            AncestorNode, AncestorTopology, ListChildren<AncestorNode>,
            NaturalChildOrder<AncestorNode, ListChildren<AncestorNode>>, ListChildren<AncestorNode>>(
            _nodes, out var ordering);

        var ancestorSets = CreateAncestorSets(_nodes.Count);
        PropagateAncestors(ordering, ancestorSets);

        return SumAncestorCounts(ancestorSets);
    }

    private static HashMap<int, bool>[] CreateAncestorSets(int count)
    {
        var ancestorSets = new HashMap<int, bool>[count];
        for (var i = 0; i < count; i++)
        {
            ancestorSets[i] = new HashMap<int, bool>();
        }

        return ancestorSets;
    }

    private static void PropagateAncestors(List<AncestorNode> ordering, HashMap<int, bool>[] ancestorSets)
    {
        foreach (var node in ordering)
        {
            // Snapshotted once per node, not once per child: HashMap<TKey,TValue>.Keys
            // materializes a fresh List on every access, and node's own ancestor set
            // never changes while its children are being updated below.
            var nodeAncestors = ancestorSets[node.Id].Keys.ToList();

            foreach (var child in node.Children)
            {
                ancestorSets[child.Id].Set(node.Id, true);

                foreach (var ancestor in nodeAncestors)
                {
                    ancestorSets[child.Id].Set(ancestor, true);
                }
            }
        }
    }

    private static int SumAncestorCounts(HashMap<int, bool>[] ancestorSets)
    {
        var total = 0;
        for (var i = 0; i < ancestorSets.Length; i++)
        {
            total += ancestorSets[i].Count;
        }

        return total;
    }

    // See AllAncestorsOfANodeInADirectedAcyclicGraphTests.Fixtures for the full
    // explanation - repeated here rather than shared because TwoSumBenchmarks/
    // MedianOfTwoSortedArraysBenchmarks establish this project keeps its own copy of
    // the solution rather than depending on the Tests project.
    private sealed class AncestorNode(int id)
    {
        public int Id { get; } = id;

        public List<AncestorNode> Children { get; } = [];
    }

    private readonly struct AncestorTopology : IGraphTopology<AncestorNode, ListChildren<AncestorNode>>
    {
        public static ListChildren<AncestorNode> GetChildren(AncestorNode node) => new(node.Children);
    }
}
