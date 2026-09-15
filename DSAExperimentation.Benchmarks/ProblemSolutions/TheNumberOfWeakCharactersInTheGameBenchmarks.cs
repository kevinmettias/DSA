using BenchmarkDotNet.Attributes;
using DSAExperimentation.LeetCode.TheNumberOfWeakCharactersInTheGame;

namespace DSAExperimentation.Benchmarks.ProblemSolutions;

// Harness only: both arms are TheNumberOfWeakCharactersInTheGameSolution's, the same
// methods TheNumberOfWeakCharactersInTheGameTests proves correct - the literal O(n^2)
// pairwise check against sorting via this repo's own MergeSort and making one linear
// pass. Each arm is handed the prepared CharacterRoster its hoisted overload takes, so
// reading LeetCode's int[][] rows into attack/defense pairs is charged to [GlobalSetup]
// rather than to the counting being measured.
//
// The attack range is deliberately narrow next to the defense range, so many characters
// share an attack value and the descending-attack/ascending-defense tie-break is
// exercised rather than only the simple strictly-decreasing case.
[MemoryDiagnoser]
public class TheNumberOfWeakCharactersInTheGameBenchmarks
{
    // LC problem number, reused as the deterministic roster seed.
    private const int RandomSeed = 1996;

    private const int MinStat = 1;
    private const int MaxAttackExclusive = 50;
    private const int MaxDefenseExclusive = 1_000;

    private CharacterRoster _roster = null!;

    [Params(200, 3_000)]
    public int Length { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        var random = new Random(RandomSeed);
        var properties = new int[Length][];

        for (var i = 0; i < Length; i++)
        {
            var attack = random.Next(MinStat, MaxAttackExclusive);
            var defense = random.Next(MinStat, MaxDefenseExclusive);

            properties[i] = [attack, defense];
        }

        _roster = CharacterRoster.Build(properties);
    }

    [Benchmark(Baseline = true)]
    public int PairwiseComparison() =>
        TheNumberOfWeakCharactersInTheGameSolution.NumberOfWeakCharactersByPairwiseComparison(_roster);

    [Benchmark]
    public int SortThenScan() =>
        TheNumberOfWeakCharactersInTheGameSolution.NumberOfWeakCharactersBySortThenScan(_roster);
}
