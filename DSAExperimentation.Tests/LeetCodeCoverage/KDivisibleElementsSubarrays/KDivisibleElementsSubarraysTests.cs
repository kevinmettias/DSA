using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.KDivisibleElementsSubarrays;

// LeetCode 2261. K Divisible Elements Subarrays: n <= 200, so every subarray can be
// enumerated directly - for each start, extend the end while the running count of
// elements divisible by p stays within k, exactly the same early-break-on-violated-
// count shape ValidParenthesisStringTests/CheckIfAParenthesesStringCanBeValidTests
// use for their own single-pass scans. Distinctness is deduplicated through this
// repo's own Set<string> (HashMap-backed), keyed by the subarray's own elements
// joined into a signature - the same "dedupe via Set<string>" composition
// DistinctEchoSubstringsTests already proves out for substrings, reused here for
// subarrays.
public sealed partial class KDivisibleElementsSubarraysTests
{
    [Fact]
    public void CountDistinctSubarrays_ClassicExampleOne_ReturnsElevenDistinctSubarrays()
    {
        int[] nums = [2, 3, 3, 2, 2];

        var count = CountDistinctSubarrays(nums, k: 2, p: 2);

        Assert.Equal(11, count);
    }

    [Fact]
    public void CountDistinctSubarrays_ClassicExampleTwo_EveryNonEmptySubarrayQualifies()
    {
        int[] nums = [1, 2, 3, 4];

        var count = CountDistinctSubarrays(nums, k: 4, p: 1);

        Assert.Equal(10, count);
    }

    [Fact]
    public void CountDistinctSubarrays_ZeroAllowedDivisibleElements_OnlyNonDivisibleRunsQualify()
    {
        int[] nums = [2, 2, 2];

        var count = CountDistinctSubarrays(nums, k: 0, p: 2);

        Assert.Equal(0, count);
    }

    private static int CountDistinctSubarrays(int[] nums, int k, int p)
    {
        var distinctSubarrays = new Set<string>();

        PopulateDistinctSubarraySignatures(nums, p, k, distinctSubarrays);

        return distinctSubarrays.Count;
    }

    private static void PopulateDistinctSubarraySignatures(int[] nums, int divisorP, int maxDivisibleCount, Set<string> distinctSubarrays)
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
                distinctSubarrays.TryAdd(subarraySignature);
            }
        }
    }
}
