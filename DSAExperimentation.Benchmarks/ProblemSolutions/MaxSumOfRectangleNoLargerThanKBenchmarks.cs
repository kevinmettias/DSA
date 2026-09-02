using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaxSumOfRectangleNoLargerThanK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaxSumOfRectangleNoLargerThanKSolution's, the same
// methods MaxSumOfRectangleNoLargerThanKTests proves correct. Columns stay fixed at
// a small constant (Cols) so both methods pay the same O(cols^2) outer-loop factor;
// Rows is the scaled [Params] axis, isolating the O(rows^2) vs. O(rows*log(rows))
// inner-window asymptotic split the same way MaximumSubarrayBenchmarks isolates
// brute-force vs. Kadane on array Length alone. A brute-force O(cols^2*rows^2) with
// both axes scaled together would blow up to a 4th-power cost long before the BST's
// per-node allocation overhead stops dominating at small n. The matrix LeetCode's
// own signature takes (int[][]) is exactly what [GlobalSetup] builds, so there is
// no separate hoisted overload to add here - construction is already charged to
// setup, not to either measured method.
[MemoryDiagnoser]
public class MaxSumOfRectangleNoLargerThanKBenchmarks
{
    private const int K = 50;
    private const int Cols = 8;
    private const int RandomSeed = 363; // LC problem number
    private const int MinCellValue = -10;
    private const int MaxCellValueExclusive = 11;

    [Params(200, 3_000)]
    public int Rows;

    private int[][] _matrix = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _matrix = Enumerable.Range(0, Rows)
            .Select(_ => Enumerable.Range(0, Cols).Select(_ => random.Next(MinCellValue, MaxCellValueExclusive)).ToArray())
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public int BruteForceWindowScan() => MaxSumOfRectangleNoLargerThanKSolution.MaxSumSubmatrixByBruteForceWindowScan(_matrix, K);

    [Benchmark]
    public int BstCeilingScan() => MaxSumOfRectangleNoLargerThanKSolution.MaxSumSubmatrixByBstCeilingScan(_matrix, K);
}
