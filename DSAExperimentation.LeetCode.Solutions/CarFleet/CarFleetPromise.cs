namespace DSAExperimentation.LeetCode.Solutions.CarFleet;

// LC 853's promise about its input: every car starts short of the target, and no two start at the same
// place. LeetCode's judge never poses an input that breaks it. A car already at the target arrives at time
// zero, which the strategies count as a fleet of its own, and two cars at one place have no order on the
// road. Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class CarFleetPromise
{
    public static bool IsKeptBy(int target, int[] position, int[] speed)
        => position.All(start => start < target) && position.Distinct().Count() == position.Length;
}
