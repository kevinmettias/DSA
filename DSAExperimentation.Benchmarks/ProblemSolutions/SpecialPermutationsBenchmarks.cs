using DSAExperimentation.LeetCode.SpecialPermutations;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are SpecialPermutationsSolution's, the same methods
// SpecialPermutationsSolutionTests proves correct - enumerate all n! orderings with
// this repo's own Backtrack.Search and check the adjacency rule once each one
// is complete, vs. threading (Remaining, Last) through this repo's own Memoizer,
// visiting each reachable state at most once regardless of how many orderings are legal.
//
// Sizes are per arm. The enumeration is O(n! * n), so it stops at 10; the bitmask
// memo is O(n^2 * 2^n) and runs on to LC 2741's own bound of 14. The two are compared
// at the sizes both run.
public class SpecialPermutationsBenchmarks
{
    private const int MaxValueExclusive = 60;

    // LC problem number, reused as the deterministic element seed.
    private const int Seed = 2741;

    private Dictionary<int, int[]> _numsBySize = [];

    public static IEnumerable<int> BaselineSizes => [8, 10];

    public static IEnumerable<int> BitmaskSizes => [.. BaselineSizes, 12, 14];

    // Every size any arm runs is built here, outside the timed region; an arm looks its own up.
    [GlobalSetup]
    public void Setup() => _numsBySize = BitmaskSizes.ToDictionary(length => length, BuildNums);

    private static int[] BuildNums(int length)
    {
        var random = new Random(Seed);
        var distinct = new HashSet<int>();

        while (distinct.Count < length)
        {
            var value = random.Next(1, MaxValueExclusive);
            distinct.Add(value);
        }

        return [.. distinct];
    }

    [Benchmark(Baseline = true)]
    [ArgumentsSource(nameof(BaselineSizes))]
    public int BruteForceBacktracking(int length) =>
        SpecialPermutationsSolution.CountByBruteForceBacktracking(_numsBySize[length]);

    [Benchmark]
    [ArgumentsSource(nameof(BitmaskSizes))]
    public int BitmaskMemo(int length) => SpecialPermutationsSolution.CountByBitmaskMemo(_numsBySize[length]);
}
