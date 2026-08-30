using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Smallest Integer Divisible by K (LC 1015): the textbook modular walk (a single int
// updated in place, remainder = (remainder*10+1) % k for up to k steps) vs. this
// repo's own BFS - Reduce.Graph + DistanceMapReduceAlgebra over a precomputed
// RemainderNode graph (OpenTheLockBenchmarks precedent), where each node's single
// outgoing edge stands in for "append one more '1' digit". K is always coprime to
// 10 (see RemainderGraphs), so both approaches walk the full distance to remainder
// 0 rather than short-circuiting on an immediate "-1".
[MemoryDiagnoser]
public class SmallestIntegerDivisibleByKBenchmarks
{
    [Params(201, 5_001)]
    public int K;

    private Dictionary<int, RemainderNode> _nodesByRemainder = null!;
    private RemainderNode _startNode = null!;

    [GlobalSetup]
    public void Setup()
    {
        (_nodesByRemainder, _startNode) = RemainderGraphs.BuildGraph(K);
    }

    [Benchmark(Baseline = true)]
    public int ModularWalk()
    {
        var remainder = 0;

        for (var length = 1; length <= K; length++)
        {
            remainder = ((remainder * 10) + 1) % K;

            if (remainder == 0)
            {
                return length;
            }
        }

        return -1;
    }

    [Benchmark]
    public int ReduceGraphBfs()
    {
        var distances = Reduce.Graph<
            RemainderNode, RemainderTopology, ListChildren<RemainderNode>,
            NaturalChildOrder<RemainderNode, ListChildren<RemainderNode>>, ListChildren<RemainderNode>,
            BreadthFirstReduceOrder<RemainderNode>,
            DistanceMapReduceAlgebra<RemainderNode>, Dictionary<RemainderNode, int>>(_startNode);

        return distances.TryGetValue(_nodesByRemainder[0], out var distance) ? distance + 1 : -1;
    }
}
