using DSAExperimentation.DataStructures.HashMap;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MatchSubstringAfterReplacement;

// LeetCode 2301. Match Substring After Replacement: for each candidate start index
// in s, sub matches if every position either equals s's character outright or is
// reachable via one of the allowed (old, new) replacement pairs. The replacement
// pairs are indexed as HashMap<char, Set<char>> - this repo's own HashMap<TKey,TValue>
// keyed by the "old" character, each value a Set<Element> (HashMap<Element,bool>,
// ARCHITECTURE.md §4.1) of characters that old is allowed to become - turning "is
// (subChar, sChar) an allowed pair" into an O(1) two-step lookup instead of scanning
// the raw mappings list on every character comparison.
public sealed partial class MatchSubstringAfterReplacementTests
{
    [Fact]
    public void IsMatch_LeetCodeExampleWithChainedMappings_ReturnsTrue()
    {
        (char Old, char New)[] mappings = [('e', '3'), ('t', '7'), ('t', '8')];

        var found = IsMatch("fool3e7bar", "leet", mappings);

        Assert.True(found);
    }

    [Fact]
    public void IsMatch_MappingOnlyAppliesInOneDirection_ReturnsFalse()
    {
        // sub's '0' characters have no mapping - the given mapping only lets an 'o'
        // in sub become '0', not the reverse - so "f00l" can never equal "fool".
        (char Old, char New)[] mappings = [('o', '0')];

        var found = IsMatch("fooleetbar", "f00l", mappings);

        Assert.False(found);
    }

    private static bool IsMatch(string s, string sub, (char Old, char New)[] mappings)
    {
        var allowed = BuildAllowedMap(mappings);

        for (var start = 0; start + sub.Length <= s.Length; start++)
        {
            if (MatchesAt(s, sub, start, allowed))
            {
                return true;
            }
        }

        return false;
    }

    private static HashMap<char, Set<char>> BuildAllowedMap((char Old, char New)[] mappings)
    {
        var allowed = new HashMap<char, Set<char>>();

        foreach (var (oldChar, newChar) in mappings)
        {
            if (!allowed.TryGetValue(oldChar, out var targets))
            {
                targets = new Set<char>();
                allowed.Set(oldChar, targets);
            }

            targets.TryAdd(newChar);
        }

        return allowed;
    }

    private static bool MatchesAt(string s, string sub, int start, HashMap<char, Set<char>> allowed)
    {
        for (var j = 0; j < sub.Length; j++)
        {
            var subChar = sub[j];
            var sChar = s[start + j];

            if (subChar == sChar)
            {
                continue;
            }

            if (!allowed.TryGetValue(subChar, out var targets) || !targets.Has(sChar))
            {
                return false;
            }
        }

        return true;
    }
}
