namespace DSAExperimentation.LeetCode.TrafficSignalColor;

// LeetCode 3894. Traffic Signal Color: map the remaining timer value to the
// signal's current state under LC's own fixed thresholds.
//
// There is only one strategy here: this is a direct classification over three
// disjoint, LC-defined ranges, not a search or an optimization - nothing in this
// repo's DataStructures/Algorithms tiers applies to a single three-way range
// check, the same shape TrafficSignalColorSolution shares with
// BalancedBinaryTreeSolution's own "there is only one strategy here" precedent.
internal static class TrafficSignalColorSolution
{
    private const int GreenTimer = 0;
    private const int OrangeTimer = 30;
    private const int MaxRedTimer = 90;

    public static string ColorByRangeCheck(int timer)
    {
        if (timer == GreenTimer)
        {
            return "Green";
        }

        if (timer == OrangeTimer)
        {
            return "Orange";
        }

        if (timer > OrangeTimer && timer <= MaxRedTimer)
        {
            return "Red";
        }

        return "Invalid";
    }
}
