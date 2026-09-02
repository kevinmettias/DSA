using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find a Value of a Mysterious Function Closest to Target (LC 1521): the textbook O(n^2)
// all-subarrays-ANDed-in-place brute force vs. the O(n log(max(arr))) approach that tracks the
// (provably small - AND only ever clears bits, never sets them) set of distinct AND values
// ending at each index, using this repo's own HashMap<TKey,TValue> as an ad hoc set via .Keys.
[MemoryDiagnoser]
public class FindValueOfMysteriousFunctionClosestToTargetBenchmarks
{
    private const int Target = 1 << 15; // mid-range target unreachable by any single value, forces a full scan
    private const int RandomSeed = 1521; // LC problem number
    private const int ValueBitWidth = 20;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, 1 << ValueBitWidth)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllSubarrays()
    {
        var best = int.MaxValue;

        for (var l = 0; l < _values.Length; l++)
        {
            var current = _values[l];
            best = Math.Min(best, Math.Abs(current - Target));

            for (var r = l + 1; r < _values.Length; r++)
            {
                current &= _values[r];
                best = Math.Min(best, Math.Abs(current - Target));
            }
        }

        return best;
    }

    [Benchmark]
    public int DistinctAndValuesHashMap()
    {
        var best = int.MaxValue;
        var endingHere = new HashMap<int, bool>();

        for (var i = 0; i < _values.Length; i++)
        {
            var next = new HashMap<int, bool>();
            next.Set(_values[i], true);

            foreach (var previous in endingHere.Keys)
            {
                next.Set(previous & _values[i], true);
            }

            foreach (var candidate in next.Keys)
            {
                best = Math.Min(best, Math.Abs(candidate - Target));
            }

            endingHere = next;
        }

        return best;
    }
}
