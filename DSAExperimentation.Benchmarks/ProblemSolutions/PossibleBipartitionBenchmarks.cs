using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.Contracts.Topologies;
using BipartiteCheckOperations = DSAExperimentation.Algorithms.Bipartiteness.BipartiteCheck;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Possible Bipartition (LC 886): a hand-rolled iterative DFS 2-coloring
// directly over the problem's own dislikes pair list (a plain sbyte[] color
// array keyed by person id, an explicit Stack<int>) against this repo's
// BipartiteCheck.IsBipartite composed over PersonNode/PersonTopology - the
// same IGraphTopology/ListChildren/NaturalChildOrder shape Is Graph
// Bipartite? (LC 785) already benchmarks, just built from a 1-indexed
// dislikes-pair input instead of an adjacency list. The generated dislikes
// graph is bipartite by construction (a fixed A/B split, only cross-group
// pairs) so neither strategy short-circuits on an early color conflict.
[MemoryDiagnoser]
public class PossibleBipartitionBenchmarks
{
    private const int HalfDivisor = 2;
    private const int CrossPairsPerPerson = 2;

    [Params(200, 5_000)]
    public int PersonCount;

    private int[][] _adjacency = null!;
    private List<PersonNode> _people = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var half = PersonCount / HalfDivisor;
        var dislikes = new List<(int A, int B)>();

        AddConnectivityPairs(dislikes, random, half);
        AddDensityPairs(dislikes, random, half);

        _adjacency = BuildAdjacency(PersonCount, dislikes);
        _people = BuildPeople(PersonCount, dislikes);
    }

    // Guarantee connectivity: every B-side person gets one cross pair back to
    // a random A-side person.
    private void AddConnectivityPairs(List<(int A, int B)> dislikes, Random random, int half)
    {
        for (var i = half; i < PersonCount; i++)
        {
            dislikes.Add((random.Next(half), i));
        }
    }

    // Extra cross-only pairs for density - still strictly A-to-B, so the
    // dislikes graph stays bipartite by construction.
    private void AddDensityPairs(List<(int A, int B)> dislikes, Random random, int half)
    {
        for (var i = 0; i < PersonCount; i++)
        {
            for (var e = 0; e < CrossPairsPerPerson; e++)
            {
                var inA = i < half;
                var target = inA ? half + random.Next(PersonCount - half) : random.Next(half);
                dislikes.Add((i, target));
            }
        }
    }

    [Benchmark(Baseline = true)]
    public bool ArrayAdjacencyIterativeDfs()
    {
        var color = new sbyte[_adjacency.Length];
        var stack = new Stack<int>();

        for (var start = 0; start < _adjacency.Length; start++)
        {
            if (color[start] != 0)
            {
                continue;
            }

            color[start] = 1;
            stack.Push(start);

            if (!Walk(stack, color))
            {
                return false;
            }
        }

        return true;
    }

    [Benchmark]
    public bool BipartiteCheckBfs()
        => BipartiteCheckOperations.IsBipartite<
            PersonNode, PersonTopology, ListChildren<PersonNode>,
            NaturalChildOrder<PersonNode, ListChildren<PersonNode>>, ListChildren<PersonNode>>(
            _people);

    private bool Walk(Stack<int> stack, sbyte[] color)
    {
        while (stack.Count > 0)
        {
            var person = stack.Pop();

            foreach (var neighbor in _adjacency[person])
            {
                if (color[neighbor] == color[person])
                {
                    return false;
                }

                if (color[neighbor] == 0)
                {
                    color[neighbor] = (sbyte)-color[person];
                    stack.Push(neighbor);
                }
            }
        }

        return true;
    }

    private static int[][] BuildAdjacency(int personCount, List<(int A, int B)> dislikes)
    {
        var adjacency = Enumerable.Range(0, personCount).Select(_ => new List<int>()).ToArray();

        foreach (var (a, b) in dislikes)
        {
            adjacency[a].Add(b);
            adjacency[b].Add(a);
        }

        return adjacency.Select(neighbors => neighbors.ToArray()).ToArray();
    }

    private static List<PersonNode> BuildPeople(int personCount, List<(int A, int B)> dislikes)
    {
        var people = Enumerable.Range(0, personCount).Select(id => new PersonNode(id)).ToList();

        foreach (var (a, b) in dislikes)
        {
            people[a].Dislikes.Add(people[b]);
            people[b].Dislikes.Add(people[a]);
        }

        return people;
    }

    // See PossibleBipartitionTests.Fixtures for the full explanation -
    // repeated here rather than shared because TwoSumBenchmarks/
    // MedianOfTwoSortedArraysBenchmarks establish this project keeps its own
    // copy of the solution rather than depending on the Tests project.
    private sealed class PersonNode(int id)
    {
        public int Id { get; } = id;

        public List<PersonNode> Dislikes { get; } = [];
    }

    private readonly struct PersonTopology : IGraphTopology<PersonNode, ListChildren<PersonNode>>
    {
        public static ListChildren<PersonNode> GetChildren(PersonNode node) => new(node.Dislikes);
    }
}
