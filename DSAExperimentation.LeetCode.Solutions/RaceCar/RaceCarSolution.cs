using DSAExperimentation.Algorithms.ShortestPaths;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;
using DSAExperimentation.DataStructures.Graph.States;
using CarState = (int Position, int Speed);

namespace DSAExperimentation.LeetCode.Solutions.RaceCar;

// LeetCode 818. Race Car: the fewest 'A'/'R' commands that put the car exactly on
// target, starting at position 0 with speed 1.
//
// Each command is one edge between (position, speed) states, so "fewest commands"
// is exactly an unweighted shortest path - the same shape OpenTheLock (LC 752)
// answers. The state space is unbounded on its own (speed doubles every 'A'), so
// both strategies search inside RaceCarStateSpace's target-scaled box: the textbook
// arm generates states on the fly and just refuses to leave it, the composed arm
// pre-materializes it as a graph and hands the whole thing to BreadthFirstDistances.
internal static class RaceCarSolution
{
    // The textbook answer: BCL Queue + HashSet over mutated (position, speed)
    // tuples, stopping at the first dequeued state that sits on the target
    // whatever its speed. Deliberately written without this repo's primitives -
    // only the bound it is handed is a problem type (ARCHITECTURE.md section 17.5).
    public static int MinCommandsByMutationQueue(int target) =>
        MinCommandsByMutationQueue(RaceCarStateSpace.For(target), target);

    public static int MinCommandsByMutationQueue(RaceCarStateSpace space, int target)
    {
        var visited = new HashSet<(int Position, int Speed)>
        {
            (RaceCarMotion.StartPosition, RaceCarMotion.StartSpeed),
        };
        var walk = new CommandWalk(space, visited, new Queue<(int Position, int Speed, int Commands)>());
        walk.Queue.Enqueue((RaceCarMotion.StartPosition, RaceCarMotion.StartSpeed, 0));

        while (walk.Queue.Count > 0)
        {
            var state = walk.Queue.Dequeue();

            if (state.Position == target)
            {
                return state.Commands;
            }

            EnqueueCommands(state, walk);
        }

        return LeetCodeAnswer.None;
    }

    private static void EnqueueCommands((int Position, int Speed, int Commands) state, CommandWalk walk)
    {
        var acceleratePosition = state.Position + state.Speed;
        var accelerateSpeed = state.Speed * RaceCarMotion.SpeedDoublingFactor;

        if (walk.Space.IsWithinBounds(acceleratePosition, accelerateSpeed) &&
            walk.Visited.Add((acceleratePosition, accelerateSpeed)))
        {
            walk.Queue.Enqueue((acceleratePosition, accelerateSpeed, state.Commands + 1));
        }

        // 'R' never moves the car, so the reversed state is always in the box.
        var reverseSpeed = state.Speed > 0 ? -RaceCarMotion.StartSpeed : RaceCarMotion.StartSpeed;

        if (walk.Visited.Add((state.Position, reverseSpeed)))
        {
            walk.Queue.Enqueue((state.Position, reverseSpeed, state.Commands + 1));
        }
    }

    // Bundles the parts of the walk that never change, so a step names one walk
    // parameter instead of the bound, the visited set and the queue.
    private readonly record struct CommandWalk(
        RaceCarStateSpace Space,
        HashSet<(int Position, int Speed)> Visited,
        Queue<(int Position, int Speed, int Commands)> Queue);

    // This repo's own BFS: BreadthFirstDistances is already "distance from a root to
    // every node", so the puzzle reduces to the cheapest of the states that share the
    // target position - the car may arrive at any speed.
    public static int MinCommandsByReduceGraph(int target) =>
        MinCommandsByReduceGraph(BuildCommandGraph(target), target);

    public static int MinCommandsByReduceGraph(StateGraph<CarState> graph, int target)
    {
        if (!graph.TryGetNode((RaceCarMotion.StartPosition, RaceCarMotion.StartSpeed), out var start))
        {
            return LeetCodeAnswer.None;
        }

        var distances = BreadthFirstDistances.From<StateNode<CarState>, StateTopology<CarState>, ListChildren<StateNode<CarState>>>(start);

        var best = BestArrival(distances, graph, RaceCarStateSpace.For(target), target);

        return best == int.MaxValue ? LeetCodeAnswer.None : best;
    }

    // The cheapest arrival at the target over every speed in the box: both signs of each power of
    // two up to its bound, as RaceCarStateSpace.Speeds lists them, walked here rather than listed so
    // the search allocates nothing beyond the walk.
    private static int BestArrival(
        Dictionary<StateNode<CarState>, int> distances, StateGraph<CarState> graph, RaceCarStateSpace box, int target)
    {
        var best = int.MaxValue;

        for (var magnitude = RaceCarMotion.StartSpeed;
            magnitude <= box.MaxSpeedMagnitude;
            magnitude *= RaceCarMotion.SpeedDoublingFactor)
        {
            var forward = ArrivalDistance(graph, distances, (target, magnitude));
            var backward = ArrivalDistance(graph, distances, (target, -magnitude));
            var atThisMagnitude = Math.Min(forward, backward);
            best = Math.Min(best, atThisMagnitude);
        }

        return best;
    }

    // The command count the BFS recorded for arriving in this state - int.MaxValue when that
    // arrival has no recorded distance at all, so it can never beat one that does.
    private static int ArrivalDistance(StateGraph<CarState> graph, Dictionary<StateNode<CarState>, int> distances, CarState arrival)
    {
        if (graph.TryGetNode(arrival, out var node) && distances.TryGetValue(node, out var distance))
        {
            return distance;
        }

        return int.MaxValue;
    }

    // The composed arm's input: every (position, speed) in the target's box, joined by the 'A' and
    // 'R' commands that stay inside it. Public so a benchmark can build it outside the timed region
    // (ARCHITECTURE.md section 17.4).
    public static StateGraph<CarState> BuildCommandGraph(int target)
        => StateGraph<CarState>.Build(new RaceCarCommands(RaceCarStateSpace.For(target)), new EveryState<CarState>());
}
