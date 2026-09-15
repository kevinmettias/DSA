using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.DataStructures.Graph.Hamming;

// Breadth-first search over one-character mutations without ever materializing a
// HammingGraph: candidates are generated on the fly and kept only when the allowed
// set contains them.
//
// This is the "textbook" half of every benchmark in this family - a BCL Queue and
// HashSet, deliberately composing none of this repo's graph engine, so that
// HammingDistances has something to be measured against. It lives here rather than
// beside any one problem because LC 127 and LC 433 are the same search, differing
// only in alphabet and in whether the answer counts nodes or edges.
internal static class HammingSearch
{
    // Edges walked from start to target, or null when no mutation path exists
    // through `allowed` - a nullable result rather than a sentinel, so each caller
    // maps "unreachable" onto whatever its own problem reports (0 for LC 127, -1
    // for LC 433) without a shared magic value in between. `start` itself need not
    // be a member of `allowed`, matching LeetCode's beginWord/startGene convention,
    // which is also why the two endpoints are distinct types and not two strings.
    public static int? MutationDistance(
        MutationStart start, MutationTarget target, Set<string> allowed, Alphabet alphabet)
    {
        var walk = new MutationWalk(alphabet, allowed, [start.Text], new Queue<(string, int)>());
        walk.Queue.Enqueue((start.Text, 0));

        while (walk.Queue.Count > 0)
        {
            var (value, distance) = walk.Queue.Dequeue();

            if (value == target.Text)
            {
                return distance;
            }

            EnqueueAllowedMutations(value, distance, walk);
        }

        return null;
    }

    private static void EnqueueAllowedMutations(string value, int distance, MutationWalk walk)
    {
        foreach (var candidate in HammingGraph.OneCharacterMutations(value, walk.Alphabet))
        {
            if (walk.Allowed.Has(candidate) && walk.Visited.Add(candidate))
            {
                walk.Queue.Enqueue((candidate, distance + 1));
            }
        }
    }

    // The three collections one breadth-first walk threads through every step,
    // plus the alphabet it mutates over.
    private readonly record struct MutationWalk(
        Alphabet Alphabet,
        Set<string> Allowed,
        HashSet<string> Visited,
        Queue<(string Value, int Distance)> Queue);
}
