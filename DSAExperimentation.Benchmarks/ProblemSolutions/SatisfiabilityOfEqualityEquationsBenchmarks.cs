using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.DisjointSet;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Satisfiability of Equality Equations (LC 990): a fresh HashSet<char>+Queue<char>
// BFS reachability check per "!=" equation over an adjacency list built from the
// "==" equations, vs. this repo's own DisjointSet(26) - O(1) Union per equality and
// O(a(26)) IsConnected per inequality, with no per-query allocation.
[MemoryDiagnoser]
public class SatisfiabilityOfEqualityEquationsBenchmarks
{
    private const int AlphabetSize = 26;
    private const int InequalityProbabilityDenominator = 5;
    private const string EqualsOperator = "==";
    private const string NotEqualsOperator = "!=";
    private const int SecondVariableIndex = 3;

    [Params(100, 500)]
    public int EquationCount;

    private string[] _equations = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _equations = new string[EquationCount];

        for (var i = 0; i < EquationCount; i++)
        {
            var a = (char)('a' + random.Next(AlphabetSize));
            var b = (char)('a' + random.Next(AlphabetSize));
            var op = random.Next(InequalityProbabilityDenominator) == 0 ? NotEqualsOperator : EqualsOperator;
            _equations[i] = $"{a}{op}{b}";
        }
    }

    [Benchmark(Baseline = true)]
    public bool AdjacencyListBfs()
    {
        var (adjacency, inequalities) = BuildAdjacencyGraph();
        return !HasContradiction(adjacency, inequalities);
    }

    private (Dictionary<char, List<char>> Adjacency, List<(char First, char Second)> Inequalities) BuildAdjacencyGraph()
    {
        var adjacency = new Dictionary<char, List<char>>();
        var inequalities = new List<(char First, char Second)>();

        foreach (var equation in _equations)
        {
            var a = equation[0];
            var b = equation[SecondVariableIndex];

            if (equation[1] == '=')
            {
                AddAdjacencyEdge(adjacency, a, b);
            }
            else
            {
                inequalities.Add((a, b));
            }
        }

        return (adjacency, inequalities);
    }

    private static bool HasContradiction(Dictionary<char, List<char>> adjacency, List<(char First, char Second)> inequalities)
    {
        foreach (var (first, second) in inequalities)
        {
            if (IsReachable(adjacency, first, second))
            {
                return true;
            }
        }

        return false;
    }

    private static void AddAdjacencyEdge(Dictionary<char, List<char>> adjacency, char a, char b)
    {
        if (!adjacency.TryGetValue(a, out var aNeighbors))
        {
            aNeighbors = [];
            adjacency[a] = aNeighbors;
        }

        if (!adjacency.TryGetValue(b, out var bNeighbors))
        {
            bNeighbors = [];
            adjacency[b] = bNeighbors;
        }

        aNeighbors.Add(b);
        bNeighbors.Add(a);
    }

    private static bool IsReachable(Dictionary<char, List<char>> adjacency, char start, char target)
    {
        if (start == target)
        {
            return true;
        }

        var frontier = CreateFrontier(start);

        while (frontier.Queue.Count > 0)
        {
            var current = frontier.Queue.Dequeue();

            if (TryExpandFrontier(adjacency, current, target, frontier))
            {
                return true;
            }
        }

        return false;
    }

    private static SearchFrontier CreateFrontier(char start)
    {
        var visited = new HashSet<char> { start };
        var queue = new Queue<char>();
        queue.Enqueue(start);
        return new SearchFrontier(visited, queue);
    }

    private static bool TryExpandFrontier(Dictionary<char, List<char>> adjacency, char current, char target, SearchFrontier frontier)
    {
        if (!adjacency.TryGetValue(current, out var neighbors))
        {
            return false;
        }

        foreach (var next in neighbors)
        {
            if (next == target)
            {
                return true;
            }

            if (frontier.Visited.Add(next))
            {
                frontier.Queue.Enqueue(next);
            }
        }

        return false;
    }

    private readonly record struct SearchFrontier(HashSet<char> Visited, Queue<char> Queue);

    [Benchmark]
    public bool DisjointSetUnionFind()
    {
        var components = new DisjointSet(AlphabetSize);

        foreach (var equation in _equations)
        {
            if (equation[1] == '=')
            {
                components.Union(equation[0] - 'a', equation[SecondVariableIndex] - 'a');
            }
        }

        foreach (var equation in _equations)
        {
            if (equation[1] == '!' && components.IsConnected(equation[0] - 'a', equation[SecondVariableIndex] - 'a'))
            {
                return false;
            }
        }

        return true;
    }
}
