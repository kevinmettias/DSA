using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.LeetCode.RaceCar;

// LeetCode 818. Race Car: the fewest 'A'/'R' commands that put the car exactly on
// target, starting at position 0 with speed 1.
//
// Each command is one edge between (position, speed) states, so "fewest commands"
// is exactly an unweighted shortest path - the same shape OpenTheLock (LC 752)
// answers. The state space is unbounded on its own (speed doubles every 'A'), so
// both strategies search inside RaceCarStateSpace's target-scaled box: the textbook
// arm generates states on the fly and just refuses to leave it, the composed arm
// pre-materializes it as a graph and hands the whole thing to Reduce.Graph.
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

        if (walk.Space.Contains(acceleratePosition, accelerateSpeed) &&
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

    // This repo's own BFS: Reduce.Graph in BreadthFirstReduceOrder with
    // DistanceMapReduceAlgebra is already "distance from a root to every node", so
    // the puzzle reduces to the cheapest of the states that share the target
    // position - the car may arrive at any speed.
    public static int MinCommandsByReduceGraph(int target) =>
        MinCommandsByReduceGraph(RaceCarStateGraph.Build(target), target);

    public static int MinCommandsByReduceGraph(RaceCarStateGraph graph, int target)
    {
        var distances = Reduce.Graph<
            RaceCarNode, RaceCarTopology, ListChildren<RaceCarNode>,
            NaturalChildOrder<RaceCarNode, ListChildren<RaceCarNode>>, ListChildren<RaceCarNode>,
            BreadthFirstReduceOrder<RaceCarNode>,
            DistanceMapReduceAlgebra<RaceCarNode>, Dictionary<RaceCarNode, int>>(graph.Start);

        var best = BestArrival(distances, graph, target);

        return best == int.MaxValue ? LeetCodeAnswer.None : best;
    }

    private static int BestArrival(
        Dictionary<RaceCarNode, int> distances, RaceCarStateGraph graph, int target)
    {
        var best = int.MaxValue;

        foreach (var speed in graph.Speeds)
        {
            var distance = ArrivalDistance(graph, distances, target, speed);

            if (distance < best)
            {
                best = distance;
            }
        }

        return best;
    }

    // The command count the BFS recorded for arriving at `target` at this speed -
    // int.MaxValue when that arrival has no recorded distance at all, so it can
    // never beat one that does.
    private static int ArrivalDistance(
        RaceCarStateGraph graph, Dictionary<RaceCarNode, int> distances, int target, int speed)
    {
        if (graph.TryGetNode(target, speed, out var node) && distances.TryGetValue(node, out var distance))
        {
            return distance;
        }

        return int.MaxValue;
    }
}
