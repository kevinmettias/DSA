namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3910 - a random connected graph on at most 13
// labeled nodes (the problem's own upper bound) with random 0/1 node values, the
// same "spanning backbone plus a little extra density" shape LabeledGraphWorkloads
// already uses for LC 3615, so both connectivity strategies walk a real graph
// with real cycles instead of a bare spanning tree.
internal static class EvenNodeSumGraphWorkloads
{
    private const int ExtraEdgesPerNode = 1;

    public static (int[] Nums, int[][] Edges) Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var nums = new int[nodeCount];

        for (var i = 0; i < nodeCount; i++)
        {
            nums[i] = random.Next(2);
        }

        var edgeSet = new HashSet<(int Low, int High)>();

        for (var i = 1; i < nodeCount; i++)
        {
            edgeSet.Add((random.Next(i), i));
        }

        for (var i = 0; i < nodeCount; i++)
        {
            for (var e = 0; e < ExtraEdgesPerNode; e++)
            {
                var target = random.Next(nodeCount);

                if (target != i)
                {
                    edgeSet.Add((Math.Min(i, target), Math.Max(i, target)));
                }
            }
        }

        var edges = edgeSet.Select(pair => new[] { pair.Low, pair.High }).ToArray();

        return (nums, edges);
    }
}
