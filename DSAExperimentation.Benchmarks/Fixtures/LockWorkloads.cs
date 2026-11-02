using DSAExperimentation.Domain.Devices.Locks;
using DSAExperimentation.LeetCode.OpenTheLock;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 752 - the lock itself is Domain/Devices/Locks and its start
// combination sits beside OpenTheLockSolution; what stays here is only the choice of how much
// of it to prune, which is a measurement decision rather than a domain one.
internal static class LockWorkloads
{
    // A random scattering of deadends that almost never disconnects the graph -
    // each node still has 8 candidate edges, so removing a few hundred of 10,000
    // nodes at random leaves it connected with overwhelming probability, keeping
    // every run's BFS genuinely reachable work.
    public static string[] BuildDeadends(int count, int seed)
    {
        var random = new Random(seed);
        var deadends = new HashSet<string>();

        while (deadends.Count < count)
        {
            var candidate = CombinationLock<FourDigitLock>.Combination(random.Next(CombinationLock<FourDigitLock>.CombinationCount));

            if (candidate != LockStart.Combination
                && candidate != LockScenario.FarthestTarget)
            {
                deadends.Add(candidate);
            }
        }

        return [.. deadends];
    }
}
