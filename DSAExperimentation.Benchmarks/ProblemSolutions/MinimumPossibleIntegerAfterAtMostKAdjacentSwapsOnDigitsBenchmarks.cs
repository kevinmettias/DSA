using DSAExperimentation.LeetCode.MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigits;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsSolution's, the same methods the
// coverage test proves correct - the O(Length^2) physical List<char> simulation against the
// Fenwick-tree greedy that answers "how many unplaced digits sit before this one" in
// O(log Length). K is fixed at LC 1505's own ceiling of 10^9 - an effectively unlimited
// swap budget, since no Length here can use more than Length^2 / 2 swaps - so both
// strategies are always forced to consider the entire remaining digit list at every slot,
// rather than an early exit on a tiny budget making brute force look artificially
// competitive.
public class MinimumPossibleIntegerAfterAtMostKAdjacentSwapsOnDigitsBenchmarks
{
    private const int UnlimitedBudget = 1_000_000_000;

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
