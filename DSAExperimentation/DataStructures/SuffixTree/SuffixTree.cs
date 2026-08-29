// A plain "using DSAExperimentation.DataStructures.SuffixArray;" plus a bare SuffixArray reference
// hits CS0118 ("SuffixArray is a namespace but is used like a type") from any file outside that
// namespace's own folder - the ARCHITECTURE.md §10.3/§15.3 namespace-segment/type-name collision,
// triggered here by a genuine cross-domain composition rather than same-namespace self-reference
// (which never collides, see RollingHash.cs referencing RollingHash from within its own namespace).
using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;

namespace DSAExperimentation.DataStructures.SuffixTree;

// The compressed trie of every suffix of a fixed text, each edge labeled by a (Start, Length) index
// range into that text rather than a copied substring. Built once from a SuffixArray (composing its
// already-public Suffixes/LongestCommonPrefixArray - licensed by the same rule DynamicArraySequence uses to compose
// DynamicArray, ARCHITECTURE.md §5 step 5/§9.4: composing another domain's already-public concrete
// representation is fine, only forcing that domain to implement your own interface is prohibited),
// then answering HasSubstring/FindOccurrences queries repeatedly.
//
// Deliberately NOT Ukkonen's online algorithm. Every stateful text Representation in this repo
// (RollingHash, SuffixArray, SuffixAutomaton) is batch/offline - full input known upfront,
// precompute once - so Ukkonen's entire complexity budget (three-coordinate active point,
// mid-algorithm suffix links, the rule-2/rule-3 case split, the global-end trick) buys online
// growth nobody here needs, at a well-documented high real-world bug rate. Building from a
// SuffixArray's sorted suffixes and LCP heights via a stack of (node, string-depth) pairs is the
// same trade Manacher's own doc comment already makes one level up (reject the textbook
// presentation for a simpler, equally-correct alternative when one exists) - and is hand-traceable,
// unlike Ukkonen's own canonical teaching examples.
//
// No Topology witness: exactly one tree shape exists for a given text (a deterministic function of
// it), the same "nothing for a witness to parametrize" reasoning Trie.cs already gives for itself -
// and stronger than Trie's own guarantee, since SuffixTree exposes no public mutation API at all
// after construction.
internal sealed class SuffixTree
{
    private readonly string _text;
    private readonly SuffixTreeNode _root;

    public SuffixTree(ReadOnlySpan<char> text)
        : this(text, new SuffixArrayStructure(text))
    {
    }

    // suffixArray must have been built over this exact text - unenforced precondition, the same
    // shape as RollingHash's own cross-instance lane/comparer law.
    public SuffixTree(ReadOnlySpan<char> text, SuffixArrayStructure suffixArray)
    {
        _text = text.ToString();
        _root = BuildFromSuffixArray(_text, suffixArray);
    }

    // O(pattern.Length) average, independent of text length, contingent on SuffixTreeNode.Children
    // giving O(1)-average per-character dispatch - a slower Children representation would silently
    // degrade this with no compiler error. The empty pattern trivially occurs.
    public bool HasSubstring(ReadOnlySpan<char> pattern) => pattern.IsEmpty || Walk(pattern) is not null;

    // O(pattern.Length + occurrences) average: one walk to the match point, then a subtree
    // enumeration bounded by the number of leaves under it (an internal node's count is bounded by
    // its own leaf count in any tree). Order is unspecified.
    //
    // The empty pattern returns every real suffix start, {0, .., text.Length - 1} - not the
    // "every insertion point including text.Length itself" convention this repo's other FindAll-
    // family members use for an empty pattern. A suffix tree indexes actual (non-empty-context)
    // suffixes, one leaf per starting index; there is no leaf for the degenerate empty suffix at
    // text.Length, the same convention SuffixArray.Suffixes itself already uses (a permutation of
    // 0..n-1, never including n).
    public IEnumerable<int> FindOccurrences(ReadOnlySpan<char> pattern)
    {
        if (pattern.IsEmpty)
        {
            return AllSuffixStartsFrom(_root);
        }

        var node = Walk(pattern);
        return node is null ? NoOccurrences() : AllSuffixStartsFrom(node);
    }

    // Iterative (explicit stack, not recursion) - the same stack-overflow-risk-class avoidance
    // already applied to SuffixAutomaton's endpos precompute, since a degenerate-depth suffix tree
    // (e.g. text of one repeated character) can chain as deep as text.Length.
    private static IEnumerable<int> AllSuffixStartsFrom(SuffixTreeNode subtreeRoot)
    {
        var pending = new Stack<SuffixTreeNode>();
        pending.Push(subtreeRoot);

        while (pending.Count > 0)
        {
            var node = pending.Pop();

            if (node.IsSuffixEnd)
            {
                yield return node.SuffixStart;
            }

            foreach (var key in node.Children.Keys)
            {
                var child = RequireChild(node, key);
                pending.Push(child);
            }
        }
    }

    // presumption: allow -- key is only ever drawn from node.Children.Keys immediately before this
    // call, so it is guaranteed present in node.Children; TryGetValue's own bool result is
    // redundant at this exact call shape, not a real "might be missing" check.
    private static SuffixTreeNode RequireChild(SuffixTreeNode node, char key)
    {
        node.Children.TryGetValue(key, out var child);
        return child!;
    }

    private static IEnumerable<int> NoOccurrences() => [];

