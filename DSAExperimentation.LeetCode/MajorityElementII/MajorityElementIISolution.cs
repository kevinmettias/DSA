using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MajorityElementII;

// LeetCode 229. Majority Element II: return every value in nums that appears
// more than floor(n / 3) times.
//
// Pre-migration the test carried a private counting helper, and the benchmark's
// two [Benchmark] arms were unwired stubs that each just returned the literal 1,
// never calling into any algorithm at all - so nothing needed reconciling beyond
// naming the counting walk once.
internal static class MajorityElementIISolution
{
    // The textbook arm the HashMap count is measured against, and the one place this
    // problem's "at most two such values exist" bound is actually used: two candidates
    // are held at once, each against its own balance, and a value matching neither
    // lowers both. Only two thirds can cancel, so anything above n/3 is left standing -
    // which is why the bound is two, and why the HashMap arm can ignore it.
    //
    // O(1) space against the HashMap's O(n), at the cost of a second full pass to count
    // the two survivors, since a candidate standing at the end is necessary but not
    // sufficient. The HashMap arm answers in one pass but stores every distinct value.
    public static List<int> MajorityByExtendedBoyerMooreVoting(int[] nums)
    {
        var first = 0;
        var second = 1;
        var firstBalance = 0;
        var secondBalance = 0;

        foreach (var n in nums)
        {
            if (firstBalance > 0 && n == first)
            {
                firstBalance++;
            }
            else if (secondBalance > 0 && n == second)
            {
                secondBalance++;
            }
            else if (firstBalance == 0)
            {
                first = n;
                firstBalance = 1;
            }
            else if (secondBalance == 0)
            {
                second = n;
                secondBalance = 1;
            }
            else
            {
                firstBalance--;
                secondBalance--;
            }
        }

        var threshold = nums.Length / 3;
        var results = new List<int>();

        if (nums.Count(n => n == first) > threshold)
        {
            results.Add(first);
        }

        // Both balances can settle on the same value, and then the two counts above are
        // the same count - adding it twice would report a winner twice.
        if (second != first && nums.Count(n => n == second) > threshold)
        {
            results.Add(second);
        }

        return results;
    }

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
