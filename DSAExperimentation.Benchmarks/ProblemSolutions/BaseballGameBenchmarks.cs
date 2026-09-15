using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.BaseballGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Baseball Game (LC 682): harness only - both arms are BaseballGameSolution's,
// the same methods BaseballGameTests proves correct. _ops never emits "C" so
// both strategies grow monotonically, keeping the comparison about push/peek/pop
// cost rather than the shared "C" underflow path.
[MemoryDiagnoser]
public class BaseballGameBenchmarks
{
    private const string DoubleOp = "D";
    private const string SumOp = "+";
    private const int OpCyclePeriod = 5;
    private const int DoubleOpRemainder = 2;
    private const int SumOpRemainder = 3;
    private const int MaxBaseScore = 50;

    private string[] _ops = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup() => _ops = BuildOperations(Length);

    private static string[] BuildOperations(int length)
    {
        var ops = new string[length];

        for (var i = 0; i < length; i++)
        {
            ops[i] = (i % OpCyclePeriod) switch
            {
                DoubleOpRemainder => DoubleOp,
                SumOpRemainder => SumOp,
                _ => ((i % MaxBaseScore) + 1).ToString(),
            };
        }

        return ops;
    }

    [Benchmark(Baseline = true)]
    public int ManualArrayCursor() => BaseballGameSolution.CalPointsByManualArrayCursor(_ops);

    [Benchmark]
    public int StackReplay() => BaseballGameSolution.CalPointsByStackReplay(_ops);
}
