using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EncodeAndDecodeTinyURL;

// LeetCode 535. Encode and Decode TinyURL: a bidirectional codec over this repo's own
// HashMap<TKey,TValue> - one map from short code to original URL for O(1) Decode, a
// second from URL to short code so re-Encoding an already-seen URL returns the same
// short code instead of minting a fresh one every call.
public sealed partial class EncodeAndDecodeTinyURLTests
{
    [Fact]
    public void Decode_AfterEncode_ReturnsOriginalUrl()
    {
        var codec = new Codec();
        const string longUrl = "https://leetcode.com/problems/design-tinyurl";

        var shortUrl = codec.Encode(longUrl);

        Assert.Equal(longUrl, codec.Decode(shortUrl));
    }

    [Fact]
    public void Encode_SameUrlTwice_ReturnsSameShortUrlAndDistinctUrlsGetDistinctCodes()
    {
        var codec = new Codec();
        const string urlA = "https://example.com/a";
        const string urlB = "https://example.com/b";

        var firstShortA = codec.Encode(urlA);
        var secondShortA = codec.Encode(urlA);
        var shortB = codec.Encode(urlB);

        Assert.Equal(firstShortA, secondShortA);
        Assert.NotEqual(firstShortA, shortB);
        Assert.Equal(urlA, codec.Decode(firstShortA));
        Assert.Equal(urlB, codec.Decode(shortB));
    }

    private sealed class Codec
    {
        private readonly HashMap<string, string> _shortToLong = new();
        private readonly HashMap<string, string> _longToShort = new();
        private int _nextId;

        public string Encode(string longUrl)
        {
            if (_longToShort.TryGetValue(longUrl, out var existing))
            {
                return existing;
            }

            var shortUrl = $"http://tinyurl.com/{_nextId++}";
            _shortToLong.Set(shortUrl, longUrl);
            _longToShort.Set(longUrl, shortUrl);
            return shortUrl;
        }

        public string Decode(string shortUrl)
        {
            _shortToLong.TryGetValue(shortUrl, out var longUrl);
            return longUrl!;
        }
    }
}
