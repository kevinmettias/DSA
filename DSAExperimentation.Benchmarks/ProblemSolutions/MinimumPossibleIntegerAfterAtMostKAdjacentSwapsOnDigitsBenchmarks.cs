using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution's, the same methods the
// coverage test proves correct - the O(Length^2) physical List<char> simulation against the
// Fenwick-tree greedy that answers "how many unplaced digits sit before this one" in
// O(log Length). K is fixed at int.MaxValue/2 (an effectively unlimited swap budget) so both
// strategies are always forced to consider the entire remaining digit list at every slot,
// rather than an early exit on a tiny budget making brute force look artificially
// competitive.
[MemoryDiagnoser]
public class MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsBenchmarks
{
    private const int UnlimitedBudget = int.MaxValue / 2;

    private const int RandomSeed = 1505; // LeetCode problem number

    private const int DigitCount = 10;

    private string _digits = "";

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _digits = new string(Enumerable.Range(0, Length).Select(_ => (char)('0' + random.Next(DigitCount))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string BruteForceListRemoval() =>
        MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution.MinIntegerByListRemoval(
            _digits, UnlimitedBudget);

    [Benchmark]
    public string FenwickTreeGreedy() =>
        MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution.MinIntegerByFenwickTreeGreedy(
            _digits, UnlimitedBudget);
}
