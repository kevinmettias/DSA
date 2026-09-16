using DSAExperimentation.Benchmarks.Fixtures;

namespace DSAExperimentation.Benchmarks.Tests.Fixtures;

// Harness coverage for NumberOfProvincesWorkloads (ARCHITECTURE 17.7). The reading depends on LC 547's
// matrix being a square symmetric 0/1 adjacency matrix with a self-loop on every city, drawn at a
// connection rate low enough that the flood fill meets several provinces rather than one dense blob.
public sealed partial class NumberOfProvincesWorkloadsTests
{
    private const int CityCount = 50;
    private const int LargestCityCount = 300;
    private const int Seed = 1;
    private const int Connected = 1;
    private const int Disconnected = 0;
    private const int ExpectedProvinceCount = 1;

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

        Assert.Equal(AnswerText.Of(matrix), AnswerText.Of(Transpose(matrix)));
        Assert.All(Enumerable.Range(0, CityCount), city => Assert.Equal(Connected, matrix[city][city]));
    }

    // How many provinces a draw falls into is not fixed by the seed, but the connection rate is, and a
    // 1-in-10 rate is far above G(n,p)'s connectivity threshold at these sizes: at both of the city
    // counts the benchmark reads this generator at, the draw comes out as ONE province, not the
    // "several provinces" the generator's own comment describes. Measured at both params rather than
    // assumed from the comment, and reported to the campaign owner as a comment/parameter mismatch.
    [Fact]
    public void BuildAdjacencyMatrix_BothBenchmarkCityCounts_ComeOutAsASingleProvince()
    {
        Assert.Equal(
            ExpectedProvinceCount,
            ProvinceCount(NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed)));
        Assert.Equal(
            ExpectedProvinceCount,
            ProvinceCount(NumberOfProvincesWorkloads.BuildAdjacencyMatrix(LargestCityCount, Seed)));
    }

    // A draw that comes out connected implies the other half of the comment's claim, which is the part
    // the strategies depend on: no city is an isolated singleton, so every arm walks a real component
    // instead of short-circuiting on lone cities.
    [Fact]
    public void BuildAdjacencyMatrix_EveryCity_HasAtLeastOneNeighbourBeyondItsOwnSelfLoop()
    {
        var matrix = NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed);

        Assert.All(Enumerable.Range(0, CityCount), city => Assert.True(NeighbourCount(matrix, city) > 0));
    }

    [Fact]
    public void BuildAdjacencyMatrix_SameSeed_ReturnsTheSameMatrix() =>
        Assert.Equal(
            AnswerText.Of(NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed)),
            AnswerText.Of(NumberOfProvincesWorkloads.BuildAdjacencyMatrix(CityCount, Seed)));

    private static int[][] Transpose(int[][] matrix) =>
    [
        .. Enumerable.Range(0, matrix.Length)
            .Select(column => Enumerable.Range(0, matrix.Length).Select(row => matrix[row][column]).ToArray()),
    ];

    private static int NeighbourCount(int[][] matrix, int city) =>
        matrix[city].Where((_, other) => other != city).Count(entry => entry == Connected);

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
}
