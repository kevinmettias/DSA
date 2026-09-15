using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.LeetCode.IteratorForCombination;

// LeetCode 1286. Iterator for Combination: a CombinationIterator over a sorted,
// distinct character string that yields every length-combinationLength combination
// in lexicographical order via next()/hasNext().
//
// Both strategies precompute the whole combination list up front and then hand back
// a cursor over it - the iterator itself is no algorithm, the same "cursor over an
// already-ordered list" shape BinarySearchTreeIteratorSolution's BstIterator has.
// The strategies differ in how that list is produced:
//
// - CreateByBitmaskEnumeration is the baseline: scan all 2^n subset masks, keep the
//   ones whose popcount matches, then sort the survivors into lexicographic order.
//   Plain BCL, as a baseline should be - the work it wastes is every mask with the
//   wrong popcount, plus the sort.
// - CreateByBacktrackEngine composes this repo's Backtrack.Search, walking indices
//   in strictly increasing order. Because characters is already sorted, that order
//   is lexicographic by construction, so nothing is generated to be discarded and no
//   sort is needed.
internal static class IteratorForCombinationSolution
{
    public static CombinationIterator CreateByBitmaskEnumeration(string characters, int combinationLength)
    {
        var results = new List<string>();
        var subsetCount = 1 << characters.Length;

        for (var mask = 0; mask < subsetCount; mask++)
        {
            var combination = BuildCombinationIfMatch(characters, combinationLength, mask);

            if (combination is not null)
            {
                results.Add(combination);
            }
        }

        results.Sort(StringComparer.Ordinal);
        return new CombinationIterator(results);
    }

    private static string? BuildCombinationIfMatch(string characters, int combinationLength, int mask)
    {
        if (int.PopCount(mask) != combinationLength)
        {
            return null;
        }

        var chars = new char[combinationLength];
        var next = 0;

        for (var bit = 0; bit < characters.Length; bit++)
        {
            if ((mask & (1 << bit)) != 0)
            {
                chars[next++] = characters[bit];
            }
        }

        return new string(chars);
    }

    public static CombinationIterator CreateByBacktrackEngine(string characters, int combinationLength)
    {
        var state = new SearchState();
        var results = RunSearch(characters, combinationLength, state);

        return new CombinationIterator(results);
    }

    // Drives Backtrack.Search over the growing character selection: a state is a
    // solution once it holds combinationLength characters, its candidates are the
    // indices still ahead of it, and unchoosing restores the Start the previous
    // choice replaced.
    private static List<string> RunSearch(string characters, int combinationLength, SearchState state)
    {
        var results = new List<string>();

        Backtrack.Search<SearchState, int>(
            state,
            isSolution: s => s.Chosen.Count == combinationLength,
            candidates: s => s.Chosen.Count == combinationLength
                ? Array.Empty<int>()
                : Enumerable.Range(s.Start, characters.Length - s.Start),
            choose: (s, index) =>
            {
                s.Starts.Push(s.Start);
                s.Chosen.Add(characters[index]);
                s.Start = index + 1;
            },
            unchoose: (s, _) =>
            {
                s.Start = s.Starts.Pop();
                s.Chosen.RemoveAt(s.Chosen.Count - 1);
            },
            onSolution: s => results.Add(new string([.. s.Chosen])));

        return results;
    }

    // LeetCode's own answer shape: the stateful object the judge drives with
    // next()/hasNext(). Both strategies hand it an already-ordered list, so it is
    // nothing but the cursor.
    internal sealed class CombinationIterator(List<string> combinations)
    {
        private readonly List<string> _combinations = combinations;
        private int _index;

        public bool HasNext() => _index < _combinations.Count;

        public string Next() => _combinations[_index++];
    }

    // The backtracking cursor: the characters chosen so far, the next index the
    // search may take, and the stack of prior Start values Unchoose restores.
    private sealed class SearchState
    {
        public List<char> Chosen { get; } = [];

        public Stack<int> Starts { get; } = new();

        public int Start { get; set; }
    }
}
