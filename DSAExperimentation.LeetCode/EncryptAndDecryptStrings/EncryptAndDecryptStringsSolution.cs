using System.Text;
using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.EncryptAndDecryptStrings;

// LeetCode 2227. Encrypt and Decrypt Strings: keys[i] encrypts to the two-letter
// values[i]; encrypt replaces every character, and decrypt(word2) reports HOW MANY
// dictionary words word2 could have been the encryption of.
//
// This is a design problem - LeetCode's own shape is a stateful object built from
// keys/values/dictionary and then queried, not a single return value - so "every
// strategy for the problem" (ARCHITECTURE.md section 17.3) takes the form of two
// classes behind the shared IEncrypter surface, the same CreateBy<Strategy>
// factory shape AllOneDataStructureSolution and LRUCacheSolution use for their
// own instance-API problems.
//
// values are NOT required to be distinct - the published example maps both 'a' and
// 'c' to "ei" - so decrypt cannot invert a two-letter group to one key. Both
// strategies therefore answer it the same way round: encrypt dictionary words and
// count the ones that land on word2. They differ only in WHEN that encryption
// happens - once per query, or once per dictionary word for the object's lifetime.
//
// Returning the empty string for a character outside keys is LeetCode's own
// specified behaviour for encrypt, not a house convention: the problem states the
// encryption cannot be carried out and "" is returned.
internal static class EncryptAndDecryptStringsSolution
{
    // Every value is a string of length 2, so an encryption is exactly twice its
    // plaintext - the buffer both strategies size their StringBuilder with.
    private const int EncryptedCharacterLength = 2;

    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one call script against either without restating it.
    internal interface IEncrypter
    {
        string Encrypt(string word1);

        int Decrypt(string word2);
    }

    // The textbook baseline this composition has to justify itself against: keep
    // the dictionary as it was handed over and, on every decrypt, re-encrypt all
    // of it to count the matches - O(dictionary size * word length) per query.
    // Deliberately BCL-only, a plain Dictionary<char, string> and the caller's own
    // array, since it is what you would write without this repo.
    public static IEncrypter CreateByDictionaryRescan(char[] keys, string[] values, string[] dictionary)
        => new DictionaryRescanEncrypter(keys, values, dictionary);

    // Every dictionary word is encrypted ONCE at construction into this repo's own
    // HashMap<string, int> frequency table, so decrypt is a single average-O(1)
    // lookup no matter how large the dictionary is - the build cost is paid once
    // and amortized over every query that follows.
    public static IEncrypter CreateByPrecomputedFrequency(char[] keys, string[] values, string[] dictionary)
        => new PrecomputedFrequencyEncrypter(keys, values, dictionary);

    private sealed class DictionaryRescanEncrypter : IEncrypter
    {
        private readonly Dictionary<char, string> _valueByKey = [];
        private readonly string[] _dictionary;

        public DictionaryRescanEncrypter(char[] keys, string[] values, string[] dictionary)
        {
            for (var i = 0; i < keys.Length; i++)
            {
                _valueByKey[keys[i]] = values[i];
            }

            _dictionary = dictionary;
        }

        public string Encrypt(string word1)
        {
            var encrypted = new StringBuilder(word1.Length * EncryptedCharacterLength);

            foreach (var character in word1)
            {
                if (!_valueByKey.TryGetValue(character, out var mapped))
                {
                    return string.Empty;
                }

                encrypted.Append(mapped);
            }

            return encrypted.ToString();
        }

        public int Decrypt(string word2)
        {
            var matches = 0;

            foreach (var word in _dictionary)
            {
                if (Encrypt(word) == word2)
                {
                    matches++;
                }
            }

            return matches;
        }
    }

    private sealed class PrecomputedFrequencyEncrypter : IEncrypter
    {
        private readonly HashMap<char, string> _valueByKey = new();
        private readonly HashMap<string, int> _dictionaryCountsByEncryption = new();

        public PrecomputedFrequencyEncrypter(char[] keys, string[] values, string[] dictionary)
        {
            for (var i = 0; i < keys.Length; i++)
            {
                _valueByKey.Set(keys[i], values[i]);
            }

            foreach (var word in dictionary)
            {
                var encrypted = Encrypt(word);
                _dictionaryCountsByEncryption.TryGetValue(encrypted, out var count);
                _dictionaryCountsByEncryption.Set(encrypted, count + 1);
            }
        }

        public string Encrypt(string word1)
        {
            var encrypted = new StringBuilder(word1.Length * EncryptedCharacterLength);

            foreach (var character in word1)
            {
                if (!_valueByKey.TryGetValue(character, out var mapped))
                {
                    return string.Empty;
                }

                encrypted.Append(mapped);
            }

            return encrypted.ToString();
        }

        public int Decrypt(string word2)
            => _dictionaryCountsByEncryption.TryGetValue(word2, out var count) ? count : 0;
    }
}
