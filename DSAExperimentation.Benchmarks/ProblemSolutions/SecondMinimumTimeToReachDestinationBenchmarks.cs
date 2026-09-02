using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Node, int Steps)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Second Minimum Time to Reach Destination (LC 2045): both strategies run the
// exact same dual-distance BFS (track each node's first AND second distinct
// edge-count depth) - what differs is the frontier. ListDequeueBfs uses a bare
// List<T> and pays an O(n) shift on every dequeue (RemoveAt(0)), the naive
// mistake this repo's own Queue<T> (Deque-backed, O(1) at both ends) exists to
// avoid. On this problem's near-single-cycle graph (m == n, per LC's own
// generation constraint) the BFS still only ever touches O(n) edges total, so the
// O(n) list-shift per dequeue is what turns the whole walk quadratic instead of
// linear.
[MemoryDiagnoser]
public class SecondMinimumTimeToReachDestinationBenchmarks
{
    private const int Time = 3;
    private const int Change = 5;
    private const int SignalPhaseModulus = 2;

    [Params(200, 2_000)]
    public int NodeCount;

    private List<int>[] _adjacency = null!;

    [GlobalSetup]
    public void Setup()
    {
        _adjacency = new List<int>[NodeCount];
        for (var i = 0; i < NodeCount; i++)
        {
            _adjacency[i] = [];
        }

        // A single cycle over every node (m == NodeCount edges), matching LC
        // 2045's own generation guarantee and giving every node a genuine second
        // route (the long way around).
        for (var i = 0; i < NodeCount; i++)
        {
            var next = (i + 1) % NodeCount;
            _adjacency[i].Add(next);
            _adjacency[next].Add(i);
        }
    }

    [Benchmark(Baseline = true)]
    public int ListDequeueBfs()
    {
        var (first, second) = InitializeDistances();
        var frontier = new List<(int Node, int Steps)> { (0, 0) };

        while (frontier.Count > 0)
        {
            ProcessListFrontier(frontier, first, second);
        }

        return SimulateTravelTime(second[NodeCount - 1]);
    }

    private void ProcessListFrontier(List<(int Node, int Steps)> frontier, int[] first, int[] second)
    {
        var current = frontier[0];
        frontier.RemoveAt(0);
        var nextSteps = current.Steps + 1;

        foreach (var neighbor in _adjacency[current.Node])
        {
            if (first[neighbor] == -1)
            {
                first[neighbor] = nextSteps;
                frontier.Add((neighbor, nextSteps));
            }
            else if (first[neighbor] != nextSteps && second[neighbor] == -1)
            {
                second[neighbor] = nextSteps;
                frontier.Add((neighbor, nextSteps));
            }
        }
    }

    [Benchmark]
    public int RepoQueueBfs()
    {
        var (first, second) = InitializeDistances();
        var frontier = new RepoQueue();
        frontier.Enqueue((0, 0));

        RunQueueBfs(frontier, first, second);

        return SimulateTravelTime(second[NodeCount - 1]);
    }

    private (int[] First, int[] Second) InitializeDistances()
    {
        var first = new int[NodeCount];
        var second = new int[NodeCount];
        Array.Fill(first, -1);
        Array.Fill(second, -1);
        first[0] = 0;

        return (first, second);
    }

    private void RunQueueBfs(RepoQueue frontier, int[] first, int[] second)
    {
        while (frontier.TryDequeue(out var current))
        {
            var nextSteps = current.Steps + 1;

            foreach (var neighbor in _adjacency[current.Node])
            {
                if (first[neighbor] == -1)
                {
                    first[neighbor] = nextSteps;
                    frontier.Enqueue((neighbor, nextSteps));
                }
                else if (first[neighbor] != nextSteps && second[neighbor] == -1)
                {
                    second[neighbor] = nextSteps;
                    frontier.Enqueue((neighbor, nextSteps));
                }
            }
        }
    }

    private static int SimulateTravelTime(int edgeCount)
    {
        var currentTime = 0;

        for (var i = 0; i < edgeCount; i++)
        {
            var cycle = currentTime / Change;
            if (cycle % SignalPhaseModulus == 1)
            {
                currentTime = (cycle + 1) * Change;
            }

            currentTime += Time;
        }

        return currentTime;
    }
}
