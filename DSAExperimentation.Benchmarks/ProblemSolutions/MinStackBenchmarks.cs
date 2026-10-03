using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.MinStack;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinStackSolution's, the same factories MinStackSolutionTests proves correct.
// [GlobalSetup] draws one fixed value sequence, so the script construction is charged to setup and
// only the replay is measured. Each replay pushes every value with a GetMin behind it, then reads
// Top and pops every value back off - the whole four-operation surface, so the scan arm's O(n)
// GetMin is charged Length times rather than once. Each arm returns every GetMin and Top answer
// in call order; Push and Pop return nothing.
public class MinStackBenchmarks
{
    private const int RandomSeed = 155; // LC problem number
    private const int ValueCeiling = 1_000_000;

    // One GetMin per push and one Top per pop.
    private const int AnswersPerValue = 2;

    private int[] _pushed = [];

    private int[] _answers = [];

    [Params(500, 2_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);

        _pushed = SeededDraws.Values(Length, 0, ValueCeiling, random);
        _answers = new int[_pushed.Length * AnswersPerValue];
    }

    [Benchmark(Baseline = true)]
    public int[] SingleListScan() => Replay(MinStackSolution.CreateBySingleListScan());

    [Benchmark]
    public int[] StackPrimitive() => Replay(MinStackSolution.CreateByStackPrimitive());

    private int[] Replay(MinStackSolution.IMinStackOperations stack)
    {
        var answered = 0;

        foreach (var value in _pushed)
        {
            stack.Push(value);
            _answers[answered++] = stack.GetMin();
        }

        for (var remaining = _pushed.Length; remaining > 0; remaining--)
        {
            _answers[answered++] = stack.Top();
            stack.Pop();
        }

        return _answers;
    }
}
