using BenchmarkDotNet.Attributes;
using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.DataStructures.Graph.Hamming;
using DSAExperimentation.DataStructures.Set;
using DSAExperimentation.LeetCode.MinimumGeneticMutation;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are MinimumGeneticMutationSolution's. Same workload
// shape as WordLadderBenchmarks, narrowed to the 4-letter DNA alphabet - which is
// the whole measurable difference between LC 433 and LC 127, now that they share
// DataStructures.Graph.Hamming.
[MemoryDiagnoser]
public class MinimumGeneticMutationBenchmarks
{
    private const int GeneLength = 8;
    private const int RandomSeed = 433; // LC problem number

    [Params(200, 2_000)]
    public int GeneCount;

    private Set<string> _bank = null!;
    private HammingGraph _graph = null!;
    private string _startGene = null!;
    private string _endGene = null!;

    [GlobalSetup]
    public void Setup()
    {
        var (genes, startGene, endGene) =
            HammingWorkloads.BuildChain(GeneCount, GeneLength, StandardAlphabets.Dna, seed: RandomSeed);

        _startGene = startGene;
        _endGene = endGene;
        _bank = new Set<string>(genes);
        _graph = HammingGraph.Build(startGene, genes);
    }

    [Benchmark(Baseline = true)]
    public int MutationQueueBfs() =>
        MinimumGeneticMutationSolution.MinMutationByMutationQueue(_startGene, _endGene, _bank);

    [Benchmark]
    public int ReduceGraphBfs() => MinimumGeneticMutationSolution.MinMutationByReduceGraph(_graph, _endGene);
}
