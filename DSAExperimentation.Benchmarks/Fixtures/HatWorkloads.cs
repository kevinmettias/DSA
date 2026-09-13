namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 1434. Everything about the hat DP itself lives in
// LeetCode/NumberOfWaysToWearDifferentHatsToEachOther; what stays here is only how
// many people to seat, how deep a hat pool to draw from and which seed to draw with
// - measurement decisions rather than domain ones.
internal static class HatWorkloads
{
    // Every person likes a few random hats out of one small shared pool, so the
    // branching factor - and therefore the number of (hat, mask) states reachable
    // from more than one path - actually grows with the person count, forcing a real
    // gap between the memoized and unmemoized arms.
    public static int[][] BuildLikedHats(int peopleCount, int hatPoolSize, int likedHatsPerPerson, int seed)
    {
        var random = new Random(seed);
        var likedHats = new int[peopleCount][];

        for (var person = 0; person < peopleCount; person++)
        {
            likedHats[person] =
                [.. Enumerable.Range(1, hatPoolSize).OrderBy(_ => random.Next()).Take(likedHatsPerPerson)];
        }

        return likedHats;
    }
}
