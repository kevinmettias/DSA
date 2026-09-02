using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.TopologicalSort;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Sort Items by Groups Respecting Dependencies (LC 1203): a naive rescan topological
// sort (CourseScheduleIIBenchmarks's own O(V^2 + V*E) strategy, run once over the item
// graph and once over the derived group graph) vs. this repo's own Kahn's-algorithm
// TopologicalSort.TrySort run the same two times. Items sit in contiguous group
// buckets by id (group(i) = i * GroupCount / ItemCount, monotonic non-decreasing in i)
// so every cross-group item edge (which only ever points from a lower id to a higher
// one, per the same fan-out setup CourseScheduleIIBenchmarks uses) points from a
// lower-or-equal group id to a higher-or-equal one - guaranteeing both the item graph
// and the derived group graph are acyclic, so both strategies run their full real
// workload instead of an early cycle bailout.
[MemoryDiagnoser]
public class SortItemsByGroupsRespectingDependenciesBenchmarks
{
    private const int GroupCount = 5;
    private const int MaxFanOut = 3;

    [Params(50, 1_000)]
    public int ItemCount;

    private List<ItemNode> _items = null!;
    private List<GroupNode> _groups = null!;

    [GlobalSetup]
    public void Setup()
    {
        _groups = Enumerable.Range(0, GroupCount).Select(id => new GroupNode(id)).ToList();
        _items = Enumerable.Range(0, ItemCount)
            .Select(id => new ItemNode(id, id * GroupCount / ItemCount))
            .ToList();

        for (var i = 0; i < ItemCount; i++)
        {
            var fanOut = Math.Min(MaxFanOut, ItemCount - 1 - i);
            for (var f = 1; f <= fanOut; f++)
            {
                var next = i + f;
                _items[i].EnabledItems.Add(_items[next]);

                if (_items[i].GroupId != _items[next].GroupId)
                {
                    _groups[_items[i].GroupId].EnabledGroups.Add(_groups[_items[next].GroupId]);
                }
            }
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRescanTwoLevelSort()
    {
        var itemOrder = NaiveRescanOrder(_items, item => item.EnabledItems);
        var groupOrder = NaiveRescanOrder(_groups, group => group.EnabledGroups);

        return Combine(itemOrder, groupOrder).Length;
    }

    [Benchmark]
    public int KahnsTwoLevelSort()
    {
        TopologicalSort.TrySort<
            ItemNode, ItemTopology, ListChildren<ItemNode>,
            NaturalChildOrder<ItemNode, ListChildren<ItemNode>>, ListChildren<ItemNode>>(
            _items, out var itemOrder);

        TopologicalSort.TrySort<
            GroupNode, GroupTopology, ListChildren<GroupNode>,
            NaturalChildOrder<GroupNode, ListChildren<GroupNode>>, ListChildren<GroupNode>>(
            _groups, out var groupOrder);

        return Combine(itemOrder, groupOrder).Length;
    }

    private int[] Combine(List<ItemNode> itemOrder, List<GroupNode> groupOrder)
    {
        var buckets = new List<int>[GroupCount];
        for (var g = 0; g < GroupCount; g++)
        {
            buckets[g] = [];
        }

        foreach (var item in itemOrder)
        {
            buckets[item.GroupId].Add(item.Id);
        }

        var result = new int[itemOrder.Count];
        var index = 0;
        foreach (var group in groupOrder)
        {
            foreach (var itemId in buckets[group.Id])
            {
                result[index++] = itemId;
            }
        }

        return result;
    }

    private static List<T> NaiveRescanOrder<T>(List<T> nodes, Func<T, List<T>> children) where T : class
    {
        var inDegree = nodes.ToDictionary(node => node, _ => 0);
        foreach (var node in nodes)
        {
            foreach (var next in children(node))
            {
                inDegree[next]++;
            }
        }

        var remaining = new List<T>(nodes);
        var order = new List<T>(nodes.Count);

        while (remaining.Count > 0 && TryTakeNextReadyNode(remaining, order, inDegree, children))
        {
        }

        return order;
    }

    private static bool TryTakeNextReadyNode<T>(
        List<T> remaining, List<T> order, Dictionary<T, int> inDegree, Func<T, List<T>> children) where T : class
    {
        var next = remaining.FirstOrDefault(node => inDegree[node] == 0);
        if (next is null)
        {
            return false;
        }

        order.Add(next);
        remaining.Remove(next);
        foreach (var dependent in children(next))
        {
            inDegree[dependent]--;
        }

        return true;
    }

    // See SortItemsByGroupsRespectingDependenciesTests.Fixtures for the full
    // explanation - repeated here rather than shared because CourseScheduleIIBenchmarks/
    // TwoSumBenchmarks establish this project keeps its own copy of the solution
    // rather than depending on the Tests project.
    private sealed class ItemNode(int id, int groupId)
    {
        public int Id { get; } = id;

        public int GroupId { get; } = groupId;

        public List<ItemNode> EnabledItems { get; } = [];
    }

    private readonly struct ItemTopology : IGraphTopology<ItemNode, ListChildren<ItemNode>>
    {
        public static ListChildren<ItemNode> GetChildren(ItemNode node) => new(node.EnabledItems);
    }

    private sealed class GroupNode(int id)
    {
        public int Id { get; } = id;

        public List<GroupNode> EnabledGroups { get; } = [];
    }

    private readonly struct GroupTopology : IGraphTopology<GroupNode, ListChildren<GroupNode>>
    {
        public static ListChildren<GroupNode> GetChildren(GroupNode node) => new(node.EnabledGroups);
    }
}
