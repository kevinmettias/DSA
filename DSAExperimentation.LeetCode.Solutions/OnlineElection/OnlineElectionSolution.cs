using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.Solutions.OnlineElection;

// LeetCode 911. Online Election: TopVotedCandidate(persons, times) is constructed
// from a chronological vote log, then q(t) reports who was leading at time t. A tie
// is won by whichever candidate voted most recently, so a running count that merely
// ties the leader still takes the lead (>=, not >).
//
// This is a design problem - LeetCode's own shape is a stateful object built from
// the vote log and answering one q(t) at a time - so each strategy is a factory
// taking persons and times and returning an ITopVotedCandidate, the object the
// catalog harness replays a call script on (ARCHITECTURE.md §17.13). A stream of
// queries is replayed by the harness, one call at a time.
//
// CreateByPerQueryRescan re-tallies the vote log from scratch for every query:
// O(n) per query, O(n * q) overall. CreateByPrecomputedBinarySearch instead builds
// a parallel "leader after vote i" array once, using this repo's own
// HashMap<int, int> for the running counts, and then answers each query with this
// repo's own BinarySearch.UpperBound over the vote times - the index just past the
// last vote cast at or before t, minus one. O(n) preprocessing plus O(log n) per
// query.
[LeetCodeQuestion("online-election", LeetCodeAnswerOrder.AsWritten)]
internal static class OnlineElectionSolution
{
    // The textbook answer: no preprocessing, just count the votes again for each
    // query. Deliberately written without this repo's primitives - it is the arm
    // the composed solution below has to justify itself against.
    public static ITopVotedCandidate CreateByPerQueryRescan(int[] persons, int[] times) =>
        new PerQueryRescanCandidate(persons, times);

    public static ITopVotedCandidate CreateByPrecomputedBinarySearch(int[] persons, int[] times) =>
        new PrecomputedBinarySearchCandidate(persons, times);

    // LeetCode's TopVotedCandidate: q answers who led the election at time t, every
    // vote cast at t counted.
    internal interface ITopVotedCandidate
    {
        int Q(int time);
    }

    // The vote log as LeetCode gives it, tallied afresh for every query.
    private sealed class PerQueryRescanCandidate(int[] persons, int[] times) : ITopVotedCandidate
    {
        // Re-counts votes from scratch over [0, t] and returns the resulting leader.
        // BCL Dictionary on purpose: this is what the tally looks like without this
        // repo.
        public int Q(int time)
        {
            var votes = new Dictionary<int, int>();
            var leader = LeetCodeAnswer.None;
            var leaderVotes = 0;

            for (var i = 0; i < times.Length && times[i] <= time; i++)
            {
                votes.TryGetValue(persons[i], out var count);
                count++;
                votes[persons[i]] = count;

                if (count >= leaderVotes)
                {
                    leader = persons[i];
                    leaderVotes = count;
                }
            }

            return leader;
        }
    }

    // The leader after each vote, worked out once when it is built, and the vote
    // times a query searches for the last vote it counts.
    private sealed class PrecomputedBinarySearchCandidate(int[] persons, int[] times) : ITopVotedCandidate
    {
        private readonly int[] _leaderAfterVote = LeaderAfterEachVote(persons);

        // One forward pass over the log: each vote updates its candidate's running
        // count, and a count that reaches the leader's own count takes the lead,
        // because the newer vote breaks the tie.
        private static int[] LeaderAfterEachVote(int[] persons)
        {
            var leaderAfterVote = new int[persons.Length];
            var votes = new HashMap<int, int>();
            var leader = LeetCodeAnswer.None;
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

                leaderAfterVote[i] = leader;
            }

            return leaderAfterVote;
        }

        public int Q(int time)
        {
            var lastVoteCounted = BinarySearch.UpperBound(times, time) - 1;

            return _leaderAfterVote[lastVoteCounted];
        }
    }
}
