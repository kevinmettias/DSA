using BenchmarkDotNet.Attributes;
using RepoCharStack = DSAExperimentation.DataStructures.Stack.Stack<char>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Number of Swaps to Make the String Balanced (LC 1963): an O(n^2) baseline
// that matches every ']' against the *nearest* still-unmatched '[' to its left via a
// backward linear scan (the LIFO nearest-unmatched-opener is exactly what a stack's
// O(1) pop already gives for free) vs. this repo's own Stack<char> resolving the same
// nearest-unmatched lookup in O(1) per character. Both compute the identical unmatched-
// closer count - matching greedily nearest-first never changes how many closers end up
// unmatched, only how expensively "nearest" gets found - so they always agree on the
// answer. _text is "]" repeated Length/2 times followed by "[" repeated Length/2 times,
// forcing every leading ']' through a full backward scan of the remaining closer run
// before BackwardScan finds its first available opener, instead of an early exit on the
// first invocation making it look artificially competitive.
[MemoryDiagnoser]
public class MinimumNumberOfSwapsToMakeTheStringBalancedBenchmarks
{
    private const int TextHalfDivisor = 2; // _text is split into two equal-length halves
    private const int ClosersFixedPerSwap = 2; // each swap resolves two unmatched closing brackets

    [Params(200, 4_000)]
    public int Length;

    private string _text = null!;

    [GlobalSetup]
    public void Setup()
    {
        var half = Length / TextHalfDivisor;
        _text = new string(']', half) + new string('[', half);
    }

    [Benchmark(Baseline = true)]
    public int BackwardScan()
    {
        var matched = new bool[_text.Length];
        var unmatchedCloseCount = 0;

        for (var i = 0; i < _text.Length; i++)
        {
            if (_text[i] != ']')
            {
                continue;
            }

            if (FindNearestUnmatchedOpener(matched, i) is var opener && opener >= 0)
            {
                matched[opener] = true;
            }
            else
            {
                unmatchedCloseCount++;
            }
        }

        return (unmatchedCloseCount + 1) / ClosersFixedPerSwap;
    }

    private int FindNearestUnmatchedOpener(bool[] matched, int beforeIndex)
    {
        for (var j = beforeIndex - 1; j >= 0; j--)
        {
            if (_text[j] == '[' && !matched[j])
            {
                return j;
            }
        }

        return -1;
    }

    [Benchmark]
    public int StackScan()
    {
        var open = new RepoCharStack();
        var swaps = 0;

        foreach (var ch in _text)
        {
            if (ch == '[')
            {
                open.Push(ch);
                continue;
            }

            if (open.TryPop(out _))
            {
                continue;
            }

            swaps++;
            open.Push('[');
        }

        return swaps;
    }
}
