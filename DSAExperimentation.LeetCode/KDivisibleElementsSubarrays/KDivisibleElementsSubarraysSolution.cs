using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.KDivisibleElementsSubarrays;

// LeetCode 2261. K Divisible Elements Subarrays: count the DISTINCT non-empty
// subarrays holding at most k elements divisible by p. n <= 200, so every
// (start, end) pair can be enumerated directly - extend each start while the running
// count of divisible elements stays within k, and break the moment it does not,
// because every longer subarray from that start violates the bound too.
//
// Enumeration is therefore identical in both strategies; the only thing they differ
// in is how distinctness is decided. Each candidate is keyed by its own elements
// joined into a signature, then deduped either through a BCL HashSet<string> or
// through this repo's HashMap-backed Set<string> - the same "dedupe by signature"
// composition DistinctEchoSubstrings uses for substrings, here over subarrays.
internal static class KDivisibleElementsSubarraysSolution
{
    // The textbook answer: same candidate enumeration, signatures deduped in a BCL
    // HashSet. Deliberately written with nothing from this repo - it is the arm the
    // Set-backed strategy below has to justify itself against.
    public static int CountDistinctByHashSetDedupe(int[] nums, int k, int p)
    {
        var distinctSubarrays = new HashSet<string>();

        RecordSubarraySignatures(nums, k, p, signature => distinctSubarrays.Add(signature));

        return distinctSubarrays.Count;
    }

    // Same enumeration, deduped through this repo's own Set<string>: membership is
    // exactly "is this signature already present", which is Set's whole contract.
    public static int CountDistinctBySetDedupe(int[] nums, int k, int p)
    {
        var distinctSubarrays = new Set<string>();

        RecordSubarraySignatures(nums, k, p, signature => distinctSubarrays.TryAdd(signature));

        return distinctSubarrays.Count;
    }

    // Shared enumeration for both dedupe strategies - only how a discovered subarray
    // signature gets recorded differs, so the walk itself cannot drift between them.
    private static void RecordSubarraySignatures(
        int[] nums, int maxDivisibleCount, int divisor, Action<string> recordSignature)
    {
        for (var start = 0; start < nums.Length; start++)
        {
            var divisibleCount = 0;

            for (var end = start; end < nums.Length; end++)
            {
                if (nums[end] % divisor == 0)
                {
                    divisibleCount++;
                }

                if (divisibleCount > maxDivisibleCount)
                {
                    break;
                }

                recordSignature(string.Join(',', nums[start..(end + 1)]));
            }
        }
    }
}
