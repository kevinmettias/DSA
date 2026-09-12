using static DSAExperimentation.LeetCode.EncodeAndDecodeTinyURL.EncodeAndDecodeTinyURLSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EncodeAndDecodeTinyURL;

// Harness only. Both strategies are EncodeAndDecodeTinyURLSolution's - this file
// replays LeetCode's published round-trip and dedup scenarios against each
// ICodecStrategy implementation, so a failure names the strategy that broke.
public sealed class EncodeAndDecodeTinyURLTests
{
    public static TheoryData<string> RoundTripExamples =>
        new() { "https://leetcode.com/problems/design-tinyurl" };

    public static TheoryData<string, string> DistinctUrlExamples =>
        new() { { "https://example.com/a", "https://example.com/b" } };

    [Theory]
    [MemberData(nameof(RoundTripExamples))]
    public void CodecByLinearScan_Decode_AfterEncode_ReturnsOriginalUrl(string longUrl) =>
        AssertRoundTrips(new CodecByLinearScan(), longUrl);

    [Theory]
    [MemberData(nameof(RoundTripExamples))]
    public void CodecByHashMap_Decode_AfterEncode_ReturnsOriginalUrl(string longUrl) =>
        AssertRoundTrips(new CodecByHashMap(), longUrl);

    [Theory]
    [MemberData(nameof(DistinctUrlExamples))]
    public void CodecByLinearScan_Encode_SameUrlTwice_ReturnsSameShortUrlAndDistinctUrlsGetDistinctCodes(
        string urlA, string urlB) =>
        AssertSameUrlDedupesAndDistinctUrlsDiffer(new CodecByLinearScan(), urlA, urlB);

    [Theory]
    [MemberData(nameof(DistinctUrlExamples))]
    public void CodecByHashMap_Encode_SameUrlTwice_ReturnsSameShortUrlAndDistinctUrlsGetDistinctCodes(
        string urlA, string urlB) =>
        AssertSameUrlDedupesAndDistinctUrlsDiffer(new CodecByHashMap(), urlA, urlB);

    private static void AssertRoundTrips(ICodecStrategy codec, string longUrl)
    {
        var shortUrl = codec.Encode(longUrl);

        Assert.Equal(longUrl, codec.Decode(shortUrl));
    }

    private static void AssertSameUrlDedupesAndDistinctUrlsDiffer(
        ICodecStrategy codec, string urlA, string urlB)
    {
        var firstShortA = codec.Encode(urlA);
        var secondShortA = codec.Encode(urlA);
        var shortB = codec.Encode(urlB);

        Assert.Equal(firstShortA, secondShortA);
        Assert.NotEqual(firstShortA, shortB);
        Assert.Equal(urlA, codec.Decode(firstShortA));
        Assert.Equal(urlB, codec.Decode(shortB));
    }
}
