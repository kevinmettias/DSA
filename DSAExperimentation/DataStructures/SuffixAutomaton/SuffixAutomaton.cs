using DSAExperimentation.DataStructures.SubstringIndex;

namespace DSAExperimentation.DataStructures.SuffixAutomaton;

// The minimal automaton (in the online-construction/"Blumer et al." sense) recognizing exactly the
// substrings of a fixed text - built once in O(text.Length) amortized states/transitions, then
// answering HasSubstring/CountOccurrences queries repeatedly. Same "precompute once, query many
// times" stateful-Representation shape as RollingHash/SuffixArray.
//
// No IEqualityComparer<char> parameter, following Trie's own precedent rather than RollingHash's:
// SuffixAutomatonState.Transitions is exact-key dispatch (structurally identical to
// TrieNode.Children), not an encoding step into a numeric hash domain the way RollingHash's
// comparer is.
//
// The suffix-link tree invariant (Length[state] > Length[Link[state]] for every non-root state) is
// established once during construction and never re-touched afterward - stronger than an external
// precondition like BinarySearch's sortedness, since no public method here mutates state after
// construction at all, the same "no caller-supplied shape could ever violate it" guarantee Trie.cs
// already claims for its own prefix-tree invariant.
//
// Of the substring-index capabilities (ARCHITECTURE §8) it answers presence, counting and the
// longest repeat, and not locating: a state knows how many positions it ends at and the first of
// them, not the rest. Nor does it convert to or from the suffix tree or array of its text cheaply:
// its suffix-link tree is the suffix tree of the reversed text, so reaching the text's own tree
// takes the automaton of the reversed text, and building this automaton afresh is O(n) anyway.
/// <summary>
/// The minimal automaton that accepts exactly the substrings of a fixed text, built once at construction.
/// </summary>
public sealed class SuffixAutomaton : ICountingSubstringIndex, ILongestRepeatIndex
{
    private const int RootIndex = 0;
    private const int NotFound = -1;

    // A substring repeats when it ends at two positions at least.
    private const int RepeatedEndCount = 2;

    // OrderByLengthDescending's bucket array is indexed by a state's own Length + 1 (one slot of
    // headroom so a zero-length state has somewhere to prefix-sum from) and needs one further slot
    // past the largest such index for the exclusive prefix-sum upper bound - two slots of padding
    // beyond the largest attainable per-state Length, which is bounded by the automaton's own
    // Length (the whole text's length).
    private const int BucketCountPadding = 2;

    private readonly List<SuffixAutomatonState> _states = new();
    private readonly int[] _endposSize;

    /// <summary>The length of the text the automaton was built from.</summary>
    public int Length { get; }

    /// <summary>The number of distinct non-empty substrings of the text, zero for an empty text.</summary>
    public long DistinctSubstringCount { get; }

    public SuffixAutomaton(ReadOnlySpan<char> text)
    {
        Length = text.Length;
        _states.Add(new SuffixAutomatonState());

        var last = RootIndex;

        foreach (var character in text)
        {
            last = Extend(last, character);
        }

        (_endposSize, DistinctSubstringCount) = ComputeEndposSizesAndDistinctCount();
    }

    // O(pattern.Length): true iff pattern occurs anywhere in text. The empty pattern trivially
    // occurs (TryWalk of zero characters always resolves to the root state), regardless of Length.
    /// <summary>Tests whether a pattern occurs in the text, in time linear in the pattern's length.</summary>
    /// <param name="pattern">The characters to look for, compared exactly.</param>
    /// <returns>True when the pattern occurs in the text; always true for an empty pattern.</returns>
    public bool HasSubstring(ReadOnlySpan<char> pattern) => TryWalk(pattern) != NotFound;

    // O(pattern.Length): occurrence count, overlaps included; 0 if pattern never occurs. The empty
    // pattern is handled separately rather than via the root state's own accumulated endposSize -
    // root's accumulated value equals text.Length (the sum of every real state's direct
    // contribution), not the Length + 1 "every insertion point" convention this repo's other
    // FindAll-family members already use for an empty pattern.
    /// <summary>
    /// Counts a pattern's occurrences in the text, overlaps included, in time linear in the pattern's length.
    /// </summary>
    /// <param name="pattern">The characters to count, compared exactly.</param>
    /// <returns>The count, which is 0 for an absent pattern and <c>Length + 1</c> for an empty one.</returns>
    public int CountOccurrences(ReadOnlySpan<char> pattern)
    {
        if (pattern.IsEmpty)
        {
            return Length + 1;
        }

        var state = TryWalk(pattern);
        return state == NotFound ? 0 : OccurrenceCountAt(state);
    }

    private int OccurrenceCountAt(int state) => _endposSize[state];

    // O(states): every substring in a state ends at the same positions, so the longest repeat is
    // the longest string of the longest state that ends at two of them, and it ends at the state's
    // first end position. The root, with no string longer than the empty one, is the answer when no
    // state repeats; its first end, one before the text, puts the empty answer at position 0.
    /// <summary>
    /// Finds the longest substring that occurs at least twice in the text, in O(text.Length).
    /// </summary>
    /// <returns>
    /// Its first occurrence; <see cref="SubstringOccurrence.Empty"/> when no character repeats.
    /// </returns>
    public SubstringOccurrence FindLongestRepeat()
    {
        var longest = RootIndex;

        for (var state = RootIndex + 1; state < _states.Count; state++)
        {
            var isRepeated = _endposSize[state] >= RepeatedEndCount;

            if (isRepeated && _states[state].Length > _states[longest].Length)
            {
                longest = state;
            }
        }

        var length = _states[longest].Length;
        return new SubstringOccurrence(_states[longest].FirstEnd - length + 1, length);
    }

