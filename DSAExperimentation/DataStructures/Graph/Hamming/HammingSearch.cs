using DSAExperimentation.DataStructures.Alphabets;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.DataStructures.Graph.Hamming;

// Breadth-first search over one-character mutations without ever materializing a
// HammingGraph: candidates are generated on the fly and kept only when the allowed
// set contains them.
//
// A BCL Queue and HashSet, deliberately composing none of this repo's graph engine. It
// lives here rather than beside any one problem because LC 127 and LC 433 are the same
// search, differing only in alphabet and in whether the answer counts nodes or edges.
// No benchmark arm calls it: each problem's baseline keeps its own copy (ARCHITECTURE
// 17.5), and its composed arms use HammingDistances.
/// <summary>
/// Breadth-first search over one-character mutations that never builds a <see cref="HammingGraph"/>.
/// </summary>
public static class HammingSearch
{
    // Edges walked from start to target, or null when no mutation path exists
    // through `allowed` - a nullable result rather than a sentinel, so each caller
    // maps "unreachable" onto whatever its own problem reports (0 for LC 127, -1
    // for LC 433) without a shared magic value in between. `start` itself need not
    // be a member of `allowed`, matching LeetCode's beginWord/startGene convention,
    // which is also why the two endpoints are distinct types and not two strings.
    /// <summary>
    /// Finds the fewest one-character mutations that turn start into target through allowed strings.
    /// </summary>
    /// <typeparam name="TAlphabet">The letters a mutation may write.</typeparam>
    /// <param name="start">The string to start from, which need not be in <paramref name="allowed"/>.</param>
    /// <param name="target">The string to reach.</param>
    /// <param name="allowed">The strings every mutation along the way must produce.</param>
    /// <returns>The mutation count of a shortest path, 0 when start is the target, or null when none exists.</returns>
    public static int? MutationDistance<TAlphabet>(MutationStart start, MutationTarget target, Set<string> allowed)
        where TAlphabet : struct, IAlphabet
    {
        var walk = new MutationWalk(allowed, [start.Text], new Queue<(string, int)>());
        walk.Queue.Enqueue((start.Text, 0));

        while (walk.Queue.Count > 0)
        {
            var (value, distance) = walk.Queue.Dequeue();

            if (value == target.Text)
            {
                return distance;
            }

            EnqueueAllowedMutations<TAlphabet>(value, distance, walk);
        }

        return null;
    }

    private static void EnqueueAllowedMutations<TAlphabet>(string value, int distance, MutationWalk walk)
        where TAlphabet : struct, IAlphabet
    {
        foreach (var candidate in HammingGraph.OneCharacterMutations<TAlphabet>(value))
        {
            if (walk.Allowed.Has(candidate) && walk.Visited.Add(candidate))
            {
                walk.Queue.Enqueue((candidate, distance + 1));
            }
        }
    }

    // The three collections one breadth-first walk threads through every step.
    private readonly record struct MutationWalk(
        Set<string> Allowed,
        HashSet<string> Visited,
        Queue<(string Value, int Distance)> Queue);
}
