using System.Numerics;
using RootMaskTree = DSAExperimentation.DataStructures.FenwickTree.FenwickTree<
    int, DSAExperimentation.DataStructures.ElementAlgebra.XorOperation<int>>;

namespace DSAExperimentation.LeetCode.PalindromicPathQueriesInATree;

// LeetCode 3841. Palindromic Path Queries in a Tree: an undirected tree over nodes
// 0..n-1 given as edges[], with a lowercase letter s[i] on every node (n and s are
// nodeCount and nodeLetters here), and a list of commands. "update u c" writes letter
// c onto node u; "query u v" asks whether the letters on the path from u to v, both
// ends included, can be rearranged into a palindrome. The answer holds one entry per
// "query", in order, each seeing every update before it.
//
// Letters rearrange into a palindrome exactly when at most one of them occurs an
// odd number of times, so a path is answered by its parity mask - bit c set when
// letter c occurs an odd number of times - having at most one bit set.
internal static class PalindromicPathQueriesInATreeSolution
{
    private const int AtMostOneOddLetter = 1;
    private const string UpdateVerb = "update";
    private const char WordSeparator = ' ';
    private const int VerbWord = 0;
    private const int NodeWord = 1;
    private const int ArgumentWord = 2;

    // Textbook: root the tree once with a BCL breadth-first search that records each
    // node's parent and depth, keep the current letters in a char[], and answer each
    // "query" by walking both endpoints up to where they meet, folding every letter
    // passed into the path's parity mask. An update is one array write; a query
    // costs its path's length, O(n) on a chain - the arm the Euler-tour Fenwick
    // below has to justify itself against.
    public static bool[] GetPalindromePathFlagsByAncestorWalk(
        int nodeCount, int[][] edges, string nodeLetters, string[] queries)
    {
        var (parent, depth) = RootByBreadthFirst(nodeCount, edges);
        var letters = nodeLetters.ToCharArray();
        var answers = new List<bool>();

        foreach (var command in queries)
        {
            var (verb, node, argument) = Parse(command);

            if (verb == UpdateVerb)
            {
                letters[node] = argument[0];
                continue;
            }

            var other = int.Parse(argument);
            var pathMask = PathMaskByWalk((parent, depth, letters), node, other);
            answers.Add(HasAtMostOneOddLetter(pathMask));
        }

        return [.. answers];
    }

    // A tree's only neighbor of a node that is not its child is its parent, so the
    // search needs no visited set. Ends once the queue drains, every node having
    // been enqueued exactly once.
    private static (int[] Parent, int[] Depth) RootByBreadthFirst(int nodeCount, int[][] edges)
    {
        var neighbors = LeetCodeAdjacency.ZeroBased<List<int>>(
            nodeCount, edges, _ => [], (slot, farId, _, _) => slot.Add(farId));
        var parent = new int[nodeCount];
        var depth = new int[nodeCount];
        parent[0] = -1;
        var frontier = new Queue<int>();
        frontier.Enqueue(0);

        while (frontier.TryDequeue(out var node))
        {
            foreach (var neighbor in neighbors[node].Where(neighbor => neighbor != parent[node]))
            {
                parent[neighbor] = node;
                depth[neighbor] = depth[node] + 1;
                frontier.Enqueue(neighbor);
            }
        }

        return (parent, depth);
    }

    // Lifts whichever endpoint is deeper - either, at equal depth - folding its
    // letter in as it leaves, until the two meet at their lowest common ancestor,
    // whose letter is folded in last. Every node on the path is folded exactly once.
    // Ends because each step lifts one endpoint a level, and both reach the root.
    private static int PathMaskByWalk(
        (int[] Parent, int[] Depth, char[] Letters) tree, int first, int second)
    {
        var (deeper, other) = (first, second);
        var mask = 0;

        while (deeper != other)
        {
            if (tree.Depth[deeper] < tree.Depth[other])
            {
                (deeper, other) = (other, deeper);
            }

            mask ^= LetterBit(tree.Letters[deeper]);
            deeper = tree.Parent[deeper];
        }

        return mask ^ LetterBit(tree.Letters[deeper]);
    }

