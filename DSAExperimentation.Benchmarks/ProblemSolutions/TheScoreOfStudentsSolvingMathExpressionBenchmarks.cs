using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TheScoreOfStudentsSolvingMathExpression;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TheScoreOfStudentsSolvingMathExpressionSolution's, the
// same methods TheScoreOfStudentsSolvingMathExpressionTests proves correct. Plain
// recursion with no caching - every (left, right) sub-interval recomputed from scratch
// each time a different split path reaches it, the same overlapping-subproblems blowup
// matrix chain multiplication has - vs. the identical recurrence over this repo's own
// Memoizer<TState,TResult>.
//
// [GlobalSetup] builds LeetCode's own input shape (the expression string) rather than
// the parsed token arrays the pre-migration benchmark hoisted: parsing is one O(n) pass,
// identical in both arms and negligible against the interval search, so no prepared-input
// overload is warranted and none could be added without either an ambiguous BCL parameter
// or a type invented for the purpose.
[MemoryDiagnoser]
public class TheScoreOfStudentsSolvingMathExpressionBenchmarks
{
    private const int WorkloadSeed = 1;
    private const int MaxDigitValueExclusive = 9;
    private const int OperatorChoiceCount = 2;
    private const int AnswerCount = 5;
    private const int MaxAnswerExclusive = 1_001;

    private string _expression = "";

    private int[] _answers = [];
    [Params(6, 10)]
    public int NumberCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(WorkloadSeed);
        var numbers = Enumerable.Range(0, NumberCount)
            .Select(_ => random.Next(1, MaxDigitValueExclusive))
            .ToArray();
        var ops = Enumerable.Range(0, NumberCount - 1)
            .Select(_ => IsPlus(random) ? '+' : '*')
            .ToArray();

        _expression = BuildExpression(numbers, ops);
        _answers = Enumerable.Range(0, AnswerCount)
            .Select(_ => random.Next(0, MaxAnswerExclusive))
            .ToArray();
    }

    private static bool IsPlus(Random random) => random.Next(0, OperatorChoiceCount) == 0;

    private static string BuildExpression(int[] numbers, char[] ops) =>
        string.Concat(Enumerable.Range(0, numbers.Length)
            .Select(i => i == 0 ? $"{numbers[i]}" : $"{ops[i - 1]}{numbers[i]}"));

    [Benchmark(Baseline = true)]
    public int UnmemoizedRecursion() =>
        TheScoreOfStudentsSolvingMathExpressionSolution.ScoreOfStudentsByUnmemoizedRecursion(
            _expression, _answers);

    [Benchmark]
    public int MemoizedIntervals() =>
        TheScoreOfStudentsSolvingMathExpressionSolution.ScoreOfStudentsByMemoizedIntervals(
            _expression, _answers);
}
