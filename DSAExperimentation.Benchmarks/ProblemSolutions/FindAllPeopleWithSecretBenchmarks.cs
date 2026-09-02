using BenchmarkDotNet.Attributes;
using DSAExperimentation.DataStructures.KeyedDisjointSet;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Find All People With Secret (LC 2092): within one timestamp group, the naive way
// to discover who gets pulled into an already-informed component is repeated
// relaxation - rescan every meeting in the group, propagate "knows" across any edge
// that's still mixed, and repeat until a full pass makes no change. That's O(k^2)
// per group in the worst case (a chain needs up to k passes over k meetings), the
// same "no hashing/union-find, just rescan" shape AccountsMergeBenchmarks'
// PairwiseEmailOverlapScan uses. RootMergedWithKeyedDisjointSet instead builds one
// KeyedDisjointSet<int> per group (this repo's own composing wrapper around
// DisjointSet, same primitive AccountsMergeTests already proved out for "union on
// shared membership") and a Set<int> to mark which resulting components touch an
// already-informed member - near O(k · a(k)) per group, no repeated rescanning.
// Meetings are generated as long same-timestamp chains (person i meets person i+1)
// specifically so a chain's tail is many hops from its informed head, forcing
// RepeatedRelaxation to actually pay its worst-case multi-pass cost instead of
// resolving in one pass.
[MemoryDiagnoser]
public class FindAllPeopleWithSecretBenchmarks
{
    private const int FirstPerson = 1;
    private const int ChainCount = 2; // one reachable chain, one unreachable chain
    private const int ReservedIdCount = 2; // person 0 + FirstPerson
    private const int UnreachableChainHeadCount = 1; // unreachable chain's own head id

    [Params(50, 400)]
    public int ChainLength;

    private int _peopleCount;
    private (int First, int Second)[][] _timeGroups = null!;

    [GlobalSetup]
    public void Setup()
    {
        // Person 0 and FirstPerson meet at time 0 (LC 2092's own initial condition),
        // then two long same-timestamp chains - one starting at FirstPerson, one
        // starting at an otherwise-unreached person - so both benchmarked methods
        // must actually walk every hop of a ChainLength-long component per group.
        // Id budget: person 0 + FirstPerson (2) + the reachable chain's ChainLength
        // new nodes + the unreachable chain's own head plus its ChainLength new
        // nodes (ChainLength + 1) = 2*ChainLength + 3 distinct ids in [0, _peopleCount).
        _peopleCount = (ChainCount * ChainLength) + ReservedIdCount + UnreachableChainHeadCount;
        var reachableChainStart = ReservedIdCount;
        var unreachableChainStart = ChainLength + reachableChainStart;

        _timeGroups =
        [
            [(0, FirstPerson)],
            BuildChain(FirstPerson, reachableChainStart, ChainLength),
            BuildChain(unreachableChainStart, unreachableChainStart + 1, ChainLength),
        ];
    }

    private static (int First, int Second)[] BuildChain(int headPerson, int chainStart, int chainLength)
    {
        var meetings = new (int First, int Second)[chainLength];
        meetings[0] = (headPerson, chainStart);

        for (var i = 1; i < chainLength; i++)
        {
            meetings[i] = (chainStart + i - 1, chainStart + i);
        }

        // Reversed so the head-connecting edge is scanned LAST within every pass:
        // a single forward scan can then only extend RepeatedRelaxation's known
        // frontier by one hop, forcing its true O(chainLength^2) worst case instead
        // of resolving the whole chain in one linear pass. Union-find's correctness
        // never depends on edge order, so RootMergedWithKeyedDisjointSet is
        // unaffected either way.
        Array.Reverse(meetings);
        return meetings;
    }

    [Benchmark(Baseline = true)]
    public int RepeatedRelaxation()
    {
        var knowsSecret = new bool[_peopleCount];
        knowsSecret[0] = true;
        knowsSecret[FirstPerson] = true;

        foreach (var group in _timeGroups)
        {
            var changed = true;

            while (changed)
            {
                changed = RelaxGroupOnce(group, knowsSecret);
            }
        }

        return CountKnowers(knowsSecret);
    }

    private static bool RelaxGroupOnce((int First, int Second)[] group, bool[] knowsSecret)
    {
        var changed = false;

        foreach (var (first, second) in group)
        {
            if (knowsSecret[first] == knowsSecret[second])
            {
                continue;
            }

            knowsSecret[first] = true;
            knowsSecret[second] = true;
            changed = true;
        }

        return changed;
    }

    [Benchmark]
    public int RootMergedWithKeyedDisjointSet()
    {
        var knowsSecret = new bool[_peopleCount];
        knowsSecret[0] = true;
        knowsSecret[FirstPerson] = true;

        foreach (var group in _timeGroups)
        {
            PropagateWithinTimestamp(group, knowsSecret);
        }

        return CountKnowers(knowsSecret);
    }

    private static void PropagateWithinTimestamp((int First, int Second)[] group, bool[] knowsSecret)
    {
        var participants = CollectParticipants(group);
        var components = BuildUnionFind(group, participants);
        var secretRoots = CollectSecretRoots(participants, knowsSecret, components);
        MarkKnownSecretHolders(participants, knowsSecret, components, secretRoots);
    }

    private static List<int> CollectParticipants((int First, int Second)[] group)
    {
        var participants = new List<int>();
        foreach (var (first, second) in group)
        {
            participants.Add(first);
            participants.Add(second);
        }

        return participants;
    }

    private static KeyedDisjointSet<int> BuildUnionFind((int First, int Second)[] group, List<int> participants)
    {
        var components = new KeyedDisjointSet<int>(participants);

        foreach (var (first, second) in group)
        {
            components.TryUnion(first, second);
        }

        return components;
    }

    private static Set<int> CollectSecretRoots(List<int> participants, bool[] knowsSecret, KeyedDisjointSet<int> components)
    {
        var secretRoots = new Set<int>();
        foreach (var person in participants)
        {
            if (knowsSecret[person] && components.TryFind(person, out var root))
            {
                secretRoots.TryAdd(root);
            }
        }

        return secretRoots;
    }

    private static void MarkKnownSecretHolders(
        List<int> participants, bool[] knowsSecret, KeyedDisjointSet<int> components, Set<int> secretRoots)
    {
        foreach (var person in participants)
        {
            if (components.TryFind(person, out var root) && secretRoots.Has(root))
            {
                knowsSecret[person] = true;
            }
        }
    }

    private static int CountKnowers(bool[] knowsSecret)
    {
        var count = 0;

        foreach (var knows in knowsSecret)
        {
            if (knows)
            {
                count++;
            }
        }

        return count;
    }
}
