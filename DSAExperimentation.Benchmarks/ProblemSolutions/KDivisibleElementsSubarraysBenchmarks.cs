using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// K Divisible Elements Subarrays (LC 2261): both strategies enumerate the same
// O(n^2)-bounded (k-truncated) set of candidate subarrays - the difference is only
// how distinctness is deduped. The baseline uses a plain BCL HashSet<string>: the
// primitive-composed version uses this repo's own Set<string> (HashMap-backed),
// the same dedupe-by-signature composition DistinctEchoSubstringsBenchmarks
// already proves out for substrings.
[MemoryDiagnoser]
public class KDivisibleElementsSubarraysBenchmarks
{
    private const int RandomSeed = 2261; // LC problem number
    private const int MaxValueExclusive = 200;
    private const int DivisorP = 3;
    private const int MaxDivisibleCount = 5;

    [Params(50, 200)]
    public int Length;

    private int[] _nums = null!;

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        _nums = Enumerable.Range(0, Length).Select(_ => random.Next(1, MaxValueExclusive)).ToArray();
    }

    [Benchmark(Baseline = true)]
    public int HashSetDeduped()
    {
        var distinctSubarrays = new HashSet<string>();

        PopulateDistinctSubarraySignatures(_nums, DivisorP, MaxDivisibleCount, signature => distinctSubarrays.Add(signature));

        return distinctSubarrays.Count;
    }

    [Benchmark]
    public int RepoSetDeduped()
    {
        var distinctSubarrays = new Set<string>();

        PopulateDistinctSubarraySignatures(_nums, DivisorP, MaxDivisibleCount, signature => distinctSubarrays.TryAdd(signature));

        return distinctSubarrays.Count;
    }

    // Shared enumeration for both dedupe strategies above - only how a discovered
    // subarray signature gets recorded (BCL HashSet.Add vs repo Set.TryAdd) differs.
    private static void PopulateDistinctSubarraySignatures(int[] nums, int divisorP, int maxDivisibleCount, Action<string> recordSubarraySignature)
    {
        for (var start = 0; start < nums.Length; start++)
        {
            var divisibleCount = 0;

            for (var end = start; end < nums.Length; end++)
            {
                if (nums[end] % divisorP == 0)
                {
                    divisibleCount++;
                }

                if (divisibleCount > maxDivisibleCount)
                {
                    break;
                }

                var subarraySignature = string.Join(',', nums[start..(end + 1)]);
                recordSubarraySignature(subarraySignature);
            }
        }
    }
}
