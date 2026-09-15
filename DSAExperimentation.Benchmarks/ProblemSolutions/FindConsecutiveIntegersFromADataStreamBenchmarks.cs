using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindConsecutiveIntegersFromADataStream;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindConsecutiveIntegersFromADataStreamSolution's, the
// same classes FindConsecutiveIntegersFromADataStreamTests proves correct - appending
// every arrival to an unbounded BCL history and rescanning its last k entries on every
// call (O(n * k)) against this repo's own Deque<int> holding a fixed-size window with
// an incrementally maintained match count (O(n)). [GlobalSetup] generates the streamed
// values, so stream generation is charged to setup rather than to the replay each arm
// measures. `Value` is rare in the stream, so the window almost never gives either arm
// a chance to short-circuit early.
[MemoryDiagnoser]
public class FindConsecutiveIntegersFromADataStreamBenchmarks
{
    private const int RandomSeed = 2526; // LC problem number
    private const int Value = 7;
    private const int K = 20;
    private const int ValueBoundExclusive = 1_000;

    private int[] _values = [];

    [Params(2_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(0, ValueBoundExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int UnboundedHistoryRescan() => CountConsecutiveHits(new FindConsecutiveIntegersFromADataStreamSolution.DataStreamByHistoryRescan(Value, K));

    [Benchmark]
    public int FixedWindowIncrementalCount() => CountConsecutiveHits(new FindConsecutiveIntegersFromADataStreamSolution.DataStreamByFixedWindow(Value, K));

    // Counts the true answers rather than discarding each Consec result, so the JIT
    // cannot eliminate the replay as dead code.
    private int CountConsecutiveHits(FindConsecutiveIntegersFromADataStreamSolution.IDataStreamStrategy stream)
    {
        var trueCount = 0;

        foreach (var num in _values)
        {
            if (stream.Consec(num))
            {
                trueCount++;
            }
        }

        return trueCount;
    }
}
