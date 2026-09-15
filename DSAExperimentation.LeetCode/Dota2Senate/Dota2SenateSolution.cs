using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.LeetCode.Dota2Senate;

// LeetCode 649. Dota2 Senate: each round the earlier-indexed senator of the two
// facing off bans the other; a senator who survives votes again only after every
// senator currently in play (both parties) has had this round's turn. Predict which
// party is left when the other has no senators remaining.
//
// The two strategies differ in how a banned senator's "next unbanned opponent" is
// found - repeatedly rescanning the circle with a BCL bool[] of banned flags, vs.
// this repo's own Queue<int>, one per party, where re-enqueuing the winner at
// index + n both records the ban and schedules the next round in one step.
internal static class Dota2SenateSolution
{
    private const string Radiant = "Radiant";
    private const string Dire = "Dire";

    // The textbook answer: a BCL bool[] of banned flags, re-scanning the circle for
    // the next unbanned opponent every time a senator acts. O(n^2) worst case.
    public static string PredictPartyVictoryByCircularRescan(string senate)
    {
        var banned = new bool[senate.Length];
        var remainingRadiant = senate.Count(c => c == 'R');
        var remainingDire = senate.Length - remainingRadiant;
        var i = 0;

        while (remainingRadiant > 0 && remainingDire > 0)
        {
            (remainingRadiant, remainingDire) =
                AdvanceCircularRescan(senate, banned, i, (remainingRadiant, remainingDire));
            i = (i + 1) % senate.Length;
        }

        return remainingRadiant > 0 ? Radiant : Dire;
    }

    // The two parties' remaining counts are one tally, not two arguments: every path
    // either returns them unchanged or decrements exactly one of them.
    private static (int RemainingRadiant, int RemainingDire) AdvanceCircularRescan(
        string senate, bool[] banned, int i, (int Radiant, int Dire) remaining)
    {
        var (remainingRadiant, remainingDire) = remaining;

        if (banned[i])
        {
            return (remainingRadiant, remainingDire);
        }

        var opponent = NextUnbanned(senate, banned, i, senate[i]);
        banned[opponent] = true;

        if (senate[opponent] == 'R')
        {
            remainingRadiant--;
        }
        else
        {
            remainingDire--;
        }

        return (remainingRadiant, remainingDire);
    }

    private static int NextUnbanned(string senate, bool[] banned, int from, char actingParty)
    {
        var j = (from + 1) % senate.Length;

        while (banned[j] || senate[j] == actingParty)
        {
            j = (j + 1) % senate.Length;
        }

        return j;
    }

    // This repo's own Queue<int>, one per party, holding each party's senator
    // indices in voting order. O(n) overall: each senator is enqueued/dequeued at
    // most twice. The queues are drained by the simulation, so - unlike a read-only
    // graph - there is nothing worth hoisting into a second overload: rebuilding
    // them per call is the only correct option.
    public static string PredictPartyVictoryByTwoQueueSimulation(string senate)
    {
        var (radiant, dire) = BuildPartyQueues(senate);
        SimulateVoting(radiant, dire);
        return radiant.Count > 0 ? Radiant : Dire;
    }

    private static (RepoQueue Radiant, RepoQueue Dire) BuildPartyQueues(string senate)
    {
        var radiant = new RepoQueue();
        var dire = new RepoQueue();

        for (var i = 0; i < senate.Length; i++)
        {
            if (senate[i] == 'R')
            {
                radiant.Enqueue(i);
            }
            else
            {
                dire.Enqueue(i);
            }
        }

        return (radiant, dire);
    }

    private static void SimulateVoting(RepoQueue radiant, RepoQueue dire)
    {
        var n = radiant.Count + dire.Count;

        while (radiant.Count > 0 && dire.Count > 0)
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
    }
}
