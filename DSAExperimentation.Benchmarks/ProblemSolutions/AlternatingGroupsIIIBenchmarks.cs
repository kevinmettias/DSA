using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.AlternatingGroupsIII;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are AlternatingGroupsIIISolution's, the same methods
// AlternatingGroupsIIITests proves correct. Workload is mostly size queries
// (four in five) with a scattering of repaints between them - size queries are
// where AlternatingRunLedger's O(log n) prefix lookups have to justify
// themselves against the baseline's O(n) circle scan; window size is drawn from
// LeetCode's own [3, Length-1] range but a random circle rarely stays
// alternating past the first couple of tiles, so the baseline's early exit keeps
// its per-query cost close to O(n) rather than the O(n * size) worst case.
[MemoryDiagnoser]
public class AlternatingGroupsIIIBenchmarks
{
    private const int Seed = 3245;
    private const int UpdateEveryNth = 5;

    private int[] _colors = [];

    private int[][] _queries = [];
    [Params(1_000, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(Seed);
        _colors = Enumerable.Range(0, Length).Select(_ => random.Next(2)).ToArray();

        var queries = new int[Length][];

        for (var i = 0; i < Length; i++)
        {
            var isRepaint = i % UpdateEveryNth == 0;
            queries[i] = isRepaint
                ? RepaintQuery(random, Length)
                : SizeQuery(random, Length);
        }

        _queries = queries;
    }

    // Only one of these two runs per query, so whichever it is draws its indices
    // and nothing else - the other must not touch `random`, or the workload's
    // draw sequence stops matching the seed it was written against.
    private static int[] RepaintQuery(Random random, int length) => [2, random.Next(length), random.Next(2)];

    private static int[] SizeQuery(Random random, int length) => [1, random.Next(3, length)];

    [Benchmark(Baseline = true)]
    public IList<int> BruteForce() =>
        AlternatingGroupsIIISolution.NumberOfAlternatingGroupsByBruteForce(_colors, _queries);

    [Benchmark]
    public IList<int> RunLengthFenwick() =>
        AlternatingGroupsIIISolution.NumberOfAlternatingGroupsByRunLengthFenwick(_colors, _queries);
}
