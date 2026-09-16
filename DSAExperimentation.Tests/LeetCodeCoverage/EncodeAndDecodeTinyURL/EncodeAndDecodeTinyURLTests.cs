using DSAExperimentation.LeetCode.EncodeAndDecodeTinyURL;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EncodeAndDecodeTinyURL;

// Harness only. Both strategies are EncodeAndDecodeTinyURLSolution's - this file
// replays LeetCode's published round-trip scenario and its dedup scenario against
// each ICodecStrategy implementation, so a failure names the strategy that broke.
public sealed class EncodeAndDecodeTinyURLTests
{
    public static TheoryData<CodecScenario> Examples =>
        new()
        {
            {
                new CodecScenario(
                    LongUrl: "https://leetcode.com/problems/design-tinyurl",
                    DistinctUrl: "https://leetcode.com/problems/two-sum")
            },
            {
                new CodecScenario(
                    LongUrl: "https://example.com/a",
                    DistinctUrl: "https://example.com/b")
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CodecByLinearScan_Decode_AfterEncode_ReturnsOriginalUrl(CodecScenario scenario) =>
        AssertRoundTrips(new EncodeAndDecodeTinyURLSolution.CodecByLinearScan(), scenario.LongUrl);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CodecByHashMap_Decode_AfterEncode_ReturnsOriginalUrl(CodecScenario scenario) =>
        AssertRoundTrips(new EncodeAndDecodeTinyURLSolution.CodecByHashMap(), scenario.LongUrl);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CodecByLinearScan_Encode_SameUrlTwice_ReturnsSameShortUrlAndDistinctUrlsGetDistinctCodes(
        CodecScenario scenario) =>
        AssertSameUrlDedupesAndDistinctUrlsDiffer(new EncodeAndDecodeTinyURLSolution.CodecByLinearScan(), scenario);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CodecByHashMap_Encode_SameUrlTwice_ReturnsSameShortUrlAndDistinctUrlsGetDistinctCodes(
        CodecScenario scenario) =>
        AssertSameUrlDedupesAndDistinctUrlsDiffer(new EncodeAndDecodeTinyURLSolution.CodecByHashMap(), scenario);

    private static void AssertRoundTrips(EncodeAndDecodeTinyURLSolution.ICodecStrategy codec, string longUrl)
    {
        var shortUrl = codec.Encode(longUrl);

        Assert.Equal(longUrl, codec.Decode(shortUrl));
    }

    private static void AssertSameUrlDedupesAndDistinctUrlsDiffer(
        EncodeAndDecodeTinyURLSolution.ICodecStrategy codec, CodecScenario scenario)
    {
        var firstShort = codec.Encode(scenario.LongUrl);
        var secondShort = codec.Encode(scenario.LongUrl);
        var distinctShort = codec.Encode(scenario.DistinctUrl);

        Assert.Equal(firstShort, secondShort);
        Assert.NotEqual(firstShort, distinctShort);
        Assert.Equal(scenario.LongUrl, codec.Decode(firstShort));
        Assert.Equal(scenario.DistinctUrl, codec.Decode(distinctShort));
    }

    // One scenario: the URL that must survive a round trip and is encoded twice, and a
    // different URL that must come back a different code. Both positions are `string` and the
    // relation between them is not symmetric, so each is named rather than left
    // interchangeable.
    public readonly record struct CodecScenario(string LongUrl, string DistinctUrl);
}
