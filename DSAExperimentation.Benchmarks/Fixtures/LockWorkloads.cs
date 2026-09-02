using DSAExperimentation.LeetCode.OpenTheLock;
using DSAExperimentation.Domain.Locks;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 752 - everything about the lock graph itself
// now lives in Domain.Locks; what stays here is only the choice of how much of it
// to prune, which is a measurement decision rather than a domain one.
internal static class LockWorkloads
{
    // Farthest possible combination from "0000": each wheel needs 5 turns (the max
    // over +1/-1 mod 10 arithmetic), forcing the full 20-turn BFS radius.
    public const string FarthestTarget = "5555";

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
            var candidate = LockWheels.Combination(random.Next(LockWheels.CombinationSpace));

            if (candidate != OpenTheLockSolution.Start && candidate != FarthestTarget)
            {
                deadends.Add(candidate);
            }
        }

        return [.. deadends];
    }
}
