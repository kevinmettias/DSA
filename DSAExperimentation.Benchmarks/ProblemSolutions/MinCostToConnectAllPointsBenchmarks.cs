using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.MinimumSpanningTrees;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Min Cost to Connect All Points (LC 1584): the textbook dense-graph Prim's
// algorithm (baseline - O(n^2), no heap, the array-scan approach this exact
// "complete graph over points" problem is usually solved with) vs. this repo's own
// Algorithms.MinimumSpanningTrees.MinimumSpanningTree.Kruskal run over the complete
// graph of Manhattan-distance edges, using the same WeightedGraphNode/
// WeightedGraphTopology fixtures CourseScheduleIVBenchmarks.cs already reuses for a
// weighted graph.
[MemoryDiagnoser]
public class MinCostToConnectAllPointsBenchmarks
{
    [Params(50, 200)]
    public int PointCount;

    private int[][] _points = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1584);
        _points = Enumerable.Range(0, PointCount)
            .Select(_ => new[] { random.Next(-1_000, 1_000), random.Next(-1_000, 1_000) })
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int DensePrim()
    {
        var n = _points.Length;
        var inTree = new bool[n];
        var minEdge = new int[n];
        Array.Fill(minEdge, int.MaxValue);
        minEdge[0] = 0;

        var total = 0;

        for (var iteration = 0; iteration < n; iteration++)
        {
            var next = -1;

            for (var candidate = 0; candidate < n; candidate++)
            {
                if (!inTree[candidate] && (next == -1 || minEdge[candidate] < minEdge[next]))
                {
                    next = candidate;
                }
            }

            inTree[next] = true;
            total += minEdge[next];

            for (var candidate = 0; candidate < n; candidate++)
            {
                if (inTree[candidate])
                {
                    continue;
                }

                var distance = ManhattanDistance(_points[next], _points[candidate]);
                if (distance < minEdge[candidate])
                {
                    minEdge[candidate] = distance;
                }
            }
        }

        return total;
    }

    [Benchmark]
    public int KruskalMst()
    {
        var nodes = Enumerable.Range(0, _points.Length).Select(id => new WeightedGraphNode(id)).ToArray();

        for (var i = 0; i < _points.Length; i++)
        {
            for (var j = i + 1; j < _points.Length; j++)
            {
                var weight = ManhattanDistance(_points[i], _points[j]);
                nodes[i].Edges.Add((weight, nodes[j]));
                nodes[j].Edges.Add((weight, nodes[i]));
            }
        }

        var mst = MinimumSpanningTree.Kruskal<
            WeightedGraphNode, WeightedGraphTopology, ListEdges<WeightedGraphNode, int>, int>(nodes);

        return mst.Sum(edge => edge.Weight);
    }

    private static int ManhattanDistance(int[] a, int[] b) => Math.Abs(a[0] - b[0]) + Math.Abs(a[1] - b[1]);
}
