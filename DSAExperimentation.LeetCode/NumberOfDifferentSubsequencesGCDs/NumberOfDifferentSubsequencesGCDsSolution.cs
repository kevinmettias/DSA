using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.NumberOfDifferentSubsequencesGCDs;

// LeetCode 1819. Number of Different Subsequences GCDs: count the distinct values
// that are the gcd of at least one non-empty subsequence of nums.
//
// Both strategies decide the same predicate for every candidate g in 1..max(nums):
// fold gcd over the elements of nums that g divides, and g is achievable exactly
// when that fold lands back on g. Taking *every* multiple of g is safe because
// adding another multiple can only shrink the running gcd - so if the whole set
// folds to g, that set is a witnessing subsequence, and if it does not, no subset
// of it can do better.
//
// They differ only in how g's multiples are found: rescanning the whole array
// (O(max * n)) against stepping candidate, 2*candidate, ... through a membership
// set (O(max log max) by the harmonic series). The private Euclidean gcd helper is
// the same one CheckIfItIsAGoodArraySolution and XOfAKindInADeckOfCardsSolution
// keep inline rather than promoting to a shared production type.
internal static class NumberOfDifferentSubsequencesGCDsSolution
{
    // The textbook answer: for each candidate, walk the whole array and fold the
    // elements it divides. Deliberately written without this repo's primitives -
    // it is the arm the multiple-walk below has to justify itself against, and it
    // scales with the array length as well as the value domain.
    public static int CountDifferentSubsequenceGcdsByWholeArrayScan(int[] nums)
    {
        var maxValue = nums.Max();
        var count = 0;

        for (var candidate = 1; candidate <= maxValue; candidate++)
        {
            if (HasSubsequenceWithGcd(nums, candidate))
            {
                count++;
            }
        }

        return count;
    }

    private static bool HasSubsequenceWithGcd(int[] nums, int candidate)
    {
        var runningGcd = 0;

        foreach (var num in nums)
        {
            if (num % candidate != 0)
            {
                continue;
            }

            runningGcd = Gcd(runningGcd, num);

            if (runningGcd == candidate)
            {
                return true;
            }
        }

        return false;
    }

    // Mark every value's presence in this repo's own Set<int> (O(1) Has, the same
    // membership role XOfAKindInADeckOfCardsSolution's HashMap plays for counts),
    // then visit only candidate's own multiples instead of the whole array. The
    // number of multiples of g up to max is max/g, so the total work is
    // max * (1 + 1/2 + 1/3 + ...) - independent of how long nums is.
    public static int CountDifferentSubsequenceGcdsBySetMultiples(int[] nums)
    {
        var present = new Set<int>(nums);
        var maxValue = nums.Max();
        var count = 0;

        for (var candidate = 1; candidate <= maxValue; candidate++)
        {
            if (HasMultipleWithGcd(present, candidate, maxValue))
            {
                count++;
            }
        }

        return count;
    }

    private static bool HasMultipleWithGcd(Set<int> present, int candidate, int maxValue)
    {
        var runningGcd = 0;

        for (var multiple = candidate; multiple <= maxValue; multiple += candidate)
        {
            if (!present.Has(multiple))
            {
                continue;
            }

            runningGcd = Gcd(runningGcd, multiple);

            if (runningGcd == candidate)
            {
                return true;
            }
        }

        return false;
    }

    private static int Gcd(int firstValue, int secondValue) =>
        secondValue == 0 ? firstValue : Gcd(secondValue, firstValue % secondValue);
}
