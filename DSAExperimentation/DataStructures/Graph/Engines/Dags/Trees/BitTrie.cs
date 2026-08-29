namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees;

// A fixed-32-level binary trie over int's two's-complement bit pattern (bit 31,
// the sign bit, down to bit 0) - the Maximum XOR of Two Numbers in an Array
// (LeetCode 421) family's building block: insert every candidate's bits, then
// greedily walk toward the OPPOSITE bit at each level (maximizes that bit
// position's contribution) to find the best achievable XOR against a query value.
//
// Real ITreeTopology witness (BitTrieTopology.cs, BitTrieChildren.cs): every
// BitTrieNode has real identity (a class, not an index into shared storage) and its
// two named Zero/One slots already give IChildren's O(1)-indexed-Get obligation
// honestly, same as BinaryTreeNode's Left/Right - see ARCHITECTURE.md §13.8's
// corrected framing for why "every BitTrieNode is only ever created internally, so
// nothing external could vary the shape" (this file's own earlier reasoning) answers
// the wrong question: determinism was never the actual test for whether ITreeTopology
// applies at all, only for whether a SECOND, injected witness axis is also needed.
// Insert/TryMaxXor stay hand-written, walking Zero/One directly - neither
// generalizes past "exactly two children keyed by one bit" the way a real n-ary walk
// would help - but the witness opens this trie to every generic Tree-tier engine
// (TreeMetrics, LowestCommonAncestor, DepthFirstTraversal, ...) for free; see
// BitTrieTests for a worked TreeMetrics.Size example. A hypothetical path-compressed
// (PATRICIA-style) bit-trie would still only ever be a second Representation, never a
// second Topology.
//
// The "prefer opposite bit" rule is NOT a Topology witness either, even though it
// superficially resembles IHeapOrder's closed two-variant (Min/Max) shape - per
// §13.2's IVisitGuard discriminator, what matters is what the closed choice is
// ABOUT: IHeapOrder decides a relation on stored data, checked by the very
// operations (Push/Pop's sift) that determine the heap's physical array layout, so
// two different IHeapOrder choices really do produce two different physically
// admissible outputs from the same input sequence. This bit preference only
// decides how TryMaxXor's OWN walk executes over an already-fixed trie shape -
// Insert never consults it, and no sequence of Insert calls ever produces two
// different trie shapes depending on it: there is exactly one trie per inserted
// set, full stop. If a MinXor sibling is ever added, it belongs here as a second
// named method (or at most an execution-strategy object in the
// IFoldEvaluationStrategy/IVisitGuard family) - never as a static-abstract
// Topology interface, no matter how many variants accumulate; see IPathHeuristic
// (§3.3) for the same "injected query-time axis, deliberately not a Topology
// witness" shape.
//
// MSB-first (bit 31 down to bit 0) is not a caller-facing choice the way heap
// order is - it is a correctness requirement, not a variant: a higher bit position
// always dominates every lower one in magnitude, so greedy-maximize only produces
// the true maximum walking most-significant-bit-first.
//
// "Maximum XOR" here means the greedy walk maximizes the result's bit pattern
// interpreted as UNSIGNED - correct and unambiguous for LeetCode 421's own
// constraint (0 <= nums[i] <= 2^31-1, so bit 31 is always 0 on every inserted
// value and can never be set in the result either). Once negative ints are
// admitted at all, this stops being the only defensible "maximum": the pattern
// with bit 31 set is the unsigned-maximal one but casts back to a NEGATIVE int
// (via unchecked), not the signed-maximal XOR. TryMaxXor always returns the
// unsigned-maximal pattern; callers working with values outside LC 421's
// non-negative range must account for that sign flip themselves.
//
// Insert/TryMaxXor are O(32) = O(1), bounded by int's fixed width, the same
// "operation cost is a real, stated contract" convention Trie.cs/Heap.cs open
// with.
internal sealed class BitTrie
{
    private const int BitWidth = 32;

    private readonly BitTrieNode _root = new();

    // Counts Insert calls, including duplicate values - not distinct values the
    // way Trie<TValue>.Count is (Trie.cs's HasValue flag has no equivalent here;
    // no per-leaf marker exists to detect "this exact value was already present"
    // without one). The only consumer is TryMaxXor's emptiness guard below, which
    // needs nothing sharper than "has Insert ever been called."
    public int Count { get; private set; }

    public BitTrieNode Root => _root;

    public void Insert(int value)
    {
        var current = _root;
        var bits = unchecked((uint)value);

        for (var i = BitWidth - 1; i >= 0; i--)
        {
            var bit = (bits >> i) & 1u;
            current = bit == 0 ? GetOrCreateZero(current) : GetOrCreateOne(current);
        }

        Count++;
    }

    private static BitTrieNode GetOrCreateZero(BitTrieNode node) => node.Zero ??= new BitTrieNode();

    private static BitTrieNode GetOrCreateOne(BitTrieNode node) => node.One ??= new BitTrieNode();

    public bool TryMaxXor(int value, out int result)
    {
        if (Count == 0)
        {
            result = 0;
            return false;
        }

        result = unchecked((int)Walk(value));
        return true;
    }

    // Precondition: Count > 0, guaranteed by TryMaxXor's own check above. Every
    // Insert walks all BitWidth levels before returning, so once the trie is
    // non-empty, every node reached mid-walk here already has >=1 non-null child -
    // "opposite absent" plus ">=1 exists" forces "same-bit present" by
    // elimination between exactly two possible children, with no null branch left
    // to defend against the way Trie.FindNode's caller-supplied-key walk must (a
    // BitTrie walk is never handed a key that might not have been inserted; it
    // always completes all BitWidth levels for the query value itself).
    private uint Walk(int value)
    {
        var current = _root;
        var bits = unchecked((uint)value);
        var xor = 0u;

        for (var i = BitWidth - 1; i >= 0; i--)
        {
            var bit = (bits >> i) & 1u;
            var (next, matchedOpposite) = DescendOneLevel(current, bit);

            if (matchedOpposite)
            {
                xor |= 1u << i;
            }

            current = next;
        }

        return xor;
    }

    private static (BitTrieNode Next, bool MatchedOpposite) DescendOneLevel(BitTrieNode current, uint bit)
    {
        var desiredIsOne = bit == 0;
        var desired = desiredIsOne ? current.One : current.Zero;

        if (desired is not null)
        {
            return (desired, true);
        }

        // presumption: allow -- the opposite child is absent, and every node
        // reached here has >=1 non-null child by construction (see Walk's own
        // precondition note), so the same-bit child must be present.
        return ((desiredIsOne ? current.Zero : current.One)!, false);
    }
}
