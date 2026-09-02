using BenchmarkDotNet.Attributes;
using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Dota2 Senate (LC 649): the naive approach repeatedly re-scans the circle looking
// for the next unbanned opponent to ban - O(n^2) worst case, which the input is
// deliberately shaped to hit: every 'R' seat first, every 'D' seat after, so early
// rounds each skip past nearly the whole opposing block before finding their target
// - vs. this repo's own Queue<int>, used twice (one per party) to hold voting order.
// O(n) overall, each senator enqueued/dequeued at most twice.
[MemoryDiagnoser]
public class Dota2SenateBenchmarks
{
    private const string Radiant = "Radiant";
    private const string Dire = "Dire";

    // Splits SenatorCount in half so the input is exactly one full block of each party.
    private const int PartySplitDivisor = 2;

    [Params(500, 20_000)]
    public int SenatorCount;

    private char[] _senate = null!;

    [GlobalSetup]
    public void Setup()
    {
        _senate = new char[SenatorCount];
        var half = SenatorCount / PartySplitDivisor;

        for (var i = 0; i < SenatorCount; i++)
        {
            _senate[i] = i < half ? 'R' : 'D';
        }
    }

    [Benchmark(Baseline = true)]
    public string CircularRescanSimulation()
    {
        var remainingRadiant = RunCircularRescan();
        return remainingRadiant > 0 ? Radiant : Dire;
    }

    [Benchmark]
    public string TwoQueueSimulation()
    {
        var (radiant, dire) = BuildPartyQueues();
        RunTwoQueueVoting(radiant, dire, _senate.Length);
        return radiant.Count > 0 ? Radiant : Dire;
    }

    private int RunCircularRescan()
    {
        var banned = new bool[_senate.Length];
        var remainingRadiant = _senate.Count(c => c == 'R');
        var remainingDire = _senate.Length - remainingRadiant;
        var i = 0;

        while (remainingRadiant > 0 && remainingDire > 0)
        {
            (remainingRadiant, remainingDire) = AdvanceCircularRescan(banned, i, remainingRadiant, remainingDire);
            i = (i + 1) % _senate.Length;
        }

        return remainingRadiant;
    }

    private (int RemainingRadiant, int RemainingDire) AdvanceCircularRescan(
        bool[] banned, int i, int remainingRadiant, int remainingDire)
    {
        if (banned[i])
        {
            return (remainingRadiant, remainingDire);
        }

        var opponent = NextUnbanned(banned, i, _senate[i]);
        banned[opponent] = true;

        if (_senate[opponent] == 'R')
        {
            remainingRadiant--;
        }
        else
        {
            remainingDire--;
        }

        return (remainingRadiant, remainingDire);
    }

    private (RepoQueue Radiant, RepoQueue Dire) BuildPartyQueues()
    {
        var radiant = new RepoQueue();
        var dire = new RepoQueue();

        for (var idx = 0; idx < _senate.Length; idx++)
        {
            if (_senate[idx] == 'R')
            {
                radiant.Enqueue(idx);
            }
            else
            {
                dire.Enqueue(idx);
            }
        }

        return (radiant, dire);
    }

    private static void RunTwoQueueVoting(RepoQueue radiant, RepoQueue dire, int n)
    {
        while (radiant.Count > 0 && dire.Count > 0)
        {
            CastVote(radiant, dire, n);
        }
    }

    private static void CastVote(RepoQueue radiant, RepoQueue dire, int n)
    {
        radiant.TryDequeue(out var radiantIndex);
        dire.TryDequeue(out var direIndex);

        if (radiantIndex < direIndex)
        {
            radiant.Enqueue(radiantIndex + n);
        }
        else
        {
            dire.Enqueue(direIndex + n);
        }
    }

    private int NextUnbanned(bool[] banned, int from, char actingParty)
    {
        var j = (from + 1) % _senate.Length;

        while (banned[j] || _senate[j] == actingParty)
        {
            j = (j + 1) % _senate.Length;
        }

        return j;
    }
}
