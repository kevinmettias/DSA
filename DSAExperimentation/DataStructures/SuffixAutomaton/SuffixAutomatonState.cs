using DSAExperimentation.DataStructures.HashMap;

namespace DSAExperimentation.DataStructures.SuffixAutomaton;

// One state of the suffix automaton. A `class`, not a `struct`: construction repeatedly
// field-mutates already-created states in place through SuffixAutomaton's own backing List<T>
// indexer (list[i].Field = x is CS1612 for a mutable struct element), the same reason
// TrieNode<TValue> is a class rather than a struct.
internal sealed class SuffixAutomatonState
{
    // Length of the longest string in this state's equivalence class.
    public int Length { get; set; }

    // Suffix link to another state; -1 only for the root (state 0).
    public int Link { get; set; } = -1;

    // True for a state created by CloneState rather than by the main extension step - the
    // discriminator SuffixAutomaton's endpos-size precompute uses to decide which states get a
    // direct occurrence contribution of 1 (every non-root, non-clone state) versus 0 (root and
    // every clone, which only ever accumulate contributions from their own children).
    public bool IsClone { get; set; }

    public HashMap<char, int> Transitions { get; } = new();
}
