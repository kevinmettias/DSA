using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Sorting;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Deletions to Make Array Divisible (LC 2344): both strategies reduce
// numsDivide to one divisor via the same O(m) Euclidean gcd fold, then diverge on how
// they find the fewest deletions from nums. numsDivide is every entry set to the same
// fixed BaseDivisor (720, chosen for its 30 divisors) so the fold is trivial and the
// reduced divisor is always exactly BaseDivisor; nums is drawn entirely from
// BaseDivisor's own divisors, so EVERY element is a valid candidate - the worst case for
// CountValidPrecedingBruteForce, which checks each one via a fresh full-array scan for
// how many smaller elements exist, O(n^2). MergeSortAndScan instead sorts nums once with
// this repo's own Algorithms.Sorting.MergeSort over an ArrayIndexedSequence<int> and
// returns the index of the first element that divides - since 1 is among BaseDivisor's
// divisors and virtually always present for n this large, the sorted scan resolves in
// O(1) after the O(n log n) sort, making the asymptotic gap visible even though both
// strategies compute the same answer.
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
    {
        var divisor = GcdOfArray(_numsDivide);
        var best = -1;

        foreach (var candidate in _nums)
        {
            if (divisor % candidate != 0)
            {
                continue;
            }

            var precedingCount = CountSmaller(candidate);
            if (best == -1 || precedingCount < best)
            {
                best = precedingCount;
            }
        }

        return best;
    }

    private int CountSmaller(int candidate)
    {
        var count = 0;

        foreach (var value in _nums)
        {
            if (value < candidate)
            {
                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int MergeSortAndScan()
    {
        var divisor = GcdOfArray(_numsDivide);
        var sorted = _nums.ToArray();
        MergeSort.Sort<int, ArrayIndexedSequence<int>>(new ArrayIndexedSequence<int>(sorted));

        for (var i = 0; i < sorted.Length; i++)
        {
            if (divisor % sorted[i] == 0)
            {
                return i;
            }
        }

        return -1;
    }

    private static int GcdOfArray(int[] values)
    {
        var divisor = values[0];

        foreach (var value in values)
        {
            divisor = Gcd(divisor, value);
        }

        return divisor;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);
}
