using DSAExperimentation.DataStructures.RollingHash;

namespace DSAExperimentation.LeetCode.CountPrefixAndSuffixPairsI;

// LeetCode 3042. Count Prefix and Suffix Pairs I: count index pairs (i < j)
// where words[i] is both a PREFIX and a SUFFIX of words[j].
//
// Both strategies answer the same question with the same signature, so the test
// harness can assert they agree and the benchmark harness can time them against
// each other without either restating the algorithm.
internal static class CountPrefixAndSuffixPairsISolution
{
    // The textbook O(n^2 * L) scan: every earlier word's characters compared
    // directly against the later word's prefix and suffix spans. The arm the
    // rolling hash strategy below has to beat.
    public static int CountPairsByBruteForce(string[] words)
    {
        var count = 0;

        for (var i = 0; i < words.Length; i++)
        {
            for (var j = i + 1; j < words.Length; j++)
            {
                if (IsPrefixAndSuffix(words[i], words[j]))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsPrefixAndSuffix(string candidate, string word)
    {
        if (candidate.Length > word.Length)
        {
            return false;
        }

        return word.AsSpan(0, candidate.Length).SequenceEqual(candidate) &&
               word.AsSpan(word.Length - candidate.Length).SequenceEqual(candidate);
    }

    // Each word gets its own this-repo RollingHash once; checking whether
    // words[i] is a prefix/suffix of words[j] then costs two O(1) hash
    // comparisons (RollingHash.cs's own doc comment: double-hashed lanes make a
    // false positive effectively impossible) instead of an O(L) character walk
    // per candidate pair.
    public static int CountPairsByRollingHash(string[] words)
    {
        var hashes = BuildHashes(words);
        return CountPairsByRollingHash(hashes);
    }

    public static int CountPairsByRollingHash(RollingHash[] hashes)
    {
        var count = 0;

        for (var i = 0; i < hashes.Length; i++)
        {
            for (var j = i + 1; j < hashes.Length; j++)
            {
                if (IsPrefixAndSuffix(hashes[i], hashes[j]))
                {
                    count++;
                }
            }
        }

        return count;
    }

    private static bool IsPrefixAndSuffix(RollingHash candidate, RollingHash word)
    {
        var length = candidate.Length;

        if (length > word.Length)
        {
            return false;
        }

        var candidateHash = candidate.Hash(0, length);
        return word.Hash(0, length) == candidateHash && word.Hash(word.Length - length, length) == candidateHash;
    }

    public static RollingHash[] BuildHashes(string[] words) => Array.ConvertAll(words, word => new RollingHash(word));
}
