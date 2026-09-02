using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.Tests.LeetCodeCoverage.OnlineElection;

// LeetCode 911. Online Election: the constructor preprocesses once into a parallel
// "leader at index i" array using this repo's own HashMap<int,int> for running vote
// counts (a tie is won by whichever candidate just voted, so >= - not > - decides
// the new leader). Query(t) then only needs the index of the last vote cast at or
// before t, found via this repo's own BinarySearch.UpperBound over an
// ArraySequence<int> of the times array minus one - O(log n) per query instead of a
// fresh O(n) rescan of every vote up to t.
public sealed partial class OnlineElectionTests
{
    [Fact]
    public void Query_LeetCodeExampleSequence_ReturnsLeaderAtEachQueriedTime()
    {
        var election = new TopVotedCandidate([0, 1, 1, 0, 0, 1, 0], [0, 5, 10, 15, 20, 25, 30]);

        AssertLeaderAt(election, 3, 0);
        AssertLeaderAt(election, 12, 1);
        AssertLeaderAt(election, 25, 1);
        AssertLeaderAt(election, 15, 0);
        AssertLeaderAt(election, 24, 0);
        AssertLeaderAt(election, 8, 1);
    }

    [Fact]
    public void Query_TieGoesToTheJustCastVote()
    {
        var election = new TopVotedCandidate([0, 1], [0, 1]);

        AssertLeaderAt(election, 1, 1);
    }

    private static void AssertLeaderAt(TopVotedCandidate election, int time, int expectedLeader)
    {
        var actual = election.Query(time);
        Assert.Equal(expectedLeader, actual);
    }

    private sealed class TopVotedCandidate
    {
        private readonly int[] _leaders;
        private readonly ArraySequence<int> _times;

        public TopVotedCandidate(int[] persons, int[] times)
        {
            _times = new ArraySequence<int>(times);
            _leaders = new int[persons.Length];

            var votes = new HashMap<int, int>();
            var leader = -1;
            var leaderVotes = 0;

            for (var i = 0; i < persons.Length; i++)
            {
                votes.TryGetValue(persons[i], out var count);
                count++;
                votes.Set(persons[i], count);

                if (count >= leaderVotes)
                {
                    leader = persons[i];
                    leaderVotes = count;
                }

                _leaders[i] = leader;
            }
        }

        public int Query(int t)
        {
            var index = BinarySearch.UpperBound(_times, t) - 1;
            return _leaders[index];
        }
    }
}
