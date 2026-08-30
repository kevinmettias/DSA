using BenchmarkDotNet.Attributes;
using RepoCharSet = DSAExperimentation.DataStructures.Set.Set<char>;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Remove Duplicate Letters (LC 316): the well-known stack-free recursive
// alternative - at every one of its up to 26 levels, rescans the remaining
// substring to find the next safe-to-place letter and reallocates a new
// substring with that letter stripped out - vs. one O(n) left-to-right pass
// through this repo's own Stack<char> (the candidate answer) and Set<char>
// (which letters are already on it), the same approach
// RemoveDuplicateLettersTests uses.
[MemoryDiagnoser]
public class RemoveDuplicateLettersBenchmarks
{
    [Params(200, 5_000)]
    public int Length;

    private string _letters = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(316);
        _letters = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(26))).ToArray());
    }

    [Benchmark(Baseline = true)]
    public string RecursiveSplit() => RecursiveSplitSolve(_letters);

    [Benchmark]
    public string StackAndSet() => StackAndSetSolve(_letters);

    private static string RecursiveSplitSolve(string s)
    {
        if (s.Length == 0)
        {
            return string.Empty;
        }

        var distinctInS = DistinctLetters(s);

        for (var candidate = 0; candidate < 26; candidate++)
        {
            if (!distinctInS[candidate])
            {
                continue;
            }

            var c = (char)('a' + candidate);
            var suffix = s[s.IndexOf(c)..];

            if (!SameDistinctLetters(distinctInS, DistinctLetters(suffix)))
            {
                continue;
            }

            return c + RecursiveSplitSolve(suffix.Replace(c.ToString(), string.Empty));
        }

        return string.Empty;
    }

    private static bool[] DistinctLetters(string s)
    {
        var seen = new bool[26];
        foreach (var c in s)
        {
            seen[c - 'a'] = true;
        }

        return seen;
    }

    private static bool SameDistinctLetters(bool[] left, bool[] right)
    {
        for (var i = 0; i < 26; i++)
        {
            if (left[i] != right[i])
            {
                return false;
            }
        }

        return true;
    }

    private static string StackAndSetSolve(string s)
    {
        var lastOccurrence = new int[26];
        for (var i = 0; i < s.Length; i++)
        {
            lastOccurrence[s[i] - 'a'] = i;
        }

        var stack = new RepoCharStack();
        var onStack = new RepoCharSet();

        for (var i = 0; i < s.Length; i++)
        {
            var c = s[i];

            if (onStack.Has(c))
            {
                continue;
            }

            while (stack.TryPeek(out var top) && top > c && lastOccurrence[top - 'a'] > i)
            {
                stack.TryPop(out _);
                onStack.TryRemove(top);
            }

            stack.Push(c);
            onStack.TryAdd(c);
        }

        var result = new char[stack.Count];
        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return new string(result);
    }
}
