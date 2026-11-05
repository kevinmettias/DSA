using DSAExperimentation.DataStructures.DynamicArray;
using DSAExperimentation.Domain.Language.English;
using ReversedWordTrie = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.Trie<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.AlphabetSlots<
        DSAExperimentation.Domain.Language.English.EnglishLowercase,
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.DenseTrieLayout,
        bool>,
    bool>;

namespace DSAExperimentation.LeetCode.Solutions.StreamOfCharacters;

// LeetCode 1032. Stream of Characters: a StreamChecker is constructed from a word
// list and then asked, one streamed letter at a time, whether any suffix of
// everything streamed so far spells one of those words. A Design problem's whole
// point is a sequence of calls against one instance, so "every strategy for the
// problem" (ARCHITECTURE.md 17.3) takes the form of two classes implementing the
// shared IStreamCheckerStrategy surface below.
[LeetCodeQuestion("stream-of-characters", LeetCodeAnswerOrder.AsWritten)]
internal static class StreamOfCharactersSolution
{
    // Each strategy as LeetCode's constructor makes it, so the catalog harness can make the object its call
    // scripts run on (ARCHITECTURE 17.13).
    public static IStreamCheckerStrategy CreateBySuffixRescan(IEnumerable<string> words) => new StreamCheckerBySuffixRescan(words);

    public static IStreamCheckerStrategy CreateByReversedTrie(IEnumerable<string> words) => new StreamCheckerByReversedTrie(words);

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

    // The standard LC 1032 technique on this repo's own trie: words
    // are inserted reversed, so walking the trie backward from the most recently
    // streamed character - through DynamicArray<char>'s O(1) indexed Get, itself
    // standing in for the running stream buffer - tests every suffix of the stream in
    // a single O(longest word) walk instead of re-testing each suffix from scratch.
    // A node's Children.ChildAt/HasValue are already public (Trie.Root exposes the entry
    // point), so this composes the existing trie node-walk directly rather than
    // reimplementing it. The dense layout: the trie is built once and then only read, a
    // letter per step of every query, and a slot per letter read directly measured 52-70%
    // faster than packed nodes' popcount (IMPROVEMENT-WORKLIST, 2026-11-02).
    internal sealed class StreamCheckerByReversedTrie : IStreamCheckerStrategy
    {
        private readonly ReversedWordTrie _reversedWords = new();
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
                node = node.Children.ChildAt(EnglishLowercase.IndexOf(_stream.Get(i)));

                if (node is not null && node.HasValue)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
