using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.IsomorphicStrings;

// LeetCode 205. Isomorphic Strings: `source` and `target` are isomorphic when
// there is a one-to-one character mapping from `source` to `target` that, applied
// to every position, turns `source` into `target`. A one-way map is not enough -
// 'a','a' -> 'b' would let two different source-characters collapse onto the same
// target-character - so both strategies keep a forward map (source -> target) and
// a backward map (target -> source) and reject the first position where either
// direction disagrees.
internal static class IsomorphicStringsSolution
{
    // The textbook answer: two BCL Dictionary<char, char> maps. Deliberately
    // written without this repo's primitives - it is the arm the composed
    // strategy below has to justify itself against.
    public static bool IsIsomorphicByDictionary(string source, string target)
    {
        var forward = new Dictionary<char, char>();
        var backward = new Dictionary<char, char>();

        for (var i = 0; i < source.Length; i++)
        {
            if (forward.TryGetValue(source[i], out var mappedForward) && mappedForward != target[i])
            {
                return false;
            }

            if (backward.TryGetValue(target[i], out var mappedBackward) && mappedBackward != source[i])
            {
                return false;
            }

            forward[source[i]] = target[i];
            backward[target[i]] = source[i];
        }

        return true;
    }

    // Same bijection check, composed from this repo's own HashMap<TKey, TValue>.
    public static bool IsIsomorphicByHashMap(string source, string target)
    {
        var forward = new HashMap<char, char>();
        var backward = new HashMap<char, char>();

        for (var i = 0; i < source.Length; i++)
        {
            if (forward.TryGetValue(source[i], out var mappedForward) && mappedForward != target[i])
            {
                return false;
            }

            if (backward.TryGetValue(target[i], out var mappedBackward) && mappedBackward != source[i])
            {
                return false;
            }

            forward.Set(source[i], target[i]);
            backward.Set(target[i], source[i]);
        }

        return true;
    }
}
