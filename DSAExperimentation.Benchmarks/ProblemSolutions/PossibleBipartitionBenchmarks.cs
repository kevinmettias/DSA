using DSAExperimentation.DataStructures;
using DSAExperimentation.LeetCode.PossibleBipartition;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PossibleBipartitionSolution's, the same methods
// PossibleBipartitionSolutionTests proves correct - a hand-rolled iterative DFS
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
public class PossibleBipartitionBenchmarks
{
    private const int CrossPairsPerPerson = 2;

    private DislikeAdjacency _adjacency = null!;

    private DislikeGraph _graph = null!;

    // Stops at LC 886's 2,000 people, whose 2.5 dislikes per person stay inside its 10^4.
    [Params(200, 2_000)]
    public int PersonCount { get; set; }

    // LC 886's pairs are unique, so a pair drawn twice keeps its first copy.
    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var half = PersonCount / AlgorithmConstants.HalvingFactor;
        var dislikes = new List<int[]>();

        AddConnectivityPairs(dislikes, random, half);
        AddDensityPairs(dislikes, random, half);

        var pairs = dislikes.DistinctBy(pair => (pair[0], pair[1])).ToArray();

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
                var target = inA ? RandomBSidePerson(random, half) : random.Next(half);
                var pair = CrossPair(i, target);
                dislikes.Add(pair);
            }
        }
    }

    // LC 886 names each pair's lower person first (1 <= ai < bi), and every A-side person
    // is numbered below every B-side one, so the A-side person leads.
    private static int[] CrossPair(int person, int other)
    {
        var low = Math.Min(person, other);
        var high = Math.Max(person, other);

        return [low + 1, high + 1];
    }

    // A random person on the B side, shifted up past the A side's own people.
    private int RandomBSidePerson(Random random, int half) =>
        half + random.Next(PersonCount - half);

    [Benchmark(Baseline = true)]
    public bool CanBipartitionByColorArrayDfs() =>
        PossibleBipartitionSolution.CanBipartitionByColorArrayDfs(_adjacency);

    [Benchmark]
    public bool CanBipartitionByBipartiteCheck() =>
        PossibleBipartitionSolution.CanBipartitionByBipartiteCheck(_graph);
}
