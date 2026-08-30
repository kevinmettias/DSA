using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Jump Game IV (LC 1345): the textbook BFS directly over the array - a Queue<int>,
// a same-value index map, and the standard "clear the group once every member has
// been enqueued" trick that keeps it from re-scanning a large equal-value run on
// every visit - vs. modeling the exact same i+1/i-1/same-value reachability as an
// implicit unweighted-hop graph (this repo's WeightedGraphNode/WeightedGraphTopology,
// the same fixtures JumpGameIIBenchmarks uses) and answering it with this repo's own
// ShortestPath.Dijkstra. Values are drawn from a range five times narrower than
// Length so equal-value groups stay small (dense enough to matter, not so dense the
// hop graph's edge count explodes past O(n)).
[MemoryDiagnoser]
public class JumpGameIVBenchmarks
{
    [Params(200, 2_000)]
    public int Length;

    private int[] _arr = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1345);
        _arr = new int[Length];

        for (var i = 0; i < Length; i++)
        {
            _arr[i] = random.Next(0, Math.Max(1, Length / 5));
        }
    }

    [Benchmark(Baseline = true)]
    public int BfsWithGroupPruning()
    {
        var n = _arr.Length;
        var indicesByValue = new Dictionary<int, List<int>>();

        for (var i = 0; i < n; i++)
        {
            if (!indicesByValue.TryGetValue(_arr[i], out var indices))
            {
                indices = [];
                indicesByValue[_arr[i]] = indices;
            }

            indices.Add(i);
        }

        var visited = new bool[n];
        visited[0] = true;
        var frontier = new Queue<int>();
        frontier.Enqueue(0);
        var steps = 0;

        while (frontier.Count > 0)
        {
            var levelSize = frontier.Count;

            for (var k = 0; k < levelSize; k++)
            {
                var i = frontier.Dequeue();

                if (i == n - 1)
                {
                    return steps;
                }

                if (indicesByValue.TryGetValue(_arr[i], out var sameValue))
                {
                    foreach (var j in sameValue)
                    {
                        if (!visited[j])
                        {
                            visited[j] = true;
                            frontier.Enqueue(j);
                        }
                    }

                    indicesByValue.Remove(_arr[i]);
                }

                if (i + 1 < n && !visited[i + 1])
                {
                    visited[i + 1] = true;
                    frontier.Enqueue(i + 1);
                }

                if (i - 1 >= 0 && !visited[i - 1])
                {
                    visited[i - 1] = true;
                    frontier.Enqueue(i - 1);
                }
            }

            steps++;
        }

        return -1;
    }

    [Benchmark]
    public int DijkstraOverHopGraph()
    {
        var n = _arr.Length;
        var nodes = new WeightedGraphNode[n];
        for (var i = 0; i < n; i++)
        {
            nodes[i] = new WeightedGraphNode(i);
        }

        var indicesByValue = new Dictionary<int, List<int>>();
        for (var i = 0; i < n; i++)
        {
            if (!indicesByValue.TryGetValue(_arr[i], out var indices))
            {
                indices = [];
                indicesByValue[_arr[i]] = indices;
            }

            indices.Add(i);
        }

        for (var i = 0; i < n; i++)
        {
            if (i + 1 < n)
            {
                nodes[i].Edges.Add((1, nodes[i + 1]));
            }

            if (i - 1 >= 0)
            {
                nodes[i].Edges.Add((1, nodes[i - 1]));
            }

            foreach (var j in indicesByValue[_arr[i]])
            {
                if (j != i)
                {
                    nodes[i].Edges.Add((1, nodes[j]));
                }
            }
        }

        var distances = ShortestPath.Dijkstra<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(nodes[0]);

        return distances[nodes[^1]];
    }
}
