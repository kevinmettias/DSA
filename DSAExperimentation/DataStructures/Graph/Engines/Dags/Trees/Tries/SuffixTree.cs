using DSAExperimentation.DataStructures.SubstringIndex;

// A plain "using DSAExperimentation.DataStructures.SuffixArray;" plus a bare SuffixArray reference
// hits CS0118 ("SuffixArray is a namespace but is used like a type") from any file outside that
// namespace's own folder - the ARCHITECTURE.md §10.3/§15.3 namespace-segment/type-name collision,
// triggered here by a genuine cross-domain composition rather than same-namespace self-reference
// (which never collides, see RollingHash.cs referencing RollingHash from within its own namespace).
using SuffixArrayStructure = DSAExperimentation.DataStructures.SuffixArray.SuffixArray;
using SuffixWalk = DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.TrieWalk<
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.SuffixTreeNode,
    DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.HashedNodeChildren<
        DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries.SuffixTreeNode>,
    char>;

namespace DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;

// The compressed trie of every suffix of a fixed text: the one trie (§13.8) with its edges
// compressed, each labelled by a (Start, Length) range of that text rather than a copied
// substring. Three injections make it one, and they are what tells it from the other
// configurations:
// - its node class, SuffixTreeNode, carries the edge into each node, the suffix ending there and
//   how many suffixes run through it;
// - its store, HashedNodeChildren, keys a node's children by the first letter on each edge, any
//   character, in a hash map;
// - its edges are read through SuffixTreeEdges, so a pattern walks it by TrieWalk's edge-label
//   walk, which finds each child by its edge's first letter as every trie does and matches the
//   rest of the edge against the text.
// Built once from a SuffixArray (composing its already-public Suffixes/LongestCommonPrefixArray -
// licensed by the same rule DynamicArraySequence uses to compose DynamicArray, ARCHITECTURE.md §5
// step 5/§9.4: composing another domain's already-public concrete representation is fine, only
// forcing that domain to implement your own interface is prohibited), then answering the
// substring-index queries (ARCHITECTURE §8) repeatedly: presence, counting and locating in time
// linear in the pattern, and the longest repeat in time linear in the tree.
//
// The two conversions between this tree and a suffix array are both standard and cheap. A suffix
// array with its common prefix lengths becomes this tree through the constructor, by the stack
// build below. This tree becomes a suffix array through ToSuffixArray: a depth-first walk that
// takes each node's children in character order meets the suffixes in sorted order, and two
// neighbours share the shallowest depth the walk passes between them.
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
// No Topology witness: its store is hashed and keeps no order, so the compiler withholds
// TrieNodeTopology, which asks for an ordered store, as it does from every hashed trie (§13.8).
/// <summary>
/// The compressed trie of every suffix of a fixed text, built from its suffix array at construction.
/// </summary>
public sealed class SuffixTree
    : ICountingSubstringIndex, ILocatingSubstringIndex, ILongestRepeatIndex
{
    // A substring repeats when at least two suffixes start with it.
    private const int RepeatedSuffixCount = 2;
    private const int NoOccurrence = 0;

    private readonly string _text;
    private readonly SuffixTreeNode _root;

    public SuffixTree(ReadOnlySpan<char> text)
        : this(text, new SuffixArrayStructure(text))
    {
    }

    // suffixArray must have been built over this exact text with the default comparer - unenforced
    // precondition, the same shape as RollingHash's own cross-instance lane/comparer law. The
    // children are keyed by exact character, so a comparer that calls two characters equal would
    // hand this build common prefixes the tree cannot branch on.
    public SuffixTree(ReadOnlySpan<char> text, SuffixArrayStructure suffixArray)
    {
        _text = text.ToString();
        _root = BuildFromSuffixArray(_text, suffixArray);
    }

    // O(pattern.Length) average, independent of text length, contingent on the hashed store giving
    // O(1)-average per-character dispatch - a slower store would silently degrade this with no
    // compiler error. The empty pattern trivially occurs.
    /// <summary>Tests whether a pattern occurs in the text, in time linear in the pattern's length.</summary>
    /// <param name="pattern">The characters to look for, compared exactly.</param>
    /// <returns>True when the pattern occurs in the text; always true for an empty pattern.</returns>
    public bool HasSubstring(ReadOnlySpan<char> pattern) => pattern.IsEmpty || Walk(pattern) is not null;

    // O(pattern.Length) average, the walk HasSubstring takes: every suffix that starts with the
    // pattern passes through the node the walk ends at, and the build counted those as it went.
    /// <summary>
    /// Counts a pattern's occurrences, overlapping ones included, in time linear in the pattern's
    /// length.
    /// </summary>
    /// <param name="pattern">The characters to count, compared exactly.</param>
    /// <returns>
    /// The count: 0 for an absent pattern, and the text's length plus one for an empty one.
    /// </returns>
    public int CountOccurrences(ReadOnlySpan<char> pattern)
    {
        if (pattern.IsEmpty)
        {
            return _text.Length + 1;
        }

        return Walk(pattern)?.SuffixCount ?? NoOccurrence;
    }

    // O(pattern.Length + occurrences) average: one walk to the match point, then a subtree
    // enumeration bounded by the number of leaves under it (an internal node's count is bounded by
    // its own leaf count in any tree). Order is unspecified.
    //
    // The empty pattern occurs at every insertion point, 0 through text.Length, as the contract
    // says. The tree has no leaf for the empty suffix at text.Length, so that answer is the range
    // itself rather than a walk.
    /// <summary>
    /// Lists each position where a pattern starts in the text, overlaps included, in no particular order.
    /// </summary>
    /// <param name="pattern">The characters to look for, compared exactly.</param>
    /// <returns>
    /// The start positions: none for an absent pattern, and every position from 0 to the text's
    /// length for an empty one.
    /// </returns>
    public IEnumerable<int> FindOccurrences(ReadOnlySpan<char> pattern)
    {
        if (pattern.IsEmpty)
        {
            return Enumerable.Range(0, _text.Length + 1);
        }

        var node = Walk(pattern);
        return node is null ? NoOccurrences() : AllSuffixStartsFrom(node);
    }

    private static IEnumerable<int> NoOccurrences() => [];

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

            foreach (var key in node.Children.Letters)
            {
                var child = RequireChild(node, key);
                pending.Push(child);
            }
        }
    }

    // The deepest node that two suffixes pass through spells the longest repeat. A node fewer than
    // two suffixes pass through has no such node below it, since a child's count is part of its
    // parent's, so the walk prunes there and visits only the branching part of the tree. A node's
    // edge ends where its path does, so the path starts its depth before that end. Iterative for
    // the same reason as AllSuffixStartsFrom.
    /// <summary>
    /// Finds the longest substring that occurs at least twice in the text, in O(text.Length).
    /// </summary>
    /// <returns>
    /// One of its occurrences, any one when several longest repeats tie;
    /// <see cref="SubstringOccurrence.Empty"/> when no character repeats.
    /// </returns>
    public SubstringOccurrence FindLongestRepeat()
    {
        var longest = SubstringOccurrence.Empty;
        var pending = new Stack<(SuffixTreeNode Node, int Depth)>();
        pending.Push((_root, 0));

        while (pending.Count > 0)
        {
            var (node, depth) = pending.Pop();

            if (node.SuffixCount >= RepeatedSuffixCount)
            {
                longest = LongerOf(longest, node, depth);
                PushChildren(pending, node, depth);
            }
        }

        return longest;
    }

    // The node's path, if it is longer than the longest repeat found so far.
    private static SubstringOccurrence LongerOf(
        SubstringOccurrence longest, SuffixTreeNode node, int depth)
    {
        if (depth <= longest.Length)
        {
            return longest;
        }

        return new SubstringOccurrence(node.Start + node.Length - depth, depth);
    }

    private static void PushChildren(
        Stack<(SuffixTreeNode Node, int Depth)> pending, SuffixTreeNode node, int depth)
    {
        foreach (var child in node.Children.Nodes)
        {
            pending.Push((child, depth + child.Length));
        }
    }

    // O(text.Length * alphabet log alphabet): each node's children are sorted once. A suffix that
    // ends at a node is a prefix of every suffix below it, so it sorts first and is taken before
    // the children. Between two suffixes taken one after the other the walk climbs to their lowest
    // common ancestor and descends again, and no node it passes is shallower than that ancestor, so
    // the shallowest depth it passes is their common prefix length.
    /// <summary>
    /// Reads the suffix array, with its common prefix lengths, off the tree's leaves.
    /// </summary>
    /// <returns>The suffix array of the text, under the default comparer's ordinal order.</returns>
    public SuffixArrayStructure ToSuffixArray()
    {
        var walk = new SortedLeafWalk(_text.Length);
        var pending = new Stack<(SuffixTreeNode Node, int Depth)>();
        pending.Push((_root, 0));

        while (pending.Count > 0)
        {
            var (node, depth) = pending.Pop();
            walk.Pass(depth - node.Length);

            if (node.IsSuffixEnd)
            {
                walk.Take(node.SuffixStart, depth);
            }

            PushChildrenInReverseOrder(pending, node, depth);
        }

        return new SuffixArrayStructure(walk.Suffixes, walk.NeighbourPrefixes);
    }

    // Largest character first, so the smallest is popped first.
    private static void PushChildrenInReverseOrder(
        Stack<(SuffixTreeNode Node, int Depth)> pending, SuffixTreeNode node, int depth)
    {
        var keys = node.Children.Letters.ToArray();
        Array.Sort(keys);

        for (var i = keys.Length - 1; i >= 0; i--)
        {
            var child = RequireChild(node, keys[i]);
            pending.Push((child, depth + child.Length));
        }
    }

    // Stack-based construction: a stack of path nodes tracks the path from root to the rightmost
    // leaf inserted so far. Each new suffix (in suffix-array order) either attaches directly under
    // the stack's current top (its own depth already equals the LCP height, the "leaf becomes
    // internal" case - no special code needed, see SuffixTreeNode's own doc comment) or requires
    // splitting the edge just popped past (see SplitEdge). Each path node carries the rank of the
    // first suffix below it, so a node leaving the path knows how many suffixes it holds.
    private static SuffixTreeNode BuildFromSuffixArray(string text, SuffixArrayStructure suffixArray)
    {
        var context = new BuildContext(
            text, suffixArray.Suffixes, suffixArray.LongestCommonPrefixArray);
        var root = new SuffixTreeNode();

        if (context.TextLength == 0)
        {
            return root;
        }

        var stack = InitializeRightmostPathStack(context, root);

        for (var rank = 1; rank < context.TextLength; rank++)
        {
            InsertSuffix(context, rank, stack);
        }

        while (stack.Count > 0)
        {
            Finish(stack.Pop(), context.TextLength);
        }

        return root;
    }

    private static Stack<PathNode> InitializeRightmostPathStack(
        BuildContext context, SuffixTreeNode root)
    {
        var stack = new Stack<PathNode>();
        stack.Push(new PathNode(root, 0, 0));

        var firstSuffixStart = context.Suffixes[0];
        var firstLeaf = CreateLeaf(firstSuffixStart, firstSuffixStart, context.TextLength);
        root.Children.SetChildFor(context.Text[firstLeaf.Start], firstLeaf);
        stack.Push(new PathNode(firstLeaf, context.TextLength - firstSuffixStart, 0));

        return stack;
    }

    // A node popped here has a suffix sorted after its whole subtree arriving, so its count is
    // final. A split node takes the place of the node it was split from, the first suffix below it
    // included.
    private static void InsertSuffix(BuildContext context, int rank, Stack<PathNode> stack)
    {
        var branchDepth = context.NeighbourPrefixes[rank - 1];
        var lastPopped = stack.Peek();

        while (stack.Peek().Depth > branchDepth)
        {
            lastPopped = stack.Pop();
            Finish(lastPopped, rank);
        }

        var top = stack.Peek();

        if (top.Depth != branchDepth)
        {
            var splitNode = SplitEdge(context.Text, top.Node, lastPopped.Node, branchDepth - top.Depth);
            top = new PathNode(splitNode, branchDepth, lastPopped.FirstRank);
            stack.Push(top);
        }

        var newLeaf = AttachLeaf(context, top.Node, rank, branchDepth);
        stack.Push(newLeaf);
    }

    // Splits child's edge at the given offset, inserting a new branch node between parent and
    // child: the child keeps its identity and its subtree, so the stores put the branch in its
    // place under the parent (SetChildFor, since the store's GetOrAddChildFor makes a node of its
    // own) and hang the child under the branch. InsertSplitNode's read of the ORIGINAL first
    // character must happen (and does, by running to completion first) before
    // ReparentShrunkChild mutates Start/Length - reversing that order would silently key the
    // parent/child maps on the wrong characters, since both keys are derived from the same
    // mutable field.
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
        parent.Children.SetChildFor(originalKey, splitNode);
        return splitNode;
    }

    private static void ReparentShrunkChild(string text, SuffixTreeNode splitNode, SuffixTreeNode child, int offset)
    {
        child.Start += offset;
        child.Length -= offset;
        splitNode.Children.SetChildFor(text[child.Start], child);
    }

    private static PathNode AttachLeaf(
        BuildContext context, SuffixTreeNode parent, int rank, int branchDepth)
    {
        var suffixStart = context.Suffixes[rank];
        var leaf = CreateLeaf(suffixStart, suffixStart + branchDepth, context.TextLength);
        parent.Children.SetChildFor(context.Text[leaf.Start], leaf);
        return new PathNode(leaf, context.TextLength - suffixStart, rank);
    }

    // presumption: allow -- key is only ever drawn from node.Children.Letters immediately before
    // this call, so it is guaranteed present in node.Children; ChildFor's null is the "might be
    // missing" answer this exact call shape never gets.
    private static SuffixTreeNode RequireChild(SuffixTreeNode node, char key) =>
        node.Children.ChildFor(key)!;

    // Finish is shared by InsertSuffix and BuildFromSuffixArray's last pass: the suffixes below a
    // node are the ranks from its first up to the rank that ended it.
    private static void Finish(PathNode pathNode, int endRank) =>
        pathNode.Node.SuffixCount = endRank - pathNode.FirstRank;

    // Walk is shared by HasSubstring, CountOccurrences and FindOccurrences, placed here last for
    // that reason. It is the one trie's walk along compressed edges: the pattern, walked as the
    // span it arrives as, ends on the node whose edge it stops in, partway along it or at its
    // end, and every suffix through that node starts with the pattern.
    private SuffixTreeNode? Walk(ReadOnlySpan<char> pattern) =>
        SuffixWalk.Find(_root, pattern, new SuffixTreeEdges(_text));

    private static SuffixTreeNode CreateLeaf(int suffixStart, int edgeStart, int textLength)
        => new() { Start = edgeStart, Length = textLength - edgeStart, SuffixStart = suffixStart };

    // Bundles what every construction-time helper needs beyond its own per-suffix arguments - the
    // text and the suffix array's two arrays, all fixed for the whole build, the same
    // "least-frequently-varying args" grouping SuffixArray's own DoublingContext/KasaiContext use.
    private readonly record struct BuildContext(
        string Text, int[] Suffixes, int[] NeighbourPrefixes)
    {
        public int TextLength => Text.Length;
    }

    // One node on the rightmost path during the build: its string depth, and the rank of the first
    // suffix below it.
    private readonly record struct PathNode(SuffixTreeNode Node, int Depth, int FirstRank);
}
