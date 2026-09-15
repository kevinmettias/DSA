using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.Candy;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CandySolution's, the same methods CandyTests proves
// correct. _ratings is strictly decreasing, the worst case for repeated
// relaxation: each pass can only propagate one extra unit of "must exceed my
// right neighbor" one position further left, forcing O(n) passes of O(n) each.
[MemoryDiagnoser]
public class CandyBenchmarks
{
    private int[] _ratings = [];

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _ratings = Enumerable.Range(0, Length).Select(i => Length - i).ToArray();

    [Benchmark(Baseline = true)]
    public int RepeatedRelaxation() => CandySolution.MinCandiesByRepeatedRelaxation(_ratings);

    [Benchmark]
    public int TwoPassSlopeConstraints() => CandySolution.MinCandiesByTwoPassSlopeConstraints(_ratings);
}
