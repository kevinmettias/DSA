using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SuffixArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Orderly Queue (LC 899), k == 1 case: the reachable set is exactly the string's n
// rotations, so the canonical approach compares all n candidate rotations directly
// (O(n) rotations x O(n) comparison each = O(n^2)) against building this repo's own
// SuffixArray over s+s once and reading off the first suffix start below n
// (O(n log^2 n) per SuffixArray.cs's own doc comment). LeetCode's own constraint
// (s.length <= 1000) never leaves brute force's comfort zone - .NET's ordinal string
// compare is fast enough that the crossover only arrives around n in the tens of
// thousands, well past that bound - so Length is deliberately set past it (measured
// locally: brute force ~400ms vs. SuffixArray ~45ms at 50k, ~1.6s vs. ~0.1s at 100k)
// to actually exercise the O(n^2) vs. O(n log^2 n) gap this composition buys, rather
// than reporting a same-order-of-magnitude number at LeetCode's own input size. The
// k > 1 case reduces to sorting s outright - already exercised against this repo's
// MergeSort by HIndex/ThreeSum/etc.'s own benchmarks, so it isn't repeated here.
[MemoryDiagnoser]
public class OrderlyQueueBenchmarks
{
    private const int RandomSeed = 899; // LC problem number
    private const int AlphabetSize = 26;

    [Params(50_000, 100_000)]
    public int Length;

    private string _s = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _s = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string BruteForceAllRotations()
    {
        var best = _s;

        for (var start = 1; start < _s.Length; start++)
        {
            var tail = _s.AsSpan(start);
            var head = _s.AsSpan(0, start);
            var rotation = string.Concat(tail, head);
            if (string.CompareOrdinal(rotation, best) < 0)
            {
                best = rotation;
            }
        }

        return best;
    }

    [Benchmark]
    public string SuffixArraySmallestRotation()
    {
        var doubled = _s + _s;
        var suffixArray = new SuffixArray(doubled);

        foreach (var start in suffixArray.Suffixes)
        {
            if (start < _s.Length)
            {
                return doubled.Substring(start, _s.Length);
            }
        }

        return _s;
    }
}
