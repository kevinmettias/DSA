using RepoQueue = DSAExperimentation.DataStructures.Queue.Queue<int>;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Dota2Senate;

// LeetCode 649. Dota2 Senate: two of this repo's own Queue<int> hold each party's
// senator indices in voting order. Each round, the earlier of the two front indices
// bans the other (that senator never votes again); the winner re-enters its own
// queue at index + n, marking that it votes again only after every senator currently
// queued (from both parties) has had this round's turn - the standard O(n)
// queue-simulation, not an O(n^2) "repeatedly rescan the circle for the next
// unbanned opponent" approach.
public sealed partial class Dota2SenateTests
{
    [Fact]
    public void PredictPartyVictory_RadiantActsFirst_RadiantWins()
        => Assert.Equal("Radiant", PredictPartyVictory("RD"));

    [Fact]
    public void PredictPartyVictory_DireOutnumbersRadiant_DireWins()
        => Assert.Equal("Dire", PredictPartyVictory("RDD"));

    private static string PredictPartyVictory(string senate)
    {
        var (radiant, dire) = BuildPartyQueues(senate);
        SimulateVoting(radiant, dire);
        return radiant.Count > 0 ? "Radiant" : "Dire";
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
