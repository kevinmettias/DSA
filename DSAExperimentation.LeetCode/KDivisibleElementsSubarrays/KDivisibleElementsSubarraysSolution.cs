using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.KDivisibleElementsSubarrays;

// LeetCode 2261. K Divisible Elements Subarrays: count the DISTINCT non-empty
// subarrays holding at most maxDivisibleCount elements divisible by divisor. n <= 200,
// so every (start, end) pair can be enumerated directly - extend each start while the
// running count of divisible elements stays within maxDivisibleCount, and break the
// moment it does not, because every longer subarray from that start violates the bound
// too.
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
    public static int CountDistinctByHashSetDedupe(int[] nums, int maxDivisibleCount, int divisor)
    {
        var distinctSubarrays = new HashSet<string>();

        RecordSubarraySignatures(nums, maxDivisibleCount, divisor, new HashSetSignatureRecorder(distinctSubarrays));

        return distinctSubarrays.Count;
    }

    // Same enumeration, deduped through this repo's own Set<string>: membership is
    // exactly "is this signature already present", which is Set's whole contract.
    public static int CountDistinctBySetDedupe(int[] nums, int maxDivisibleCount, int divisor)
    {
        var distinctSubarrays = new Set<string>();

        RecordSubarraySignatures(nums, maxDivisibleCount, divisor, new SetSignatureRecorder(distinctSubarrays));

        return distinctSubarrays.Count;
    }

    // Shared enumeration for both dedupe strategies - only how a discovered subarray
    // signature gets recorded differs, so the walk itself cannot drift between them.
    private static void RecordSubarraySignatures(
        int[] nums, int maxDivisibleCount, int divisor, ISignatureRecorder recorder)
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

                var signature = string.Join(',', nums[start..(end + 1)]);
                recorder.Record(signature);
            }
        }
    }

    // Where a discovered signature goes. The enumeration above is identical for both
    // strategies, so what each one chooses here is the only thing that differs between
    // them - and naming that choice as a type gives each store its own place to say
    // what "already present" means: Add's ignored result for the BCL HashSet, TryAdd's
    // for this repo's Set.
    private interface ISignatureRecorder
    {
        void Record(string signature);
    }

    private sealed class HashSetSignatureRecorder(HashSet<string> signatures) : ISignatureRecorder
    {
        public void Record(string signature) => signatures.Add(signature);
    }

    private sealed class SetSignatureRecorder(Set<string> signatures) : ISignatureRecorder
    {
        public void Record(string signature) => signatures.TryAdd(signature);
    }
}
