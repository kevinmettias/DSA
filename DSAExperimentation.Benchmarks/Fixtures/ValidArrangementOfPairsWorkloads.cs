namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 2097 - pairCount pairs over nodeCount nodes that form one
// trail, each pair starting where the one before it ended. The trail is walked from node 0, every
// step to a target drawn from the current node's still-unused ones, so no pair repeats, no pair
// joins a node to itself, and a valid arrangement exists: the three things LC 2097 promises. The
// pairs are then shuffled, so neither arm is handed the arrangement itself. A walk that reaches a
// node with every target used throws rather than return pairs that have no arrangement.
internal static class ValidArrangementOfPairsWorkloads
{
    // The trail BuildPairs shuffles, in walking order.
    public static int[][] BuildTrail(int pairCount, int nodeCount, int seed) =>
        BuildTrail(pairCount, nodeCount, new Random(seed));

    public static int[][] BuildPairs(int pairCount, int nodeCount, int seed)
    {
        var random = new Random(seed);
        var trail = BuildTrail(pairCount, nodeCount, random);
        var order = SeededSequences.ShuffledZeroTo(pairCount, random);

        return [.. order.Select(index => trail[index])];
    }

    private static int[][] BuildTrail(int pairCount, int nodeCount, Random random)
    {
        var unusedTargets = Enumerable.Range(0, nodeCount)
            .Select(node => Enumerable.Range(0, nodeCount).Where(target => target != node).ToList())
            .ToArray();
        var trail = new int[pairCount][];
        var current = 0;

        for (var i = 0; i < pairCount; i++)
        {
            var next = TakeUnusedTarget(unusedTargets[current], random);
            trail[i] = [current, next];
            current = next;
        }

        return trail;
    }

    // Draws one of a node's unused targets and removes it, by moving the last one into its place.
    private static int TakeUnusedTarget(List<int> targets, Random random)
    {
        if (targets.Count == 0)
        {
            throw new InvalidOperationException("The trail reached a node whose every target is already used.");
        }

        var drawn = random.Next(targets.Count);
        var target = targets[drawn];
        targets[drawn] = targets[^1];
        targets.RemoveAt(targets.Count - 1);

        return target;
    }
}
