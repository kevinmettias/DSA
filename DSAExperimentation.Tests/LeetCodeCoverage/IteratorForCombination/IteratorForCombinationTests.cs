using DSAExperimentation.Algorithms.Backtracking;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IteratorForCombination;

// LeetCode 1286. Iterator for Combination: this repo's Backtrack.Search
// (CombinationSumIII/PalindromePartitioning precedent) enumerates every
// length-combinationLength combination of characters' indices in strictly
// increasing order - which is already lexicographic order since characters
// is given sorted - so the iterator itself is just a cursor over that
// precomputed, already-ordered list, the same "no algorithm primitive
// beyond a cursor" shape BinarySearchTreeIteratorTests' stack-backed
// iterator already uses.
public sealed partial class IteratorForCombinationTests
{
    [Fact]
    public void CombinationIterator_ClassicExample_YieldsInLexicographicalOrder()
    {
        var iterator = new CombinationIterator("abc", 2);

        Assert.Equal("ab", iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal("ac", iterator.Next());
        Assert.True(iterator.HasNext());
        Assert.Equal("bc", iterator.Next());
        Assert.False(iterator.HasNext());
    }

    [Fact]
    public void CombinationIterator_CombinationLengthEqualsCharacterCount_YieldsSingleCombination()
    {
        var iterator = new CombinationIterator("wxyz", 4);

        Assert.Equal("wxyz", iterator.Next());
        Assert.False(iterator.HasNext());
    }

    private sealed class CombinationIterator
    {
        private readonly List<string> _combinations;
        private int _index;

        public CombinationIterator(string characters, int combinationLength)
            => _combinations = GenerateCombinations(characters, combinationLength);

        public bool HasNext() => _index < _combinations.Count;

        public string Next() => _combinations[_index++];

        private static List<string> GenerateCombinations(string characters, int combinationLength)
        {
            var results = new List<string>();
            var state = new State();

            Backtrack.Search<State, int>(
                state,
                isSolution: x => IsSolution(x, combinationLength),
                candidates: x => Candidates(x, combinationLength, characters.Length),
                choose: (x, index) => Choose(x, index, characters),
                unchoose: (x, _) => Unchoose(x),
                onSolution: x => results.Add(new string([.. x.Chosen])));

            return results;
        }

        private static bool IsSolution(State x, int combinationLength) => x.Chosen.Count == combinationLength;

        private static IEnumerable<int> Candidates(State x, int combinationLength, int characterCount)
            => x.Chosen.Count == combinationLength ? [] : Enumerable.Range(x.Start, characterCount - x.Start);

        private static void Choose(State x, int index, string characters)
        {
            x.Starts.Push(x.Start);
            x.Chosen.Add(characters[index]);
            x.Start = index + 1;
        }

        private static void Unchoose(State x)
        {
            x.Start = x.Starts.Pop();
            x.Chosen.RemoveAt(x.Chosen.Count - 1);
        }

        private sealed class State
        {
            public List<char> Chosen { get; } = [];
            public Stack<int> Starts { get; } = new();
            public int Start { get; set; }
        }
    }
}
