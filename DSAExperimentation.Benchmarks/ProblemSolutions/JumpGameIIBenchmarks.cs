using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game II (LC 45): the textbook O(n) greedy two-pointer scan vs. modeling the
// same reachability as an implicit unweighted-hop graph (index i -> every index one
// jump away) and answering it with this repo's own ShortestPath.Dijkstra. Jump
// distances are capped at 10 so the implicit graph stays sparse (O(n) edges, not
// O(n^2)) - the point of the comparison is the primitive composition, not stress
// testing Dijkstra on a dense graph.
[MemoryDiagnoser]
public class JumpGameIIBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int[] _jumps = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _jumps = new int[Length];

        for (var i = 0; i < Length - 1; i++)
        {
            _jumps[i] = random.Next(1, 11);
        }
    }

    [Benchmark(Baseline = true)]
    public int GreedyTwoPointer()
    {
        var jumps = 0;
        var currentEnd = 0;
        var farthest = 0;

        for (var i = 0; i < _jumps.Length - 1; i++)
        {
            farthest = Math.Max(farthest, i + _jumps[i]);

            if (i == currentEnd)
            {
                jumps++;
                currentEnd = farthest;
            }
        }

        return jumps;
    }

    [Benchmark]
    public int DijkstraOverHopGraph()
    {
        var nodes = new WeightedGraphNode[_jumps.Length];

        for (var i = 0; i < _jumps.Length; i++)
        {
            nodes[i] = new WeightedGraphNode(i);
        }

        for (var i = 0; i < _jumps.Length; i++)
        {
            var reach = Math.Min(i + _jumps[i], _jumps.Length - 1);

            for (var j = i + 1; j <= reach; j++)
            {
                nodes[i].Edges.Add((1, nodes[j]));
            }
        }

        var distances = ShortestPath.Dijkstra<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(nodes[0]);

        return distances[nodes[^1]];
    }
}
