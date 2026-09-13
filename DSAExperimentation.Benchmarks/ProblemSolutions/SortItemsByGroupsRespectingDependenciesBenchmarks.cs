using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.SortItemsByGroupsRespectingDependencies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SortItemsByGroupsRespectingDependenciesSolution's, the
// same methods SortItemsByGroupsRespectingDependenciesTests proves correct. A naive
// rescan topological sort (CourseScheduleIIBenchmarks's own O(V^2 + V*E) strategy, run
// once over the item graph and once over the derived group graph) vs. this repo's own
// Kahn's-algorithm TopologicalSort.TrySort run the same two times.
//
// Items sit in contiguous group buckets by id (group(i) = i * GroupCount / ItemCount,
// monotonic non-decreasing in i) so every cross-group item edge (which only ever points
// from a lower id to a higher one, per the same fan-out setup CourseScheduleIIBenchmarks
// uses) points from a lower-or-equal group id to a higher-or-equal one - guaranteeing
// both the item graph and the derived group graph are acyclic, so both strategies run
// their full real workload instead of an early cycle bailout.
//
// Each arm is handed the prepared item and group graphs its hoisted overload takes, so
// graph construction is charged to [GlobalSetup] rather than to the sort being measured.
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
                AddEdge(i, i + f);
            }
        }
    }

    private void AddEdge(int from, int to)
    {
        _items[from].EnabledItems.Add(_items[to]);

        if (_items[from].GroupId != _items[to].GroupId)
        {
            _groups[_items[from].GroupId].EnabledGroups.Add(_groups[_items[to].GroupId]);
        }
    }

    [Benchmark(Baseline = true)]
    public int NaiveRescanTwoLevelSort() =>
        SortItemsByGroupsRespectingDependenciesSolution.SortItemsByNaiveRescan(_items, _groups).Length;

    [Benchmark]
    public int KahnsTwoLevelSort() =>
        SortItemsByGroupsRespectingDependenciesSolution.SortItemsByKahnsTopologicalSort(_items, _groups).Length;
}
