using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.FindAllPeopleWithSecret;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are FindAllPeopleWithSecretSolution's, the same methods
// FindAllPeopleWithSecretTests proves correct. Each arm is handed the prepared
// MeetingSchedule its hoisted overload takes, so bucketing the meetings by
// timestamp is charged to [GlobalSetup] rather than to the propagation being
// measured.
//
// The workload is two long same-timestamp chains - one hanging off the informed
// person, one starting from an otherwise-unreached person - so a chain's tail is
// many hops from its informed head and RepeatedRelaxation has to pay its
// worst-case multi-pass cost instead of resolving in one pass.
[MemoryDiagnoser]
public class FindAllPeopleWithSecretBenchmarks
{
    private const int FirstPerson = 1;
    private const int ChainCount = 2; // one reachable chain, one unreachable chain
    private const int ReservedIdCount = 2; // person 0 + FirstPerson
    private const int UnreachableChainHeadCount = 1; // unreachable chain's own head id
    private const int SeedMeetingTime = 0;
    private const int ReachableChainTime = 1;
    private const int UnreachableChainTime = 2;

    [Params(50, 400)]
    public int ChainLength;

    private MeetingSchedule _schedule;

    [GlobalSetup]
    public void Setup()
    {
        // Person 0 and FirstPerson meet at time 0 (LC 2092's own initial
        // condition), then two long same-timestamp chains at times 1 and 2 - one
        // starting at FirstPerson, one starting at an otherwise-unreached person -
        // so both benchmarked methods must walk every hop of a ChainLength-long
        // component per group. Id budget: person 0 + FirstPerson (2) + the
        // reachable chain's ChainLength new nodes + the unreachable chain's own
        // head plus its ChainLength new nodes (ChainLength + 1) = 2*ChainLength + 3
        // distinct ids in [0, peopleCount).
        var peopleCount = (ChainCount * ChainLength) + ReservedIdCount + UnreachableChainHeadCount;
        var reachableChainStart = ReservedIdCount;
        var unreachableChainStart = ChainLength + reachableChainStart;

        var meetings = new List<(int First, int Second, int Time)> { (0, FirstPerson, SeedMeetingTime) };
        meetings.AddRange(BuildChain(FirstPerson, reachableChainStart, ChainLength, ReachableChainTime));
        meetings.AddRange(
            BuildChain(unreachableChainStart, unreachableChainStart + 1, ChainLength, UnreachableChainTime));

        _schedule = MeetingSchedule.Build(peopleCount, meetings.ToArray(), FirstPerson);
    }

    private static (int First, int Second, int Time)[] BuildChain(
        int headPerson, int chainStart, int chainLength, int time)
    {
        var meetings = new (int First, int Second, int Time)[chainLength];
        meetings[0] = (headPerson, chainStart, time);

        for (var i = 1; i < chainLength; i++)
        {
            meetings[i] = (chainStart + i - 1, chainStart + i, time);
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
    public int[] RepeatedRelaxation() =>
        FindAllPeopleWithSecretSolution.FindAllPeopleByRepeatedRelaxation(_schedule);

    [Benchmark]
    public int[] RootMergedWithKeyedDisjointSet() =>
        FindAllPeopleWithSecretSolution.FindAllPeopleByKeyedDisjointSet(_schedule);
}
