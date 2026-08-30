using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Design HashSet (LC 705): a plain List<int> baseline (Contains-scan before every
// Add, indexed Remove that shifts every trailing element) vs. this repo's own
// Set<int> (HashMap<Element,bool>-backed, the InsertDeleteGetRandomO1Benchmarks
// precedent of composing this repo's HashMap for a "design" problem instead of a
// one-shot algorithm) - O(n) per call vs. O(1) average per call.
[MemoryDiagnoser]
public class DesignHashSetBenchmarks
{
    [Params(200, 5_000)]
    public int Count;

    private int[] _addOrder = null!;
    private int[] _containsOrder = null!;

    [GlobalSetup]
    public void Setup()
    {
        _addOrder = Enumerable.Range(0, Count).ToArray();

        var random = new Random(705);
        _containsOrder = _addOrder.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int ListBased()
    {
        var values = new List<int>();

        foreach (var value in _addOrder)
        {
            if (!values.Contains(value))
            {
                values.Add(value);
            }
        }

        var hits = 0;

        foreach (var value in _containsOrder)
        {
            if (values.Contains(value))
            {
                hits++;
            }
        }

        return hits;
    }

    [Benchmark]
    public int SetBacked()
    {
        var values = new Set<int>();

        foreach (var value in _addOrder)
        {
            values.TryAdd(value);
        }

        var hits = 0;

        foreach (var value in _containsOrder)
        {
            if (values.Has(value))
            {
                hits++;
            }
        }

        return hits;
    }
}
