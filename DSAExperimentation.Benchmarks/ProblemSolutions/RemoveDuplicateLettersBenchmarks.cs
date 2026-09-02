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
    private const int RandomSeed = 316; // LC problem number
    private const int AlphabetSize = 26;

    [Params(200, 5_000)]
    public int Length;

    private string _letters = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _letters = new string(Enumerable.Range(0, Length).Select(_ => (char)('a' + random.Next(AlphabetSize))).ToArray());
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

        for (var candidate = 0; candidate < AlphabetSize; candidate++)
        {
            var placed = TryPlaceCandidate(s, distinctInS, candidate);

            if (placed is not null)
            {
                return placed;
            }
        }

        return string.Empty;
    }

    private static string? TryPlaceCandidate(string s, bool[] distinctInS, int candidate)
    {
        if (!distinctInS[candidate])
        {
            return null;
        }

        var c = (char)('a' + candidate);
        var suffix = s[s.IndexOf(c)..];

        if (!SameDistinctLetters(distinctInS, DistinctLetters(suffix)))
        {
            return null;
        }

        var remainder = suffix.Replace(c.ToString(), string.Empty);

        return c + RecursiveSplitSolve(remainder);
    }

    private static bool[] DistinctLetters(string s)
    {
        var seen = new bool[AlphabetSize];
        foreach (var c in s)
        {
            seen[c - 'a'] = true;
        }

        return seen;
    }

    private static bool SameDistinctLetters(bool[] left, bool[] right)
    {
        for (var i = 0; i < AlphabetSize; i++)
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
        var lastOccurrence = ComputeLastOccurrence(s);
        var stack = new RepoCharStack();
        var onStack = new RepoCharSet();
        var state = new DuplicateLetterState(lastOccurrence, stack, onStack);

        for (var i = 0; i < s.Length; i++)
        {
            ProcessLetter(s[i], i, state);
        }

        var result = new char[stack.Count];
        for (var i = result.Length - 1; i >= 0; i--)
        {
            stack.TryPop(out result[i]);
        }

        return new string(result);
    }

    private static int[] ComputeLastOccurrence(string s)
    {
        var lastOccurrence = new int[AlphabetSize];

        for (var i = 0; i < s.Length; i++)
        {
            lastOccurrence[s[i] - 'a'] = i;
        }

        return lastOccurrence;
    }

    private static void ProcessLetter(char c, int index, DuplicateLetterState state)
    {
        if (state.OnStack.Has(c))
        {
            return;
        }

        while (state.Stack.TryPeek(out var top) && top > c && state.LastOccurrence[top - 'a'] > index)
        {
            state.Stack.TryPop(out _);
            state.OnStack.TryRemove(top);
        }

        state.Stack.Push(c);
        state.OnStack.TryAdd(c);
    }

    private readonly record struct DuplicateLetterState(int[] LastOccurrence, RepoCharStack Stack, RepoCharSet OnStack);
}
