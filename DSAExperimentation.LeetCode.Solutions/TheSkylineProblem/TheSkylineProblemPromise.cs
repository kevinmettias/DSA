namespace DSAExperimentation.LeetCode.Solutions.TheSkylineProblem;

// LC 218's promise about its input: every building ends to the right of where it starts, and the
// buildings come sorted by their left edge. LeetCode's judge never poses an input that breaks it. A
// building of no width has a corner the sweep line counts and the brute force does not. Generated
// cases do not always keep the promise, so a harness that runs them asks this first.
internal static class TheSkylineProblemPromise
{
    public static bool IsKeptBy(int[][] buildings)
        => buildings.All(building => building[0] < building[1]) && buildings.Zip(buildings.Skip(1)).All(pair => pair.First[0] <= pair.Second[0]);
}
