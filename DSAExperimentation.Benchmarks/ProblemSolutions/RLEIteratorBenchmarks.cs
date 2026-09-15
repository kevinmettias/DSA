using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RLEIterator;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RLEIteratorSolution's, the same factories
// RLEIteratorTests proves correct. The canonical approach eagerly decompresses the
// whole run-length encoding into a flat int[] and walks it with an index cursor -
// O(total element count) time and memory up front, which on LeetCode's own
// constraints (individual run counts up to 1e9) can be far larger than the
// encoding itself. The alternative consumes runs lazily from this repo's own FIFO
// Queue<(int,int)> - O(number of runs) memory and O(total next-calls + number of
// runs) time. Construction stays inside each measured method deliberately: building
// the iterator IS the difference between the two strategies.
[MemoryDiagnoser]
public class RLEIteratorBenchmarks
{
    private const int RunCount = 100;
    private const int RandomSeed = 900;
    private const int MaxRunValueExclusive = 1_000;
    private const int MaxQueryLength = 50;

    private int[] _encoding = [];

    private int[] _queries = [];
    [Params(2_000, 200_000)]
    public int TotalCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _encoding = BuildEncoding(random, TotalCount);
        _queries = BuildQueries(random, TotalCount);
    }

    private static int[] BuildEncoding(Random random, int totalCount)
    {
        var encoding = new int[RunCount * RunEncoding.ValuesPerRun];
        var remaining = totalCount;

        for (var run = 0; run < RunCount; run++)
        {
            var runsLeft = RunCount - run;
            var isLastRun = run == RunCount - 1;
            var count = isLastRun ? remaining : Math.Max(1, remaining / runsLeft);
            remaining -= count;

            encoding[run * RunEncoding.ValuesPerRun] = count;
            encoding[(run * RunEncoding.ValuesPerRun) + 1] = random.Next(1, MaxRunValueExclusive);
        }

        return encoding;
    }

    private static int[] BuildQueries(Random random, int totalCount)
    {
        var queries = new List<int>();
        var remaining = totalCount;

        while (remaining > 0)
        {
            var candidateLength = random.Next(1, MaxQueryLength);
            var n = Math.Min(remaining, candidateLength);
            queries.Add(n);
            remaining -= n;
        }

        return [.. queries];
    }

    [Benchmark(Baseline = true)]
    public int DecompressedArrayCursor() => Drain(RLEIteratorSolution.CreateByDecompressedArray(_encoding));

    [Benchmark]
    public int RunLengthQueue() => Drain(RLEIteratorSolution.CreateByRunLengthQueue(_encoding));

    private int Drain(IRleIterator iterator)
    {
        var last = -1;

        foreach (var n in _queries)
        {
            last = iterator.Next(n);
        }

        return last;
    }
}
