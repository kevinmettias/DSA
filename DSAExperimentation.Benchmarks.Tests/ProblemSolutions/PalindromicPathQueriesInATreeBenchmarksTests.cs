using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.Benchmarks.ProblemSolutions;

namespace DSAExperimentation.Benchmarks.Tests.ProblemSolutions;

// Harness coverage for PalindromicPathQueriesInATreeBenchmarks (ARCHITECTURE 17.9), for what the
// generic arm check cannot see. At the smallest size the baseline's answer is held to one derived
// here without either strategy: each query searches outward from one endpoint until it reaches the
// other and counts the letters on the way back, after replaying every update before it - and that
// answer must hold both outcomes, which is what drawing letters from a three-letter alphabet
// promises. At the largest size, which only the composed arm runs, the answer must carry exactly
// one flag per "query" command the workload drew.
public sealed partial class PalindromicPathQueriesInATreeBenchmarksTests
{
    private const int SmallestNodeCount = 1_000;
    private const int LargestNodeCount = 50_000;

    // Mirrors PalindromicPathQueriesInATreeBenchmarks' own private RandomSeed.
    private const int Seed = 3841;

    private const int AtMostOneOddLetter = 1;
    private const int VerbWord = 0;
    private const int NodeWord = 1;
    private const int ArgumentWord = 2;

    [Fact]
    public void AncestorWalk_SmallestNodeCount_MatchesAnIndependentPathCount()
    {
        var (edges, letters, commands) = PalindromicPathQueryWorkloads.Build(SmallestNodeCount, Seed);
        var expected = AnswerBySearchingEachPath(edges, letters, commands);

        Assert.Contains(true, expected);
        Assert.Contains(false, expected);
        Assert.Equal(expected, BuildHarness().AncestorWalk(SmallestNodeCount));
    }

    [Fact]
    public void EulerFenwick_LargestNodeCount_AnswersEveryQueryCommandOnce()
    {
        var (_, _, commands) = PalindromicPathQueryWorkloads.Build(LargestNodeCount, Seed);
        var queryCount = commands.Count(command => command.StartsWith(PalindromicPathQueryWorkloads.QueryVerb));

        Assert.Equal(queryCount, BuildHarness().EulerFenwick(LargestNodeCount).Length);
    }

    private static PalindromicPathQueriesInATreeBenchmarks BuildHarness()
    {
        var harness = new PalindromicPathQueriesInATreeBenchmarks();
        harness.Setup();

        return harness;
    }

    // Replays the script in order: an update rewrites one letter, and a query is answered from the
    // letters as they stand at that point.
    private static List<bool> AnswerBySearchingEachPath(int[][] edges, string letters, string[] commands)
    {
        var neighbors = UndirectedNeighbors(edges, letters.Length);
        var current = letters.ToCharArray();
        var answers = new List<bool>();

        foreach (var words in commands.Select(command => command.Split(' ')))
        {
            var node = int.Parse(words[NodeWord]);
            var argument = words[ArgumentWord];

            if (words[VerbWord] == PalindromicPathQueryWorkloads.UpdateVerb)
            {
                current[node] = argument[0];
                continue;
            }

            var oddLetters = OddLettersOnPath(neighbors, current, node, int.Parse(argument));
            answers.Add(oddLetters.Count <= AtMostOneOddLetter);
        }

        return answers;
    }

    private static List<int>[] UndirectedNeighbors(int[][] edges, int nodeCount)
    {
        var neighbors = Enumerable.Range(0, nodeCount).Select(_ => new List<int>()).ToArray();

        foreach (var edge in edges)
        {
            neighbors[edge[0]].Add(edge[1]);
            neighbors[edge[1]].Add(edge[0]);
        }

        return neighbors;
    }

    // Breadth-first from start records how each node was first reached; walking those links back
    // from end visits exactly the path's nodes, each letter toggled in or out of the odd set.
    private static HashSet<char> OddLettersOnPath(List<int>[] neighbors, char[] letters, int start, int end)
    {
        var cameFrom = new Dictionary<int, int> { [start] = start };
        var frontier = new Queue<int>([start]);

        while (frontier.TryDequeue(out var node))
        {
            foreach (var neighbor in neighbors[node].Where(neighbor => !cameFrom.ContainsKey(neighbor)))
            {
                cameFrom[neighbor] = node;
                frontier.Enqueue(neighbor);
            }
        }

        var odd = new HashSet<char>();

        for (var node = end; node != start; node = cameFrom[node])
        {
            ToggleLetter(odd, letters[node]);
        }

        ToggleLetter(odd, letters[start]);

        return odd;
    }

    private static void ToggleLetter(HashSet<char> odd, char letter)
    {
        if (!odd.Add(letter))
        {
            odd.Remove(letter);
        }
    }
}
