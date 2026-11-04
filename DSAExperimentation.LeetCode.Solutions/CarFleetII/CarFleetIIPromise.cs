namespace DSAExperimentation.LeetCode.Solutions.CarFleetII;

// LC 1776's promise about its input: the cars are listed by position, each strictly ahead of the one
// before it. LeetCode's judge never poses an input that breaks it, and both strategies lean on it: a
// car only ever catches the cars after it in the list. Generated cases do not always keep the
// promise, so a harness that runs them asks this first.
internal static class CarFleetIIPromise
{
    public static bool IsKeptBy(int[][] cars) => cars.Zip(cars.Skip(1)).All(pair => pair.First[0] < pair.Second[0]);
}
