using DSAExperimentation.Benchmarks.Fixtures;
using DSAExperimentation.LeetCode.Harness;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for NumberOfProvincesWorkloads (ARCHITECTURE 17.7). The reading depends on LC 547's
// matrix being a square symmetric 0/1 adjacency matrix with a self-loop on every city, drawn at a
// connection rate low enough that the flood fill meets several provinces rather than one dense blob.
public sealed partial class NumberOfProvincesWorkloadsTests
{
    private const int CityCount = 50;
    private const int LargestCityCount = 200;
    private const int Seed = 1;
    private const int Connected = 1;
    private const int Disconnected = 0;
    private const int FewestProvinces = 2;

    // The generator's two expected neighbours per city, give or take half: a draw's mean at these
    // sizes sits within a few tenths of two, and a rate that ignored the city count, as the old
    // 1-in-10 did, would give 4.9 neighbours at 50 cities and 19.9 at 200.
    private const double FewestMeanNeighbours = 1.0;
    private const double MostMeanNeighbours = 3.0;

    [Fact]
    public void BuildAdjacencyMatrix_CityCount_ReturnsASquareMatrix()
    {
        var matrix = NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed);

        Assert.Equal(CityCount, matrix.Length);
        Assert.All(matrix, row => Assert.Equal(CityCount, row.Length));
    }

    [Fact]
    public void BuildAdjacencyMatrix_EveryEntry_IsZeroOrOne()
    {
        var matrix = NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed);

        Assert.All(
            matrix.SelectMany(row => row),
            entry => Assert.True(entry == Connected || entry == Disconnected));
    }

    // LC 547 reads isConnected as undirected, so the matrix is a promise about both directions at once:
    // a generator writing only one triangle would leave the strategies reading different graphs.
    [Fact]
    public void BuildAdjacencyMatrix_Matrix_IsSymmetricWithASelfLoopOnEveryCity()
    {
        var matrix = NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed);

        Assert.Equal(AnswerGraphText.Of(matrix), AnswerGraphText.Of(Transpose(matrix)));
        Assert.All(Enumerable.Range(0, CityCount), city => Assert.Equal(Connected, matrix[city][city]));
    }

    // The answer is the province count, so a draw that came out as one province at both of the
    // city counts the benchmark reads, as the old 1-in-10 rate's did, could not tell a counting arm
    // from one that returns 1. Several provinces at both, and fewer than one per city, so the arms
    // also merge real components rather than meeting only lone cities.
    [Fact]
    public void BuildAdjacencyMatrix_BothBenchmarkCityCounts_FormSeveralProvinces()
    {
        var smallest = NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed);
        var largest = NumberOfProvincesWorkloads.BuildAdjacencyMatrix(LargestCityCount, Seed);

        Assert.InRange(ProvinceCount(smallest), FewestProvinces, CityCount - 1);
        Assert.InRange(ProvinceCount(largest), FewestProvinces, LargestCityCount - 1);
    }

    // The rate scales with the city count, so the graph keeps its shape across the benchmark's
    // sizes: a city has about two neighbours at 50 cities and at 200 alike.
    [Fact]
    public void BuildAdjacencyMatrix_BothBenchmarkCityCounts_GiveACityAboutTwoNeighbours()
    {
        var smallest = NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed);
        var largest = NumberOfProvincesWorkloads.BuildAdjacencyMatrix(LargestCityCount, Seed);

        Assert.InRange(MeanNeighbourCount(smallest), FewestMeanNeighbours, MostMeanNeighbours);
        Assert.InRange(MeanNeighbourCount(largest), FewestMeanNeighbours, MostMeanNeighbours);
    }

    [Fact]
    public void BuildAdjacencyMatrix_SameSeed_ReturnsTheSameMatrix() =>
        Assert.Equal(
            AnswerGraphText.Of(NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed)),
            AnswerGraphText.Of(NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed)));

    private static int[][] Transpose(int[][] matrix) =>
    [
        .. Enumerable.Range(0, matrix.Length)
            .Select(column => Enumerable.Range(0, matrix.Length).Select(row => matrix[row][column]).ToArray()),
    ];

    private static int ProvinceCount(int[][] matrix)
    {
        var visited = new HashSet<int>();
        var provinceCount = 0;

        foreach (var city in Enumerable.Range(0, matrix.Length))
        {
            if (visited.Add(city))
            {
                provinceCount++;
                Visit(matrix, city, visited);
            }
        }

        return provinceCount;
    }

    private static void Visit(int[][] matrix, int start, HashSet<int> visited)
    {
        var pending = new Queue<int>();
        pending.Enqueue(start);

        while (pending.Count > 0)
        {
            var city = pending.Dequeue();

            foreach (var neighbour in Enumerable.Range(0, matrix.Length))
            {
                if (matrix[city][neighbour] == Connected && visited.Add(neighbour))
                {
                    pending.Enqueue(neighbour);
                }
            }
        }
    }

    private static double MeanNeighbourCount(int[][] matrix) =>
        Enumerable.Range(0, matrix.Length).Average(city => NeighbourCount(matrix, city));

    private static int NeighbourCount(int[][] matrix, int city) =>
        matrix[city].Where((_, other) => other != city).Count(entry => entry == Connected);
}
