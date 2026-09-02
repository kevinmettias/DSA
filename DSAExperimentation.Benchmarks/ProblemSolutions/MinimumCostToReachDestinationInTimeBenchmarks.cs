using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Cost to Reach Destination in Time (LC 1928): the textbook unmemoized DFS
// over every fee-collecting walk within the time budget (each city i can step to
// i+1 in 1 time unit or i+2 in 2 time units, the same "two edges per node" chain
// shape NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks already uses to force
// genuine Fibonacci-many-paths blowup) vs. this repo's own ShortestPath.Dijkstra
// over the (city, elapsedTime) state-expanded graph, whose node count is only
// O(N^2) - one node per (city, arrival-time) pair actually reachable within the
// budget - so it stays polynomial while the naive walk stays exponential.
[MemoryDiagnoser]
public class MinimumCostToReachDestinationInTimeBenchmarks
{
    private const int FeeAlphabet = 7;
    private const int LongStepSize = 2;

    // Kept modest, same reasoning as NumberOfRestrictedPathsFromFirstToLastNodeBenchmarks:
    // NaiveDfs's blowup here really is O(golden-ratio^N).
    [Params(20, 30)]
    public int N;

    private int[] _passingFees = null!;
    private int _maxTime;

    [GlobalSetup]
    public void Setup()
    {
        _passingFees = new int[N + 1];

        for (var city = 0; city <= N; city++)
        {
            _passingFees[city] = (city % FeeAlphabet) + 1;
        }

        // Generous enough that every step-1/step-2 combination fits the budget, so
        // NaiveDfs never prunes early on time and genuinely explores every walk.
        _maxTime = N;
    }

    [Benchmark(Baseline = true)]
    public int NaiveDfs() => MinCostFrom(city: 0, remainingTime: _maxTime);

    private int MinCostFrom(int city, int remainingTime)
    {
        if (city == N)
        {
            return _passingFees[city];
        }

        var best = int.MaxValue;

        if (remainingTime >= 1)
        {
            var next = MinCostFrom(city + 1, remainingTime - 1);
            best = next < int.MaxValue ? Math.Min(best, next) : best;
        }

        if (city + LongStepSize <= N && remainingTime >= LongStepSize)
        {
            var next = MinCostFrom(city + LongStepSize, remainingTime - LongStepSize);
            best = next < int.MaxValue ? Math.Min(best, next) : best;
        }

        return best == int.MaxValue ? int.MaxValue : _passingFees[city] + best;
    }

    [Benchmark]
    public int StateExpandedDijkstra()
    {
        var (start, nodes) = BuildStateGraph();
        var distances = ShortestPath.Dijkstra<
            TimeCityNode, TimeCityEdgeTopology, ListEdges<TimeCityNode, int>, int>(start);
        var best = FindBestCostAtDestination(nodes, distances);

        return best == int.MaxValue ? -1 : _passingFees[0] + best;
    }

    private (TimeCityNode Start, Dictionary<(int City, int Time), TimeCityNode> Nodes) BuildStateGraph()
    {
        var nodes = new Dictionary<(int City, int Time), TimeCityNode>();
        var start = GetOrCreate(nodes, city: 0, time: 0);
        var frontier = new Queue<TimeCityNode>();
        frontier.Enqueue(start);
        var state = new SearchState(nodes, frontier);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current.City + 1 <= N && current.Time + 1 <= _maxTime)
            {
                Connect(state, current, current.City + 1, current.Time + 1);
            }

            if (current.City + LongStepSize <= N && current.Time + LongStepSize <= _maxTime)
            {
                Connect(state, current, current.City + LongStepSize, current.Time + LongStepSize);
            }
        }

        return (start, nodes);
    }

    private int FindBestCostAtDestination(
        Dictionary<(int City, int Time), TimeCityNode> nodes, Dictionary<TimeCityNode, int> distances)
    {
        var best = int.MaxValue;

        foreach (var (key, node) in nodes)
        {
            if (key.City == N && distances.TryGetValue(node, out var cost) && cost < best)
            {
                best = cost;
            }
        }

        return best;
    }

    private void Connect(SearchState state, TimeCityNode current, int nextCity, int nextTime)
    {
        var key = (nextCity, nextTime);
        var isNew = !state.Nodes.ContainsKey(key);
        var next = GetOrCreate(state.Nodes, nextCity, nextTime);
        current.Edges.Add((_passingFees[nextCity], next));

        if (isNew)
        {
            state.Pending.Enqueue(next);
        }
    }

    private readonly record struct SearchState(
        Dictionary<(int City, int Time), TimeCityNode> Nodes, Queue<TimeCityNode> Pending);

    private static TimeCityNode GetOrCreate(Dictionary<(int City, int Time), TimeCityNode> nodes, int city, int time)
    {
        var key = (city, time);

        if (!nodes.TryGetValue(key, out var node))
        {
            node = new TimeCityNode(city, time);
            nodes[key] = node;
        }

        return node;
    }

    // See MinimumCostToReachDestinationInTimeTests.Fixtures for the full
    // explanation - repeated here rather than shared because TwoSumBenchmarks/
    // MedianOfTwoSortedArraysBenchmarks establish this project keeps its own copy
    // of the solution rather than depending on the Tests project.
    private sealed class TimeCityNode(int city, int time)
    {
        public int City { get; } = city;

        public int Time { get; } = time;

        public List<(int Fee, TimeCityNode Target)> Edges { get; } = [];
    }

    private readonly struct TimeCityEdgeTopology
        : IEdgeTopology<TimeCityNode, ListEdges<TimeCityNode, int>, int>
    {
        public static ListEdges<TimeCityNode, int> GetEdges(TimeCityNode node) => new(node.Edges);
    }
}
