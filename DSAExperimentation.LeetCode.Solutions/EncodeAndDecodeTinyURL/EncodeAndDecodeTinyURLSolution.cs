using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.Solutions.EncodeAndDecodeTinyURL;

// LeetCode 535. Encode and Decode TinyURL: a bidirectional codec - Decode must be
// O(1) per LC's own follow-up, and re-Encoding an already-seen URL must return the
// same short code instead of minting a fresh one every call. A Design problem's
// whole point is a sequence of mutating calls against one instance, so "every
// strategy for the problem" (ARCHITECTURE.md 17.3) takes the form of two classes
// implementing the shared ICodecStrategy surface below.
//
// LeetCode leaves the short form free and judges only the round trip: its judge
// makes one codec, encodes the url, decodes that short url and prints the result,
// which must be the url it started from. RoundTripBy<Strategy> is that judge over
// each class. Both mint short codes from a counter rather than at random, so a
// replay gives the same short url every time.
[LeetCodeQuestion("encode-and-decode-tinyurl", LeetCodeAnswerOrder.AsWritten)]
internal static class EncodeAndDecodeTinyURLSolution
{
    public static string RoundTripByLinearScan(string url)
    {
        var codec = new CodecByLinearScan();
        var shortUrl = codec.Encode(url);
        return codec.Decode(shortUrl);
    }

    public static string RoundTripByHashMap(string url)
    {
        var codec = new CodecByHashMap();
        var shortUrl = codec.Encode(url);
        return codec.Decode(shortUrl);
    }

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either strategy without
    // restating it.
    internal interface ICodecStrategy
    {
        string Encode(string longUrl);

        string Decode(string shortUrl);
    }

    // The textbook answer: every (short, long) pair kept in a flat BCL List,
    // matched by a linear scan in both directions - deliberately without this
    // repo's HashMap, the arm CodecByHashMap has to justify itself against.
    internal sealed class CodecByLinearScan : ICodecStrategy
    {
        private readonly List<(string Short, string Long)> _pairs = [];
        private int _nextId;

        public string Encode(string longUrl)
        {
            foreach (var (existingShort, existingLong) in _pairs)
            {
                if (existingLong == longUrl)
                {
                    return existingShort;
                }
            }

            var shortUrl = $"http://tinyurl.com/{_nextId++}";
            _pairs.Add((shortUrl, longUrl));
            return shortUrl;
        }

        public string Decode(string shortUrl)
        {
            foreach (var (existingShort, existingLong) in _pairs)
            {
                if (existingShort == shortUrl)
                {
                    return existingLong;
                }
            }

            return string.Empty;
        }
    }

    // This repo's own HashMap<TKey,TValue>: one map from short code to original
    // URL for O(1) Decode, a second from URL to short code so re-Encoding an
    // already-seen URL returns the same short code in O(1) instead of scanning
    // every pair recorded so far.
    internal sealed class CodecByHashMap : ICodecStrategy
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
