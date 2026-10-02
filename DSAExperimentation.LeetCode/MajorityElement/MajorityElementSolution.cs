using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MajorityElement;

// LeetCode 169. Majority Element: the array is guaranteed to have an element
// appearing more than n/2 times, so a running count can return as soon as one
// candidate crosses that threshold instead of scanning to the end.
internal static class MajorityElementSolution
{
    // The textbook arm the HashMap count is measured against: one candidate held
    // against a running balance, raised when a value matches it and lowered when one
    // does not, and replaced outright once the balance falls to zero. Every pair of
    // differing values cancels, and a strict majority has nothing left to cancel it,
    // so the surviving candidate is the answer.
    //
    // It is O(1) space against the HashMap's O(n), and it never allocates - but it has
    // to read the whole array, where the HashMap arm can return the moment a key
    // crosses n/2. Which of those dominates is the question the pair exists to ask.
    public static int MajorityByBoyerMooreVoting(int[] nums)
    {
        var candidate = nums[0];
        var balance = 0;

        foreach (var n in nums)
        {
            if (balance == 0)
            {
                candidate = n;
                balance = 1;
            }
            else if (n == candidate)
            {
                balance++;
            }
            else
            {
                balance--;
            }
        }

        return candidate;
    }

    public static int MajorityByHashMap(int[] nums)
    {
        var counts = new HashMap<int, int>();

        foreach (var n in nums)
        {
            counts.TryGetValue(n, out var count);
            count++;

            if (count > nums.Length / 2)
            {
                return n;
            }

            counts.Set(n, count);
        }

        return nums[0];
    }
}
