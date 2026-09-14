using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumDeletionsToMakeArrayDivisible;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumDeletionsToMakeArrayDivisibleSolution's, proved
// equivalent by MinimumDeletionsToMakeArrayDivisibleTests. They share the same O(m) Euclidean
// gcd fold and diverge on how they find the fewest deletions.
//
// The workload is chosen to make the asymptotic gap visible: numsDivide is every entry set to
// the same BaseDivisor (720, for its 30 divisors) so the fold is trivial and the reduced
// divisor is always exactly BaseDivisor, and nums is drawn entirely from BaseDivisor's own
// divisors, so EVERY element is a valid candidate - the worst case for the candidate scan,
// which pays a fresh full-array count per element, O(n^2). The MergeSort arm sorts once in
// O(n log n) and, since 1 is among those divisors and virtually always present at this n,
// resolves its scan in O(1).
[MemoryDiagnoser]
public class MinimumDeletionsToMakeArrayDivisibleBenchmarks
{
    private const int RandomSeed = 2344;
    private const int BaseDivisor = 720;

    private static readonly int[] DivisorsOfBaseDivisor =
        [1, 2, 3, 4, 5, 6, 8, 9, 10, 12, 15, 16, 18, 20, 24, 30, 36, 40, 45, 48, 60, 72, 80, 90, 120, 144, 180, 240, 360, 720];

    [Params(200, 3_000)]
    public int Length;

    private int[] _nums = null!;
    private int[] _numsDivide = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => DivisorsOfBaseDivisor[random.Next(DivisorsOfBaseDivisor.Length)]).ToArray();
        _numsDivide = Enumerable.Repeat(BaseDivisor, Length).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int CountValidPrecedingBruteForce()
        => MinimumDeletionsToMakeArrayDivisibleSolution.MinDeletionsByCandidateScan(_nums, _numsDivide);

    [Benchmark]
    public int MergeSortAndScan()
        => MinimumDeletionsToMakeArrayDivisibleSolution.MinDeletionsByMergeSort(_nums, _numsDivide);
}
