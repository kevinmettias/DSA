using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.Tests.LeetCodeCoverage.StreamOfCharacters;

// LeetCode 1032. Stream of Characters: the standard reversed-word trie technique,
// built directly on this repo's own LowercaseTrie<TValue> - words are inserted
// reversed so that walking the trie backward from the most recently streamed
// character (through DynamicArray<char>'s O(1) indexed Get, itself standing in for
// the running stream buffer) tests every suffix of the stream in a single O(longest
// word) walk instead of re-testing each suffix from scratch. LowercaseTrieNode's
// Children/HasValue are already public (LowercaseTrie<TValue>.Root exposes the
// entry point), so this composes the existing trie node-walk directly rather than
// reimplementing it.
public sealed partial class StreamOfCharactersTests
{
    [Fact]
    public void Query_LeetCodeExampleSequence_ReturnsSuffixMatchPerCharacter()
    {
        var checker = new StreamChecker(["cd", "f", "kl"]);

        bool[] expected = [false, false, false, true, false, true, false, false, false, false, false, true];
        char[] stream = ['a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l'];

        for (var i = 0; i < stream.Length; i++)
        {
            Assert.Equal(expected[i], checker.Query(stream[i]));
        }
    }

    [Fact]
    public void Query_OverlappingWords_MatchesShortestAndLongestSuffix()
    {
        var checker = new StreamChecker(["ab", "ba"]);

        bool[] expected = [false, true, true, true];
        char[] stream = ['a', 'b', 'a', 'b'];

        for (var i = 0; i < stream.Length; i++)
        {
            Assert.Equal(expected[i], checker.Query(stream[i]));
        }
    }

    private sealed class StreamChecker
    {
        private readonly LowercaseTrie<bool> _reversedWords = new();
        private readonly DynamicArray<char> _stream = new();

        public StreamChecker(string[] words)
        {
            foreach (var word in words)
            {
                _reversedWords.Set(new string(word.Reverse().ToArray()), true);
            }
        }

        public bool Query(char letter)
        {
            _stream.Add(letter);

            var node = _reversedWords.Root;

            for (var i = _stream.Count - 1; i >= 0 && node is not null; i--)
            {
                node = node.Children[_stream.Get(i) - 'a'];

                if (node is not null && node.HasValue)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
