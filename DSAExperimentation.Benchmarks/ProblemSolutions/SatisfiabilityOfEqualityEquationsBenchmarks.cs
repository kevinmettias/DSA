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
            var a = (char)('a' + random.Next(26));
            var b = (char)('a' + random.Next(26));
            var op = random.Next(5) == 0 ? "!=" : "==";
            _equations[i] = $"{a}{op}{b}";
        }
    }

    [Benchmark(Baseline = true)]
    public bool AdjacencyListBfs()
    {
        var adjacency = new Dictionary<char, List<char>>();
        var inequalities = new List<(char First, char Second)>();

        foreach (var equation in _equations)
        {
            var a = equation[0];
            var b = equation[3];

            if (equation[1] == '=')
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
            else
            {
                inequalities.Add((a, b));
            }
        }

        foreach (var (first, second) in inequalities)
        {
            if (IsReachable(adjacency, first, second))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsReachable(Dictionary<char, List<char>> adjacency, char start, char target)
    {
        if (start == target)
        {
            return true;
        }

        var visited = new HashSet<char> { start };
        var queue = new Queue<char>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();

            if (!adjacency.TryGetValue(current, out var neighbors))
            {
                continue;
            }

            foreach (var next in neighbors)
            {
                if (next == target)
                {
                    return true;
                }

                if (visited.Add(next))
                {
                    queue.Enqueue(next);
                }
            }
        }

        return false;
    }

    [Benchmark]
    public bool DisjointSetUnionFind()
    {
        var components = new DisjointSet(26);

        foreach (var equation in _equations)
        {
            if (equation[1] == '=')
            {
                components.Union(equation[0] - 'a', equation[3] - 'a');
            }
        }

        foreach (var equation in _equations)
        {
            if (equation[1] == '!' && components.IsConnected(equation[0] - 'a', equation[3] - 'a'))
            {
                return false;
            }
        }

        return true;
    }
}
