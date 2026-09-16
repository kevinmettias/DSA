using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumProductSubarray;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumProductSubarraySolution's, the same methods
// MaximumProductSubarrayTests proves correct - the O(n^2) all-subarrays brute
// force vs. the O(n) single pass tracking both a running min and a running max
// product.
//
// LC 152's contract is not just |nums[i]| <= 10: it also guarantees that the
// product of any subarray fits in a 32-bit integer, which is what lets both arms
// report int. An unconstrained [-10, 10] draw does not respect that guarantee -
// over free runs of ~20 values the subarray products pass 2^63, so even the long
// accumulators wrap, and the two arms then legitimately disagree about a question
// the problem never settled (the same "input outside the problem's contract" shape
// an earlier AccountsMerge generator had). Closing a zero-free run before its
// running product can pass the cap keeps every subarray product inside the
// contract, so both arms compute an exact answer and agreement means agreement.
[MemoryDiagnoser]
public class MaximumProductSubarrayBenchmarks
{
    private const int RandomSeed = 152; // LC problem number
    private const int ValueMagnitude = 10;

    // Well inside int.MaxValue, so the contract holds with room to spare, and far
    // inside long, so neither accumulator can wrap while the run is open.
    private const int MaxRunProductMagnitude = 1_000_000;

    private int[] _values = [];

    [Params(200, 5_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _values = BuildValues(random, Length);
    }

    private static int[] BuildValues(Random random, int length)
    {
        var values = new int[length];
        var runProduct = 1;

        for (var i = 0; i < length; i++)
        {
            var value = random.Next(-ValueMagnitude, ValueMagnitude + 1);

            if (value != 0 && Math.Abs(runProduct * value) > MaxRunProductMagnitude)
            {
                values[i] = 0;
                runProduct = 1;

                continue;
            }

            values[i] = value;
            runProduct = value == 0 ? 1 : runProduct * value;
        }

        return values;
    }

    [Benchmark(Baseline = true)]
    public int BruteForceAllSubarrays() => MaximumProductSubarraySolution.MaxProductByBruteForce(_values);

    [Benchmark]
    public int MinMaxSinglePass() => MaximumProductSubarraySolution.MaxProductByMinMaxScan(_values);
}
