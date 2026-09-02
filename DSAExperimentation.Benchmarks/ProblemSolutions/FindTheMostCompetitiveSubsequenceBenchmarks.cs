using BenchmarkDotNet.Attributes;
using CompetitiveStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find the Most Competitive Subsequence (LC 1673): the naive round-by-round baseline -
// (n-k) separate O(n) scans, each removing the first descending element - against this
// repo's own Stack<int> monotonic sweep, which removes every poppable element in one
// O(n) pass. Both apply the exact same greedy rule (RemoveKDigitsBenchmarks'
// precedent), one element removal at a time vs. all of them within a single pass.
[MemoryDiagnoser]
public class FindTheMostCompetitiveSubsequenceBenchmarks
{
    private const int SubsequenceLengthDivisor = 3;

    [Params(500, 5_000)]
    public int Length;

    private int[] _nums = null!;
    private int _k;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(1);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(0, Length)).ToArray();
        _k = Length / SubsequenceLengthDivisor;
    }

    [Benchmark(Baseline = true)]
    public int RepeatedFirstDescentRemoval()
    {
        var current = _nums.ToList();

        while (current.Count > _k)
        {
            var removeIndex = current.Count - 1;

            for (var i = 0; i < current.Count - 1; i++)
            {
                if (current[i] > current[i + 1])
                {
                    removeIndex = i;
                    break;
                }
            }

            current.RemoveAt(removeIndex);
        }

        return current.Count;
    }

    [Benchmark]
    public int MonotonicStackSweep()
    {
        var stack = new CompetitiveStack();

        for (var i = 0; i < _nums.Length; i++)
        {
            while (stack.Count > 0 && stack.TryPeek(out var top) && top > _nums[i]
                   && stack.Count - 1 + (_nums.Length - i) >= _k)
            {
                stack.TryPop(out _);
            }

            if (stack.Count < _k)
            {
                stack.Push(_nums[i]);
            }
        }

        return stack.Count;
    }
}
