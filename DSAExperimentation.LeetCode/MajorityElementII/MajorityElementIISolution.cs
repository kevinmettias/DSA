using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MajorityElementII;

// LeetCode 229. Majority Element II: return every value in nums that appears
// more than floor(n / 3) times. LC guarantees at most two such values exist,
// but this strategy doesn't lean on that bound - it just counts and filters.
//
// There is exactly one strategy: pre-migration the test carried a private
// counting helper, and the benchmark's two [Benchmark] arms were unwired stubs
// that each just returned the literal 1, never calling into any algorithm at
// all - so nothing needed reconciling beyond naming this walk once.
internal static class MajorityElementIISolution
{
    public static List<int> MajorityByHashMap(int[] nums)
    {
        var counts = new HashMap<int, int>();

        foreach (var n in nums)
        {
            counts.TryGetValue(n, out var count);
            counts.Set(n, count + 1);
        }

        var threshold = nums.Length / 3;

        return counts.Keys
            .Where(k =>
            {
                counts.TryGetValue(k, out var count);
                return count > threshold;
            })
            .ToList();
    }
}
