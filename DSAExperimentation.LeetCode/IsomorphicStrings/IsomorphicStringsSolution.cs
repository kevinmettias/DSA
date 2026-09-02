using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.IsomorphicStrings;

// LeetCode 205. Isomorphic Strings: s and t are isomorphic when there is a
// one-to-one character mapping from s to t that, applied to every position,
// turns s into t. A one-way map is not enough - 'a','a' -> 'b' would let two
// different s-characters collapse onto the same t-character - so both
// strategies keep a forward map (s -> t) and a backward map (t -> s) and
// reject the first position where either direction disagrees.
internal static class IsomorphicStringsSolution
{
    // The textbook answer: two BCL Dictionary<char, char> maps. Deliberately
    // written without this repo's primitives - it is the arm the composed
    // strategy below has to justify itself against.
    public static bool IsIsomorphicByDictionary(string s, string t)
    {
        var forward = new Dictionary<char, char>();
        var backward = new Dictionary<char, char>();

        for (var i = 0; i < s.Length; i++)
        {
            if (forward.TryGetValue(s[i], out var mappedForward) && mappedForward != t[i])
            {
                return false;
            }

            if (backward.TryGetValue(t[i], out var mappedBackward) && mappedBackward != s[i])
            {
                return false;
            }

            forward[s[i]] = t[i];
            backward[t[i]] = s[i];
        }

        return true;
    }

    // Same bijection check, composed from this repo's own HashMap<TKey, TValue>.
    public static bool IsIsomorphicByHashMap(string s, string t)
    {
        var forward = new HashMap<char, char>();
        var backward = new HashMap<char, char>();

        for (var i = 0; i < s.Length; i++)
        {
            if (forward.TryGetValue(s[i], out var mappedForward) && mappedForward != t[i])
            {
                return false;
            }

            if (backward.TryGetValue(t[i], out var mappedBackward) && mappedBackward != s[i])
            {
                return false;
            }

            forward.Set(s[i], t[i]);
            backward.Set(t[i], s[i]);
        }

        return true;
    }
}
