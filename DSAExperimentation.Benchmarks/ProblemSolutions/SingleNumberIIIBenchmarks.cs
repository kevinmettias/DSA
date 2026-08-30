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
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var pairCount = Length / 2 - 1;
        var pairs = Enumerable.Range(0, pairCount).Select(_ => random.Next(1, 1_000_000)).ToArray();

        var values = new List<int>(pairs.Length * 2 + 2);
        values.AddRange(pairs);
        values.AddRange(pairs);
        values.Add(-1);
        values.Add(-2);

        _values = values.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce()
    {
        var result = new List<int>(2);

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

        var result = new List<int>(2);
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
