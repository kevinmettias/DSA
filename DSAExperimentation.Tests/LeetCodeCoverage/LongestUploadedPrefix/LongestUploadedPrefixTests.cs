using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LongestUploadedPrefix;

// LeetCode 2424. Longest Uploaded Prefix: every Upload just needs "has video been
// seen" membership, so this repo's own Set<int> (backed by HashMap) tracks the
// uploaded videos, and a frontier pointer advances past every already-seen video
// number immediately after each upload. The pointer only ever moves forward, so the
// total work across every call is O(number of uploads) amortized, not O(uploads^2)
// - no ordered/range structure is needed, since the frontier check only ever asks
// about one specific next value at a time.
public sealed partial class LongestUploadedPrefixTests
{
    [Fact]
    public void Upload_LeetCodeExampleSequence_MatchesExpectedLongestValues()
    {
        var server = new LUPrefixOperations();

        server.Upload(3);
        server.Upload(1);
        Assert.Equal(1, server.Longest());

        server.Upload(2);
        Assert.Equal(3, server.Longest());
    }

    [Fact]
    public void Longest_BeforeAnyUpload_ReturnsZero()
    {
        var server = new LUPrefixOperations();

        Assert.Equal(0, server.Longest());
    }

    [Fact]
    public void Upload_WithGapAfterPrefix_StopsAtTheGap()
    {
        var server = new LUPrefixOperations();

        server.Upload(1);
        server.Upload(2);
        server.Upload(4);

        Assert.Equal(2, server.Longest());

        server.Upload(3);

        Assert.Equal(4, server.Longest());
    }

    [Fact]
    public void Upload_SameVideoTwice_DoesNotCorruptTheFrontier()
    {
        var server = new LUPrefixOperations();

        server.Upload(1);
        server.Upload(1);

        Assert.Equal(1, server.Longest());
    }

    // n (the stream's declared capacity in LeetCode's own LUPrefix(int n)) plays no
    // role in this solution's correctness - the frontier advances purely off which
    // video numbers have actually been uploaded - so it is intentionally not stored.
    private sealed class LUPrefixOperations
    {
        private readonly Set<int> _uploaded = new();
        private int _longest;

        public void Upload(int video)
        {
            _uploaded.TryAdd(video);

            while (_uploaded.Has(_longest + 1))
            {
                _longest++;
            }
        }

        public int Longest() => _longest;
    }
}
