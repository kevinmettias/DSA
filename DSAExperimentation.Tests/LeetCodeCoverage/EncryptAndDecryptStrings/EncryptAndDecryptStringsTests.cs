using System.Text;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.EncryptAndDecryptStrings;

// LeetCode 2227. Encrypt and Decrypt Strings: encrypt is a straight per-character
// lookup through this repo's own HashMap<char,string> (keys[i] -> values[i],
// TwoSumTests/DesignParkingSystemTests precedent). decrypt needs the count of
// dictionary words that encrypt to a given target - values are not required to be
// unique, so re-encrypting the whole dictionary per call would be the naive
// approach. Precomputing every dictionary word's encryption once into a second
// HashMap<string,int> frequency table turns each decrypt call into an O(1) lookup
// instead.
public sealed partial class EncryptAndDecryptStringsTests
{
    [Fact]
    public void Encrypt_LeetCodeExample_ReturnsConcatenatedMapping()
    {
        var encryptor = BuildLeetCodeExampleEncryptor();

        Assert.Equal("eizfeiam", encryptor.Encrypt("abcd"));
    }

    [Fact]
    public void Decrypt_LeetCodeExample_ReturnsMatchingDictionaryWordCount()
    {
        var encryptor = BuildLeetCodeExampleEncryptor();

        Assert.Equal(2, encryptor.Decrypt("eizfeiam"));
    }

    [Fact]
    public void Decrypt_NoDictionaryWordEncryptsToTarget_ReturnsZero()
    {
        var encryptor = BuildLeetCodeExampleEncryptor();

        Assert.Equal(0, encryptor.Decrypt("zfzfzfzf"));
    }

    private static Encryptor BuildLeetCodeExampleEncryptor()
    {
        char[] keys = ['a', 'b', 'c', 'd'];
        string[] values = ["ei", "zf", "ei", "am"];
        string[] dictionary = ["abcd", "acbd", "adbc", "badc", "dacb", "cadb", "cbda", "abad"];

        return new Encryptor(keys, values, dictionary);
    }

    private sealed class Encryptor
    {
        private readonly HashMap<char, string> _valueByKey = new();
        private readonly HashMap<string, int> _encryptedDictionaryCounts = new();

        public Encryptor(char[] keys, string[] values, string[] dictionary)
        {
            for (var i = 0; i < keys.Length; i++)
            {
                _valueByKey.Set(keys[i], values[i]);
            }

            foreach (var word in dictionary)
            {
                var encrypted = Encrypt(word);
                _encryptedDictionaryCounts.TryGetValue(encrypted, out var count);
                _encryptedDictionaryCounts.Set(encrypted, count + 1);
            }
        }

        // Empty string on an unmapped character is this repo's own choice for the
        // (constraint-excluded) case a caller passes a character outside keys - there
        // is no LeetCode-specified behavior to match here since word1 is guaranteed to
        // only contain characters present in keys.
        public string Encrypt(string word)
        {
            var result = new StringBuilder(word.Length * 2);

            foreach (var c in word)
            {
                if (!_valueByKey.TryGetValue(c, out var mapped))
                {
                    return string.Empty;
                }

                result.Append(mapped);
            }

            return result.ToString();
        }

        public int Decrypt(string word) => _encryptedDictionaryCounts.TryGetValue(word, out var count) ? count : 0;
    }
}
