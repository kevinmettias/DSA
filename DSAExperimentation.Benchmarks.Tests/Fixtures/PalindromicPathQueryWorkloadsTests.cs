using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Conventions;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for PalindromicPathQueryWorkloads (ARCHITECTURE 17.7). The LC 3841 reading
// depends on the edges really describing a tree over [0, nodeCount), on that tree being long and
// thin - no parent more than AttachmentWindow ids back, so the last node sits at least
// (nodeCount - 1) / AttachmentWindow below node 0 - and on a command script that mixes updates
// with queries. Every count and draw is checked against LeetCode's bounds at the largest size the
// benchmark builds.
public sealed partial class PalindromicPathQueryWorkloadsTests
{
    private const int NodeCount = 1_000;
    private const int LargestNodeCount = 50_000;
    private const int Seed = 3841; // LC problem number
    private const int Root = 0;

    // LC 3841: 1 <= n <= 5 * 10^4 and 1 <= queries.length <= 5 * 10^4.
    private const int MaxNodeCount = 50_000;
    private const int MaxCommandCount = 50_000;

    private const int EdgeFieldCount = 2;
    private const int CommandWordCount = 3;
    private const int VerbWord = 0;
    private const int NodeWord = 1;
    private const int ArgumentWord = 2;
    private const char LowestLetter = 'a';
    private const char HighestLetter = 'z';

    // Even odds over a thousand draws: each kind should land well inside this band.
    private const int FewestOfEachCommandKind = 400;

    [Fact]
    public void Build_LargestNodeCount_StaysInsideLeetCodesBounds()
    {
        var (edges, letters, commands) = PalindromicPathQueryWorkloads.Build(LargestNodeCount, Seed);

        Assert.InRange(letters.Length, 1, MaxNodeCount);
        Assert.InRange(commands.Length, 1, MaxCommandCount);
        Assert.Equal(letters.Length - 1, edges.Length);
        Assert.All(edges, edge => AssertEdgeInRange(edge, letters.Length));
        Assert.All(letters, letter => Assert.InRange(letter, LowestLetter, HighestLetter));
        Assert.All(commands, command => AssertCommandInRange(command, letters.Length));
    }

    // n - 1 edges that connect every node to node 0 are a tree. Each parent is at most
    // AttachmentWindow ids back, so following parents from the last node to the root takes at
    // least (n - 1) / AttachmentWindow steps.
    [Fact]
    public void Build_Edges_FormALongThinTreeRootedAtNodeZero()
    {
        var (edges, _, _) = PalindromicPathQueryWorkloads.Build(NodeCount, Seed);
        var depth = DepthsFromRoot(edges, NodeCount);

        Assert.Equal(NodeCount - 1, edges.Length);
        Assert.DoesNotContain(-1, depth);
        Assert.True(depth[NodeCount - 1] >= (NodeCount - 1) / PalindromicPathQueryWorkloads.AttachmentWindow);
    }

    [Fact]
    public void Build_Letters_ComeFromTheFirstLetterCountLettersOnly()
    {
        var (_, letters, _) = PalindromicPathQueryWorkloads.Build(NodeCount, Seed);
        var highestDrawn = (char)(LowestLetter + PalindromicPathQueryWorkloads.LetterCount - 1);

        Assert.All(letters, letter => Assert.InRange(letter, LowestLetter, highestDrawn));
    }

    [Fact]
    public void Build_Commands_MixUpdatesWithQueries()
    {
        var (_, _, commands) = PalindromicPathQueryWorkloads.Build(NodeCount, Seed);
        var updates = commands.Count(command => command.StartsWith(PalindromicPathQueryWorkloads.UpdateVerb));
        var queries = commands.Count(command => command.StartsWith(PalindromicPathQueryWorkloads.QueryVerb));

        Assert.Equal(NodeCount, updates + queries);
        Assert.True(updates >= FewestOfEachCommandKind);
        Assert.True(queries >= FewestOfEachCommandKind);
    }

    [Fact]
    public void Build_SameSeed_ReturnsTheSameWorkload()
    {
        var (edges, letters, commands) = PalindromicPathQueryWorkloads.Build(NodeCount, Seed);
        var (repeatEdges, repeatLetters, repeatCommands) = PalindromicPathQueryWorkloads.Build(NodeCount, Seed);

        Assert.Equal(AnswerGraphText.Of(edges), AnswerGraphText.Of(repeatEdges));
        Assert.Equal(letters, repeatLetters);
        Assert.Equal(commands, repeatCommands);
    }

    private static void AssertEdgeInRange(int[] edge, int nodeCount)
    {
        Assert.Equal(EdgeFieldCount, edge.Length);
        Assert.All(edge, node => Assert.InRange(node, 0, nodeCount - 1));
    }

    // "update u c" with c a lowercase letter, or "query u v"; every node inside [0, n).
    private static void AssertCommandInRange(string command, int nodeCount)
    {
        var words = command.Split(' ');

        Assert.Equal(CommandWordCount, words.Length);
        Assert.InRange(int.Parse(words[NodeWord]), 0, nodeCount - 1);
        AssertArgumentInRange(words[VerbWord], words[ArgumentWord], nodeCount);
    }

    // An update's argument is the one lowercase letter it writes; a query's is the path's other end.
    private static void AssertArgumentInRange(string verb, string argument, int nodeCount)
    {
        if (verb == PalindromicPathQueryWorkloads.UpdateVerb)
        {
            Assert.Single(argument);
            Assert.InRange(argument[0], LowestLetter, HighestLetter);

            return;
        }

        Assert.Equal(PalindromicPathQueryWorkloads.QueryVerb, verb);
        Assert.InRange(int.Parse(argument), 0, nodeCount - 1);
    }

    // Breadth-first from node 0 over the undirected edges; a node never reached keeps -1.
    private static int[] DepthsFromRoot(int[][] edges, int nodeCount)
    {
        var neighbors = Enumerable.Range(0, nodeCount).Select(_ => new List<int>()).ToArray();

        foreach (var edge in edges)
        {
            neighbors[edge[0]].Add(edge[1]);
            neighbors[edge[1]].Add(edge[0]);
        }

        var depth = Enumerable.Repeat(-1, nodeCount).ToArray();
        depth[Root] = 0;
        var frontier = new Queue<int>([Root]);

        while (frontier.TryDequeue(out var node))
        {
            foreach (var neighbor in neighbors[node].Where(neighbor => depth[neighbor] < 0))
            {
                depth[neighbor] = depth[node] + 1;
                frontier.Enqueue(neighbor);
            }
        }

        return depth;
    }
}
