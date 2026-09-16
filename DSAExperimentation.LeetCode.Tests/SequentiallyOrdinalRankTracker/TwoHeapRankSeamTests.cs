using DSAExperimentation.LeetCode.SequentiallyOrdinalRankTracker;

namespace DSAExperimentation.LeetCode.Tests.SequentiallyOrdinalRankTracker;

// The seam between SequentiallyOrdinalRankTrackerSolution's two trackers.
// CreateByResortEveryGet re-sorts everything seen so far on each query.
// CreateByTwoHeaps composes two DataStructures.Heaps - a max-heap whose root is the
// worst of the current best, and a min-heap whose root is the best of the rest -
// and keeps them balanced so no query ever sorts.
//
// The balance is the whole contract, and it is only visible across calls: each Add
// pushes then immediately demotes one element to the other heap, and each Get
// promotes one back, so the pair only answers correctly when the two heaps have
// moved in lockstep since the first call. A track that is right after one Add can
// still be wrong at the tenth query, which is why every test below replays a full
// Add/Get script against both and compares the whole transcript.
public sealed partial class TwoHeapRankSeamTests
{
    [Fact]
    public void Get_InterleavedWithAdds_MatchesResortEveryGet()
    {
        RankOperation[] script =
        [
            RankOperation.Add("brazil", 2),
            RankOperation.Add("bradford", 3),
            RankOperation.Add("paris", 2),
            RankOperation.Ask(),
            RankOperation.Add("rome", 4),
            RankOperation.Ask(),
            RankOperation.Add("oslo", 1),
            RankOperation.Ask(),
        ];

        Assert.Equal(["bradford", "bradford", "brazil"], Replay(script, useTwoHeaps: true));
        AssertSameTranscript(script);
    }

    // Equal scores are broken by the lexicographically smaller name, so the two
    // heaps' order witnesses have to agree about a tie as well as about a score -
    // and the moment a tie is broken wrongly the heaps demote the wrong element.
    [Fact]
    public void Get_TiedScores_BreaksTheTieByAscendingNameOnBothArms()
    {
        RankOperation[] script =
        [
            RankOperation.Add("delta", 5),
            RankOperation.Add("alpha", 5),
            RankOperation.Add("charlie", 5),
            RankOperation.Ask(),
            RankOperation.Ask(),
            RankOperation.Ask(),
        ];

        Assert.Equal(["alpha", "charlie", "delta"], Replay(script, useTwoHeaps: true));
        AssertSameTranscript(script);
    }

    // More queries than adds before the next add: every Get promotes one element in,
    // so the two heaps must stay balanced across a run of queries with no add between
    // them.
    [Fact]
    public void Get_RunOfQueriesWithoutAdds_MatchesResortEveryGet()
    {
        RankOperation[] script =
        [
            RankOperation.Add("a", 9),
            RankOperation.Add("b", 8),
            RankOperation.Add("c", 7),
            RankOperation.Add("d", 6),
            RankOperation.Ask(),
            RankOperation.Ask(),
            RankOperation.Ask(),
            RankOperation.Ask(),
        ];

        Assert.Equal(["a", "b", "c", "d"], Replay(script, useTwoHeaps: true));
        AssertSameTranscript(script);
    }

    // One location and one query: the heaps hold a single element between them, so
    // every push and every pop is on a structure that has no other entry to fall
    // back on.
    [Fact]
    public void Get_SingleLocation_MatchesResortEveryGet()
    {
        RankOperation[] script = [RankOperation.Add("only", 1), RankOperation.Ask()];

        Assert.Equal(["only"], Replay(script, useTwoHeaps: true));
        AssertSameTranscript(script);
    }

    // Ascending scores: every new location is the best seen so far, so each query
    // returns the newest one - the case where a demote that let the new element stay
    // on the wrong heap would be invisible until the next query.
    [Fact]
    public void Get_AscendingScores_MatchesResortEveryGet()
    {
        RankOperation[] script =
        [
            RankOperation.Add("a", 1),
            RankOperation.Add("b", 2),
            RankOperation.Ask(),
            RankOperation.Add("c", 3),
            RankOperation.Ask(),
            RankOperation.Add("d", 4),
            RankOperation.Ask(),
            RankOperation.Ask(),
        ];

        AssertSameTranscript(script);
    }

    // Descending scores: every new location is worse than everything already seen, so
    // the demote path runs on every add and the promotion on every query walks back
    // through what was demoted.
    [Fact]
    public void Get_DescendingScores_MatchesResortEveryGet()
    {
        RankOperation[] script =
        [
            RankOperation.Add("a", 40),
            RankOperation.Add("b", 30),
            RankOperation.Ask(),
            RankOperation.Add("c", 20),
            RankOperation.Ask(),
            RankOperation.Add("d", 10),
            RankOperation.Ask(),
            RankOperation.Ask(),
        ];

        AssertSameTranscript(script);
    }

    private static void AssertSameTranscript(RankOperation[] script)
        => Assert.Equal(Replay(script, useTwoHeaps: false), Replay(script, useTwoHeaps: true));

    private static List<string> Replay(RankOperation[] script, bool useTwoHeaps)
    {
        var tracker = useTwoHeaps
            ? SequentiallyOrdinalRankTrackerSolution.CreateByTwoHeaps()
            : SequentiallyOrdinalRankTrackerSolution.CreateByResortEveryGet();
        var transcript = new List<string>();

        foreach (var operation in script)
        {
            if (operation.IsAdd)
            {
                tracker.Add(operation.Name, operation.Score);
                continue;
            }

            transcript.Add(tracker.Get());
        }

        return transcript;
    }

    private readonly record struct RankOperation(bool IsAdd, string Name, int Score)
    {
        public static RankOperation Add(string name, int score) => new(IsAdd: true, name, score);

        public static RankOperation Ask() => new(IsAdd: false, string.Empty, Score: 0);
    }
}