    // The classic online extension step: create a new state for the whole-prefix-so-far
    // equivalence class, walk up Link from the previous "last" state adding a transition to it
    // until either running off the top (root's Link, -1) or finding an existing transition for character -
    // then either reuse that transition's target directly (its Length is already consistent) or
    // CLONE it (split it into two states, preserving its transitions/link) before linking cur to
    // the clone.
    private int Extend(int last, char character)
    {
        var cur = NewState();
        _states[cur].Length = _states[last].Length + 1;
        _states[cur].FirstEnd = _states[last].Length;

        var ancestorState = last;

        while (ancestorState != -1 && !_states[ancestorState].Transitions.HasKey(character))
        {
            _states[ancestorState].Transitions.Set(character, cur);
            ancestorState = _states[ancestorState].Link;
        }

        _states[cur].Link = ancestorState == -1 ? RootIndex : LinkForExistingTransition(ancestorState, character);
        return cur;
    }

    private int LinkForExistingTransition(int ancestorState, char character)
    {
        _states[ancestorState].Transitions.TryGetValue(character, out var existingTarget);
        var transitionAlreadyConsistent = _states[existingTarget].Length == _states[ancestorState].Length + 1;
        return transitionAlreadyConsistent ? existingTarget : CloneState(ancestorState, existingTarget, character);
    }

    // Splits existingTarget into two states: clone (taking over existingTarget's transitions and Link) and existingTarget itself
    // (unchanged Length/transitions, now pointing its own Link at clone). clone.Link MUST be read
    // from existingTarget's OLD Link before the final _states[existingTarget].Link = clone assignment overwrites it -
    // swapping this order compiles fine and silently corrupts the link tree.
    private int CloneState(int ancestorState, int existingTarget, char character)
    {
        var clone = NewState();
        _states[clone].Length = _states[ancestorState].Length + 1;
        _states[clone].Link = _states[existingTarget].Link;
        _states[clone].FirstEnd = _states[existingTarget].FirstEnd;
        _states[clone].IsClone = true;
        CopyTransitions(_states[existingTarget], _states[clone]);
        RewireTransitionsToClone(ancestorState, character, existingTarget, clone);
        _states[existingTarget].Link = clone;
        return clone;
    }

    private static void CopyTransitions(SuffixAutomatonState source, SuffixAutomatonState destination)
    {
        foreach (var key in source.Transitions.Keys)
        {
            source.Transitions.TryGetValue(key, out var target);
            destination.Transitions.Set(key, target);
        }
    }

    private void RewireTransitionsToClone(int ancestorState, char character, int existingTarget, int clone)
    {
        var walkState = ancestorState;

        while (walkState != -1 && HasTransitionTo(_states[walkState], character, existingTarget))
        {
            _states[walkState].Transitions.Set(character, clone);
            walkState = _states[walkState].Link;
        }
    }

    private static bool HasTransitionTo(SuffixAutomatonState state, char character, int target)
        => state.Transitions.TryGetValue(character, out var actual) && actual == target;

    // No recursion (avoids the stack-overflow risk class a degenerate-depth link tree could
    // trigger, the same reason BinaryTree's own traversal utilities avoid unbounded recursion
    // depth): a counting sort by Length (bounded [0, Length], so O(states + Length), never a
    // comparison sort) gives a descending-Length order, then one linear propagation pass accumulates
    // each non-clone/non-root state's direct occurrence contribution of 1 up through Link, and
    // - in the same pass, order-independent - sums Length[state] - Length[Link[state]] over every
    // non-root state for the total distinct-substring count.
    private (int[] EndposSize, long DistinctSubstringCount) ComputeEndposSizesAndDistinctCount()
    {
        var endposSize = InitialDirectContributions();
        var distinctSubstringCount = 0L;

        foreach (var state in OrderByLengthDescending())
        {
            if (state == RootIndex)
            {
                continue;
            }

            var link = _states[state].Link;
            distinctSubstringCount += _states[state].Length - _states[link].Length;
            endposSize[link] += endposSize[state];
        }

        return (endposSize, distinctSubstringCount);
    }

    private int[] InitialDirectContributions()
    {
        var endposSize = new int[_states.Count];

        for (var i = 0; i < _states.Count; i++)
        {
            if (i != RootIndex && !_states[i].IsClone)
            {
                endposSize[i] = 1;
            }
        }

        return endposSize;
    }

    private int[] OrderByLengthDescending()
    {
        var bucketStart = new int[Length + BucketCountPadding];

        foreach (var state in _states)
        {
            bucketStart[state.Length + 1]++;
        }

        for (var len = 0; len <= Length; len++)
        {
            bucketStart[len + 1] += bucketStart[len];
        }

        var order = new int[_states.Count];
        var cursor = (int[])bucketStart.Clone();

        for (var i = 0; i < _states.Count; i++)
        {
            order[cursor[_states[i].Length]++] = i;
        }

        Array.Reverse(order);
        return order;
    }

    private int TryWalk(ReadOnlySpan<char> pattern)
    {
        var current = RootIndex;

        foreach (var character in pattern)
        {
            if (!_states[current].Transitions.TryGetValue(character, out var next))
            {
                return NotFound;
            }

            current = next;
        }

        return current;
    }

    private int NewState()
    {
        _states.Add(new SuffixAutomatonState());
        return _states.Count - 1;
    }
}
