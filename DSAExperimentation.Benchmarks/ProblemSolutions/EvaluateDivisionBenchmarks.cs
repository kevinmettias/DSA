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
            _equations[i] = ($"v{i}", $"v{i + 1}", 2.0);
        }

        _firstVariable = "v0";
        _lastVariable = $"v{VariableCount - 1}";
    }

    [Benchmark(Baseline = true)]
    public double DictionaryBased()
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

        foreach (var (dividend, divisor, value) in _equations)
        {
            AddEdge(dividend, divisor, value);
            AddEdge(divisor, dividend, 1.0 / value);
        }

        if (!neighborsByNode.ContainsKey(_firstVariable) || !neighborsByNode.ContainsKey(_lastVariable))
        {
            return -1.0;
        }

        var visited = new HashSet<string> { _firstVariable };
        var pending = new Stack<(string Node, double Product)>();
        pending.Push((_firstVariable, 1.0));

        while (pending.TryPop(out var current))
        {
            if (current.Node == _lastVariable)
            {
                return current.Product;
            }

            if (!neighborsByNode.TryGetValue(current.Node, out var neighbors))
            {
                continue;
            }

            foreach (var (neighbor, weight) in neighbors)
            {
                if (visited.Add(neighbor))
                {
                    pending.Push((neighbor, current.Product * weight));
                }
            }
        }

        return -1.0;
    }

    [Benchmark]
    public double HashMapStackComposed()
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

        foreach (var (dividend, divisor, value) in _equations)
        {
            AddEdge(dividend, divisor, value);
            AddEdge(divisor, dividend, 1.0 / value);
        }

        if (!neighborsByNode.HasKey(_firstVariable) || !neighborsByNode.HasKey(_lastVariable))
        {
            return -1.0;
        }

        var visited = new HashSet<string> { _firstVariable };
        var pending = new FrontierStack();
        pending.Push((_firstVariable, 1.0));

        while (pending.TryPop(out var current))
        {
            if (current.Node == _lastVariable)
            {
                return current.Product;
            }

            neighborsByNode.TryGetValue(current.Node, out var neighbors);

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

        return -1.0;
    }
}
