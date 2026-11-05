namespace DSAExperimentation.LeetCode.Solutions.FindServersThatHandledMostNumberOfRequests;

// LC 1606's promise about its input: arrival and load have one entry each per request. LeetCode's
// judge never poses an input that breaks it. Both strategies read a request's load at its arrival's
// index. Generated cases do not always keep the promise, so a harness that runs them asks this first.
internal static class FindServersThatHandledMostNumberOfRequestsPromise
{
    public static bool IsKeptBy(int serverCount, int[] arrival, int[] load) => arrival.Length == load.Length;
}
