using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NRepeatedElementInSize2NArray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NRepeatedElementInSize2NArraySolution's, the same methods
// NRepeatedElementInSize2NArrayTests proves correct. The O(n^2) pairwise scan is the
// baseline the O(n) Set<int> pass has to beat.
[MemoryDiagnoser]
public class NRepeatedElementInSize2NArrayBenchmarks
{
    private const int ArrayLengthMultiplier = 2;

    [Params(200, 20_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Length is always even (2n): n distinct values first, THEN n copies of
        // the repeated value (0) last - matching the problem's own "n + 1 unique
        // elements" shape, deliberately unshuffled. This forces the pairwise scan
        // to burn a full wasted inner pass on every one of the n distinct leading
        // values (each is truly unique, so no j ever matches) before it reaches
        // the repeat, giving genuine O(n^2) worst-case behavior instead of an
        // accidental near-instant match - the same "force the full scan" intent
        // TwoSumBenchmarks achieves via an unreachable target.
        var n = Length / ArrayLengthMultiplier;
        var values = new List<int>(Length);

        for (var i = 1; i <= n; i++)
        {
            values.Add(i);
        }

        for (var i = 0; i < n; i++)
        {
            values.Add(0);
        }

        _values = [.. values];
    }

    [Benchmark(Baseline = true)]
    public int PairwiseScan() =>
        NRepeatedElementInSize2NArraySolution.FindRepeatedByPairwiseScan(_values);

    [Benchmark]
    public int TrackingSet() =>
        NRepeatedElementInSize2NArraySolution.FindRepeatedByTrackingSet(_values);
}
