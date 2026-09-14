using System.Globalization;
using static DSAExperimentation.LeetCode.EncryptAndDecryptStrings.EncryptAndDecryptStringsSolution;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EncryptAndDecryptStrings;

// Harness only: both strategies live in EncryptAndDecryptStringsSolution,
// including the re-encrypt-the-dictionary-per-query baseline the pre-migration
// benchmark kept to itself. LeetCode's own shape here is a stateful object across
// a sequence of calls, so Examples encodes the constructor's three arguments plus
// a call script instead of a single argument tuple - the same shape
// DesignBitsetTests uses for its own instance-API problem. EncrypterCall.Apply
// renders each result as the string LeetCode's judge output shows for it, so one
// expected value per call covers both encrypt's string and decrypt's count.
public sealed class EncryptAndDecryptStringsTests
{
    // (keys, values, dictionary, call script, one expected result per call)
    public static TheoryData<char[], string[], string[], EncrypterCall[], string[]> Examples =>
        new()
        {
            // LeetCode's published example. 'a' and 'c' both map to "ei", so
            // "abcd" and "abad" encrypt identically and decrypt reports 2.
            {
                ['a', 'b', 'c', 'd'],
                ["ei", "zf", "ei", "am"],
                ["abcd", "acbd", "adbc", "badc", "dacb", "cadb", "cbda", "abad"],
                [
                    EncrypterCall.Encrypt("abcd"),
                    EncrypterCall.Decrypt("eizfeiam"),
                ],
                ["eizfeiam", "2"]
            },

            // No dictionary word encrypts to this, even though every two-letter
            // group in it is a legal value.
            {
                ['a', 'b', 'c', 'd'],
                ["ei", "zf", "ei", "am"],
                ["abcd", "acbd", "adbc", "badc", "dacb", "cadb", "cbda", "abad"],
                [EncrypterCall.Decrypt("zfzfzfzf")],
                ["0"]
            },

            // A character outside keys cannot be encrypted at all, so encrypt
            // reports the empty string - LeetCode's own stated behaviour.
            {
                ['a', 'b', 'c', 'd'],
                ["ei", "zf", "ei", "am"],
                ["abcd"],
                [EncrypterCall.Encrypt("abce"), EncrypterCall.Encrypt("dcba")],
                ["", "ameizfei"]
            },

            // Duplicate values with single-character dictionary words: "ei" is the
            // encryption of both "a" and "b", so it decrypts to 2, while "eiei"
            // matches only "ab" - "aa", "ba" and "bb" are not in the dictionary.
            {
                ['a', 'b'],
                ["ei", "ei"],
                ["a", "b", "ab"],
                [
                    EncrypterCall.Encrypt("a"),
                    EncrypterCall.Encrypt("ab"),
                    EncrypterCall.Decrypt("ei"),
                    EncrypterCall.Decrypt("eiei"),
                ],
                ["ei", "eiei", "2", "1"]
            },

            // Nothing in word2 is a value at all, and its length is odd besides -
            // no dictionary word can encrypt to it.
            {
                ['a', 'b'],
                ["ei", "zf"],
                ["a", "b", "ab", "ba"],
                [EncrypterCall.Decrypt("xyx"), EncrypterCall.Decrypt("zfei")],
                ["0", "1"]
            },

            // Repeats in the dictionary count once each, so the same word listed
            // twice makes its encryption decrypt to 2.
            {
                ['a', 'b'],
                ["ei", "zf"],
                ["ab", "ab", "ba"],
                [EncrypterCall.Decrypt("eizf"), EncrypterCall.Decrypt("zfei")],
                ["2", "1"]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByDictionaryRescan_LeetCodeExamples_MatchesExpectedSequence(
        char[] keys, string[] values, string[] dictionary, EncrypterCall[] calls, string[] expected) =>
        RunScript(CreateByDictionaryRescan(keys, values, dictionary), calls, expected);

    [Theory]
    [MemberData(nameof(Examples))]
    public void CreateByPrecomputedFrequency_LeetCodeExamples_MatchesExpectedSequence(
        char[] keys, string[] values, string[] dictionary, EncrypterCall[] calls, string[] expected) =>
        RunScript(CreateByPrecomputedFrequency(keys, values, dictionary), calls, expected);

    private static void RunScript(IEncrypter encrypter, EncrypterCall[] calls, string[] expected)
    {
        for (var i = 0; i < calls.Length; i++)
        {
            Assert.Equal(expected[i], calls[i].Apply(encrypter));
        }
    }
}

// One call in an Encrypter script: which of the two operations to invoke and on
// what word. Pure dispatch, built via the named factories below so a script reads
// like the LeetCode call sequence it replays.
public readonly record struct EncrypterCall
{
    private readonly Kind _kind;
    private readonly string _word;

    private EncrypterCall(Kind kind, string word)
    {
        _kind = kind;
        _word = word;
    }

    public static EncrypterCall Encrypt(string word1) => new(Kind.Encrypt, word1);

    public static EncrypterCall Decrypt(string word2) => new(Kind.Decrypt, word2);

    // The string LeetCode's own judge output shows for this call - the ciphertext
    // itself for encrypt, the decimal count for decrypt - so one expected value
    // per call covers both operations uniformly.
    internal string Apply(IEncrypter encrypter) => _kind == Kind.Encrypt
        ? encrypter.Encrypt(_word)
        : encrypter.Decrypt(_word).ToString(CultureInfo.InvariantCulture);

    private enum Kind
    {
        Encrypt,
        Decrypt,
    }
}
