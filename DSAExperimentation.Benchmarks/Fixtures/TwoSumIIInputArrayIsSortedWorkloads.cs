namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 167 - a non-decreasing array of length values inside its
// [-1000, 1000] whose only pair summing to Target is the last two. Every earlier value climbs
// evenly from -1,000 to at most 499 (repeating once length passes 1,500, as LC 167's 3 * 10^4
// values in 2,001 must), and the last two are 500 and 500: an earlier value's complement is at
// least 501, past the array's largest value, so exactly one pair meets Target = 1,000, as LC 167
// promises, and it is the last one either strategy reaches.
internal static class TwoSumIIInputArrayIsSortedWorkloads
{
    public const int Target = PlantedValue + PlantedValue;

    private const int ClimbStart = -1_000;
    private const int PlantedValue = 500;

    // How many distinct values the climb below the planted pair can take: -1,000 to 499.
    private const int ClimbSpan = PlantedValue - ClimbStart;

    private const int PlantedCount = 2;

    public static int[] BuildNums(int length)
    {
        var climbLength = length - PlantedCount;
        var nums = new int[length];

        for (var i = 0; i < climbLength; i++)
        {
            nums[i] = ClimbStart + (int)((long)i * ClimbSpan / climbLength);
        }

        nums[^2] = PlantedValue;
        nums[^1] = PlantedValue;

        return nums;
    }
}
