using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.NumberOfIntegersWithPopcountDepthEqualToKII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are NumberOfIntegersWithPopcountDepthEqualToKIISolution's,
// the same methods NumberOfIntegersWithPopcountDepthEqualToKIITests proves
// correct (OpenTheLockBenchmarks precedent for the hoisted prepared-input
// overload - PopcountDepthFenwickIndex here plays LockGraph's role). Building
// the index is real, isolable setup cost, so [IterationSetup] rebuilds it
// fresh before every invocation rather than [GlobalSetup] doing it once: the
// same queries reapply the same updates on every iteration, and an index left
// over from the previous run would make each later update a same-depth no-op
// instead of the real Add(-1)/Add(+1) pair being measured
// (MatrixCellsInDistanceOrderBenchmarks' own per-iteration reset precedent).
// N stays moderate for the brute-force arm - it rescans up to N elements per
// range query - while the Fenwick-bucket arm scales to the real problem's N
// and query count each up to 1e5 trivially.
[MemoryDiagnoser]
public class NumberOfIntegersWithPopcountDepthEqualToKIIBenchmarks
{
    private const int Seed = 3624; // LC problem number
    private const int QueryCount = 2_000;
    private const long MaxValue = 1_000_000_000_000_000; // 10^15, LC's own bound
    private const int UpdateQuery = 2;

    [Params(200, 2_000)]
    public int N;

    private long[] _nums = null!;
    private long[][] _queries = null!;
    private PopcountDepthFenwickIndex _index = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        var nums = new long[N];

        for (var i = 0; i < N; i++)
        {
            nums[i] = random.NextInt64(1, MaxValue);
        }

        _nums = nums;

        var queries = new long[QueryCount][];

        for (var i = 0; i < QueryCount; i++)
        {
            if (i % 2 == 0)
            {
                var left = random.Next(N);
                var right = left + random.Next(N - left);
                var k = random.Next(0, PopcountDepthFenwickIndex.MaxTrackedDepth + 1);
                queries[i] = [1, left, right, k];
            }
            else
            {
                queries[i] = [UpdateQuery, random.Next(N), random.NextInt64(1, MaxValue)];
            }
        }

        _queries = queries;
    }

    [IterationSetup]
    public void IterationSetup() => _index = PopcountDepthFenwickIndex.Build(_nums);

    [Benchmark(Baseline = true)]
    public int[] BruteForce() =>
        NumberOfIntegersWithPopcountDepthEqualToKIISolution.PopcountDepthByBruteForce(_nums, _queries);

    [Benchmark]
    public int[] FenwickBuckets() =>
        NumberOfIntegersWithPopcountDepthEqualToKIISolution.PopcountDepthByFenwickBuckets(_index, _queries);
}
