using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Build a Matrix With Conditions (LC 2392): solving it is exactly two independent
// topological sorts (row order, column order) over the 1..k value range, so this
// mirrors CourseScheduleIIBenchmarks' own naive-rescan-vs-Kahn's contrast, just run
// twice per invocation - once per axis - the same real workload BuildMatrix itself
// does. Conditions form a guaranteed-acyclic DAG (every condition points from a
// lower value to a higher one, capped fan-out) so both strategies run their full
// real workload instead of an early cycle bailout.
[MemoryDiagnoser]
public class BuildAMatrixWithConditionsBenchmarks
{
    private const int MaxFanOut = 3;

    [Params(50, 1_000)]
    public int K;

    private List<ValueNode> _rowNodes = null!;
    private List<ValueNode> _colNodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _rowNodes = BuildChainNodes(K);
        _colNodes = BuildChainNodes(K);
    }

    private static List<ValueNode> BuildChainNodes(int k)
    {
        var nodes = Enumerable.Range(1, k).Select(id => new ValueNode(id)).ToList();

        for (var i = 0; i < k; i++)
        {
            var fanOut = Math.Min(MaxFanOut, k - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                nodes[i].After.Add(nodes[i + f]);
            }
        }

        return nodes;
    }

    [Benchmark(Baseline = true)]
    public int NaiveRescanBothOrders()
        => NaiveOrder(_rowNodes).Count + NaiveOrder(_colNodes).Count;

    private static List<ValueNode> NaiveOrder(List<ValueNode> nodes)
    {
        var inDegree = nodes.ToDictionary(node => node, _ => 0);
        foreach (var node in nodes)
        {
            foreach (var next in node.After)
            {
                inDegree[next]++;
            }
        }

        var remaining = new List<ValueNode>(nodes);
        var order = new List<ValueNode>(nodes.Count);

        while (remaining.Count > 0 && TryAdvanceNaiveRescan(remaining, inDegree, order))
        {
        }

        return order;
    }

    private static bool TryAdvanceNaiveRescan(
        List<ValueNode> remaining, Dictionary<ValueNode, int> inDegree, List<ValueNode> order)
    {
        var next = remaining.FirstOrDefault(node => inDegree[node] == 0);

        if (next is null)
        {
            return false;
        }

        order.Add(next);
        remaining.Remove(next);

        foreach (var dependent in next.After)
        {
            inDegree[dependent]--;
        }

        return true;
    }

    [Benchmark]
    public int KahnsTopologicalSortBothOrders()
    {
        TopologicalSort.TrySort<
            ValueNode, ValueTopology, ListChildren<ValueNode>,
            NaturalChildOrder<ValueNode, ListChildren<ValueNode>>, ListChildren<ValueNode>>(
            _rowNodes, out var rowOrdering);

        TopologicalSort.TrySort<
            ValueNode, ValueTopology, ListChildren<ValueNode>,
            NaturalChildOrder<ValueNode, ListChildren<ValueNode>>, ListChildren<ValueNode>>(
            _colNodes, out var colOrdering);

        return rowOrdering.Count + colOrdering.Count;
    }

    // See BuildAMatrixWithConditionsTests.Fixtures for the full explanation -
    // repeated here rather than shared because TwoSumBenchmarks/
    // CourseScheduleIIBenchmarks establish this project keeps its own copy of the
    // solution rather than depending on the Tests project.
    private sealed class ValueNode(int id)
    {
        public int Id { get; } = id;

        public List<ValueNode> After { get; } = [];
    }

    private readonly struct ValueTopology : IGraphTopology<ValueNode, ListChildren<ValueNode>>
    {
        public static ListChildren<ValueNode> GetChildren(ValueNode node) => new(node.After);
    }
}