    // Composed: TreeTour lays the tree out in pre-order, where every subtree is one
    // contiguous run of positions. A node's root mask - the parity mask of the path
    // from the root down to it - changes, on an update at v, by the same
    // old-letter-XOR-new-letter delta at every position in v's subtree and nowhere
    // else: a range update with point reads. This repo's FenwickTree<int,
    // XorOperation<int>> holds the root masks as a difference array: toggling a
    // subtree is two point Adds, at its first position and one past its last, and a
    // node's root mask is the PrefixQuery at its position. XOR is a group, each mask
    // its own inverse, which is what FenwickTree asks of its operation. Queries read
    // single positions only, so the plain FenwickTree over differences is enough:
    // RangeFenwickTree's second tree serves range reads, and its IScaledGroupOperation
    // is one XorOperation does not implement.
    //
    // A path's mask is then rootMask(u) XOR rootMask(v) XOR bit(letter at lca): every
    // node from the root down to the lca sits on both root paths and cancels, the
    // lca included, so its letter goes back in once. TreeTour finds the lca by a
    // range minimum over its own SegmentTree. Every command is O(log n), and the
    // initial masks are n subtree toggles, O(n log n).
    public static bool[] GetPalindromePathFlagsByEulerFenwick(
        int nodeCount, int[][] edges, string nodeLetters, string[] queries)
    {
        var tour = TreeTour.Build(nodeCount, edges);

        return GetPalindromePathFlagsByEulerFenwick(tour, nodeLetters, queries);
    }

    public static bool[] GetPalindromePathFlagsByEulerFenwick(TreeTour tour, string nodeLetters, string[] queries)
    {
        var letters = nodeLetters.ToCharArray();
        var rootMasks = new RootMaskTree(letters.Length);

        for (var node = 0; node < letters.Length; node++)
        {
            ToggleSubtree((tour, rootMasks), node, LetterBit(letters[node]));
        }

        return AnswerCommands((tour, rootMasks, letters), queries);
    }

    private static bool[] AnswerCommands(
        (TreeTour Tour, RootMaskTree RootMasks, char[] Letters) state, string[] queries)
    {
        var answers = new List<bool>();

        foreach (var command in queries)
        {
            var (verb, node, argument) = Parse(command);

            if (verb == UpdateVerb)
            {
                var letterChange = LetterBit(state.Letters[node]) ^ LetterBit(argument[0]);
                ToggleSubtree((state.Tour, state.RootMasks), node, letterChange);
                state.Letters[node] = argument[0];
                continue;
            }

            var other = int.Parse(argument);
            var pathMask = PathMaskByRootMasks(state, node, other);
            answers.Add(HasAtMostOneOddLetter(pathMask));
        }

        return [.. answers];
    }

    // XORs letterBits into the root mask of node and of everything below it: the
    // difference array takes it at the subtree's first position and takes it back
    // one past the last, unless the subtree runs to the end of the tour.
    private static void ToggleSubtree((TreeTour Tour, RootMaskTree RootMasks) masks, int node, int letterBits)
    {
        var (first, last) = masks.Tour.SubtreeOf(node);
        var pastLast = last + 1;
        masks.RootMasks.Add(first, letterBits);

        if (pastLast < masks.RootMasks.Count)
        {
            masks.RootMasks.Add(pastLast, letterBits);
        }
    }

    private static int PathMaskByRootMasks(
        (TreeTour Tour, RootMaskTree RootMasks, char[] Letters) state, int first, int second)
    {
        var ancestor = state.Tour.LowestCommonAncestor(first, second);
        var firstRootMask = state.RootMasks.PrefixQuery(state.Tour.PositionOf(first));
        var secondRootMask = state.RootMasks.PrefixQuery(state.Tour.PositionOf(second));

        return firstRootMask ^ secondRootMask ^ LetterBit(state.Letters[ancestor]);
    }

    // Every command is three words: the verb, a node, and then either the new
    // letter (update) or the path's other endpoint (query).
    private static (string Verb, int Node, string Argument) Parse(string command)
    {
        var words = command.Split(WordSeparator);

        return (words[VerbWord], int.Parse(words[NodeWord]), words[ArgumentWord]);
    }

    private static bool HasAtMostOneOddLetter(int pathMask) =>
        BitOperations.PopCount((uint)pathMask) <= AtMostOneOddLetter;

    private static int LetterBit(char letter) => 1 << (letter - 'a');
}
