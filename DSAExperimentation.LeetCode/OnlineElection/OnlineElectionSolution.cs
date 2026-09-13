using DSAExperimentation.Algorithms.Searching;
using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Sequence;

namespace DSAExperimentation.LeetCode.OnlineElection;

// LeetCode 911. Online Election: TopVotedCandidate(persons, times) is constructed
// from a chronological vote log, then q(t) reports who was leading at time t. A tie
// is won by whichever candidate voted most recently, so a running count that merely
// ties the leader still takes the lead (>=, not >).
//
// The design-problem framing is a stream of q(t) calls; the answer that stream
// produces is the sequence of leaders, so both strategies here take the whole query
// stream and return its answers in order - the shape that can be asserted and
// measured against itself.
//
// LeadersByPerQueryRescan re-tallies the vote log from scratch for every query:
// O(n) per query, O(n * q) overall. LeadersByPrecomputedBinarySearch instead builds
// a parallel "leader after vote i" array once, using this repo's own
// HashMap<int, int> for the running counts, and then answers each query with this
// repo's own BinarySearch.UpperBound over an ArraySequence<int> of the vote times -
// the index just past the last vote cast at or before t, minus one. O(n)
// preprocessing plus O(log n) per query.
internal static class OnlineElectionSolution
{
    // The textbook answer: no preprocessing, just count the votes again for each
    // query. Deliberately written without this repo's primitives - it is the arm
    // the composed solution below has to justify itself against.
    public static int[] LeadersByPerQueryRescan(int[] persons, int[] times, int[] queries)
    {
        var leaders = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            leaders[i] = LeaderAtOrBefore(persons, times, queries[i]);
        }

        return leaders;
    }

    // Re-counts votes from scratch over [0, queryTime] and returns the resulting
    // leader. BCL Dictionary on purpose: this is what the tally looks like without
    // this repo.
    private static int LeaderAtOrBefore(int[] persons, int[] times, int queryTime)
    {
        var votes = new Dictionary<int, int>();
        var leader = LeetCodeAnswer.None;
        var leaderVotes = 0;

        for (var i = 0; i < times.Length && times[i] <= queryTime; i++)
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

    public static int[] LeadersByPrecomputedBinarySearch(int[] persons, int[] times, int[] queries)
    {
        var leaderAfterVote = LeaderAfterEachVote(persons);
        var voteTimes = new ArraySequence<int>(times);
        var leaders = new int[queries.Length];

        for (var i = 0; i < queries.Length; i++)
        {
            var index = BinarySearch.UpperBound(voteTimes, queries[i]) - 1;
            leaders[i] = leaderAfterVote[index];
        }

        return leaders;
    }

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
}
