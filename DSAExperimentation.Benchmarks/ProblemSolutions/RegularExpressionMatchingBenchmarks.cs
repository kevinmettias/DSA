using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.DynamicProgramming;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Regular Expression Matching (LC 10): uncached recursive branching vs. the same
// recurrence routed through this repo's Memoizer.
[MemoryDiagnoser]
public class RegularExpressionMatchingBenchmarks
{
    private string _text = null!;
    private string _pattern = null!;

    [Params(8, 14)]
    public int Repetitions;

    [GlobalSetup]
    public void Setup()
    {
        _text = new string('a', Repetitions) + "b";
        _pattern = string.Concat(Enumerable.Repeat("a*", Repetitions)) + "b";
    }

    [Benchmark(Baseline = true)]
    public bool Recursive() => MatchRecursive(0, 0);

    [Benchmark]
    public bool Memoized()
        => Memoizer.Memoize<(int Text, int Pattern), bool>((0, 0), MatchFrom);

    private bool MatchFrom((int Text, int Pattern) state, Func<(int Text, int Pattern), bool> match)
    {
        var (textIndex, patternIndex) = state;
        if (patternIndex == _pattern.Length)
        {
            return textIndex == _text.Length;
        }

        var firstMatches = textIndex < _text.Length
            && (_pattern[patternIndex] == _text[textIndex] || _pattern[patternIndex] == '.');

        if (patternIndex + 1 < _pattern.Length && _pattern[patternIndex + 1] == '*')
        {
            return match((textIndex, patternIndex + 2))
                || (firstMatches && match((textIndex + 1, patternIndex)));
        }

        return firstMatches && match((textIndex + 1, patternIndex + 1));
    }

    private bool MatchRecursive(int textIndex, int patternIndex)
    {
        if (patternIndex == _pattern.Length)
        {
            return textIndex == _text.Length;
        }

        var firstMatches = textIndex < _text.Length
            && (_pattern[patternIndex] == _text[textIndex] || _pattern[patternIndex] == '.');

        if (patternIndex + 1 < _pattern.Length && _pattern[patternIndex + 1] == '*')
        {
            return MatchRecursive(textIndex, patternIndex + 2)
                || (firstMatches && MatchRecursive(textIndex + 1, patternIndex));
        }

        return firstMatches && MatchRecursive(textIndex + 1, patternIndex + 1);
    }
}
