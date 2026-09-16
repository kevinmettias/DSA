using System.Globalization;
using DSAExperimentation.LeetCode.EncryptAndDecryptStrings;
using IEncrypter = DSAExperimentation.LeetCode.EncryptAndDecryptStrings.EncryptAndDecryptStringsSolution.IEncrypter;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EncryptAndDecryptStrings;

// Harness only: both strategies live in EncryptAndDecryptStringsSolution,
// including the re-encrypt-the-dictionary-per-query baseline the pre-migration
// benchmark kept to itself. LeetCode's own shape here is a stateful object across
// a sequence of calls, so EncrypterScript encodes the constructor's three arguments
// plus a call script instead of a single argument tuple - the same shape
// DesignBitsetTests uses for its own instance-API problem. EncrypterCall.Apply
// renders each result as the string LeetCode's judge output shows for it, so one
// expected value per call covers both encrypt's string and decrypt's count.
//
// The solution class and its IEncrypter are named at each use rather than pulled in
// by a `using static`: a wildcard import drops every member in as a bare identifier,
// so a reader meeting CreateByPrecomputedFrequency has nothing on the line telling
// them whose it is.
public sealed partial class EncryptAndDecryptStringsTests
{
    public static TheoryData<EncrypterScript> Examples =>
        new()
        {
            // LeetCode's published example. 'a' and 'c' both map to "ei", so
            // "abcd" and "abad" encrypt identically and decrypt reports 2.
            {
                new EncrypterScript(
                    Keys: ['a', 'b', 'c', 'd'],
                    Values: ["ei", "zf", "ei", "am"],
                    Dictionary: ["abcd", "acbd", "adbc", "badc", "dacb", "cadb", "cbda", "abad"],
                    Calls:
                    [
                        EncrypterCall.Encrypt("abcd"),
                        EncrypterCall.Decrypt("eizfeiam"),
                    ],
                    Expected: ["eizfeiam", "2"])
            },

            // No dictionary word encrypts to this, even though every two-letter
            // group in it is a legal value.
            {
                new EncrypterScript(
                    Keys: ['a', 'b', 'c', 'd'],
                    Values: ["ei", "zf", "ei", "am"],
                    Dictionary: ["abcd", "acbd", "adbc", "badc", "dacb", "cadb", "cbda", "abad"],
                    Calls: [EncrypterCall.Decrypt("zfzfzfzf")],
                    Expected: ["0"])
            },

            // A character outside keys cannot be encrypted at all, so encrypt
            // reports the empty string - LeetCode's own stated behaviour.
            {
                new EncrypterScript(
                    Keys: ['a', 'b', 'c', 'd'],
                    Values: ["ei", "zf", "ei", "am"],
                    Dictionary: ["abcd"],
                    Calls: [EncrypterCall.Encrypt("abce"), EncrypterCall.Encrypt("dcba")],
                    Expected: ["", "ameizfei"])
            },

            // Duplicate values with single-character dictionary words: "ei" is the
            // encryption of both "a" and "b", so it decrypts to 2, while "eiei"
            // matches only "ab" - "aa", "ba" and "bb" are not in the dictionary.
            {
                new EncrypterScript(
                    Keys: ['a', 'b'],
                    Values: ["ei", "ei"],
                    Dictionary: ["a", "b", "ab"],
                    Calls:
                    [
                        EncrypterCall.Encrypt("a"),
                        EncrypterCall.Encrypt("ab"),
                        EncrypterCall.Decrypt("ei"),
                        EncrypterCall.Decrypt("eiei"),
                    ],
                    Expected: ["ei", "eiei", "2", "1"])
            },

            // Nothing in word2 is a value at all, and its length is odd besides -
            // no dictionary word can encrypt to it.
            {
                new EncrypterScript(
                    Keys: ['a', 'b'],
                    Values: ["ei", "zf"],
                    Dictionary: ["a", "b", "ab", "ba"],
                    Calls: [EncrypterCall.Decrypt("xyx"), EncrypterCall.Decrypt("zfei")],
                    Expected: ["0", "1"])
            },

            // Repeats in the dictionary count once each, so the same word listed
            // twice makes its encryption decrypt to 2.
            {
                new EncrypterScript(
                    Keys: ['a', 'b'],
                    Values: ["ei", "zf"],
                    Dictionary: ["ab", "ab", "ba"],
                    Calls: [EncrypterCall.Decrypt("eizf"), EncrypterCall.Decrypt("zfei")],
                    Expected: ["2", "1"])
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByDictionaryRescan_LeetCodeExamples_MatchesExpectedSequence(EncrypterScript script)
    {
        var encrypter = EncryptAndDecryptStringsSolution.CreateByDictionaryRescan(
            script.Keys, script.Values, script.Dictionary);

        Assert.Equal(script.Expected, RunScript(encrypter, script.Calls));
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByPrecomputedFrequency_LeetCodeExamples_MatchesExpectedSequence(EncrypterScript script)
    {
        var encrypter = EncryptAndDecryptStringsSolution.CreateByPrecomputedFrequency(
            script.Keys, script.Values, script.Dictionary);

        Assert.Equal(script.Expected, RunScript(encrypter, script.Calls));
    }

    private static string[] RunScript(IEncrypter encrypter, EncrypterCall[] calls) =>
        [.. calls.Select(call => call.Apply(encrypter))];

    // One LeetCode example: the encrypter's constructor arguments, the calls to replay
    // against it, and the judge output for each call. Every argument is named where it
    // is passed, because three arrays in a row say nothing about which is the keys, the
    // values and the dictionary. Nested because it is only ever used inside this test
    // class - it is this harness's own vocabulary, not a type another file would import.
    public readonly record struct EncrypterScript(
        char[] Keys, string[] Values, string[] Dictionary, EncrypterCall[] Calls, string[] Expected);

    // One call in an Encrypter script: which of the two operations to invoke and on
    // what word. Pure dispatch, built via the named factories below so a script reads
    // like the LeetCode call sequence it replays. Nested because it is only ever used
    // inside this test class - it is this harness's own vocabulary, not a type another
    // file would import.
    public readonly record struct EncrypterCall(EncrypterCall.OpKind kind, string word)
    {
        public static EncrypterCall Encrypt(string word) => new(OpKind.Encrypt, word);

        public static EncrypterCall Decrypt(string word) => new(OpKind.Decrypt, word);

        // The string LeetCode's own judge output shows for this call - the ciphertext
        // itself for encrypt, the decimal count for decrypt - so one expected value
        // per call covers both operations uniformly.
        internal string Apply(IEncrypter encrypter) => kind == OpKind.Encrypt
            ? encrypter.Encrypt(word)
            : DecryptCount(encrypter, word);

        // Decrypting and then rendering the count is two links - a pipeline, not a
        // value - and a conditional expression's arm holds one value. Naming the pair
        // and calling it keeps the arm lazy, where hoisting it into a local above the
        // expression would run the decrypt even for an Encrypt call.
        private static string DecryptCount(IEncrypter encrypter, string ciphertext) =>
            encrypter.Decrypt(ciphertext).ToString(CultureInfo.InvariantCulture);

        public enum OpKind
        {
            Encrypt,
            Decrypt,
        }
    }
}
