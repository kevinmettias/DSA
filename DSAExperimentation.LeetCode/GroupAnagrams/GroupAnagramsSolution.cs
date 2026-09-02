using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.LeetCode.GroupAnagrams;

// LeetCode 49. Group Anagrams: bucket strings that are letter-for-letter
// permutations of one another. Both strategies key each string by its sorted
// letters and differ only in the map that collects the buckets - a BCL
// Dictionary, or this repo's own HashMap.
internal static class GroupAnagramsSolution
{
    // The textbook baseline: a BCL Dictionary keyed on each word's sorted letters.
    public static List<List<string>> GroupByDictionary(IEnumerable<string> values)
    {
        var map = new Dictionary<string, List<string>>();

        foreach (var value in values)
        {
            var key = SortedLetters(value);

            if (!map.TryGetValue(key, out var group))
            {
                group = [];
                map[key] = group;
            }

            group.Add(value);
        }

        return map.Values.ToList();
    }

    // The same bucketing, over this repo's own HashMap instead of the BCL's.
    public static List<List<string>> GroupByHashMap(IEnumerable<string> values)
    {
        var map = new HashMap<string, List<string>>();

        foreach (var value in values)
        {
            var key = SortedLetters(value);

            if (!map.TryGetValue(key, out var group))
            {
                group = [];
                map.Set(key, group);
            }

            group.Add(value);
        }

        return map.Values.ToList();
    }

    private static string SortedLetters(string value)
    {
        var chars = value.ToCharArray();
        Array.Sort(chars);
        return new string(chars);
    }
}
