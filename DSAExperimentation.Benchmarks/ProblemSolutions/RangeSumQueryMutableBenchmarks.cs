using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.RangeSumQueryMutable;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are RangeSumQueryMutableSolution's, the same factories
// RangeSumQueryMutableTests proves correct - a raw-array baseline (Update writes the slot
// directly, O(1); SumRange rescans the range, O(n)) vs. this repo's own
// SegmentTree<int,SumOperation<int>> (Update and Query both O(log n)). Both process the same
// interleaved stream of update/sumRange calls against a fresh instance built by their own factory
// each iteration, so mutation from one run never leaks into the next.
[MemoryDiagnoser]
public class RangeSumQueryMutableBenchmarks
{
    private const int OperationCount = 500;
    private const int RandomSeed = 307; // LC problem number
    private const int ValueBound = 1_000;
    private const int OperationTypeCount = 2;

    private int[] _initial = [];

    private (bool IsUpdate, int A, int B)[] _operations = [];
    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _initial = Enumerable.Range(0, Length).Select(_ => random.Next(-ValueBound, ValueBound)).ToArray();

        _operations = new (bool IsUpdate, int A, int B)[OperationCount];
        for (var i = 0; i < OperationCount; i++)
        {
            if (random.Next(OperationTypeCount) == 0)
            {
                _operations[i] = (true, random.Next(0, Length), random.Next(-ValueBound, ValueBound));
            }
            else
            {
                var left = random.Next(0, Length);
                var right = random.Next(left, Length);
                _operations[i] = (false, left, right);
            }
        }
    }

    [Benchmark(Baseline = true)]
    public long ArrayRescan() => Replay(RangeSumQueryMutableSolution.CreateByArrayRescan(_initial));

    [Benchmark]
    public long SegmentTreeQuery() => Replay(RangeSumQueryMutableSolution.CreateBySegmentTreeQuery(_initial));

    // Sums every returned SumRange result rather than discarding it, so the JIT can't eliminate
    // the replay as dead code - the same "return the real answer, not a weaker proxy" shape
    // OpenTheLockBenchmarks/LRUCacheBenchmarks already follow.
    private long Replay(INumArray numArray)
    {
        var total = 0L;

        foreach (var (isUpdate, a, b) in _operations)
        {
            if (isUpdate)
            {
                numArray.Update(a, b);
            }
            else
            {
                total += numArray.SumRange(a, b);
            }
        }

        return total;
    }
}
