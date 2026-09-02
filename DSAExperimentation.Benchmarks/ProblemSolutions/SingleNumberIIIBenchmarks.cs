using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Single Number III (LC 260): the textbook O(n^2) "for each element, scan
// the rest of the array for a duplicate" brute force vs. this repo's own
// HashMap<int,int> for O(n) frequency counting - the same one-pass
// lookup-table technique TwoSumBenchmarks already uses for indices, just
// counting occurrences instead. Values are drawn from a wide range so no
// unrelated pair collides into a false duplicate.
[MemoryDiagnoser]
public class SingleNumberIIIBenchmarks
{
    // Each pair consists of two identical values contributed to _values.
    private const int ElementsPerPair = 2;

    // Exclusive upper bound for paired values.
    private const int ValueUpperBound = 1_000_000;

    // LC260 guarantees exactly two numbers appear exactly once.
    private const int SingletonCount = 2;

    // The second of the two singleton (appears-once) values seeded into the array.
    private const int SecondSingletonValue = -2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var pairCount = Length / ElementsPerPair - 1;
        var pairs = Enumerable.Range(0, pairCount).Select(_ => random.Next(1, ValueUpperBound)).ToArray();

        var values = new List<int>(pairs.Length * ElementsPerPair + SingletonCount);
        values.AddRange(pairs);
        values.AddRange(pairs);
        values.Add(-1);
        values.Add(SecondSingletonValue);

        _values = values.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce()
    {
        var result = new List<int>(SingletonCount);

        for (var i = 0; i < _values.Length; i++)
        {
            var isDuplicate = false;

            for (var j = 0; j < _values.Length; j++)
            {
                if (i != j && _values[i] == _values[j])
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                result.Add(_values[i]);
            }
        }

        return result.ToArray();
    }

    [Benchmark]
    public int[] HashMapFrequencyCount()
    {
        var counts = new HashMap<int, int>();

        foreach (var n in _values)
        {
            counts.TryGetValue(n, out var existing);
            counts.Set(n, existing + 1);
        }

        var result = new List<int>(SingletonCount);
        foreach (var key in counts.Keys)
        {
            counts.TryGetValue(key, out var count);
            if (count == 1)
            {
                result.Add(key);
            }
        }

        return result.ToArray();
    }
}
