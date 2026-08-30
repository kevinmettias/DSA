using BenchmarkDotNet.Attributes;
using DSAExperimentation.Algorithms.Reducing;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Contracts.Ordering;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Minimum Genetic Mutation (LC 433): the textbook mutate-every-position BFS
// (Queue<string> plus a HashSet<string> bank, generating each candidate gene on
// the fly) vs. this repo's own BFS - Reduce.Graph + DistanceMapReduceAlgebra over
// a precomputed GeneMutationNode graph, the same composition WordLadderBenchmarks
// already proves for a 26-letter alphabet, narrowed here to LC 433's 4-letter DNA
// alphabet. _genes is a connected mutation chain (GeneMutationGraphs.BuildChain),
// so endGene is always genuinely reachable and both strategies do a real full
// BFS instead of failing fast.
[MemoryDiagnoser]
public class MinimumGeneticMutationBenchmarks
{
    private const int GeneLength = 8;
    private const string Alphabet = "ACGT";

    [Params(200, 2_000)]
    public int GeneCount;

    private HashSet<string> _bank = null!;
    private string _startGene = null!;
    private string _endGene = null!;
    private Dictionary<string, GeneMutationNode> _nodesByGene = null!;
    private GeneMutationNode _startNode = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (genes, startGene, endGene) = GeneMutationGraphs.BuildChain(GeneCount, GeneLength, seed: 433);
        _bank = [.. genes];
        _startGene = startGene;
        _endGene = endGene;

        (_nodesByGene, _startNode) = GeneMutationGraphs.BuildGraph(genes, startGene);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs()
    {
        var visited = new HashSet<string> { _startGene };
        var queue = new Queue<(string Gene, int Distance)>();
        queue.Enqueue((_startGene, 0));
        var buffer = new char[GeneLength];

        while (queue.Count > 0)
        {
            var (gene, distance) = queue.Dequeue();

            if (gene == _endGene)
            {
                return distance;
            }

            gene.CopyTo(buffer);

            for (var i = 0; i < GeneLength; i++)
            {
                var original = buffer[i];

                foreach (var letter in Alphabet)
                {
                    if (letter == original)
                    {
                        continue;
                    }

                    buffer[i] = letter;
                    var candidate = new string(buffer);

                    if (_bank.Contains(candidate) && visited.Add(candidate))
                    {
                        queue.Enqueue((candidate, distance + 1));
                    }
                }

                buffer[i] = original;
            }
        }

        return -1;
    }

    [Benchmark]
    public int ReduceGraphBfs()
    {
        var distances = Reduce.Graph<
            GeneMutationNode, GeneMutationTopology, ListChildren<GeneMutationNode>,
            NaturalChildOrder<GeneMutationNode, ListChildren<GeneMutationNode>>, ListChildren<GeneMutationNode>,
            BreadthFirstReduceOrder<GeneMutationNode>,
            DistanceMapReduceAlgebra<GeneMutationNode>, Dictionary<GeneMutationNode, int>>(_startNode);

        return distances.TryGetValue(_nodesByGene[_endGene], out var distance) ? distance : -1;
    }
}
