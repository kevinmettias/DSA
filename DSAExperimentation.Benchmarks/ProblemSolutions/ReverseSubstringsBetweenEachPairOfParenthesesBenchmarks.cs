using BenchmarkDotNet.Attributes;
using RepoCharListStack = DSAExperimentation.DataStructures.Stack.Stack<System.Collections.Generic.List<char>>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Reverse Substrings Between Each Pair of Parentheses (LC 1190): repeatedly
// splicing the innermost pair with IndexOf/LastIndexOf/Substring (rebuilding the
// whole string once per pair - quadratic overall) vs. a single pass through this
// repo's own Stack<List<char>>, where each character is only ever re-reversed once
// per enclosing paren level.
[MemoryDiagnoser]
public class ReverseSubstringsBetweenEachPairOfParenthesesBenchmarks
{
    private const string GroupBody = "abcdef";

    [Params(500, 5_000)]
    public int GroupCount;

    private string _input = null!;

    [GlobalSetup]
    public void Setup()
    {
        var builder = new System.Text.StringBuilder();
        for (var i = 0; i < GroupCount; i++)
        {
            builder.Append('(').Append(GroupBody).Append(')');
        }

        _input = builder.ToString();
    }

    [Benchmark(Baseline = true)]
    public string NaiveRepeatedSplice()
    {
        var current = _input;

        while (true)
        {
            var closeIndex = current.IndexOf(')');
            if (closeIndex < 0)
            {
                break;
            }

            var openIndex = current.LastIndexOf('(', closeIndex);
            var inner = current.Substring(openIndex + 1, closeIndex - openIndex - 1);
            var reversedInner = new string(inner.Reverse().ToArray());
            var beforeGroup = current.AsSpan(0, openIndex);
            var afterGroup = current.AsSpan(closeIndex + 1);
            current = string.Concat(beforeGroup, reversedInner, afterGroup);
        }

        return current;
    }

    [Benchmark]
    public string StackOfCharBuffers()
    {
        var groups = new RepoCharListStack();
        var current = new List<char>();

        foreach (var ch in _input)
        {
            current = ProcessChar(ch, groups, current);
        }

        return new string(current.ToArray());
    }

    private static List<char> ProcessChar(char ch, RepoCharListStack groups, List<char> current)
    {
        switch (ch)
        {
            case '(':
                groups.Push(current);
                return [];
            case ')':
                current.Reverse();
                if (groups.TryPop(out var enclosing))
                {
                    enclosing.AddRange(current);
                    return enclosing;
                }

                return current;
            default:
                current.Add(ch);
                return current;
        }
    }
}
