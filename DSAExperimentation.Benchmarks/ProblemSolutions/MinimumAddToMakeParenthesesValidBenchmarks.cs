using DSAExperimentation.LeetCode.MinimumAddToMakeParenthesesValid;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumAddToMakeParenthesesValidSolution's, the same
// methods MinimumAddToMakeParenthesesValidSolutionTests proves correct - a running-counter
// balance walk that stores nothing vs. this repo's Stack<char> holding each
// unmatched opener explicitly. The random bracket string is built once in
// [GlobalSetup], so only the walk itself is measured. Length stops at LC 921's own
// bound of 1,000 characters.
public class MinimumAddToMakeParenthesesValidBenchmarks
{
    private const int RandomSeed = 921; // LC problem number
    private const int BracketTypeExclusiveBound = 2; private string _brackets = "";

    // picks between '(' and ')'

    [Params(100, 1_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var chars = new char[Length];

        for (var i = 0; i < Length; i++)
        {
            chars[i] = IsOpenBracket(random) ? '(' : ')';
        }

        _brackets = new string(chars);
    }

    // One draw per position, in the same order the loop above consumes them.
    private static bool IsOpenBracket(Random random) => random.Next(0, BracketTypeExclusiveBound) == 0;

    [Benchmark(Baseline = true)]
    public int RunningCounter() =>
        MinimumAddToMakeParenthesesValidSolution.MinAddToMakeValidByRunningCounter(_brackets);

    [Benchmark]
    public int StackOfOpeners() =>
        MinimumAddToMakeParenthesesValidSolution.MinAddToMakeValidByOpenerStack(_brackets);
}
