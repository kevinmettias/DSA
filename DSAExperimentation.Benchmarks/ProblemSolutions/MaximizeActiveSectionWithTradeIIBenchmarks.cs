using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximizeActiveSectionWithTradeII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximizeActiveSectionWithTradeIISolution's, the
// same methods MaximizeActiveSectionWithTradeIITests proves correct. The
// range-max-index arm is handed the ActiveSectionTradeIndex its hoisted
// overload takes, built once in [GlobalSetup], so the O(n log n) preprocessing
// is charged to setup rather than to the per-query answering being measured.
// Query widths are random anywhere in [0, Length), including near-full-length
// ones, so the run-scan baseline pays a genuinely varying O(range) cost per
// query rather than a uniformly small one.
[MemoryDiagnoser]
public class MaximizeActiveSectionWithTradeIIBenchmarks
{
    private const int Seed = 3501;
    private const int MaxQueryCount = 200;
    private const int ZeroOutOfFiveWeight = 2; private string _s = "";

    private int[][] _queries = [];
    private ActiveSectionTradeIndex _index = null!;
    // biases toward more zero-runs, so trades have something to work with

    [Params(200, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _s = BuildBinaryString(Length, random);
        _queries = BuildQueries(Length, random);
        _index = new ActiveSectionTradeIndex(_s);
    }

    private static string BuildBinaryString(int length, Random random)
    {
        var chars = new char[length];

        for (var i = 0; i < length; i++)
        {
            var isZero = random.Next(5) < ZeroOutOfFiveWeight;
            chars[i] = isZero ? '0' : '1';
        }

        return new string(chars);
    }

    private static int[][] BuildQueries(int length, Random random)
    {
        var queryCount = Math.Min(length, MaxQueryCount);
        var queries = new int[queryCount][];

        for (var i = 0; i < queryCount; i++)
        {
            var left = random.Next(0, length);
            var right = random.Next(left, length);
            queries[i] = [left, right];
        }

        return queries;
    }

    [Benchmark(Baseline = true)]
    public int[] RunScan() => MaximizeActiveSectionWithTradeIISolution.MaxActiveAfterTradeByRunScan(_s, _queries);

    [Benchmark]
    public int[] RangeMaxIndex() =>
        MaximizeActiveSectionWithTradeIISolution.MaxActiveAfterTradeByRangeMaxIndex(_index, _queries);
}
