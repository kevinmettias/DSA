using BenchmarkDotNet.Attributes;
using NextGreaterStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Next Greater Element IV (LC 2454): the O(n^2) brute-force scan (walking forward
// from each index, counting values greater than it, until the second one turns up)
// vs. the O(n) two-monotonic-stack sweep over this repo's own Stack<int> - the same
// NextGreaterElementII precedent, generalized from "first greater" to "second
// greater" by promoting waitingForFirst's resolved indices into a second waiting
// stack instead of resolving them outright.
[MemoryDiagnoser]
public class NextGreaterElementIVBenchmarks
{
    private const int RandomSeed = 2454; // LeetCode problem number

    private const int MaxElementValue = 1_000;

    private const int RequiredGreaterCount = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _values = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxElementValue)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] BruteForce()
    {
        var n = _values.Length;
        var result = new int[n];
        Array.Fill(result, -1);

        for (var i = 0; i < n; i++)
        {
            var greaterCount = 0;

            for (var j = i + 1; j < n; j++)
            {
                if (_values[j] <= _values[i])
                {
                    continue;
                }

                greaterCount++;

                if (greaterCount == RequiredGreaterCount)
                {
                    result[i] = _values[j];
                    break;
                }
            }
        }

        return result;
    }

    [Benchmark]
    public int[] TwoMonotonicStacks()
    {
        var n = _values.Length;
        var result = new int[n];
        Array.Fill(result, -1);
        var waitingForFirst = new NextGreaterStack();
        var waitingForSecond = new NextGreaterStack();
        var promoted = new NextGreaterStack();

        for (var i = 0; i < n; i++)
        {
            while (waitingForSecond.TryPeek(out var second) && _values[second] < _values[i])
            {
                waitingForSecond.TryPop(out _);
                result[second] = _values[i];
            }

            while (waitingForFirst.TryPeek(out var first) && _values[first] < _values[i])
            {
                waitingForFirst.TryPop(out _);
                promoted.Push(first);
            }

            while (promoted.TryPop(out var index))
            {
                waitingForSecond.Push(index);
            }

            waitingForFirst.Push(i);
        }

        return result;
    }
}
