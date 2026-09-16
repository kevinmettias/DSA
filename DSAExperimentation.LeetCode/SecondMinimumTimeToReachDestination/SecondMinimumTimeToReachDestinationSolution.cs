using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<(int Intersection, int Steps)>;

namespace DSAExperimentation.LeetCode.SecondMinimumTimeToReachDestination;

// LeetCode 2045. Second Minimum Time to Reach Destination: every road takes the same
// `time` minutes and every intersection's signal flips every `change` minutes, so a
// trip's total time is a strictly increasing function of how many roads it uses.
// "Second minimum time" therefore reduces to the second distinct EDGE COUNT that
// reaches the destination - a dual-distance BFS that records each intersection's
// first AND second distinct depth instead of stopping at the first - after which the
// red lights are plain arithmetic, not a data-structure concern.
//
// Both strategies run exactly that BFS - the relaxation is EdgeCounts.TryRecord in
// both, so the walks agree by construction. What differs is the frontier, which is
// the whole of the comparison and so is written out in each arm rather than hidden
// behind a shared iterator neither would allocate on its own: a bare List<T> dequeued
// from the front, paying an O(n) shift per step, against this repo's own Deque-backed
// Queue<Element>, which is O(1) at both ends.
internal static class SecondMinimumTimeToReachDestinationSolution
{
    // A depth this intersection has not been reached at yet.
    private const int Unvisited = -1;

    // Signals alternate green, red, green, red - so an odd cycle index is a red one.
    private const int SignalPhaseModulus = 2;

    // The textbook mistake: a List<T> standing in for a queue, RemoveAt(0) on every
    // step. Deliberately written without this repo's primitives - it is the arm the
    // Queue-backed walk below has to justify itself against.
    public static int SecondMinimumTimeByListFrontier(int intersectionCount, int[][] edges, int time, int change)
    {
        var network = IntersectionNetwork.Build(intersectionCount, edges);

        return SecondMinimumTimeByListFrontier(network, time, change);
    }

    public static int SecondMinimumTimeByListFrontier(IntersectionNetwork network, int time, int change)
    {
        var depths = EdgeCounts.For(network);
        var frontier = new List<(int Intersection, int Steps)> { (network.Start, 0) };

        while (frontier.Count > 0)
        {
            var current = frontier[0];
            frontier.RemoveAt(0);
            var nextSteps = current.Steps + 1;

            foreach (var neighbor in network.RoadsFrom(current.Intersection))
            {
                if (depths.TryRecord(neighbor, nextSteps))
                {
                    frontier.Add((neighbor, nextSteps));
                }
            }
        }

        return TravelTimeFor(depths.SecondAt(network.Destination), time, change);
    }

    // The same walk over this repo's own Queue<Element>, whose Deque representation
    // pops the front without shifting anything.
    public static int SecondMinimumTimeByQueueFrontier(int intersectionCount, int[][] edges, int time, int change)
    {
        var network = IntersectionNetwork.Build(intersectionCount, edges);

        return SecondMinimumTimeByQueueFrontier(network, time, change);
    }

    public static int SecondMinimumTimeByQueueFrontier(IntersectionNetwork network, int time, int change)
    {
        var depths = EdgeCounts.For(network);
        var frontier = new RepoQueue();
        frontier.Enqueue((network.Start, 0));

        while (frontier.TryDequeue(out var current))
        {
            var nextSteps = current.Steps + 1;

            foreach (var neighbor in network.RoadsFrom(current.Intersection))
            {
                if (depths.TryRecord(neighbor, nextSteps))
                {
                    frontier.Enqueue((neighbor, nextSteps));
                }
            }
        }

        return TravelTimeFor(depths.SecondAt(network.Destination), time, change);
    }

    // Leaving at minute 0 and driving `edgeCount` roads, waiting out the red light at
    // every intersection whose signal is red on arrival.
    private static int TravelTimeFor(int edgeCount, int time, int change)
    {
        var currentTime = 0;

        for (var i = 0; i < edgeCount; i++)
        {
            var cycle = currentTime / change;

            if (cycle % SignalPhaseModulus == 1)
            {
                currentTime = (cycle + 1) * change;
            }

            currentTime += time;
        }

        return currentTime;
    }

    // Each intersection's first and second DISTINCT BFS depth. A repeat of a depth
    // already recorded is not a second route, which is what keeps the walk linear.
    private readonly record struct EdgeCounts(int[] First, int[] Second)
    {
        public static EdgeCounts For(IntersectionNetwork network)
        {
            var first = new int[network.IntersectionCount];
            var second = new int[network.IntersectionCount];
            Array.Fill(first, Unvisited);
            Array.Fill(second, Unvisited);
            first[network.Start] = 0;

            return new EdgeCounts(first, second);
        }

        public int SecondAt(int intersection) => Second[intersection];

        public bool TryRecord(int intersection, int steps)
        {
            if (First[intersection] == Unvisited)
            {
                First[intersection] = steps;

                return true;
            }

            if (First[intersection] != steps && Second[intersection] == Unvisited)
            {
                Second[intersection] = steps;

                return true;
            }

            return false;
        }
    }
}
