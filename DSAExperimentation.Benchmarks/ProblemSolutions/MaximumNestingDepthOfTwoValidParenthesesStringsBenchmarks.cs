using BenchmarkDotNet.Attributes;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Maximum Nesting Depth of Two Valid Parentheses Strings (LC 1111): a naive
// baseline that recomputes each position's nesting depth by rescanning every
// character before it (O(n^2) overall) vs. this repo's own Stack<char>
// tracking the running depth incrementally in a single O(n) left-to-right
// pass - the same "explicit repo Stack" move
// MaximumNestingDepthOfTwoValidParenthesesStringsTests itself makes.
[MemoryDiagnoser]
public class MaximumNestingDepthOfTwoValidParenthesesStringsBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _sequence = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1111);
        var builder = new System.Text.StringBuilder(Length);
        var openRemaining = Length / 2;
        var closeRemaining = Length / 2;

        while (openRemaining > 0 || closeRemaining > 0)
        {
            if (openRemaining > 0 && (closeRemaining == openRemaining || random.Next(2) == 0))
            {
                builder.Append('(');
                openRemaining--;
            }
            else
            {
                builder.Append(')');
                closeRemaining--;
            }
        }

        _sequence = builder.ToString();
    }

    [Benchmark(Baseline = true)]
    public int[] RecomputeDepthPerPosition()
    {
        var groups = new int[_sequence.Length];

        for (var i = 0; i < _sequence.Length; i++)
        {
            var depthBefore = 0;

            for (var j = 0; j < i; j++)
            {
                depthBefore += _sequence[j] == '(' ? 1 : -1;
            }

            groups[i] = _sequence[i] == '(' ? (depthBefore + 1) % 2 : depthBefore % 2;
        }

        return groups;
    }

    [Benchmark]
    public int[] StackTrackedSinglePass()
    {
        var groups = new int[_sequence.Length];
        var openers = new RepoCharStack();

        for (var i = 0; i < _sequence.Length; i++)
        {
            if (_sequence[i] == '(')
            {
                openers.Push(_sequence[i]);
                groups[i] = openers.Count % 2;
            }
            else
            {
                groups[i] = openers.Count % 2;
                openers.TryPop(out _);
            }
        }

        return groups;
    }
}
