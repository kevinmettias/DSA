using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.HashMap;
using RepoIntStack = DSAExperimentation.DataStructures.Stack.Stack<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Next Greater Element I (LC 496): the O(n*m) per-query rescan of nums2 vs. a single
// O(m) monotonic-decreasing sweep through this repo's own Stack<int> that records each
// value's next-greater element into a HashMap<int,int> - nums1's answers then become
// O(1) lookups, for O(n+m) overall. nums2 is a random permutation of distinct values so
// no query short-circuits on an early match, forcing PerQueryRescan through its full
// worst-case inner scan.
[MemoryDiagnoser]
public class NextGreaterElementIBenchmarks
{
    [Params(200, 2_500)]
    public int Length;

    private int[] _nums1 = null!;
    private int[] _nums2 = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(496);
        _nums2 = Enumerable.Range(1, Length).OrderBy(_ => random.Next()).ToArray();
        _nums1 = _nums2.OrderBy(_ => random.Next()).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int[] PerQueryRescan()
    {
        var result = new int[_nums1.Length];

        for (var i = 0; i < _nums1.Length; i++)
        {
            var position = Array.IndexOf(_nums2, _nums1[i]);
            var answer = -1;

            for (var j = position + 1; j < _nums2.Length; j++)
            {
                if (_nums2[j] > _nums1[i])
                {
                    answer = _nums2[j];
                    break;
                }
            }

            result[i] = answer;
        }

        return result;
    }

    [Benchmark]
    public int[] MonotonicStackSweep()
    {
        var nextGreater = new HashMap<int, int>();
        var decreasing = new RepoIntStack();

        foreach (var value in _nums2)
        {
            while (decreasing.TryPeek(out var top) && top < value)
            {
                decreasing.TryPop(out _);
                nextGreater.Set(top, value);
            }

            decreasing.Push(value);
        }

        var result = new int[_nums1.Length];

        for (var i = 0; i < _nums1.Length; i++)
        {
            result[i] = nextGreater.TryGetValue(_nums1[i], out var greater) ? greater : -1;
        }

        return result;
    }
}
