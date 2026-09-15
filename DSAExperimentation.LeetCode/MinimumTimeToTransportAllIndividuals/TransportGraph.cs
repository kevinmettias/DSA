using System.Numerics;

namespace DSAExperimentation.LeetCode.MinimumTimeToTransportAllIndividuals;

// Every (mask, stage) pair for n individuals and m environmental stages,
// wired with every legal round LC 3594's own rules allow - the domain model,
// not an answer to any one query about it (LockGraph's own framing). Built
// eagerly rather than discovered lazily by the search itself, the same
// choice LockGraph and DigitStepGraph both make; n <= 12 and m <= 5 keep the
// full (mask, stage, group-subset) enumeration well under a second.
internal sealed class TransportGraph(
    Dictionary<(int Mask, int Stage), TransportState> nodes, int fullMask)
{
    public Dictionary<(int Mask, int Stage), TransportState> Nodes { get; } = nodes;

    public int FullMask { get; } = fullMask;
    public TransportState Source => Nodes[(0, 0)];

    public static TransportGraph Build(int[] time, int capacity, double[] mul)
    {
        var fullMask = (1 << time.Length) - 1;
        var nodes = BuildNodes(fullMask, mul.Length);

        WireRounds(nodes, time, capacity, mul);

        return new TransportGraph(nodes, fullMask);
    }

    private static Dictionary<(int, int), TransportState> BuildNodes(int fullMask, int stageCount)
    {
        var nodes = new Dictionary<(int, int), TransportState>();

        for (var mask = 0; mask <= fullMask; mask++)
        {
            for (var stage = 0; stage < stageCount; stage++)
            {
                nodes[(mask, stage)] = new TransportState(mask, stage);
            }
        }

        return nodes;
    }

    private static void WireRounds(
        Dictionary<(int, int), TransportState> nodes, int[] time, int capacity, double[] mul)
    {
        var fullMask = (1 << time.Length) - 1;

        foreach (var ((mask, stage), node) in nodes)
        {
            if (mask == fullMask)
            {
                continue;
            }

            foreach (var (roundTime, nextMask, nextStage) in Rounds((mask, stage), time, capacity, mul))
            {
                node.Edges.Add((roundTime, nodes[(nextMask, nextStage)]));
            }
        }
    }

    // Every legal combined round reachable from (mask, stage): send a
    // nonempty group of at most `capacity` still-near-side individuals
    // across, then - unless that group finished everyone - send one already-
    // arrived individual back. Pure river-crossing arithmetic, so a caller
    // that never materializes the graph (the brute-force baseline) can use it
    // too - the same split LockGraph.WheelTurnNeighbors makes for OpenTheLock.
    public static IEnumerable<(double Time, int Mask, int Stage)> Rounds(
        (int Mask, int Stage) state, int[] time, int capacity, double[] mul)
    {
        var fullMask = (1 << time.Length) - 1;
        var complement = fullMask & ~state.Mask;

        for (var group = complement; group != 0; group = (group - 1) & complement)
        {
            if (BitOperations.PopCount((uint)group) > capacity)
            {
                continue;
            }

            foreach (var round in CrossingRounds(group, state, time, mul))
            {
                yield return round;
            }
        }
    }

    // What one candidate group of crossers produces from here: the crossing on its own
    // when it finishes everyone, otherwise the crossing followed by each single-person
    // return trip Returns offers for it. `fullMask` is re-derived rather than threaded
    // through, since it is a function of `time` alone.
    private static IEnumerable<(double Time, int Mask, int Stage)> CrossingRounds(
        int group, (int Mask, int Stage) state, int[] time, double[] mul)
    {
        var fullMask = (1 << time.Length) - 1;
        var crossingTime = MaxTime(group, time) * mul[state.Stage];
        var afterCrossingStage = Advance(state.Stage, crossingTime, mul.Length);
        var arrivedMask = state.Mask | group;

        if (arrivedMask == fullMask)
        {
            yield return (crossingTime, arrivedMask, afterCrossingStage);
            yield break;
        }

        foreach (var round in Returns((arrivedMask, afterCrossingStage), crossingTime, time, mul))
        {
            yield return round;
        }
    }

    private static int MaxTime(int group, int[] time)
    {
        var max = 0;

        for (var person = 0; person < time.Length; person++)
        {
            if ((group & (1 << person)) != 0 && time[person] > max)
            {
                max = time[person];
            }
        }

        return max;
    }

    private static IEnumerable<(double Time, int Mask, int Stage)> Returns(
        (int Mask, int Stage) afterCrossing, double crossingTime, int[] time, double[] mul)
    {
        for (var person = 0; person < time.Length; person++)
        {
            if ((afterCrossing.Mask & (1 << person)) == 0)
            {
                continue;
            }

            var returnTime = time[person] * mul[afterCrossing.Stage];
            var afterReturnStage = Advance(afterCrossing.Stage, returnTime, mul.Length);
            var maskAfterReturn = afterCrossing.Mask & ~(1 << person);

            yield return (crossingTime + returnTime, maskAfterReturn, afterReturnStage);
        }
    }

    // LC 3594's own stage-cycling law: elapsed minutes advance the stage by
    // floor(elapsed) % m steps, relative to whatever stage the crossing or
    // return departed from.
    private static int Advance(int stage, double elapsed, int stageCount) =>
        (stage + ((int)Math.Floor(elapsed) % stageCount)) % stageCount;
}
