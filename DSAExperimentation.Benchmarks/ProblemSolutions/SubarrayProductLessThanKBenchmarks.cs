using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Subarray Product Less Than K (LC 713): BruteForce is the textbook O(n^2) - for
// every start index, extend the running product right until it stops being < k.
// LogPrefixLowerBound instead sums Math.Log(nums[i]) into a prefix array (all nums
// are positive, so this running sum is monotonically non-decreasing - the raw
// running PRODUCT would overflow almost immediately at these lengths) and finds
// each start's valid-window boundary via this repo's own BinarySearch.LowerBound
// over an ArraySequence<double> witness, O(n log n) - the same
// search-on-a-derived-monotonic-sequence idiom MinimumSizeSubarraySumBenchmarks
// (LC 209) already uses for its own prefix-sum array. nums are deliberately all 1s
// (product never reaches K) so BruteForce's inner loop never breaks early and is
// forced through its real O(n^2) worst case - the same "force the real worst case"
// convention TwoSumBenchmarks (unreachable target) and
// LongestSubstringWithoutRepeatingCharactersBenchmarks (all-distinct characters)
// already establish.
[MemoryDiagnoser]
public class SubarrayProductLessThanKBenchmarks
{
    private const int K = 2;

    [Params(200, 5_000)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup() => _nums = Enumerable.Repeat(1, Length).ToArray();

    [Benchmark(Baseline = true)]
    public int BruteForce()
    {
        var count = 0;

        for (var start = 0; start < _nums.Length; start++)
        {
            long product = 1;
            for (var end = start; end < _nums.Length; end++)
            {
                product *= _nums[end];
                if (product >= K)
                {
                    break;
                }

                count++;
            }
        }

        return count;
    }

    [Benchmark]
    public int LogPrefixLowerBound()
    {
        var logPrefix = new double[_nums.Length + 1];
        for (var i = 0; i < _nums.Length; i++)
        {
            logPrefix[i + 1] = logPrefix[i] + Math.Log(_nums[i]);
        }

        var sequence = new ArraySequence<double>(logPrefix);
        var count = 0;
        var logK = Math.Log(K);

        for (var i = 0; i < _nums.Length; i++)
        {
            var target = logPrefix[i] + logK;
            var end = BinarySearch.LowerBound<double, ArraySequence<double>>(sequence, target);
            count += Math.Max(0, end - i - 1);
        }

        return count;
    }
}
