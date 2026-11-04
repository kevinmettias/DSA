namespace DSAExperimentation.LeetCode.Solutions.LoudAndRich;

// LC 851's promise about its input: the observations of who is richer are consistent, so they form
// no cycle. LeetCode's judge never poses an input that breaks it. Around a cycle the topological
// pass never reaches some people, where the per-person walk does. Generated cases do not always
// keep the promise, so a harness that runs them asks this first.
internal static class LoudAndRichPromise
{
    public static bool IsKeptBy(int[][] richer, int[] quiet)
    {
        var poorer = Enumerable.Range(0, quiet.Length).Select(_ => new List<int>()).ToArray();
        var richerCount = new int[quiet.Length];

        foreach (var pair in richer)
        {
            poorer[pair[0]].Add(pair[1]);
            richerCount[pair[1]]++;
        }

        return CountOrdered(poorer, richerCount) == quiet.Length;
    }

    // Kahn's ordering: everyone it reaches has no richer person left on a cycle.
    private static int CountOrdered(List<int>[] poorer, int[] richerCount)
    {
        var ready = new Queue<int>(Enumerable.Range(0, richerCount.Length).Where(person => richerCount[person] == 0));
        var ordered = 0;

        while (ready.Count > 0)
        {
            ordered++;

            foreach (var next in poorer[ready.Dequeue()].Where(next => --richerCount[next] == 0))
            {
                ready.Enqueue(next);
            }
        }

        return ordered;
    }
}
