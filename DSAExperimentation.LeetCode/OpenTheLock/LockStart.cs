namespace DSAExperimentation.LeetCode.OpenTheLock;

// The combination every lock starts at, and LC 752's fixed origin for both
// strategies: the graph query and the on-the-fly mutation queue both begin here,
// and a deadend sitting on it makes the lock unopenable. Separate from
// OpenTheLockSolution because the benchmark that builds a deadend set has to leave
// this combination out for its target to stay reachable - the two are one setting
// stated once.
internal static class LockStart
{
    public const string Combination = "0000";
}
