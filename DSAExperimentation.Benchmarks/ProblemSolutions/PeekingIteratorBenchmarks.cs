using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.PeekingIterator;
using static DSAExperimentation.LeetCode.PeekingIterator.PeekingIteratorSolution;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are PeekingIteratorSolution's, the same factories
// PeekingIteratorTests proves correct. Both drive the same peek-peek-next
// pattern to full exhaustion, summing every returned value so the JIT can't
// eliminate the drain as dead code.
[MemoryDiagnoser]
public class PeekingIteratorBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup() => _values = Enumerable.Range(0, Length).ToArray();

    [Benchmark(Baseline = true)]
    public long IndexTracked() => Drain(CreateByIndexTracked(_values));

    [Benchmark]
    public long QueuePrimitive() => Drain(CreateByQueuePrimitive(_values));

    private static long Drain(IPeekingIterator iterator)
    {
        var sum = 0L;

        while (iterator.HasNext())
        {
            sum += iterator.Peek();
            sum += iterator.Next();
        }

        return sum;
    }
}
