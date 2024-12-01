namespace TransportationProblem;

class TransportationTask
{
  public int N { get; set; }
  public int M { get; set; }
  public int[] Supplies { get; set; }
  public int[] Demands { get; set; }
  public int[,] Costs { get; set; }

  public static async Task<TransportationTask> LoadFromFileAsync(string path)
  {
    string[] lines = await File.ReadAllLinesAsync(path);
    int index = 0;

    var sizes = lines[index++].Split().Select(int.Parse).ToArray();
    int N = sizes[0];
    int M = sizes[1];

    int[] supplies = lines[index++].Split().Select(int.Parse).ToArray();
    int[] demands = lines[index++].Split().Select(int.Parse).ToArray();

    int[,] costs = new int[N, M];
    Parallel.For(0, N, i =>
    {
      var costLine = lines[index + i].Split().Select(int.Parse).ToArray();
      for (int j = 0; j < M; j++)
      {
        costs[i, j] = costLine[j];
      }
    });

    return new TransportationTask
    {
      N = N,
      M = M,
      Supplies = supplies,
      Demands = demands,
      Costs = costs
    };
  }
}
