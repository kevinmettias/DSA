using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Ugly Number III (LC 1201): counting candidates one at a time (brute force,
// O(answer)) vs. BinarySearch.LowerBound over the monotone "count(x) >= n" virtual
// sequence (O(log(answer))) - the same shape NthMagicalNumberBenchmarks already
// exercises, extended from a two-term to a three-term inclusion-exclusion
// predicate per index.
[MemoryDiagnoser]
public class UglyNumberIIIBenchmarks
{
    private const int A = 2;
    private const int B = 3;
    private const int C = 5;

    [Params(2_000, 50_000)]
    public int N;

    [Benchmark(Baseline = true)]
    public int BruteForceCount()
    {
        var count = 0;
        var x = 0;

        while (count < N)
        {
            x++;

            if (x % A == 0 || x % B == 0 || x % C == 0)
            {
                count++;
            }
        }

        return x;
    }

    [Benchmark]
    public int BinarySearchOnCount()
    {
        var lcmAb = Lcm(A, B);
        var lcmAc = Lcm(A, C);
        var lcmBc = Lcm(B, C);
        var smallerOfBAndC = Math.Min(B, C);
        var upperBound = (int)((long)N * Math.Min(A, smallerOfBAndC));
        var lcmAbc = LcmCapped(lcmAb, C, upperBound);

        var sequence = new UglyCountSequence(N, A, B, C, lcmAb, lcmAc, lcmBc, lcmAbc, upperBound);

        return BinarySearch.LowerBound<int, UglyCountSequence>(sequence, 1);
    }

    private static long Gcd(long x, long y) => y == 0 ? x : Gcd(y, x % y);

    private static long Lcm(long x, long y) => x / Gcd(x, y) * y;

    private static long LcmCapped(long x, long y, long cap) => x > cap ? cap + 1 : Lcm(x, y);

    private readonly struct UglyCountSequence(
        int n, int a, int b, int c, long lcmAb, long lcmAc, long lcmBc, long lcmAbc, int upperBound)
        : IRandomAccessSequence<int>
    {
        public int Length => upperBound + 1;

        public int Get(int index)
        {
            long x = index;
            var count = (x / a) + (x / b) + (x / c) - (x / lcmAb) - (x / lcmAc) - (x / lcmBc) + (x / lcmAbc);
            return count >= n ? 1 : 0;
        }
    }
}
