using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

namespace DSAExperimentation.LeetCode.StreamOfCharacters;

// LeetCode 1032. Stream of Characters: a StreamChecker is constructed from a word
// list and then asked, one streamed letter at a time, whether any suffix of
// everything streamed so far spells one of those words. A Design problem's whole
// point is a sequence of calls against one instance, so "every strategy for the
// problem" (ARCHITECTURE.md 17.3) takes the form of two classes implementing the
// shared IStreamCheckerStrategy surface below.
internal static class StreamOfCharactersSolution
{
    // The shared surface both strategies implement, so the test and benchmark
    // harnesses can replay one stream against either strategy without restating it.
    internal interface IStreamCheckerStrategy
    {
        bool Query(char letter);
    }

    // The textbook answer: keep every streamed character in a BCL List<char> and,
    // on each query, materialize and hash every suffix up to the longest word.
    // Deliberately written without this repo's primitives - it is the arm the trie
    // strategy has to justify itself against.
    internal sealed class StreamCheckerBySuffixRescan : IStreamCheckerStrategy
    {
        private readonly HashSet<string> _words;
        private readonly int _maxWordLength;
        private readonly List<char> _stream = [];

        public StreamCheckerBySuffixRescan(IEnumerable<string> words)
        {
            _words = new HashSet<string>(words);
            _maxWordLength = _words.Count == 0 ? 0 : _words.Max(word => word.Length);
        }

        public bool Query(char letter)
        {
            _stream.Add(letter);

            var longestCandidate = Math.Min(_stream.Count, _maxWordLength);

            for (var length = 1; length <= longestCandidate; length++)
            {
                var suffix = new string(_stream.GetRange(_stream.Count - length, length).ToArray());

                if (_words.Contains(suffix))
                {
                    return true;
                }
            }

            return false;
        }
    }

    // The standard LC 1032 technique on this repo's own LowercaseTrie<TValue>: words
    // are inserted reversed, so walking the trie backward from the most recently
    // streamed character - through DynamicArray<char>'s O(1) indexed Get, itself
    // standing in for the running stream buffer - tests every suffix of the stream in
    // a single O(longest word) walk instead of re-testing each suffix from scratch.
    // LowercaseTrieNode's Children/HasValue are already public (LowercaseTrie<TValue>.Root
    // exposes the entry point), so this composes the existing trie node-walk directly
    // rather than reimplementing it.
    internal sealed class StreamCheckerByReversedTrie : IStreamCheckerStrategy
    {
        private readonly LowercaseTrie<bool> _reversedWords = new();
        private readonly DynamicArray<char> _stream = new();

        public StreamCheckerByReversedTrie(IEnumerable<string> words)
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
