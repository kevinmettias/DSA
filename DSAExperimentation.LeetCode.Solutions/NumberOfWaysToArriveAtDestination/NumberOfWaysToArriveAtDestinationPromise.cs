namespace DSAExperimentation.LeetCode.Solutions.NumberOfWaysToArriveAtDestination;

// LC 1976's promise about its input: at most one road joins any two intersections, and none joins
// one to itself. LeetCode's judge never poses an input that breaks it. A second road between two
// intersections doubles the ways one strategy counts and not the other. Generated cases do not
// always keep the promise, so a harness that runs them asks this first.
internal static class NumberOfWaysToArriveAtDestinationPromise
{
    public static bool IsKeptBy(int intersectionCount, int[][] roads)
        => roads.All(road => road[0] != road[1])
            && roads.Select(road => (Math.Min(road[0], road[1]), Math.Max(road[0], road[1]))).Distinct().Count() == roads.Length;
}
