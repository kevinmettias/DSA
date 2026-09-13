using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PossibleBipartition;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PossibleBipartitionSolution's, the same methods
// PossibleBipartitionTests proves correct - a hand-rolled iterative DFS
// 2-coloring over a plain neighbour array (an sbyte[] group array keyed by
// person id, an explicit Stack<int>) against this repo's
// BipartiteCheck.IsBipartite, a multi-root BFS 2-coloring composed from
// IGraphTopology/ListChildren/NaturalChildOrder with a Dictionary<TNode,bool>
// group map. Both walk every person and dislike exactly once at O(V+E); the
// split under [MemoryDiagnoser] is the dictionary/heap-object overhead the
// composed primitive pays for its generality against the raw array baseline. The
// generated dislikes graph is bipartite by construction (a fixed A/B split, only
// cross-group pairs) so neither strategy short-circuits on an early conflict -
// both are forced through their full worst-case walk.
//
// Preparing either input shape from the pair list is input construction, so both
// are charged to [GlobalSetup] and handed to the strategies' prepared-input
// overloads.
[MemoryDiagnoser]
public class PossibleBipartitionBenchmarks
{
    private const int HalfDivisor = 2;
    private const int CrossPairsPerPerson = 2;

    [Params(200, 5_000)]
    public int PersonCount;

    private DislikeAdjacency _adjacency = null!;
    private DislikeGraph _graph = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var half = PersonCount / HalfDivisor;
        var dislikes = new List<int[]>();

        AddConnectivityPairs(dislikes, random, half);
        AddDensityPairs(dislikes, random, half);

        var pairs = dislikes.ToArray();

        _adjacency = DislikeAdjacency.Build(PersonCount, pairs);
        _graph = DislikeGraph.Build(PersonCount, pairs);
    }

    // Guarantee connectivity: every B-side person gets one cross pair back to a
    // random A-side person. People are numbered from 1, matching LeetCode's own
    // input, so each generated index is shifted up by one.
    private void AddConnectivityPairs(List<int[]> dislikes, Random random, int half)
    {
        for (var i = half; i < PersonCount; i++)
        {
            dislikes.Add([random.Next(half) + 1, i + 1]);
        }
    }

    // Extra cross-only pairs for density - still strictly A-to-B, so the dislikes
    // graph stays bipartite by construction.
    private void AddDensityPairs(List<int[]> dislikes, Random random, int half)
    {
        for (var i = 0; i < PersonCount; i++)
        {
            for (var e = 0; e < CrossPairsPerPerson; e++)
            {
                var inA = i < half;
                var target = inA ? half + random.Next(PersonCount - half) : random.Next(half);
                dislikes.Add([i + 1, target + 1]);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public bool ArrayAdjacencyIterativeDfs() =>
        PossibleBipartitionSolution.PossibleBipartitionByColorArrayDfs(_adjacency);

    [Benchmark]
    public bool BipartiteCheckBfs() =>
        PossibleBipartitionSolution.PossibleBipartitionByBipartiteCheck(_graph);
}
