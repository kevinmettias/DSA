using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Open the Lock (LC 752): the textbook mutate-every-wheel BFS (Queue<string> plus a
// HashSet<string> visited set, generating each of the 8 candidate turns on the fly)
// vs. this repo's own BFS - Reduce.Graph + DistanceMapReduceAlgebra over a
// precomputed LockNode graph, the same "distance to some specific target"
// composition WordLadderBenchmarks already uses for LC 127. Target is always
// "5555" (LC's own farthest combination from "0000", forcing the full 20-turn BFS
// radius); DeadendCount varies how much of the 10,000-node graph is pruned away.
[MemoryDiagnoser]
public class OpenTheLockBenchmarks
{
    [Params(0, 500)]
    public int DeadendCount;

    private HashSet<string> _deadends = null!;
    private Dictionary<string, LockNode> _nodesByCombo = null!;
    private LockNode _startNode = null!;

    [GlobalSetup]
    public void Setup()
    {
        _deadends = LockGraphs.BuildDeadends(DeadendCount, seed: 752);
        (_nodesByCombo, _startNode) = LockGraphs.BuildGraph(_deadends);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs()
    {
        if (_deadends.Contains(LockGraphs.Start))
        {
            return -1;
        }

        var visited = new HashSet<string> { LockGraphs.Start };
        var queue = new Queue<(string Combo, int Turns)>();
        queue.Enqueue((LockGraphs.Start, 0));

        while (queue.Count > 0)
        {
            var (combo, turns) = queue.Dequeue();

            if (combo == LockGraphs.FarthestTarget)
            {
                return turns;
            }

            foreach (var neighbor in LockGraphs.WheelTurnNeighbors(combo))
            {
                if (!_deadends.Contains(neighbor) && visited.Add(neighbor))
                {
                    queue.Enqueue((neighbor, turns + 1));
                }
            }
        }

        return -1;
    }

    [Benchmark]
    public int ReduceGraphBfs()
    {
        var distances = Reduce.Graph<
            LockNode, LockTopology, ListChildren<LockNode>,
            NaturalChildOrder<LockNode, ListChildren<LockNode>>, ListChildren<LockNode>,
            BreadthFirstReduceOrder<LockNode>,
            DistanceMapReduceAlgebra<LockNode>, Dictionary<LockNode, int>>(_startNode);

        return _nodesByCombo.TryGetValue(LockGraphs.FarthestTarget, out var targetNode) &&
            distances.TryGetValue(targetNode, out var distance)
                ? distance
                : -1;
    }
}
