using BenchmarkDotNet.Attributes;
using RepoStampStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Stamping The Sequence (LC 936): both strategies run the identical
// reverse-simulation scan (find a stampable window, turn it to '?', record its
// start index) - discovery order is always the reverse of the chronological stamp
// order, so every discovered index has to be prepended back to forward order.
// ListPrepend does that with a plain List<int>.Insert(0, i) - O(k) per insertion,
// O(m^2) over m discovered stamps. StackAndReverse instead records with this
// repo's own Stack<int> (the same repo-Stack move StampingTheSequenceTests itself
// makes) and reverses once at the end via Pop - O(m) total. A BenchmarkDotNet dry
// run confirms the real crossover: at Repeats=200 (m~200) StackAndReverse trails
// (higher per-call constant cost through DynamicArray), but at Repeats=20,000
// (m~20,000) List's O(m^2) shifting dominates and StackAndReverse wins by ~3x -
// the quadratic term overtaking the constant-factor gap, not benchmark noise.
[MemoryDiagnoser]
public class StampingTheSequenceBenchmarks
{
    private const string Stamp = "abcd";

    [Params(200, 20_000)]
    public int Repeats;

    private string _target = string.Empty;

    [GlobalSetup]
    public void Setup()
    {
        var repeatedStamp = Enumerable.Repeat(Stamp, Repeats);
        _target = string.Concat(repeatedStamp);
    }

    [Benchmark(Baseline = true)]
    public int ListPrepend()
    {
        var windowCount = _target.Length - Stamp.Length + 1;
        var chars = _target.ToCharArray();
        var done = new bool[windowCount];
        var order = new List<int>();
        var turnedCount = 0;

        for (var round = 0; round < windowCount && turnedCount < _target.Length; round++)
        {
            if (!StampPass(chars, done, order, ref turnedCount))
            {
                break;
            }
        }

        return order.Count;
    }

    private static bool StampPass(char[] chars, bool[] done, List<int> order, ref int turnedCount)
    {
        var stampedThisRound = false;

        for (var i = 0; i < done.Length; i++)
        {
            if (done[i] || !TryStampWindow(chars, i, ref turnedCount))
            {
                continue;
            }

            done[i] = true;
            stampedThisRound = true;
            order.Insert(0, i);
        }

        return stampedThisRound;
    }

    [Benchmark]
    public int StackAndReverse()
    {
        var windowCount = _target.Length - Stamp.Length + 1;
        var chars = _target.ToCharArray();
        var done = new bool[windowCount];
        var order = new RepoStampStack();
        var turnedCount = 0;

        for (var round = 0; round < windowCount && turnedCount < _target.Length; round++)
        {
            if (!StampPass(chars, done, order, ref turnedCount))
            {
                break;
            }
        }

        var result = new int[order.Count];

        for (var k = 0; k < result.Length; k++)
        {
            order.TryPop(out result[k]);
        }

        return result.Length;
    }

    private static bool StampPass(char[] chars, bool[] done, RepoStampStack order, ref int turnedCount)
    {
        var stampedThisRound = false;

        for (var i = 0; i < done.Length; i++)
        {
            if (done[i] || !TryStampWindow(chars, i, ref turnedCount))
            {
                continue;
            }

            done[i] = true;
            stampedThisRound = true;
            order.Push(i);
        }

        return stampedThisRound;
    }

    private static bool TryStampWindow(char[] chars, int start, ref int turnedCount)
    {
        if (!CanStampWindow(chars, start, out var hasLiveCharacter) || !hasLiveCharacter)
        {
            return false;
        }

        for (var k = 0; k < Stamp.Length; k++)
        {
            if (chars[start + k] != '?')
            {
                chars[start + k] = '?';
                turnedCount++;
            }
        }

        return true;
    }

    private static bool CanStampWindow(char[] chars, int start, out bool hasLiveCharacter)
    {
        hasLiveCharacter = false;

        for (var k = 0; k < Stamp.Length; k++)
        {
            var current = chars[start + k];

            if (current == '?')
            {
                continue;
            }

            if (current != Stamp[k])
            {
                return false;
            }

            hasLiveCharacter = true;
        }

        return true;
    }
}
