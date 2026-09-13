using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MinimumAddToMakeParenthesesValid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumAddToMakeParenthesesValidSolution's, the same
// methods MinimumAddToMakeParenthesesValidTests proves correct - a running-counter
// balance walk that stores nothing vs. this repo's Stack<char> holding each
// unmatched opener explicitly. The random bracket string is built once in
// [GlobalSetup], so only the walk itself is measured.
[MemoryDiagnoser]
public class MinimumAddToMakeParenthesesValidBenchmarks
{
    private const int RandomSeed = 921; // LC problem number
    private const int BracketTypeExclusiveBound = 2; // picks between '(' and ')'

    [Params(1_000, 20_000)]
    public int Length;

    private string _brackets = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = random.Next(0, BracketTypeExclusiveBound) == 0 ? '(' : ')';
        }

        _brackets = new string(chars);
    }

    [Benchmark(Baseline = true)]
    public int RunningCounter() =>
        MinimumAddToMakeParenthesesValidSolution.MinAddToMakeValidByRunningCounter(_brackets);

    [Benchmark]
    public int StackOfOpeners() =>
        MinimumAddToMakeParenthesesValidSolution.MinAddToMakeValidByOpenerStack(_brackets);
}
