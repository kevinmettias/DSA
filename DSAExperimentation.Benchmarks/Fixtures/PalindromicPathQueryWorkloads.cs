using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.LeetCode.PalindromicPathQueriesInATree;

namespace DSAExperimentation.Benchmarks.Fixtures;

// Benchmark workload sizing for LC 3841 - how big the tree is, how its letters are
// drawn and how many commands run against it; what a command means is
// PalindromicPathQueriesInATreeSolution's job.
//
// The tree is long and thin: node i hangs off one of the AttachmentWindow nodes just
// before it, so no parent is more than AttachmentWindow ids back and node
// nodeCount - 1 sits at depth at least (nodeCount - 1) / AttachmentWindow below node 0.
// A path between two random nodes is then a sizable fraction of the tree - the length
// the ancestor walk pays on every query and the Fenwick never does - while nodes still
// branch. Each edge is written in a random orientation, since LeetCode's edges are
// undirected.
//
// Letters come from the first LetterCount letters only. Over all 26, a path hundreds of
// nodes long almost never rearranges into a palindrome, so every answer would be false;
// over three, a path's parity mask is near uniform over 8 values, and the 4 with at
// most one bit set answer true.
//
// There is one command per node, each an update or a query with even odds, its nodes
// and letter drawn uniformly - so both counts stay inside LeetCode's 5 * 10^4.
internal static class PalindromicPathQueryWorkloads
{
    public const int AttachmentWindow = 8;
    public const int LetterCount = 3;
    public const string UpdateVerb = PalindromicPathQueriesInATreeSolution.UpdateVerb;
    public const string QueryVerb = "query";

    private const int CoinFaces = 2;

    public static (int[][] Edges, string Letters, string[] Commands) Build(int nodeCount, int seed)
    {
        var random = new Random(seed);
        var edges = WindowedTreeEdges(nodeCount, random);
        var letters = DrawLetters(nodeCount, random);
        var commands = Enumerable.Range(0, nodeCount).Select(_ => DrawCommand(nodeCount, random)).ToArray();

        return (edges, letters, commands);
    }

    private static int[][] WindowedTreeEdges(int nodeCount, Random random)
    {
        var edges = new int[nodeCount - 1][];

        for (var node = 1; node < nodeCount; node++)
        {
            var earliestParent = Math.Max(0, node - AttachmentWindow);
            var edge = new[] { random.Next(earliestParent, node), node };

            if (IsHeads(random))
            {
                Array.Reverse(edge);
            }

            edges[node - 1] = edge;
        }

        return edges;
    }

    private static string DrawCommand(int nodeCount, Random random)
    {
        var node = random.Next(nodeCount);
        var isUpdate = IsHeads(random);

        return isUpdate
            ? $"{UpdateVerb} {node} {DrawLetters(1, random)}"
            : $"{QueryVerb} {node} {random.Next(nodeCount)}";
    }

    private static string DrawLetters(int count, Random random)
    {
        var indices = SeededDraws.Values(count, 0, LetterCount, random);
        var alphabet = StandardAlphabets.LowercaseLatin.Characters;

        return new string(Array.ConvertAll(indices, index => alphabet[index]));
    }

    // A fair coin: heads on one of CoinFaces equally likely draws.
    private static bool IsHeads(Random random) => random.Next(CoinFaces) == 0;
}
