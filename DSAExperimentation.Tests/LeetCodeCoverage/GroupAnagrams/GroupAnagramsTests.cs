using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.Tests.LeetCodeCoverage.GroupAnagrams;

public sealed partial class GroupAnagramsTests
{
    [Fact]
    public void GroupAnagrams_ClassicExample_GroupsWordsBySortedLetters()
    {
        var groups = Group(["eat", "tea", "tan", "ate", "nat", "bat"]);
        Assert.Contains(groups, g => g.Order().SequenceEqual(["ate", "eat", "tea"]));
        Assert.Contains(groups, g => g.Order().SequenceEqual(["nat", "tan"]));
        Assert.Contains(groups, g => g.SequenceEqual(["bat"]));
    }

    private static List<List<string>> Group(string[] values)
    {
        var map = new HashMap<string, List<string>>();
        foreach (var value in values)
        {
            var chars = value.ToCharArray(); Array.Sort(chars); var key = new string(chars);
            if (!map.TryGetValue(key, out var group)) { group = []; map.Set(key, group); }
            group.Add(value);
        }
        return map.Values.ToList();
    }
}
