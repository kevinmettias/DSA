using DSAExperimentation.DataStructures.Graph.Engines.Dags.Trees.Tries;
using DSAExperimentation.LeetCode.Solutions.ImplementTrie;
using DSAExperimentation.Tests.DataStructures.Graph.Engines.Dags.Trees.Tries;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ImplementTrie;

// LC 208's textbook trie held to IPrefixSet's contract: it inherits every fact PrefixSetTests states,
// the same facts the one trie's set view meets under each store, since the textbook node chain is the
// strategy that specializes that layer (ARCHITECTURE §17.5).
public sealed partial class ArrayNodeTrieTests : PrefixSetTests
{
    protected override IPrefixSet CreateSet() => ImplementTrieSolution.CreateByArrayNodes();
}
