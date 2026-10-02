using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinStack;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinStackSolution's, the same factories MinStackTests proves correct.
// [GlobalSetup] draws one fixed value sequence, so the script construction is charged to setup and
// only the replay is measured. Each replay pushes every value with a GetMin behind it, then reads
// Top and pops every value back off - the whole four-operation surface, so the scan arm's O(n)
// GetMin is charged Length times rather than once.
[MemoryDiagnoser]
public class MinStackBenchmarks
{
    private const int RandomSeed = 155; // LC problem number
    private const int ValueCeiling = 1_000_000;

    private int[] _pushed = [];

    [Params(500, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _pushed = Enumerable.Range(0, Length).Select(_ => random.Next(ValueCeiling)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int SingleListScan() => Replay(MinStackSolution.CreateBySingleListScan());

    [Benchmark]
    public int StackPrimitive() => Replay(MinStackSolution.CreateByStackPrimitive());

    private int Replay(MinStackSolution.IMinStackOperations stack)
    {
        var checksum = 0;

        foreach (var value in _pushed)
        {
            stack.Push(value);
            checksum += stack.GetMin();
        }

        for (var remaining = _pushed.Length; remaining > 0; remaining--)
        {
            checksum += stack.Top();
            stack.Pop();
        }

        return checksum;
    }
}
