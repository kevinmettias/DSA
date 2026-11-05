namespace DSAExperimentation.LeetCode.Solutions.ThreeSumClosest;

// LC 16's promise about its input: each input has exactly one solution, so no two sums of three lie
// equally close to the target on either side of it. LeetCode's judge never poses an input that breaks
// it. Of two sums that tie, the brute force keeps the first it meets and the two-pointer sweep the
// first it moves to. Generated cases do not always keep the promise, so a harness that runs them asks
// this first.
internal static class ThreeSumClosestPromise
{
    public static bool IsKeptBy(int[] nums, int target)
    {
        var gaps = SweptSums([.. nums.Order()], target).Select(sum => sum - target).ToList();
        var nearestBelow = gaps.Where(gap => gap < 0).Select(gap => -gap).DefaultIfEmpty(int.MaxValue).Min();
        var nearestAbove = gaps.Where(gap => gap > 0).DefaultIfEmpty(int.MaxValue).Min();
        return gaps.Contains(0) || nearestBelow != nearestAbove;
    }

    // Every sum the two-pointer sweep visits. For each first number the pointers pass through the
    // largest sum under the target and the smallest over it, so the closest sum on each side is here.
    private static IEnumerable<int> SweptSums(int[] sorted, int target)
    {
        for (var first = 0; first < sorted.Length - 2; first++)
        {
            var (low, high) = (first + 1, sorted.Length - 1);

            while (low < high)
            {
                var sum = sorted[first] + sorted[low] + sorted[high];
                yield return sum;

                if (sum < target)
                {
                    low++;
                }
                else
                {
                    high--;
                }
            }
        }
    }
}
