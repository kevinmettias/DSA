using DSAExperimentation.Algorithms.ShortestPaths.Hamming;
using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.DataStructures.Set;

namespace DSAExperimentation.LeetCode.MinimumGeneticMutation;

// LeetCode 433. Minimum Genetic Mutation: fewest single-character mutations from
// startGene to endGene, where every intermediate gene must be in the bank.
//
// Structurally identical to Word Ladder (127) over the 4-letter DNA alphabet
// instead of 26 letters, and answered in edges walked rather than nodes visited -
// so its composed arm shares DataStructures.Graph.Hamming outright with WordLadder's,
// and the two differ only in the "+1" WordLadder applies and the alphabet they
// mutate over.
internal static class MinimumGeneticMutationSolution
{
    // The four letters a gene is spelled in, restated rather than read from
    // Domain/Dna's DnaBases: a baseline reaches no library type, not even a constant
    // (ARCHITECTURE 17.5).
    private const string GeneLetters = "ACGT";

    // The textbook answer: BCL Queue + HashSet over candidates generated on the
    // fly, never materializing the graph.
    public static int MinMutationByMutationQueue(StartGene startGene, EndGene endGene, IEnumerable<string> bank)
    {
        var bankSet = new HashSet<string>(bank);

        return MinMutation(startGene, endGene, new HashSetBank(bankSet));
    }

    // The same search over a bank its caller prepared once, read only through Has.
    public static int MinMutationByMutationQueue(StartGene startGene, EndGene endGene, Set<string> bank) =>
        MinMutation(startGene, endGene, new PreparedBank(bank));

    private static int MinMutation<TBank>(StartGene startGene, EndGene endGene, TBank bank)
        where TBank : struct, IGeneBank
    {
        // The graph strategy seeds startGene itself, so an endGene equal to it is
        // reachable even when the bank omits it - matched here for parity.
        if (!bank.Has(endGene.Name) && endGene.Name != startGene.Name)
        {
            return LeetCodeAnswer.None;
        }

        return MutationDistance(startGene.Name, endGene.Name, bank) ?? LeetCodeAnswer.None;
    }

    // Mutations walked from start to target, or null when no path stays inside the bank.
    // start itself need not be in the bank, matching LeetCode's startGene convention.
    private static int? MutationDistance<TBank>(string start, string target, TBank bank)
        where TBank : struct, IGeneBank
    {
        var walk = new GeneWalk<TBank>(bank, [start], new Queue<(string Gene, int Distance)>());
        walk.Queue.Enqueue((start, 0));

        while (walk.Queue.Count > 0)
        {
            var (gene, distance) = walk.Queue.Dequeue();

            if (gene == target)
            {
                return distance;
            }

            EnqueueBankedMutations(gene, distance, walk);
        }

        return null;
    }

    // Queues every bank gene one mutation away from gene, the first time the walk meets it.
    private static void EnqueueBankedMutations<TBank>(string gene, int distance, GeneWalk<TBank> walk)
        where TBank : struct, IGeneBank
    {
        foreach (var candidate in OneLetterMutations(gene))
        {
            if (walk.Bank.Has(candidate) && walk.Visited.Add(candidate))
            {
                walk.Queue.Enqueue((candidate, distance + 1));
            }
        }
    }

    // Every gene that differs from gene in exactly one position, spelled over GeneLetters.
    private static IEnumerable<string> OneLetterMutations(string gene)
    {
        var buffer = gene.ToCharArray();

        for (var position = 0; position < buffer.Length; position++)
        {
            var original = buffer[position];

            foreach (var replacement in GeneLetters)
            {
                if (replacement != original)
                {
                    buffer[position] = replacement;
                    yield return new string(buffer);
                }
            }

            buffer[position] = original;
        }
    }

    // Membership in the bank, the one thing the search asks of it - answered by the
    // LeetCode-shaped overload's own HashSet or by the Set its caller prepared.
    private interface IGeneBank
    {
        bool Has(string gene);
    }

    private readonly struct HashSetBank(HashSet<string> genes) : IGeneBank
    {
        public bool Has(string gene) => genes.Contains(gene);
    }

    private readonly struct PreparedBank(Set<string> genes) : IGeneBank
    {
        public bool Has(string gene) => genes.Has(gene);
    }

    // The bank one breadth-first walk tests candidates against, and the visited set and
    // queue it threads through every step.
    private readonly record struct GeneWalk<TBank>(
        TBank Bank,
        HashSet<string> Visited,
        Queue<(string Gene, int Distance)> Queue)
        where TBank : struct, IGeneBank;

    // This repo's own BFS over the materialized bank graph.
    public static int MinMutationByReduceGraph(StartGene startGene, EndGene endGene, IEnumerable<string> bank)
    {
        var graph = HammingGraph.Build(startGene.Name, bank);

        return MinMutationByReduceGraph(graph, endGene.Name);
    }

    public static int MinMutationByReduceGraph(HammingGraph graph, string endGene)
    {
        if (!graph.TryGetNode(endGene, out var endNode))
        {
            return LeetCodeAnswer.None;
        }

        var distances = HammingDistances.From(graph.Root);

        return distances.TryGetValue(endNode, out var distance) ? distance : LeetCodeAnswer.None;
    }

    // LC 433's two endpoints, named for the roles they play here rather than left as two
    // adjacent `string` positions a caller could hand over the wrong way round with the
    // compiler none the wiser. `startGene` is the gene the walk begins at and `endGene`
    // the one it is trying to reach; a mutation count is not symmetric between them.
    internal readonly record struct StartGene(string Name);

    internal readonly record struct EndGene(string Name);
}
