using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumInsertionsToBalanceAParenthesesString;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumInsertionsToBalanceAParenthesesStringSolution's.
// A uniformly random bracket string keeps openers and closers interleaved, so the
// scalar counter and the explicit Stack<char> of openers both stay busy for the whole
// scan rather than degenerating into one long run of either bracket.
[MemoryDiagnoser]
public class MinimumInsertionsToBalanceAParenthesesStringBenchmarks
{
    private const int RandomSeed = 1541; // LC problem number
    private const int BracketKindCount = 2;

    private string _brackets = "";

    [Params(1_000, 20_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            var isOpener = random.Next(0, BracketKindCount) == 0;
            chars[i] = isOpener ? '(' : ')';
        }

        _brackets = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int RunningCounter() =>
        MinimumInsertionsToBalanceAParenthesesStringSolution.MinInsertionsByRunningCounter(_brackets);

    [Benchmark]
    public int StackOfOpeners() =>
        MinimumInsertionsToBalanceAParenthesesStringSolution.MinInsertionsByOpenerStack(_brackets);
}