    // Stack-based construction: a stack of (node, string-depth) pairs tracks the path from root to
    // the rightmost leaf inserted so far. Each new suffix (in suffix-array order) either attaches
    // directly under the stack's current top (its own depth already equals the LCP height, the
    // "leaf becomes internal" case - no special code needed, see SuffixTreeNode's own doc comment)
    // or requires splitting the edge just popped past (see SplitEdge).
    private static SuffixTreeNode BuildFromSuffixArray(string text, SuffixArrayStructure suffixArray)
    {
        var context = new BuildContext(text, text.Length);
        var root = new SuffixTreeNode();

        if (context.N == 0)
        {
            return root;
        }

        var suffixes = suffixArray.Suffixes;
        var stack = InitializeRightmostPathStack(context, root, suffixes[0]);

        for (var i = 1; i < context.N; i++)
        {
            InsertSuffix(context, suffixes[i], suffixArray.LongestCommonPrefixArray[i - 1], stack);
        }

        return root;
    }

    private static Stack<(SuffixTreeNode Node, int Depth)> InitializeRightmostPathStack(
        BuildContext context, SuffixTreeNode root, int firstSuffixStart)
    {
        var stack = new Stack<(SuffixTreeNode Node, int Depth)>();
        stack.Push((root, 0));

        var firstLeaf = CreateLeaf(firstSuffixStart, firstSuffixStart, context.N);
        root.Children.Set(context.Text[firstLeaf.Start], firstLeaf);
        stack.Push((firstLeaf, context.N - firstSuffixStart));

        return stack;
    }

    private static void InsertSuffix(BuildContext context, int suffixStart, int branchDepth, Stack<(SuffixTreeNode Node, int Depth)> stack)
    {
        var lastPopped = stack.Peek();

        while (stack.Peek().Depth > branchDepth)
        {
            lastPopped = stack.Pop();
        }

        var top = stack.Peek();

        if (top.Depth != branchDepth)
        {
            var splitNode = SplitEdge(context.Text, top.Node, lastPopped.Node, branchDepth - top.Depth);
            stack.Push((splitNode, branchDepth));
            top = (splitNode, branchDepth);
        }

        var newLeaf = AttachLeaf(context, top.Node, suffixStart, branchDepth);
        stack.Push(newLeaf);
    }

    // Splits child's edge at the given offset, inserting a new branch node between parent and
    // child. InsertSplitNode's read of the ORIGINAL first character must happen (and does, by
    // running to completion first) before ReparentShrunkChild mutates Start/Length - reversing
    // that order would silently key the parent/child maps on the wrong characters, since both
    // keys are derived from the same mutable field.
    private static SuffixTreeNode SplitEdge(string text, SuffixTreeNode parent, SuffixTreeNode child, int offset)
    {
        var splitNode = InsertSplitNode(text, parent, child, offset);
        ReparentShrunkChild(text, splitNode, child, offset);
        return splitNode;
    }

    private static SuffixTreeNode InsertSplitNode(string text, SuffixTreeNode parent, SuffixTreeNode child, int offset)
    {
        var originalKey = text[child.Start];
        var splitNode = new SuffixTreeNode { Start = child.Start, Length = offset };
        parent.Children.Set(originalKey, splitNode);
        return splitNode;
    }

    private static void ReparentShrunkChild(string text, SuffixTreeNode splitNode, SuffixTreeNode child, int offset)
    {
        child.Start += offset;
        child.Length -= offset;
        splitNode.Children.Set(text[child.Start], child);
    }

    private static (SuffixTreeNode Node, int Depth) AttachLeaf(BuildContext context, SuffixTreeNode parent, int suffixStart, int branchDepth)
    {
        var leaf = CreateLeaf(suffixStart, suffixStart + branchDepth, context.N);
        parent.Children.Set(context.Text[leaf.Start], leaf);
        return (leaf, context.N - suffixStart);
    }

    // Walk is shared by BOTH HasSubstring AND FindOccurrences (two distinct callers), placed here last
    // for that reason - contrast BuildFromSuffixArray/InitializeRightmostPathStack/InsertSuffix/
    // SplitEdge/InsertSplitNode/ReparentShrunkChild/AttachLeaf above, each with exactly one caller,
    // placed immediately after it in DFS call order.
    private SuffixTreeNode? Walk(ReadOnlySpan<char> pattern)
    {
        var current = _root;
        var consumed = 0;

        while (consumed < pattern.Length)
        {
            if (!current.Children.TryGetValue(pattern[consumed], out var next))
            {
                return null;
            }

            var edgeMatchLength = Math.Min(next.Length, pattern.Length - consumed);

            if (!HasMatchingEdgeCharacters(next, consumed, edgeMatchLength, pattern))
            {
                return null;
            }

            consumed += edgeMatchLength;
            current = next;
        }

        return current;
    }

    private bool HasMatchingEdgeCharacters(SuffixTreeNode edge, int consumed, int edgeMatchLength, ReadOnlySpan<char> pattern)
    {
        for (var offset = 0; offset < edgeMatchLength; offset++)
        {
            if (_text[edge.Start + offset] != pattern[consumed + offset])
            {
                return false;
            }
        }

        return true;
    }

    private static SuffixTreeNode CreateLeaf(int suffixStart, int edgeStart, int textLength)
        => new() { Start = edgeStart, Length = textLength - edgeStart, SuffixStart = suffixStart };

    // Bundles the text/length pair every construction-time helper needs beyond its own per-suffix
    // arguments - both fixed for the whole build, the same "least-frequently-varying args" grouping
    // SuffixArray's own DoublingContext/KasaiContext already use.
    private readonly record struct BuildContext(string Text, int N);
}
