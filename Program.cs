using Optimizers;
using TransportationTaskSolver;

class Program
{
  const string inputFilePath = "data/in.txt";
  const string outputFilePath = "data/out.txt";
  static async Task Main(string[] args)
  {
    TransportationTask task = await TransportationTask.LoadFromFileAsync(inputFilePath);
    List<ITransportationTaskSolver> solvers = new List<ITransportationTaskSolver>()
    {
      new NorthwestCornerMethod(),
      new LeastCostMethod()
    };

    List<Task<(int[,], int)>> tasks = new List<Task<(int[,], int)>>();

    foreach (var solver in solvers)
    {
      tasks.Add(Task.Run(async () =>
      {
        int[,] X = await solver.SolveAsync(task);
        MODIOptimizer optimizer = new MODIOptimizer();
        X = await optimizer.OptimizeAsync(task, X);
        int totalCost = CalculateTotalCost(X, task.Costs);
        return (X, totalCost);
      }));
    }

    var results = await Task.WhenAll(tasks);

    var bestResult = results.OrderBy(r => r.Item2).First();
    int[,] bestPlan = bestResult.Item1;
    int minimalTotalCost = bestResult.Item2;

    await WriteOutputAsync(outputFilePath, bestPlan, minimalTotalCost, task.N, task.M);
  }

  static int CalculateTotalCost(int[,] X, int[,] C)
  {
    int totalCost = 0;
    int N = X.GetLength(0);
    int M = X.GetLength(1);

    for (int i = 0; i < N; i++)
    {
      for (int j = 0; j < M; j++)
      {
        totalCost += X[i, j] * C[i, j];
      }
    }
    return totalCost;
  }

  static async Task WriteOutputAsync(string path, int[,] X, int totalCost, int N, int M)
  {
    using (StreamWriter writer = new StreamWriter(path))
    {
      await writer.WriteLineAsync(totalCost.ToString());
      for (int i = 0; i < N; i++)
      {
        string line = string.Join(" ", Enumerable.Range(0, M).Select(j => X[i, j]));
        await writer.WriteLineAsync(line);
      }
    }
  }
}
