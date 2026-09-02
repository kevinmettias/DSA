using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using FrontierStack = DSAExperimentation.DataStructures.Stack.Stack<(string Node, double Product)>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Evaluate Division (LC 399): a plain Dictionary<string, Dictionary<string,double>> +
// BCL Stack<(string,double)> DFS baseline vs. this repo's HashMap<string, HashMap<string,
// double>> + this repo's own Stack<(string,double)> - same shape as the EvaluateDivision
// coverage test, repeated here rather than shared per the CourseScheduleIIBenchmarks/
// TwoSumBenchmarks precedent. The variables form one long chain (v0/v1 = 2.0,
// v1/v2 = 2.0, ...) so every query below walks the full chain instead of an early exit.
[MemoryDiagnoser]
public class EvaluateDivisionBenchmarks
{
    private const double EdgeWeight = 2.0;
    private const string FirstVariableName = "v0";

    [Params(200, 5_000)]
    public int VariableCount;

    private (string Dividend, string Divisor, double Value)[] _equations = null!;
    private string _firstVariable = null!;
    private string _lastVariable = null!;

    [GlobalSetup]
    public void Setup()
    {
        _equations = new (string, string, double)[VariableCount - 1];

        for (var i = 0; i < VariableCount - 1; i++)
        {
            _equations[i] = ($"v{i}", $"v{i + 1}", EdgeWeight);
        }

        _firstVariable = FirstVariableName;
        _lastVariable = $"v{VariableCount - 1}";
    }

    [Benchmark(Baseline = true)]
    public double DictionaryBased()
    {
        var neighborsByNode = BuildDictionaryGraph(_equations);

        if (!neighborsByNode.ContainsKey(_firstVariable) || !neighborsByNode.ContainsKey(_lastVariable))
        {
            return -1.0;
        }

        return DictionaryDfs(neighborsByNode, _firstVariable, _lastVariable);
    }

    private static Dictionary<string, Dictionary<string, double>> BuildDictionaryGraph(
        (string Dividend, string Divisor, double Value)[] equations)
    {
        var neighborsByNode = new Dictionary<string, Dictionary<string, double>>();

        void AddEdge(string from, string to, double weight)
        {
            if (!neighborsByNode.TryGetValue(from, out var neighbors))
            {
                neighbors = new Dictionary<string, double>();
                neighborsByNode[from] = neighbors;
            }

            neighbors[to] = weight;
        }

        foreach (var (dividend, divisor, value) in equations)
        {
            AddEdge(dividend, divisor, value);
            AddEdge(divisor, dividend, 1.0 / value);
        }

        return neighborsByNode;
    }

    private static double DictionaryDfs(
        Dictionary<string, Dictionary<string, double>> neighborsByNode, string firstVariable, string lastVariable)
    {
        var visited = new HashSet<string> { firstVariable };
        var pending = new Stack<(string Node, double Product)>();
        pending.Push((firstVariable, 1.0));

        while (pending.TryPop(out var current))
        {
            if (current.Node == lastVariable)
            {
                return current.Product;
            }

            if (neighborsByNode.TryGetValue(current.Node, out var neighbors))
            {
                PushUnvisitedNeighbors(neighbors, current, visited, pending);
            }
        }

        return -1.0;
    }

    private static void PushUnvisitedNeighbors(
        Dictionary<string, double> neighbors,
        (string Node, double Product) current,
        HashSet<string> visited,
        Stack<(string Node, double Product)> pending)
    {
        foreach (var (neighbor, weight) in neighbors)
        {
            if (visited.Add(neighbor))
            {
                pending.Push((neighbor, current.Product * weight));
            }
        }
    }

    [Benchmark]
    public double HashMapStackComposed()
    {
        var neighborsByNode = BuildHashMapGraph(_equations);

        if (!neighborsByNode.HasKey(_firstVariable) || !neighborsByNode.HasKey(_lastVariable))
        {
            return -1.0;
        }

        return HashMapDfs(neighborsByNode, _firstVariable, _lastVariable);
    }

    private static HashMap<string, HashMap<string, double>> BuildHashMapGraph(
        (string Dividend, string Divisor, double Value)[] equations)
    {
        var neighborsByNode = new HashMap<string, HashMap<string, double>>();

        void AddEdge(string from, string to, double weight)
        {
            if (!neighborsByNode.TryGetValue(from, out var neighbors))
            {
                neighbors = new HashMap<string, double>();
                neighborsByNode.Set(from, neighbors);
            }

            neighbors.Set(to, weight);
        }

        foreach (var (dividend, divisor, value) in equations)
        {
            AddEdge(dividend, divisor, value);
            AddEdge(divisor, dividend, 1.0 / value);
        }

        return neighborsByNode;
    }

    private static double HashMapDfs(
        HashMap<string, HashMap<string, double>> neighborsByNode, string firstVariable, string lastVariable)
    {
        var visited = new HashSet<string> { firstVariable };
        var pending = new FrontierStack();
        pending.Push((firstVariable, 1.0));

        while (pending.TryPop(out var current))
        {
            if (current.Node == lastVariable)
            {
                return current.Product;
            }

            neighborsByNode.TryGetValue(current.Node, out var neighbors);
            PushUnvisitedNeighbors(neighbors, current, visited, pending);
        }

        return -1.0;
    }

    private static void PushUnvisitedNeighbors(
        HashMap<string, double> neighbors,
        (string Node, double Product) current,
        HashSet<string> visited,
        FrontierStack pending)
    {
        foreach (var neighbor in neighbors.Keys)
        {
            if (!visited.Add(neighbor))
            {
                continue;
            }

            neighbors.TryGetValue(neighbor, out var weight);
            pending.Push((neighbor, current.Product * weight));
        }
    }
}
