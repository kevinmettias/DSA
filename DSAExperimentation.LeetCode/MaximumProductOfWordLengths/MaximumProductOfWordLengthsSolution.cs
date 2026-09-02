using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.MaximumProductOfWordLengths;

// LeetCode 318. Maximum Product of Word Lengths: given a list of words, find the
// largest product of two words' lengths where the two words share no letter.
//
// The two strategies differ in how they test "shares no letter": the textbook
// approach rescans both words' characters for every pair; this repo's own
// approach collapses each word to a 26-bit letter-presence mask once, dedupes
// words sharing a mask down to the longest one via this repo's own
// HashMap<int,int>, then tests disjointness with one bitwise AND per pair.
internal static class MaximumProductOfWordLengthsSolution
{
    // The textbook pairwise scan: for every pair of words, walks every
    // character of one against every character of the other looking for a
    // shared letter. Deliberately written without this repo's primitives - it
    // is the arm the composed solution below has to justify itself against.
    public static int MaxProductByCharacterScan(string[] words)
    {
        var best = 0;

        for (var i = 0; i < words.Length; i++)
        {
            for (var j = i + 1; j < words.Length; j++)
            {
                if (SharesLetter(words[i], words[j]))
                {
                    continue;
                }

                best = Math.Max(best, words[i].Length * words[j].Length);
            }
        }

        return best;
    }

    private static bool SharesLetter(string a, string b)
    {
        foreach (var x in a)
        {
            foreach (var y in b)
            {
                if (x == y)
                {
                    return true;
                }
            }
        }

        return false;
    }

    // Collapses every word to a 26-bit letter-presence mask, keeping only the
    // longest word for each distinct mask via this repo's own
    // HashMap<int,int>, then scans mask pairs for disjointness with a single
    // bitwise AND instead of rescanning characters.
    public static int MaxProductByBitmaskHashMap(string[] words)
    {
        var maskToMaxLength = new HashMap<int, int>();

        foreach (var word in words)
        {
            RecordLongestForMask(maskToMaxLength, word);
        }

        var masks = maskToMaxLength.Keys.ToArray();
        var lengths = new int[masks.Length];
        for (var i = 0; i < masks.Length; i++)
        {
            maskToMaxLength.TryGetValue(masks[i], out lengths[i]);
        }

        return BestDisjointPairProduct(masks, lengths);
    }

    private static void RecordLongestForMask(HashMap<int, int> maskToMaxLength, string word)
    {
        var mask = 0;
        foreach (var c in word)
        {
            mask |= 1 << (c - 'a');
        }

        if (maskToMaxLength.TryGetValue(mask, out var existingLength) && existingLength >= word.Length)
        {
            return;
        }

        maskToMaxLength.Set(mask, word.Length);
    }

    private static int BestDisjointPairProduct(int[] masks, int[] lengths)
    {
        var best = 0;

        for (var i = 0; i < masks.Length; i++)
        {
            for (var j = i + 1; j < masks.Length; j++)
            {
                if ((masks[i] & masks[j]) == 0)
                {
                    best = Math.Max(best, lengths[i] * lengths[j]);
                }
            }
        }

        return best;
    }
}
