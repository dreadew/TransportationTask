using TransportationProblem;

namespace TransportationTaskSolver;

interface ITransportationTaskSolver
{
  Task<int[,]> SolveAsync(TransportationTask task);
}

class NorthwestCornerMethod : ITransportationTaskSolver
{
  public Task<int[,]> SolveAsync(TransportationTask task)
  {
    return Task.Run(() =>
    {
      int[,] X = new int[task.N, task.M];
      int[] supply = (int[])task.Supplies.Clone();
      int[] demand = (int[])task.Demands.Clone();
      int i = 0, j = 0;

      while (i < task.N && j < task.M)
      {
        int quantity = Math.Min(supply[i], demand[j]);
        X[i, j] = quantity;
        supply[i] -= quantity;
        demand[j] -= quantity;

        if (supply[i] == 0) i++;
        else if (demand[j] == 0) j++;
      }

      return X;
    });
  }
}

class LeastCostMethod : ITransportationTaskSolver
{
  public Task<int[,]> SolveAsync(TransportationTask task)
  {
    return Task.Run(() =>
    {
      int N = task.N;
      int M = task.M;
      int[,] X = new int[N, M];
      int[] supply = (int[])task.Supplies.Clone();
      int[] demand = (int[])task.Demands.Clone();

      var cells = from i in Enumerable.Range(0, N)
                  from j in Enumerable.Range(0, M)
                  orderby task.Costs[i, j]
                  select new { i, j };

      foreach (var cell in cells)
      {
        int quantity = Math.Min(supply[cell.i], demand[cell.j]);
        if (quantity > 0)
        {
          X[cell.i, cell.j] = quantity;
          supply[cell.i] -= quantity;
          demand[cell.j] -= quantity;
        }

        if (supply.All(s => s == 0) && demand.All(d => d == 0))
          break;
      }

      return X;
    });
  }
}

class VogelsApproximationMethod : ITransportationTaskSolver
{
  public Task<int[,]> SolveAsync(TransportationTask problem)
  {
    return Task.Run(() =>
    {
      int N = problem.N;
      int M = problem.M;
      int[,] X = new int[N, M];
      int[] supply = (int[])problem.Supplies.Clone();
      int[] demand = (int[])problem.Demands.Clone();
      bool[] rowDone = new bool[N];
      bool[] colDone = new bool[M];

      while (supply.Any(s => s > 0) && demand.Any(d => d > 0))
      {
        int[] rowPenalties = new int[N];
        int[] colPenalties = new int[M];

        Parallel.For(0, N, i =>
                {
                  if (!rowDone[i])
                  {
                    var costs = Enumerable.Range(0, M).Where(j => !colDone[j]).Select(j => problem.Costs[i, j]).OrderBy(c => c).Take(2).ToArray();
                    rowPenalties[i] = costs.Length > 1 ? costs[1] - costs[0] : costs[0];
                  }
                });

        Parallel.For(0, M, j =>
                {
                  if (!colDone[j])
                  {
                    var costs = Enumerable.Range(0, N).Where(i => !rowDone[i]).Select(i => problem.Costs[i, j]).OrderBy(c => c).Take(2).ToArray();
                    colPenalties[j] = costs.Length > 1 ? costs[1] - costs[0] : costs[0];
                  }
                });

        int maxRowPenalty = rowPenalties.Max();
        int maxColPenalty = colPenalties.Max();

        if (maxRowPenalty >= maxColPenalty)
        {
          int i = Array.IndexOf(rowPenalties, maxRowPenalty);
          var minCost = Enumerable.Range(0, M).Where(j => !colDone[j]).OrderBy(j => problem.Costs[i, j]).First();
          int j = minCost;
          int quantity = Math.Min(supply[i], demand[j]);
          X[i, j] = quantity;
          supply[i] -= quantity;
          demand[j] -= quantity;

          if (supply[i] == 0) rowDone[i] = true;
          if (demand[j] == 0) colDone[j] = true;
        }
        else
        {
          int j = Array.IndexOf(colPenalties, maxColPenalty);
          var minCost = Enumerable.Range(0, N).Where(i => !rowDone[i]).OrderBy(i => problem.Costs[i, j]).First();
          int i = minCost;
          int quantity = Math.Min(supply[i], demand[j]);
          X[i, j] = quantity;
          supply[i] -= quantity;
          demand[j] -= quantity;

          if (supply[i] == 0) rowDone[i] = true;
          if (demand[j] == 0) colDone[j] = true;
        }
      }

      return X;
    });
  }
}
