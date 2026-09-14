using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.MaximumEmployeesToBeInvitedToAMeeting;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MaximumEmployeesToBeInvitedToAMeetingSolution's, the
// same methods MaximumEmployeesToBeInvitedToAMeetingTests proves correct. The
// manual peel is handed LeetCode's own int[] favorite, and the composed arm the
// prepared EmployeeGraph its hoisted overload takes, so building the node graph is
// charged to [GlobalSetup] rather than to the search being measured.
//
// The workload is several mutual (2-cycle) pairs plus one length-5 cycle, with
// every remaining node chaining into an earlier node, so both strategies exercise
// the peel, the multi-cycle walk and the chain-length DP together rather than
// short-circuiting on an all-cycle or all-chain input.
[MemoryDiagnoser]
public class MaximumEmployeesToBeInvitedToAMeetingBenchmarks
{
    private const int TwoCyclePairCount = 3; // several separate mutual pairs, so bonuses sum across pairs
    private const int LongCycleLength = 5; // one cycle length >= 3, to exercise the "longest cycle" branch
    private const int MutualPairSize = 2; // a mutual pair occupies two consecutive ids in the workload

    // The deterministic chain seed this benchmark has always used.
    private const int ChainSeed = 1;

    [Params(200, 5_000)]
    public int NodeCount;

    private int[] _favorite = null!;
    private EmployeeGraph _graph;

    [GlobalSetup]
    public void Setup()
    {
        _favorite = BuildFavorites(NodeCount);
        _graph = EmployeeGraph.Build(_favorite);
    }

    [Benchmark(Baseline = true)]
    public int ManualPeelAndCycleWalk() =>
        MaximumEmployeesToBeInvitedToAMeetingSolution.MaximumInvitedByManualPeelAndCycleWalk(_favorite);

    [Benchmark]
    public int GraphPrimitiveComposition() =>
        MaximumEmployeesToBeInvitedToAMeetingSolution.MaximumInvitedByGraphPrimitiveComposition(_graph);

    private static int[] BuildFavorites(int nodeCount)
    {
        var favorite = new int[nodeCount];
        var idx = AddTwoCyclePairs(favorite);
        idx = AddLongCycle(favorite, idx);
        AddRandomChains(favorite, idx);

        return favorite;
    }

    private static int AddTwoCyclePairs(int[] favorite)
    {
        var idx = 0;
        for (var p = 0; p < TwoCyclePairCount; p++)
        {
            favorite[idx] = idx + 1;
            favorite[idx + 1] = idx;
            idx += MutualPairSize;
        }

        return idx;
    }

    private static int AddLongCycle(int[] favorite, int idx)
    {
        var longCycleStart = idx;
        for (var i = 0; i < LongCycleLength; i++)
        {
            favorite[longCycleStart + i] = longCycleStart + ((i + 1) % LongCycleLength);
        }

        return idx + LongCycleLength;
    }

    private static void AddRandomChains(int[] favorite, int idx)
    {
        var random = new Random(ChainSeed);
        for (var i = idx; i < favorite.Length; i++)
        {
            favorite[i] = random.Next(i); // a uniformly random strictly-earlier node
        }
    }
}
