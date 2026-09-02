using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Shortest Distance to Target String in a Circular Array (LC 2515): a direct
// min(diff, n-diff) linear scan over the raw words array vs. this repo's own BFS -
// Reduce.Graph + DistanceMapReduceAlgebra over a precomputed CircularArrayNode
// graph (SmallestIntegerDivisibleByKBenchmarks precedent), where each node's two
// edges stand in for "one step left" / "one step right" around the circle. The
// target sits diametrically opposite startIndex (CircularArrayGraphs), so neither
// approach gets to short-circuit on an immediate neighbor match.
[MemoryDiagnoser]
public class ShortestDistanceToTargetStringInACircularArrayBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private CircularArrayNode[] _nodes = null!;
    private string[] _words = null!;
    private int _startIndex;

    [GlobalSetup]
    public void Setup()
    {
        (_nodes, _words, _startIndex) = CircularArrayGraphs.BuildGraph(Length);
    }

    [Benchmark(Baseline = true)]
    public int LinearScan()
    {
        var n = _words.Length;
        var best = -1;

        for (var i = 0; i < n; i++)
        {
            if (_words[i] != CircularArrayGraphs.Target)
            {
                continue;
            }

            var diff = Math.Abs(i - _startIndex);
            var distance = Math.Min(diff, n - diff);

            if (best == -1 || distance < best)
            {
                best = distance;
            }
        }

        return best;
    }

    [Benchmark]
    public int ReduceGraphBfs()
    {
        var distances = Reduce.Graph<
            CircularArrayNode, CircularArrayTopology, ListChildren<CircularArrayNode>,
            NaturalChildOrder<CircularArrayNode, ListChildren<CircularArrayNode>>, ListChildren<CircularArrayNode>,
            BreadthFirstReduceOrder<CircularArrayNode>,
            DistanceMapReduceAlgebra<CircularArrayNode>, Dictionary<CircularArrayNode, int>>(_nodes[_startIndex]);

        var best = -1;

        foreach (var node in _nodes)
        {
            if (node.Word != CircularArrayGraphs.Target || !distances.TryGetValue(node, out var distance))
            {
                continue;
            }

            if (best == -1 || distance < best)
            {
                best = distance;
            }
        }

        return best;
    }
}
