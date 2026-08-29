namespace DSAExperimentation.DataStructures.AhoCorasick;

// Builds a multi-pattern matching automaton once from a set of patterns (a constructor argument,
// not a per-query argument), then answers FindAll queries against arbitrary text in
// O(text.Length + matches found) - the classic Aho-Corasick construction: a trie of every pattern,
// augmented with a failure link per node (KMP's failure function generalized from one pattern to a
// whole trie) and an output link per node (the nearest failure-ancestor that itself completes a
// pattern, letting a match report skip straight to the next real match instead of walking the full
// failure chain).
//
// Unlike RollingHash/RollingHashSearch, there is no pattern-agnostic layer underneath this to split
// out into a separate Algorithms/ file: the automaton IS built from the pattern set, with no second
// substitutable representation - ARCHITECTURE.md §5 step 7's exact test for "this is the data
// structure, not an algorithm generic over one" - so build and query co-locate here under
// DataStructures/AhoCorasick/, the same pairing shape as Heap/HeapArray or Stack/DynamicArray.
//
// No Topology witness: every AhoCorasickNode is created only internally during construction from
// patterns, the same "no external caller-supplied shape could ever violate the invariant" reasoning
// Trie.cs's own doc comment already gives for itself.
internal sealed class AhoCorasick
{
    private readonly AhoCorasickNode _root;
    private readonly List<int> _patternLengths = new();

    public AhoCorasick(IEnumerable<string> patterns)
        : this(patterns, EqualityComparer<char>.Default)
    {
    }

    public AhoCorasick(IEnumerable<string> patterns, IEqualityComparer<char> comparer)
    {
        _root = new AhoCorasickNode(comparer);

        foreach (var pattern in patterns)
        {
            Insert(pattern, comparer);
        }

        BuildFailureAndOutputLinks();
    }

    private void Insert(string pattern, IEqualityComparer<char> comparer)
    {
        var current = _root;

        foreach (var ch in pattern)
        {
            if (!current.Children.TryGetValue(ch, out var next))
            {
                next = new AhoCorasickNode(comparer);
                current.Children.Set(ch, next);
            }

            current = next;
        }

        current.PatternIndices.Add(_patternLengths.Count);
        _patternLengths.Add(pattern.Length);
    }

    // BFS by increasing depth. Root's direct children get Fail = root hardcoded as a base case,
    // never derived from the general formula - applying GoTo(Fail(root), c) uniformly to depth-1
    // nodes would need root.Fail itself, which is deliberately left null (never a self-loop) so
    // that skipping this base case fails loudly (NullReferenceException at construction) instead of
    // looping forever in GoTo.
    private void BuildFailureAndOutputLinks()
    {
        var queue = new Queue<AhoCorasickNode>();

        foreach (var key in _root.Children.Keys)
        {
            var child = RequireChild(_root, key);
            EstablishFailureAndOutputLink(child, _root);
            queue.Enqueue(child);
        }

        while (queue.Count > 0)
        {
            LinkChildren(queue.Dequeue(), queue);
        }
    }

    private void LinkChildren(AhoCorasickNode parent, Queue<AhoCorasickNode> queue)
    {
        foreach (var key in parent.Children.Keys)
        {
            var child = RequireChild(parent, key);

            // presumption: allow -- parent was only ever enqueued (in
            // BuildFailureAndOutputLinks' initial foreach, or here) after its own Fail was
            // already assigned by EstablishFailureAndOutputLink, so every dequeued node's
            // Fail is guaranteed non-null here.
            var fail = GoTo(parent.Fail!, key);
            EstablishFailureAndOutputLink(child, fail);
            queue.Enqueue(child);
        }
    }

    // O(text.Length + matches found): one GoTo transition per character (amortized O(1) via the
    // failure-link fallback, the same amortized argument PrefixFunctionSearch.Advance already
    // relies on) plus one OutputLink-chain walk per position, which only ever touches real match
    // nodes. Returned in ascending Start order (ties broken by ascending PatternIndex) for a
    // predictable, test-friendly contract - Start does not increase monotonically with the scan
    // position on its own once patterns of different lengths are involved, unlike every
    // single-pattern FindAll elsewhere in this repo.
    public List<AhoCorasickMatch> FindAll(ReadOnlySpan<char> text)
    {
        var matches = new List<AhoCorasickMatch>();
        ReportMatchesAt(_root, 0, matches);

        var current = _root;

        for (var i = 0; i < text.Length; i++)
        {
            current = GoTo(current, text[i]);
            ReportMatchesAt(current, i + 1, matches);
        }

        matches.Sort(CompareByStartThenPatternIndex);
        return matches;
    }

    private void ReportMatchesAt(AhoCorasickNode node, int consumedCount, List<AhoCorasickMatch> matches)
    {
        for (var output = node; output is not null; output = output.OutputLink)
        {
            foreach (var patternIndex in output.PatternIndices)
            {
                matches.Add(new AhoCorasickMatch(consumedCount - _patternLengths[patternIndex], patternIndex));
            }
        }
    }

    private static int CompareByStartThenPatternIndex(AhoCorasickMatch left, AhoCorasickMatch right)
        => left.Start != right.Start ? left.Start.CompareTo(right.Start) : left.PatternIndex.CompareTo(right.PatternIndex);

    // EstablishFailureAndOutputLink, RequireChild, and GoTo are each shared by two or more distinct
    // callers above (BuildFailureAndOutputLinks/LinkChildren; LinkChildren/FindAll), placed here
    // last for that reason - contrast Insert/BuildFailureAndOutputLinks/LinkChildren/FindAll/
    // ReportMatchesAt/CompareByStartThenPatternIndex above, each with exactly one caller, placed
    // immediately after it in DFS call order.
    private static void EstablishFailureAndOutputLink(AhoCorasickNode node, AhoCorasickNode fail)
    {
        node.Fail = fail;
        node.OutputLink = fail.PatternIndices.Count > 0 ? fail : fail.OutputLink;
    }

    // presumption: allow -- key is only ever drawn from node.Children.Keys immediately before this
    // call (every call site above), so it is guaranteed present in node.Children; TryGetValue's own
    // bool result is redundant at this exact call shape, not a real "might be missing" check.
    private static AhoCorasickNode RequireChild(AhoCorasickNode node, char key)
    {
        node.Children.TryGetValue(key, out var child);
        return child!;
    }

    // The one transition helper shared by both construction (LinkChildren above) and the scan above
    // (FindAll), mirroring PrefixFunctionSearch.Advance's reuse by both ComputeFailureFunction and
    // SearchWithMatcher: falls back through failure links until either root or a real child
    // transition is found. Terminates because every node's Fail points to a strictly shallower node
    // (established by the BFS construction order above), so the chain always reaches root.
    private AhoCorasickNode GoTo(AhoCorasickNode node, char character)
    {
        var current = node;

        while (current != _root && !current.Children.HasKey(character))
        {
            // presumption: allow -- Fail is null only for root, and this loop's own condition
            // (current != _root) guarantees current is never root here, the same "only meaningful
            // once the caller's own check passed" contract HashMap.TryGetValue's out-parameter uses.
            current = current.Fail!;
        }

        return current.Children.TryGetValue(character, out var next) ? next : _root;
    }
}
