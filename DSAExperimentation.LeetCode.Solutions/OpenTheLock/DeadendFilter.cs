using DSAExperimentation.DataStructures.Graph.States;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.Solutions.OpenTheLock;

// LC 752's deadends are this problem's, not the lock's: combinations it refuses, so the composed
// arm's graph of the lock's turns leaves them out.
internal readonly struct DeadendFilter(Set<string> deadends) : IStateFilter<string>
{
    public bool IsKept(string state) => !deadends.Has(state);
}
