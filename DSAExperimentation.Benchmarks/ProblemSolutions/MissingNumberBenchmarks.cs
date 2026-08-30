using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Missing Number (LC 268): the brute-force "for each candidate in [0, n],
// scan the whole array for it" O(n^2) check vs. this repo's own Set<int>
// for O(n) membership - the same TryAdd/Has composition ContainsDuplicate-
// Benchmarks already uses. Values are shuffled so BruteForce can't exploit
// any short-circuit ordering from an already-sorted input.
[MemoryDiagnoser]
public class MissingNumberBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        var missing = random.Next(0, Length + 1);

        _values = Enumerable.Range(0, Length + 1)
            .Where(n => n != missing)
            .OrderBy(_ => random.Next())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        for (var candidate = 0; candidate <= _values.Length; candidate++)
        {
            var found = false;

            for (var i = 0; i < _values.Length; i++)
            {
                if (_values[i] == candidate)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                return candidate;
            }
        }

        return _values.Length;
    }

    [Benchmark]
    public int SetMembership()
    {
        var present = new Set<int>();

        foreach (var n in _values)
        {
            present.TryAdd(n);
        }

        for (var candidate = 0; candidate <= _values.Length; candidate++)
        {
            if (!present.Has(candidate))
            {
                return candidate;
            }
        }

        return _values.Length;
    }
}
