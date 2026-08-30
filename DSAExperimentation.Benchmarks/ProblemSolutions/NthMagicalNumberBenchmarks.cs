using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Nth Magical Number (LC 878): counting candidates one at a time (brute force,
// O(answer)) vs. BinarySearch.LowerBound over the monotone "count(x) >= n" virtual
// sequence (O(log(answer))) - the same shape SqrtX/FirstBadVersion already exercise,
// just with a two-term inclusion-exclusion predicate per index instead of a single
// comparison.
[MemoryDiagnoser]
public class NthMagicalNumberBenchmarks
{
    private const int A = 6;
    private const int B = 10;

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

            if (x % A == 0 || x % B == 0)
            {
                count++;
            }
        }

        return x;
    }

    [Benchmark]
    public int BinarySearchOnCount()
    {
        var lcm = (long)A / Gcd(A, B) * B;
        var upperBound = (int)((long)N * Math.Min(A, B));
        var sequence = new MagicalCountSequence(N, A, B, lcm, upperBound);

        return BinarySearch.LowerBound<int, MagicalCountSequence>(sequence, 1);
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    private readonly struct MagicalCountSequence(int n, int a, int b, long lcm, int upperBound)
        : IRandomAccessSequence<int>
    {
        public int Length => upperBound + 1;

        public int Get(int index) => (index / a) + (index / b) - (index / lcm) >= n ? 1 : 0;
    }
}
