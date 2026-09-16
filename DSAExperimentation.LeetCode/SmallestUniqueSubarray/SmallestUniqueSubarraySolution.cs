using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.LeetCode.SmallestUniqueSubarray;

// LeetCode 3934. Smallest Unique Subarray: find the minimum length L such that
// some length-L subarray occurs exactly once among all length-L subarrays.
//
// P(L) = "some length-L subarray is unique" is monotonic once true: if the
// subarray at position p is unique, extend it by one element on whichever side
// stays in bounds (right, unless p is already flush against the end, in which
// case left). If that length-(L+1) extension occurred twice, both occurrences
// would share the same length-L prefix (or suffix), which - since one of those
// occurrences IS the extension rooted at p - forces the OTHER occurrence's
// length-L slice to equal the subarray at p, contradicting its uniqueness.
// So "the smallest feasible L" is a genuine binary-search boundary (L = n's
// single whole-array subarray is always feasible, giving a valid upper bound).
internal static class SmallestUniqueSubarraySolution
{
    // Textbook: for each length from 1 up, bucket every window by an exact
    // delimited-string key in a BCL Dictionary (no collision risk at all) and
    // stop at the first length with a count-1 bucket. The arm the repo's own
    // RollingHash strategy below has to beat.
    public static int SmallestUniqueLengthByBruteForce(int[] nums)
    {
        var n = nums.Length;

        for (var length = 1; length <= n; length++)
        {
            if (HasUniqueWindowByBruteForce(nums, length))
            {
                return length;
            }
        }

        return n;
    }

    private static bool HasUniqueWindowByBruteForce(int[] nums, int length)
    {
        var counts = new Dictionary<string, int>();

        for (var start = 0; start + length <= nums.Length; start++)
        {
            // string.Join has no (char, int[], int, int) slice overload - only
            // (char, string?[], int, int) - so passing nums/start/length directly
            // would silently bind to the params object?[] overload instead
            // (joining nums.ToString() with the start/length integers, not the
            // window's actual values). Skip/Take slices first so the IEnumerable<T>
            // overload joins the real values.
            var key = string.Join(',', nums.Skip(start).Take(length));
            counts[key] = counts.GetValueOrDefault(key) + 1;
        }

        return counts.Values.Any(count => count == 1);
    }

    // Composed: coordinate-compress nums into a shared char alphabet via this
    // repo's own HashMap<int,char> - the same compression the LongestCommonSubpath
    // coverage already uses, since RollingHash itself only operates over
    // ReadOnlySpan<char> - then binary-search the smallest feasible length,
    // screening each candidate with one RollingHash sweep whose window hashes are
    // grouped in this repo's own HashMap<RollingHashValue,int>. A bucket of size 1
    // is certain, never a false positive: two genuinely equal windows are the same
    // bytes and so always hash identically, so a truly duplicated window can never
    // land alone in its bucket. Only a false NEGATIVE is possible (a unique window
    // colliding into someone else's bucket), at RollingHash's own already-documented
    // double-hashing collision probability.
    public static int SmallestUniqueLengthByRollingHash(int[] nums)
    {
        var n = nums.Length;
        var hash = new RollingHash(Encode(nums));

        var low = 1;
        var high = n;

        while (low < high)
        {
            var mid = low + (high - low) / 2;

            if (HasUniqueWindowByRollingHash(hash, n, mid))
            {
                high = mid;
            }
            else
            {
                low = mid + 1;
            }
        }

        return low;
    }

    private static char[] Encode(int[] nums)
    {
        var codes = new HashMap<int, char>();

        foreach (var value in nums)
        {
            if (!codes.HasKey(value))
            {
                codes.Set(value, (char)codes.Count);
            }
        }

        var encoded = new char[nums.Length];

        for (var i = 0; i < nums.Length; i++)
        {
            codes.TryGetValue(nums[i], out encoded[i]);
        }

        return encoded;
    }

    private static bool HasUniqueWindowByRollingHash(RollingHash hash, int arrayLength, int length)
    {
        var counts = new HashMap<RollingHashValue, int>();

        for (var start = 0; start + length <= arrayLength; start++)
        {
            var value = hash.Hash(start, length);
            counts.TryGetValue(value, out var count);
            counts.Set(value, count + 1);
        }

        return counts.Values.Any(count => count == 1);
    }
}
