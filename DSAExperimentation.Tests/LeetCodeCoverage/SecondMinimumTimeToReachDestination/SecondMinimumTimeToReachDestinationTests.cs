using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Node, int Steps)>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.SecondMinimumTimeToReachDestination;

// LeetCode 2045. Second Minimum Time to Reach Destination: every road takes the
// same `time` minutes, so "second minimum time" reduces to a second-shortest
// path measured in EDGE COUNT - a dual-distance BFS variant of the level-by-level
// peel MinimumHeightTreesTests already uses, tracking each node's first AND
// second distinct BFS depth instead of stopping at the first. This repo's own
// Queue<(int,int)> (DataStructures.Queue.Queue<Element>) is the frontier, the
// same primitive MinimumCostToReachDestinationInTimeTests uses for its own
// state-expansion BFS. Once the second-shortest edge count is known, the
// traffic-signal wait at each intersection is a plain arithmetic simulation, not
// itself a data-structure concern.
public sealed partial class SecondMinimumTimeToReachDestinationTests
{
    [Fact]
    public void SecondMinimumTime_ClassicExampleWithBranchingGraph_WaitsOutOneRedLight()
    {
        int[][] edges = [[1, 2], [1, 3], [1, 4], [3, 4], [4, 5]];

        var minutes = SecondMinimumTime(n: 5, edges, time: 3, change: 5);

        Assert.Equal(13, minutes);
    }

    [Fact]
    public void SecondMinimumTime_TwoNodesForcesBackAndForthTravel_WaitsOutTwoRedLights()
    {
        int[][] edges = [[1, 2]];

        var minutes = SecondMinimumTime(n: 2, edges, time: 3, change: 2);

        Assert.Equal(11, minutes);
    }

    private static int SecondMinimumTime(int n, int[][] edges, int time, int change)
    {
        var adjacency = BuildAdjacency(n, edges);
        var (_, secondShortest) = ComputeFirstAndSecondEdgeCounts(n, adjacency);

        return SimulateTravelTime(secondShortest[n - 1], time, change);
    }

    private static List<int>[] BuildAdjacency(int n, int[][] edges)
    {
        var adjacency = new List<int>[n];
        for (var i = 0; i < n; i++)
        {
            adjacency[i] = [];
        }

        foreach (var edge in edges)
        {
            var (a, b) = (edge[0] - 1, edge[1] - 1);
            adjacency[a].Add(b);
            adjacency[b].Add(a);
        }

        return adjacency;
    }

    private static (int[] First, int[] Second) ComputeFirstAndSecondEdgeCounts(int n, List<int>[] adjacency)
    {
        var first = new int[n];
        var second = new int[n];
        Array.Fill(first, -1);
        Array.Fill(second, -1);
        first[0] = 0;

        var frontier = new RepoQueue();
        frontier.Enqueue((0, 0));

        var edgeCounts = new EdgeCounts(first, second);

        while (frontier.TryDequeue(out var current))
        {
            RelaxNeighbors(adjacency, current, edgeCounts, frontier);
        }

        return (first, second);
    }

    private readonly record struct EdgeCounts(int[] First, int[] Second);

    private static void RelaxNeighbors(
        List<int>[] adjacency, (int Node, int Steps) current, EdgeCounts edgeCounts, RepoQueue frontier)
    {
        var nextSteps = current.Steps + 1;

        foreach (var neighbor in adjacency[current.Node])
        {
            if (edgeCounts.First[neighbor] == -1)
            {
                edgeCounts.First[neighbor] = nextSteps;
                frontier.Enqueue((neighbor, nextSteps));
            }
            else if (edgeCounts.First[neighbor] != nextSteps && edgeCounts.Second[neighbor] == -1)
            {
                edgeCounts.Second[neighbor] = nextSteps;
                frontier.Enqueue((neighbor, nextSteps));
            }
        }
    }

    private static int SimulateTravelTime(int edgeCount, int time, int change)
    {
        var currentTime = 0;

        for (var i = 0; i < edgeCount; i++)
        {
            var cycle = currentTime / change;
            if (cycle % 2 == 1)
            {
                currentTime = (cycle + 1) * change;
            }

            currentTime += time;
        }

        return currentTime;
    }
}
