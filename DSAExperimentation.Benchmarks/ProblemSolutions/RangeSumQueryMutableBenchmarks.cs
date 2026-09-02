using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Range Sum Query - Mutable (LC 307): a raw-array baseline (Update writes the slot directly,
// O(1); SumRange rescans the range, O(n)) vs. this repo's own SegmentTree<int,SumOperation<int>>
// (Update and Query both O(log n)). Both process the same interleaved stream of update/sumRange
// calls, rebuilt fresh each iteration so mutation from one run never leaks into the next.
[MemoryDiagnoser]
public class RangeSumQueryMutableBenchmarks
{
    private const int OperationCount = 500;
    private const int RandomSeed = 307; // LC problem number
    private const int ValueBound = 1_000;
    private const int OperationTypeCount = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _initial = null!;
    private (bool IsUpdate, int A, int B)[] _operations = null!;

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
    public long ArrayRescan()
    {
        var nums = (int[])_initial.Clone();
        var total = 0L;

        foreach (var (isUpdate, a, b) in _operations)
        {
            if (isUpdate)
            {
                nums[a] = b;
            }
            else
            {
                for (var i = a; i <= b; i++)
                {
                    total += nums[i];
                }
            }
        }

        return total;
    }

    [Benchmark]
    public long SegmentTreeQuery()
    {
        var tree = new SegmentTree<int, SumOperation<int>>(_initial);
        var total = 0L;

        foreach (var (isUpdate, a, b) in _operations)
        {
            if (isUpdate)
            {
                tree.Update(a, b);
            }
            else
            {
                total += tree.Query(a, b);
            }
        }

        return total;
    }
}
