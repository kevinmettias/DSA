using DSAExperimentation.LeetCode.CountSequencesToK;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are CountSequencesToKSolution's, the same methods
// CountSequencesToKSolutionTests proves correct. k is fixed at 1, always
// reachable (every "leave unchanged" sequence lands on it), so both arms do
// real search work rather than short-circuiting on an unreachable target.
//
// Sizes are per arm. The brute-force arm's search is 3^Length, so it stops at 12;
// the prime-exponent memo's states grow only polynomially and it runs on to LC 3850's
// own bound of 19, and the two are compared at the lengths both run.
public class CountSequencesToKBenchmarks
{
    private const int Seed = 3850;
    private const long Target = 1;

    private Dictionary<int, int[]> _numsByLength = [];

    public static IEnumerable<int> BruteForceSizes => [8, 12];

    public static IEnumerable<int> PrimeExponentMemoSizes => [.. BruteForceSizes, 16, 19];

    // Every length any arm runs is drawn here, outside the timed region, each from its own
    // generator on the same seed; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _numsByLength = PrimeExponentMemoSizes.ToDictionary(length => length, BuildNums);

    private static int[] BuildNums(int length)
    {
        var random = new Random(Seed);
        var nums = new int[length];

        for (var i = 0; i < length; i++)
        {
            nums[i] = random.Next(1, 7);
        }

        return nums;
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BruteForceSizes))]
    public long BruteForceSearch(int length) =>
        CountSequencesToKSolution.CountSequencesByBruteForceSearch(_numsByLength[length], Target);

    [Benchmark]
    [ArgumentsSource(nameof(PrimeExponentMemoSizes))]
    public long PrimeExponentMemo(int length) =>
        CountSequencesToKSolution.CountSequencesByPrimeExponentMemo(_numsByLength[length], Target);
}
