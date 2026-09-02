using System.Numerics;

namespace DSAExperimentation.LeetCode.MinimumTimeToTransportAllIndividuals;

// Every (mask, stage) pair for n individuals and m environmental stages,
// wired with every legal round LC 3594's own rules allow - the domain model,
// not an answer to any one query about it (LockGraph's own framing). Built
// eagerly rather than discovered lazily by the search itself, the same
// choice LockGraph and DigitStepGraph both make; n <= 12 and m <= 5 keep the
// full (mask, stage, group-subset) enumeration well under a second.
internal sealed class TransportGraph
{
    private TransportGraph(Dictionary<(int Mask, int Stage), TransportState> nodes, int fullMask)
    {
        Nodes = nodes;
        FullMask = fullMask;
    }

    public Dictionary<(int Mask, int Stage), TransportState> Nodes { get; }
    public int FullMask { get; }
    public TransportState Source => Nodes[(0, 0)];

    public static TransportGraph Build(int[] time, int capacity, double[] mul)
    {
        var fullMask = (1 << time.Length) - 1;
        var nodes = BuildNodes(fullMask, mul.Length);

        WireRounds(nodes, time, capacity, mul, fullMask);

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
        Dictionary<(int, int), TransportState> nodes, int[] time, int capacity, double[] mul, int fullMask)
    {
        foreach (var ((mask, stage), node) in nodes)
        {
            if (mask == fullMask)
            {
                continue;
            }

            foreach (var (roundTime, nextMask, nextStage) in Rounds(mask, stage, time, capacity, mul, fullMask))
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
        int mask, int stage, int[] time, int capacity, double[] mul, int fullMask)
    {
        var complement = fullMask & ~mask;

        for (var group = complement; group != 0; group = (group - 1) & complement)
        {
            if (BitOperations.PopCount((uint)group) > capacity)
            {
                continue;
            }

            var crossingTime = MaxTime(group, time) * mul[stage];
            var afterCrossingStage = Advance(stage, crossingTime, mul.Length);
            var arrivedMask = mask | group;

            if (arrivedMask == fullMask)
            {
                yield return (crossingTime, arrivedMask, afterCrossingStage);
                continue;
            }

            foreach (var round in Returns(arrivedMask, afterCrossingStage, crossingTime, time, mul))
            {
                yield return round;
            }
        }
    }

    private static IEnumerable<(double Time, int Mask, int Stage)> Returns(
        int arrivedMask, int afterCrossingStage, double crossingTime, int[] time, double[] mul)
    {
        for (var person = 0; person < time.Length; person++)
        {
            if ((arrivedMask & (1 << person)) == 0)
            {
                continue;
            }

            var returnTime = time[person] * mul[afterCrossingStage];
            var afterReturnStage = Advance(afterCrossingStage, returnTime, mul.Length);
            var maskAfterReturn = arrivedMask & ~(1 << person);

            yield return (crossingTime + returnTime, maskAfterReturn, afterReturnStage);
        }
    }

    // LC 3594's own stage-cycling law: elapsed minutes advance the stage by
    // floor(elapsed) % m steps, relative to whatever stage the crossing or
    // return departed from.
    private static int Advance(int stage, double elapsed, int stageCount) =>
        (stage + ((int)Math.Floor(elapsed) % stageCount)) % stageCount;

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
}
