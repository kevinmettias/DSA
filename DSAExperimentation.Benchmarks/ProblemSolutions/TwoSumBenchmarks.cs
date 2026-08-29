using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Two Sum (LC 1): the canonical O(n^2) brute force vs. the O(n) one-pass HashMap
// approach, using this repo's own HashMap<TKey,TValue>. _target is deliberately
// unreachable (all values positive, target negative) so BOTH strategies are forced
// through their full worst-case scan instead of an early-exit on the first
// invocation making brute force look artificially competitive.
[MemoryDiagnoser]
public class TwoSumBenchmarks
{
    private const int Target = -1;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, 1_000)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public bool BruteForce()
    {
        for (var i = 0; i < _values.Length; i++)
        {
            for (var j = i + 1; j < _values.Length; j++)
            {
                if (_values[i] + _values[j] == Target)
                {
                    return true;
                }
            }
        }

        return false;
    }

    [Benchmark]
    public bool HashMapOnePass()
    {
        var seen = new HashMap<int, int>();

        for (var i = 0; i < _values.Length; i++)
        {
            if (seen.HasKey(Target - _values[i]))
            {
                return true;
            }

            seen.Set(_values[i], i);
        }

        return false;
    }
}
