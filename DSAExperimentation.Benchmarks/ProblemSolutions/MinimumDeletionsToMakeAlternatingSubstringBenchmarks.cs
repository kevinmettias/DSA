using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumDeletionsToMakeAlternatingSubstring;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are
// MinimumDeletionsToMakeAlternatingSubstringSolution's, the same methods
// MinimumDeletionsToMakeAlternatingSubstringTests proves correct.
//
// Every query is type-2 (a full range query, never a flip), forcing
// DirectScan through its O(r - l) rescan on every single one - the arm the
// Fenwick range-sum strategy has to beat.
[MemoryDiagnoser]
public class MinimumDeletionsToMakeAlternatingSubstringBenchmarks
{
    private const int RandomSeed = 3777; // LC problem number
    private const int QueryCount = 200;

    private string _s = "";

    private int[][] _queries = [];
    [Params(500, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            var isA = random.Next(2) == 0;
            chars[i] = isA ? 'A' : 'B';
        }

        _s = new string(chars);
        _queries = Enumerable.Range(0, QueryCount).Select(_ => BuildRangeQuery(random, Length)).ToArray();
    }

    private static int[] BuildRangeQuery(Random random, int length)
    {
        var left = random.Next(length);
        var right = random.Next(left, length);

        return [2, left, right];
    }

    [Benchmark(Baseline = true)]
    public int[] DirectScan() =>
        MinimumDeletionsToMakeAlternatingSubstringSolution.ProcessQueriesByDirectScan(_s, _queries);

    [Benchmark]
    public int[] FenwickAdjacency() =>
        MinimumDeletionsToMakeAlternatingSubstringSolution.ProcessQueriesByFenwickAdjacency(_s, _queries);
}
