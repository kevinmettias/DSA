using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shortest Path Visiting All Nodes (LC 847): the textbook true multi-source BFS
// (a plain Queue<(int,int,int)> seeded with every node's own single-bit start state
// at once, plus a HashSet<(int,int)> visited set) vs. this repo's own BFS -
// Reduce.Graph + DistanceMapReduceAlgebra over a precomputed VisitStateNode graph,
// run once per possible start and minimized - the same "distance to some specific
// condition" composition OpenTheLockBenchmarks/SlidingPuzzleBenchmarks already use,
// adapted from "one start, one target" to "every start, first full-mask state" (see
// ShortestPathVisitingAllNodesTests for why looping single-source Reduce.Graph calls
// and taking the minimum is equivalent to true multi-source BFS). The repo-primitive
// side is expected to do strictly more total work here - it recomputes a full
// distance map per start instead of sharing one frontier across all of them - so this
// benchmark is also an honest look at that cost, the same spirit
// ShortestPathAlgorithmBenchmarks' own FloydWarshall entry already documents.
[MemoryDiagnoser]
public class ShortestPathVisitingAllNodesBenchmarks
{
    [Params(8, 11)]
    public int NodeCount;

    private int[][] _graph = null!;
    private VisitStateNode[] _startNodes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _graph = ShortestPathGraphs.BuildRandomConnectedGraph(NodeCount, seed: 847);
        (_, _startNodes) = ShortestPathGraphs.BuildStateGraph(_graph);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs()
    {
        var fullMask = (1 << NodeCount) - 1;
        var visited = new HashSet<(int Node, int Mask)>();
        var queue = new Queue<(int Node, int Mask, int Steps)>();

        for (var start = 0; start < NodeCount; start++)
        {
            if (visited.Add((start, 1 << start)))
            {
                queue.Enqueue((start, 1 << start, 0));
            }
        }

        while (queue.Count > 0)
        {
            var (node, mask, steps) = queue.Dequeue();

            if (mask == fullMask)
            {
                return steps;
            }

            foreach (var neighbor in _graph[node])
            {
                var nextMask = mask | (1 << neighbor);

                if (visited.Add((neighbor, nextMask)))
                {
                    queue.Enqueue((neighbor, nextMask, steps + 1));
                }
            }
        }

        return -1;
    }

    [Benchmark]
    public int ReduceGraphBfs()
    {
        var fullMask = (1 << NodeCount) - 1;
        var shortest = int.MaxValue;

        foreach (var startNode in _startNodes)
        {
            var distances = Reduce.Graph<
                VisitStateNode, VisitStateTopology, ListChildren<VisitStateNode>,
                NaturalChildOrder<VisitStateNode, ListChildren<VisitStateNode>>, ListChildren<VisitStateNode>,
                BreadthFirstReduceOrder<VisitStateNode>,
                DistanceMapReduceAlgebra<VisitStateNode>, Dictionary<VisitStateNode, int>>(startNode);

            foreach (var (state, distance) in distances)
            {
                if (state.Mask == fullMask && distance < shortest)
                {
                    shortest = distance;
                }
            }
        }

        return shortest;
    }
}
